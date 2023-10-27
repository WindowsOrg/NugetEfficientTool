namespace Kybs0.Csproj.Analyzer
{
    /// <summary>
    /// Nuget 包信息类
    /// </summary>
    public class NugetInfo
    {
        /// <summary>
        /// 构造一条 Nuget 包信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="version">版本号</param>
        /// <param name="targetFramework">目标框架</param>
        public NugetInfo(string name, string version, string targetFramework = null)
        {
            Name = name;
            Version = AdjustVersionValue(version);
            TargetFramework = targetFramework;
        }

        private string AdjustVersionValue(string version)
        {
            if (string.IsNullOrWhiteSpace(version) || !version.Contains("."))
            {
                return version;
            }
            var versionTags = version.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (versionTags.Length >= 4 && version.EndsWith(".0"))
            {
                version = version.Substring(0, version.Length - 2);
            }
            return version;
        }

        /// <summary>
        /// 构造一条 Nuget 包信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="version">版本号</param>
        /// <param name="nugetDllInfo">Dll 信息</param>
        public NugetInfo(string name, string version, NugetDllInfo nugetDllInfo) : this(name, version)
        {
            NugetDllInfo = nugetDllInfo;
        }

        protected NugetInfo(NugetInfo nugetInfo) : this(nugetInfo.Name, nugetInfo.Version, nugetInfo.TargetFramework)
        {
            NugetDllInfo = nugetInfo.NugetDllInfo;
        }

        /// <summary>
        /// Nuget 名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Nuget 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Nuget 目标框架
        /// </summary>
        public string TargetFramework { get; set; }

        /// <summary>
        /// Dll 信息
        /// </summary>
        public NugetDllInfo NugetDllInfo { get; set; }
    }
}