using System;
using System.IO;
using System.Linq;
using NugetEfficientTool.Nuget;

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
        /// <param name="nugetName"></param>
        /// <param name="nugetVersion"></param>
        public NugetFixStrategy(string nugetName, string nugetVersion)
        {
            NugetName = nugetName;
            NugetVersion = nugetVersion;
        }
        /// <summary>
        /// 构造一条 Nuget 包修复策略
        /// </summary>
        /// <param name="nugetName">名称</param>
        /// <param name="nugetVersion">版本号</param>
        /// <param name="targetFramework"></param>
        /// <param name="dllFilePath"></param>
        public NugetFixStrategy(string nugetName, string nugetVersion, string targetFramework, string dllFilePath) : this(nugetName,
            nugetVersion)
        {
            TargetFramework = targetFramework;
            NugetDllInfo = string.IsNullOrEmpty(dllFilePath) ? null : new NugetDllInfo(dllFilePath, null);
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