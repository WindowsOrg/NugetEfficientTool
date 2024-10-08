﻿using System.Text.RegularExpressions;

namespace NugetEfficientTool.Nuget
{
    /// <summary>
    /// 解决方案文件辅助类
    /// </summary>
    public static class SolutionFiles
    {
        /// <summary>
        /// 获取sln文件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="slnFile"></param>
        /// <returns></returns>
        public static bool TryGetSlnFile(string folder, out string slnFile)
        {
            slnFile = null;
            if (!TryGetSlnFiles(folder, out var files))
            {
                return false;
            }
            slnFile = files[0];
            return true;
        }

        /// <summary>
        /// 获取sln文件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="slnFiles"></param>
        /// <returns></returns>
        public static bool TryGetSlnFiles(string folder, out List<string> slnFiles)
        {
            slnFiles = new List<string>();

            if (!Directory.Exists(folder))
            {
                return false;
            }

            var slnFileList = Directory.GetFiles(folder, "*.sln", SearchOption.AllDirectories);
            if (slnFileList.Length > 0)
            {
                slnFiles = slnFileList.ToList();
                return true;
            }
            return false;
        }

        public static IEnumerable<string> GetProjectFiles(string solutionFile)
        {
            if (!File.Exists(solutionFile))
            {
                throw new ArgumentException($"解决方案{nameof(solutionFile)},不能为空");
            }
            var directory = Path.GetDirectoryName(Path.GetFullPath(solutionFile));
            var text = File.ReadAllText(solutionFile);

            return FindProjectFiles();

            IEnumerable<string> FindProjectFiles()
            {
                var regex = new Regex(
                    @"Project\(""{[\w-]+}""\)\s*=\s*""[\w\.]+"",\s*""(?<csprojPath>.+\.csproj)"",\s*""{[\w-]+}""");
                var matches = regex.Matches(text);
                foreach (Match match in matches)
                {
                    var csprojPath = match.Groups["csprojPath"].Value;
                    var path = Path.Combine(directory, csprojPath);
                    yield return path;
                }
            }
        }

        /// <summary>
        /// 获取目录下所有的 Nuget 配置文件
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetConfigFilesInFolder(string folder)
        {
            var packagesConfigs = GetFilesFromDirectory(folder, "packages.config");
            var csProjs = GetFilesFromDirectory(folder, "*.csproj");
            return packagesConfigs.Concat(csProjs);
        }
        /// <summary>
        /// 获取所有Sln文件中的项目配置文件
        /// </summary>
        /// <param name="solutionFile"></param>
        /// <returns></returns>
        public static List<string> GetConfigFilesInSln(string solutionFile)
        {
            var projectFiles = GetProjectFiles(solutionFile);
            var projectDirectories = projectFiles.Select(Path.GetDirectoryName).Distinct();
            var nugetConfigFiles = new List<string>();
            foreach (var projectDirectory in projectDirectories)
            {
                nugetConfigFiles.AddRange(GetConfigFilesInFolder(projectDirectory));
            }

            return nugetConfigFiles;
        }
        /// <summary>
        /// 获取所有Sln文件中的项目相关配置文件
        /// </summary>
        /// <param name="solutionFile"></param>
        /// <param name="nugetName">Nuget名称</param>
        /// <returns></returns>
        public static IEnumerable<string> GetConfigFilesInSln(string solutionFile, string nugetName)
        {
            var nugetConfigFiles = GetConfigFilesInSln(solutionFile);
            var csProjConfigFiles = nugetConfigFiles.Where(i => NugetConfig.GetNugetConfigType(i) == NugetConfigType.CsProj).ToList();
            foreach (var csProjConfigFile in csProjConfigFiles)
            {
                //筛除所有不相关的配置文件
                var containsNuget = File.ReadAllLines(csProjConfigFile).Any(line => line.Contains(nugetName));
                if (!containsNuget)
                {
                    var csProjectFolder = Path.GetDirectoryName(csProjConfigFile);
                    var configFilesInSameFolder = nugetConfigFiles.Where(i => Path.GetDirectoryName(i) == csProjectFolder).ToList();
                    foreach (var configFile in configFilesInSameFolder)
                    {
                        nugetConfigFiles.Remove(configFile);
                    }
                }
            }
            return nugetConfigFiles;
        }

        /// <summary>
        /// 从指定目录中获取所有文件（含子文件夹）
        /// </summary>
        /// <param name="folder">待遍历的目录</param>
        /// <param name="searchPattern">搜索字符串</param>
        /// <returns>获取到的文件路径列表</returns>
        public static IEnumerable<string> GetFilesFromDirectory(string folder, string searchPattern = null)
        {
            if (!Directory.Exists(folder))
            {
                return new List<string>();
            }
            searchPattern ??= "*";

            var files = Directory.EnumerateFiles(folder, searchPattern);
            foreach (var directory in Directory.GetDirectories(folder))
            {
                files = files.Concat(GetFilesFromDirectory(directory, searchPattern));
            }

            return files;
        }
    }
}
