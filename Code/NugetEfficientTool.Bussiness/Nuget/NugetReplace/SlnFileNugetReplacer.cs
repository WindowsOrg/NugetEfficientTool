using System.IO;
using System.Linq;
using System.Text;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// 解决方案Nuget替换类
    /// </summary>
    class SlnFileNugetReplacer : INugetFileReplacer
    {
        private readonly string _solutionFile;
        private readonly string _nugetName;
        private readonly string _newProjectId;
        private readonly string _sourceProjectFile;

        public SlnFileNugetReplacer(string solutionFile, string nugetName, string newProjectId, string sourceProjectFile) :
            this(solutionFile, nugetName, sourceProjectFile)
        {
            _newProjectId = newProjectId;
        }
        public SlnFileNugetReplacer(string solutionFile, string nugetName, string sourceProjectFile)
        {
            _solutionFile = solutionFile;
            _nugetName = nugetName;
            _sourceProjectFile = sourceProjectFile;
        }

        private const string StartProjectRex = "Project(\"{";
        private const int GuidLength = 36;
        private const int GuidRightParcelLength = 2;
        /// <summary>
        /// 替换Nuget包
        /// </summary>
        public ReplacedFileRecord ReplaceNuget()
        {
            var solutionFileLines = File.ReadAllLines(_solutionFile).ToList();
            //添加Project
            var previousProjectIndex = solutionFileLines.FindLastIndex(i => i.Contains(StartProjectRex));
            var previousProjectLine = solutionFileLines[previousProjectIndex];
            var solutionId = previousProjectLine.Replace(StartProjectRex, string.Empty).Substring(0, GuidLength);
            var newProjectLine = $"{StartProjectRex}{solutionId}}}\") = \"{_nugetName}\", \"{_sourceProjectFile}\", \"{{{_newProjectId}}}\"";
            solutionFileLines.Insert(previousProjectIndex + 2, "EndProject");
            solutionFileLines.Insert(previousProjectIndex + 2, newProjectLine);
            //添加编译配置
            var projectConfigStartIndex = solutionFileLines.FindIndex(i => i.Contains("ProjectConfigurationPlatforms"));
            var projectConfigEndIndex = solutionFileLines.FindIndex(projectConfigStartIndex + 1, i => i.Contains("EndGlobalSection"));
            solutionFileLines.Insert(projectConfigEndIndex, $"{{{_newProjectId}}}.Debug|Any CPU.Build.0 = Release|Any CPU");
            solutionFileLines.Insert(projectConfigEndIndex, $"{{{_newProjectId}}}.Debug|Any CPU.Build.0 = Release|Any CPU");
            solutionFileLines.Insert(projectConfigEndIndex, $"{{{_newProjectId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            solutionFileLines.Insert(projectConfigEndIndex, $"{{{_newProjectId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            File.WriteAllLines(_solutionFile, solutionFileLines, Encoding.UTF8);
            return null;
        }
        /// <summary>
        /// 恢复Nuget包
        /// </summary>
        public void RevertNuget()
        {
            var solutionFileLines = File.ReadAllLines(_solutionFile).ToList();
            //找到源代码引用
            var referenceLineIndex = solutionFileLines.FindIndex(i => i.Contains($"\"{_sourceProjectFile}\""));
            if (referenceLineIndex > -1)
            {
                var sourceProjectLine = solutionFileLines[referenceLineIndex];
                var sourceProjectReferenceId = sourceProjectLine.Substring(sourceProjectLine.Length - GuidLength - GuidRightParcelLength, GuidLength);
                //删除引用 -- 删除俩行，StartProject和EndProject
                solutionFileLines.RemoveAt(referenceLineIndex);
                solutionFileLines.RemoveAt(referenceLineIndex);
                //查找编译配置并删除
                var allSourceProjectBuildConfigs = solutionFileLines.FindAll(i=>i.Contains($"{{{sourceProjectReferenceId}}}")).ToList();
                foreach (var allSourceProjectBuildConfig in allSourceProjectBuildConfigs)
                {
                    solutionFileLines.Remove(allSourceProjectBuildConfig);
                }
                //保存
                File.WriteAllLines(_solutionFile, solutionFileLines, Encoding.UTF8);
            }
        }
    }
}
