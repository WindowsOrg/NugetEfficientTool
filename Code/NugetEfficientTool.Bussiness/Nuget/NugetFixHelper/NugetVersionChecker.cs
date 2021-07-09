using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NugetEfficientTool.Business;

namespace NugetEfficientTool.Business
{
    public class NugetVersionChecker
    {
        /// <summary>
        /// 构造一个 Nuget 版本检查器
        /// </summary>
        /// <param name="solutionFilePath">解决方案路径</param>
        public NugetVersionChecker(string solutionFilePath)
        {
            if (solutionFilePath == null)
            {
                throw new ArgumentNullException(nameof(solutionFilePath));
            }

            if (!File.Exists(solutionFilePath))
            {
                throw new FileNotFoundException(solutionFilePath);
            }

            _solutionFilePath = solutionFilePath;
        }

        #region 私有变量

        private readonly string _solutionFilePath;

        #endregion

        #region 对外字段 & 方法

        /// <summary>
        /// 检测Nuget问题
        /// </summary>
        public void Check()
        {
            var projectFiles = SolutionFileHelper.GetProjectFiles(_solutionFilePath);
            var projectDirectories = projectFiles.Select(Path.GetDirectoryName);
            var nugetConfigFiles = new List<string>();
            foreach (var projectDirectory in projectDirectories)
            {
                nugetConfigFiles.AddRange(SolutionFileHelper.GetNugetConfigFiles(projectDirectory));
            }

            var badFormatNugetConfigList = new List<NugetConfigReader>();
            var goodFormatNugetInfoExList = new List<NugetInfoEx>();
            foreach (var nugetConfigFile in nugetConfigFiles)
            {
                var nugetConfigReader = new NugetConfigReader(nugetConfigFile);
                if (nugetConfigReader.IsGoodFormat())
                {
                    goodFormatNugetInfoExList.AddRange(nugetConfigReader.PackageInfoExs);
                }
                else
                {
                    badFormatNugetConfigList.Add(nugetConfigReader);
                }
            }

            ErrorFormatNugetConfigs = badFormatNugetConfigList;
            MismatchVersionNugetInfoExs = GetMismatchVersionNugets(goodFormatNugetInfoExList);
            var nugetMismatchVersionMessage = CreateNugetMismatchVersionMessage(MismatchVersionNugetInfoExs);
            foreach (var errorFormatNugetConfig in ErrorFormatNugetConfigs)
            {
                Message = StringSplicer.SpliceWithDoubleNewLine(Message, errorFormatNugetConfig.ErrorMessage);
            }

            Message = StringSplicer.SpliceWithDoubleNewLine(Message, nugetMismatchVersionMessage);
            if (string.IsNullOrEmpty(Message))
            {
                Message = "完美无瑕！";
            }
        }

        /// <summary>
        /// 异常 Nuget 配置文件列表
        /// </summary>
        public IEnumerable<NugetConfigReader> ErrorFormatNugetConfigs { get; private set; }

        public IEnumerable<VersionUnusualNugetInfoExGroup> MismatchVersionNugetInfoExs { get; private set; }

        /// <summary>
        /// 检测信息
        /// </summary>
        public string Message { get; private set; }

        #endregion

        #region 私有方法

        private IEnumerable<VersionUnusualNugetInfoExGroup> GetMismatchVersionNugets(
             IEnumerable<NugetInfoEx> nugetPackageInfoExs)
        {
            var mismatchVersionNugetGroupList = new List<VersionUnusualNugetInfoExGroup>();
            var nugetPackageInfoGroups = nugetPackageInfoExs.GroupBy(x => x.Name);
            foreach (var nugetPackageInfoGroup in nugetPackageInfoGroups)
            {
                var groupByConfigPath = nugetPackageInfoGroup.GroupBy(x => x.ConfigPath);
                if (groupByConfigPath.All(x => x.Count() == 1) &&
                    nugetPackageInfoGroup.Select(x => x.Version).Distinct().Count() == 1)
                {
                    continue;
                }

                mismatchVersionNugetGroupList.Add(new VersionUnusualNugetInfoExGroup(nugetPackageInfoGroup));
            }

            return mismatchVersionNugetGroupList;
        }

        private string CreateNugetMismatchVersionMessage(
             IEnumerable<VersionUnusualNugetInfoExGroup> mismatchVersionNugetInfoExs)
        {
            var nugetMismatchVersionMessage = string.Empty;
            foreach (var mismatchVersionNugetInfoEx in mismatchVersionNugetInfoExs)
            {
                var headMessage = $"{mismatchVersionNugetInfoEx.NugetName} 存在版本异常：";
                var detailMessage = string.Empty;
                foreach (var nugetPackageInfo in mismatchVersionNugetInfoEx.VersionUnusualNugetInfoExs)
                {
                    var mainDetailMessage = $"  {nugetPackageInfo.Version}，{nugetPackageInfo.ConfigPath}";
                    detailMessage = StringSplicer.SpliceWithNewLine(detailMessage, mainDetailMessage);
                }

                var singleNugetMismatchVersionMessage = StringSplicer.SpliceWithNewLine(headMessage, detailMessage);
                nugetMismatchVersionMessage = StringSplicer.SpliceWithDoubleNewLine(nugetMismatchVersionMessage,
                    singleNugetMismatchVersionMessage);
            }

            return nugetMismatchVersionMessage;
        }

        #endregion
    }
}