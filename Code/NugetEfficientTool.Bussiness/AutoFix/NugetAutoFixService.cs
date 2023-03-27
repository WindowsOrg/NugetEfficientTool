using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget自动修复
    /// </summary>
    public class NugetAutoFixService
    {
        private readonly string _inputPath;

        /// <summary>
        /// 提供类<see cref="NugetAutoFixService"/>实例的初始化
        /// </summary>
        /// <param name="inputPath"></param>
        public NugetAutoFixService(string inputPath)
        {
            _inputPath = inputPath;
        }

        /// <summary>
        /// 修复
        /// </summary>
        /// <returns>有异常</returns>
        public bool Fix()
        {
            if (!TryGetSlnFiles(_inputPath, out var solutionFiles))
            {
                return false;
            }
            if (!CanFix(solutionFiles, out var versionChecker))
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

        private bool CanFix(List<string> solutionFiles, out VersionErrorChecker versionChecker)
        {
            versionChecker = new VersionErrorChecker(solutionFiles);
            versionChecker.Check();
            return !string.IsNullOrEmpty(versionChecker.Message);
        }
        private bool TryGetSlnFiles(string solutionText, out List<string> solutionFiles)
        {
            solutionFiles = new List<string>();
            if (string.IsNullOrWhiteSpace(solutionText))
            {
                Console.WriteLine("解决方案路径不能为空……");
                return false;
            }
            if (File.Exists(solutionText) && Path.GetExtension(solutionText) == ".sln")
            {
                solutionFiles.Add(solutionText);
                return true;
            }
            if (!File.Exists(solutionText) &&
                Directory.Exists(solutionText) &&
                SolutionFileHelper.TryGetSlnFiles(solutionText, out solutionFiles) &&
                solutionFiles.Count > 0)
            {
                return true;
            }
            Console.WriteLine("找不到指定的解决方案，这是啥情况？？？");
            return false;
        }

        public string Message { get; private set; }
    }
}
