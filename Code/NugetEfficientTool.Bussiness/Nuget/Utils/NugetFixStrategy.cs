using System;
using System.IO;
using System.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget 包修复策略
    /// </summary>
    public class NugetFixStrategy
    {
        /// <summary>
        /// 构造一条 Nuget 包修复策略
        /// </summary>
        /// <param name="nugetName">名称</param>
        /// <param name="nugetVersion">版本号</param>
        public NugetFixStrategy(string nugetName, string nugetVersion, string targetFramework) : this(nugetName,
            nugetVersion)
        {
            TargetFramework = targetFramework;
            var userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var folder = Path.Combine(userProfileFolder, ".nuget", "packages", nugetName, nugetVersion, "lib",
                TargetFramework);
            var dllFilePath = Path.Combine(folder, $"{nugetName}.dll");
            // 不一定使用 nuget name 命名
            if (!File.Exists(dllFilePath))
            {
                string[] dllFileList;
                if (!Directory.Exists(folder))
                {
                    dllFileList = new string[0];
                }
                else
                {
                    dllFileList = Directory.GetFiles(folder, "*.dll");
                }
                if (dllFileList.Length == 0)
                {
                    throw new ArgumentException($"找不到 {dllFilePath}，无法进行修复。要不您老人家先试着编译一下，还原下 Nuget 包，然后再来看看？");
                }
                if (dllFileList.Length == 1)
                {
                    dllFilePath = dllFileList[0];
                }
                else
                {
                    var file = dllFileList.FirstOrDefault(temp => temp.ToLower().Contains(nugetName.ToLower()));
                    if (file != null)
                    {
                        dllFilePath = file;
                    }
                    else
                    {
                        dllFilePath = dllFileList[0];
                    }
                }
            }

            NugetDllInfo = new NugetDllInfo(dllFilePath, null);
        }

        /// <summary>
        /// 构造一条 Nuget 包修复策略
        /// </summary>
        /// <param name="nugetName">名称</param>
        /// <param name="nugetVersion">版本号</param>
        /// <param name="nugetDllInfo">Dll 信息</param>
        public NugetFixStrategy(string nugetName, string nugetVersion, NugetDllInfo nugetDllInfo) : this(
            nugetName, nugetVersion)
        {
            NugetDllInfo = nugetDllInfo ?? throw new ArgumentNullException(nameof(nugetDllInfo));

            if (!string.IsNullOrEmpty(nugetDllInfo.DllPath) && File.Exists(nugetDllInfo.DllPath))
            {
                TargetFramework = CsProj.GetTargetFrameworkOfDll(nugetDllInfo.DllPath);
            }
        }

        private NugetFixStrategy(string nugetName, string nugetVersion)
        {
            NugetName = nugetName;
            NugetVersion = nugetVersion;
        }

        /// <summary>
        /// Nuget 名称
        /// </summary>
        public string NugetName { get; }

        /// <summary>
        /// Nuget 版本号
        /// </summary>
        public string NugetVersion { get; }

        /// <summary>
        /// Nuget 目标框架
        /// </summary>
        public string TargetFramework { get; }

        /// <summary>
        /// Dll 信息
        /// </summary>
        public NugetDllInfo NugetDllInfo { get; }
    }
}