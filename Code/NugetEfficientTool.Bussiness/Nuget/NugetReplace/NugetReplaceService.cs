using System;
using System.IO;
using System.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget替换
    /// </summary>
    public class NugetReplaceService
    {
        private readonly string _projectId;

        public NugetReplaceService(string projectId)
        {
            _projectId = projectId;
        }

        /// <summary>
        /// 是否完成了替换操作
        /// </summary>
        /// <param name="solutionFile"></param>
        /// <param name="sourceProjectFile"></param>
        /// <returns></returns>
        public bool HasReplaced(string solutionFile, string sourceProjectFile)
        {
            var solutionFileLines = File.ReadAllLines(solutionFile);
            //是否有相应的源码引用
            var hasReplaced = solutionFileLines.Any(i => i.Contains($"\"{sourceProjectFile}\""));
            return hasReplaced;
        }
        /// <summary>
        /// 替换Nuget包为源代码
        /// </summary>
        /// <param name="solutionFile">解决方案</param>
        /// <param name="nugetName">Nuget包</param>
        /// <param name="sourceProjectFile">源代码csProject</param>
        public ReplacedNugetInfo Replace(string solutionFile, string nugetName, string sourceProjectFile)
        {
            if (HasReplaced(solutionFile, sourceProjectFile))
            {
                return null;
            }
            var replacedNugetInfo = new ReplacedNugetInfo()
            {
                SolutionFile = solutionFile,
                Name = nugetName,
                SourceCsprojPath = sourceProjectFile
            };
            var newProjectId = Guid.NewGuid().ToString().ToUpper();
            //先获取所有配置文件
            var configFilesInSln = SolutionFileHelper.GetConfigFilesInSln(solutionFile, nugetName);
            //替换解决方案文件 - Sln file
            new SlnFileNugetReplacer(solutionFile, nugetName, newProjectId, sourceProjectFile).ReplaceNuget();
            //替换项目配置文件
            foreach (var configFile in configFilesInSln)
            {
                var nugetConfigType = NugetConfig.GetNugetConfigType(configFile);
                switch (nugetConfigType)
                {
                    //csProject file
                    case NugetConfigType.CsProj:
                        {
                            var replacedFileRecord = new CsProjNugetReplacer(configFile, nugetName, newProjectId, sourceProjectFile).ReplaceNuget();
                            if (replacedFileRecord != null)
                            {
                                replacedNugetInfo.Records.Add(replacedFileRecord);
                            }
                        }
                        break;
                    //package file
                    case NugetConfigType.PackagesConfig:
                        {
                            var replacedFileRecord = new PackageFileNugetReplacer(configFile, nugetName).ReplaceNuget();
                            if (replacedFileRecord != null)
                            {
                                replacedNugetInfo.Records.Add(replacedFileRecord);
                            }
                        }
                        break;
                }
            }
            return replacedNugetInfo;
        }
        /// <summary>
        /// 替换为原Nuget包
        /// </summary>
        public bool Revert(string solutionFile, string nugetName, string sourceProjectFile)
        {
            if (!HasReplaced(solutionFile, sourceProjectFile))
            {
                return false;
            }

            var replacedNugetInfo = NugetReplaceCacheManager.GetReplacedNugetInfo(_projectId, solutionFile, nugetName);
            if (replacedNugetInfo == null)
            {
                throw new InvalidOperationException($"没有{solutionFile}的Nuget替换记录，不能恢复");
            }
            //恢复解决方案文件 - Sln file
            new SlnFileNugetReplacer(solutionFile, replacedNugetInfo.Name, sourceProjectFile).RevertNuget();
            //恢复项目配置文件
            var configFilesInSln = SolutionFileHelper.GetConfigFilesInSln(solutionFile, replacedNugetInfo.Name);
            foreach (var configFile in configFilesInSln)
            {
                var replacedFileRecord = replacedNugetInfo.Records.FirstOrDefault(i => i.FileName == configFile);
                if (replacedFileRecord == null)
                {
                    //MessageBox.Show($"没有{configFile}的Nuget替换记录，不能恢复");
                    //return false;
                    continue;
                }
                var nugetConfigType = NugetConfig.GetNugetConfigType(configFile);
                switch (nugetConfigType)
                {
                    //csProject file
                    case NugetConfigType.CsProj:
                        {
                            new CsProjNugetReplacer(configFile, replacedFileRecord, sourceProjectFile).RevertNuget();
                        }
                        break;
                    //package file
                    case NugetConfigType.PackagesConfig:
                        {
                            new PackageFileNugetReplacer(configFile, replacedFileRecord).RevertNuget();
                        }
                        break;
                }
            }

            replacedNugetInfo.Records.Clear();
            NugetReplaceCacheManager.ClearReplacedNugetInfo(_projectId, replacedNugetInfo.SolutionFile, replacedNugetInfo.Name);
            return true;
        }
    }
}
