using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget自动修复
    /// </summary>
    public class NugetAutoFixService
    {
        private readonly string _solutionFile;

        /// <summary>
        /// 提供类<see cref="NugetAutoFixService"/>实例的初始化
        /// </summary>
        /// <param name="solutionFile"></param>
        public NugetAutoFixService(string solutionFile)
        {
            _solutionFile = solutionFile;
        }

        /// <summary>
        /// 修复
        /// </summary>
        /// <returns>有异常</returns>
        public bool Fix()
        {
            if (!CanFix(out var versionChecker))
            {
                return false;
            }
            Message = StringSplicer.SpliceWithDoubleNewLine(Message, versionChecker.Message);
            if (versionChecker.ErrorFormatNugetFiles?.Count() > 0)
            {
                return true;
            }
            //获取修复版本
            var fixStrategies = new List<NugetFixStrategy>();
            foreach (var mismatchVersionNugetGroup in versionChecker.MismatchVersionNugets)
            {
                var nugetName = mismatchVersionNugetGroup.NugetName;
                var nugetVersions = mismatchVersionNugetGroup.FileNugetInfos.Select(x => x.Version).Distinct().ToList();
                //版本大小倒序
                nugetVersions.Sort(NugetVersionContrast.VersionDescendingComparison);
                fixStrategies.Add(new NugetFixStrategy(nugetName, nugetVersions.First()));
            }
            if (!fixStrategies.Any())
            {
                return true;
            }
            //修复
            var versionFixer = new NugetMismatchVersionGroupFix(versionChecker.MismatchVersionNugets, fixStrategies);
            var repairLog = versionFixer.Fix();
            Message = StringSplicer.SpliceWithDoubleNewLine(Message, repairLog);
            return true;
        }

        private bool CanFix(out VersionErrorChecker versionChecker)
        {
            versionChecker = new VersionErrorChecker(_solutionFile);
            versionChecker.Check();
            return !string.IsNullOrEmpty(versionChecker.Message);
        }

        public string Message { get; private set; }
    }
}
