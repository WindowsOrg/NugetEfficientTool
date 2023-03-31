using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget版本问题检查器
    /// </summary>
    public class VersionErrorChecker
    {
        /// <summary>
        /// 构造一个 Nuget 版本检查器
        /// </summary>
        /// <param name="solutionFiles">解决方案路径</param>
        public VersionErrorChecker(List<string> solutionFiles)
        {
            if (solutionFiles.Count == 0)
            {
                throw new ArgumentNullException(nameof(solutionFiles));
            }
            _solutionFiles = solutionFiles;
        }

        #region 对外字段 & 方法

        /// <summary>
        /// 检测Nuget引用问题
        /// </summary>
        public void Check()
        {
            var solutionFiles = _solutionFiles;
            var projectFiles = solutionFiles.SelectMany(SolutionFileHelper.GetProjectFiles);
            var projectDirectories = projectFiles.Select(Path.GetDirectoryName).Distinct();
            var nugetConfigFiles = new List<string>();
            foreach (var projectDirectory in projectDirectories)
            {
                nugetConfigFiles.AddRange(SolutionFileHelper.GetConfigFilesInFolder(projectDirectory));
            }
            //获取nuget相关信息
            var badFormatNugetFiles = new List<NugetConfigReader>();
            var nugetFiles = new List<FileNugetInfo>();
            foreach (var nugetConfigFile in nugetConfigFiles)
            {
                var nugetConfigReader = new NugetConfigReader(nugetConfigFile);
                if (nugetConfigReader.IsGoodFormat())
                {
                    nugetFiles.AddRange(nugetConfigReader.NugetInfoGroups);
                }
                else
                {
                    badFormatNugetFiles.Add(nugetConfigReader);
                }
            }
            //格式问题及版本问题
            ErrorFormatNugetFiles = badFormatNugetFiles;
            MismatchVersionNugets = GetMismatchVersionNugets(nugetFiles);
            //设置nuget问题异常显示
            var nugetMismatchVersionMessage = CreateNugetMismatchVersionMessage(MismatchVersionNugets);
            foreach (var errorFormatNugetConfig in ErrorFormatNugetFiles)
            {
                Message = StringSplicer.SpliceWithDoubleNewLine(Message, errorFormatNugetConfig.ErrorMessage);
            }
            Message = StringSplicer.SpliceWithDoubleNewLine(Message, nugetMismatchVersionMessage);
        }

        /// <summary>
        /// 筛选Nuget版本冲突列表
        /// </summary>
        /// <param name="fileNugetInfos"></param>
        /// <returns></returns>
        private List<FileNugetInfo> FilterFormatNugets(IEnumerable<FileNugetInfo> fileNugetInfos)
        {
            //不需要关心System、Microsoft等系统相关版本
            var nugetInfos = fileNugetInfos.Where(i => !i.Name.StartsWith("System.") &&
                                                       !i.Name.StartsWith("Microsoft.")).ToList();
            return nugetInfos;
        }

        /// <summary>
        /// 异常 Nuget 配置文件列表
        /// </summary>
        public IEnumerable<NugetConfigReader> ErrorFormatNugetFiles { get; private set; }

        public IEnumerable<FileNugetInfoGroup> MismatchVersionNugets { get; private set; }

        /// <summary>
        /// 检测信息
        /// </summary>
        public string Message { get; private set; }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取版本不匹配Nuget列表
        /// </summary>
        /// <param name="nugetPackageInfoExs"></param>
        /// <returns></returns>
        private IEnumerable<FileNugetInfoGroup> GetMismatchVersionNugets(
             IEnumerable<FileNugetInfo> nugetPackageInfoExs)
        {
            nugetPackageInfoExs = FilterFormatNugets(nugetPackageInfoExs);

            var mismatchVersionNugetGroupList = new List<FileNugetInfoGroup>();
            var nugetPackageInfoGroups = nugetPackageInfoExs.GroupBy(x => x.Name);
            foreach (var nugetPackageInfoGroup in nugetPackageInfoGroups)
            {
                //因为CsProj与package获取nuget信息，都有一定缺陷，所以需要彼此信息进行补偿。
                CompensateNugetInfos(nugetPackageInfoGroup.ToList());
                //筛选掉没问题的数据
                if (nugetPackageInfoGroup.Select(x => x.Version).Distinct().Count() == 1)
                {
                    continue;
                }

                mismatchVersionNugetGroupList.Add(new FileNugetInfoGroup(nugetPackageInfoGroup));
            }

            return mismatchVersionNugetGroupList;
        }

        /// <summary>
        /// 对Nuget信息进行补偿
        /// </summary>
        /// <param name="nugetInfoExs"></param>
        /// <returns></returns>
        private void CompensateNugetInfos(IEnumerable<FileNugetInfo> nugetInfoExs)
        {
            var nugetInfoExGroups = nugetInfoExs.GroupBy(i => Path.GetDirectoryName(i.ConfigPath)).ToList();
            foreach (var nugetInfoExGroup in nugetInfoExGroups)
            {
                var nugetInfoExsInGroup = nugetInfoExGroup.ToList();
                if (nugetInfoExsInGroup.Count != 2)
                {
                    continue;
                }

                var csProjNugetInfoEx = nugetInfoExsInGroup.FirstOrDefault(i => Path.GetExtension(i.ConfigPath) == ".csproj");
                var packageNugetInfoEx = nugetInfoExsInGroup.FirstOrDefault(i => Path.GetExtension(i.ConfigPath) == ".config");
                if (csProjNugetInfoEx == null || packageNugetInfoEx == null)
                {
                    continue;
                }
                csProjNugetInfoEx.TargetFramework = packageNugetInfoEx.TargetFramework;
                csProjNugetInfoEx.Version = packageNugetInfoEx.Version;
                packageNugetInfoEx.NugetDllInfo = csProjNugetInfoEx.NugetDllInfo;
            }
        }

        private string CreateNugetMismatchVersionMessage(
             IEnumerable<FileNugetInfoGroup> mismatchVersionNugetInfoExs)
        {
            var nugetMismatchVersionMessage = string.Empty;
            foreach (var mismatchVersionNugetInfoEx in mismatchVersionNugetInfoExs)
            {
                var headMessage = $"{mismatchVersionNugetInfoEx.NugetName} 存在版本异常：";
                var detailMessage = string.Empty;
                foreach (var nugetPackageInfo in mismatchVersionNugetInfoEx.FileNugetInfos)
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

        #region private fields

        private readonly List<string> _solutionFiles;

        #endregion
    }
}