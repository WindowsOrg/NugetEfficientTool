namespace Kybs0.Csproj.Analyzer
{
    /// <summary>
    /// Nuget 配置文件助手类
    /// </summary>
    public class NugetConfig
    {
        /// <summary>
        /// 获取 Nuget 配置文件类型
        /// </summary>
        /// <param name="configPath">Nuget 配置文件路径</param>
        /// <returns>Nuget 配置文件类型</returns>
        public static NugetConfigType GetNugetConfigType( string configPath)
        {
            if (configPath == null)
            {
                throw new ArgumentNullException(nameof(configPath));
            }

            switch (Path.GetExtension(configPath))
            {
                case ".config":
                    return NugetConfigType.PackagesConfig;
                case ".csproj":
                    return NugetConfigType.CsProj;
                default:
                    return NugetConfigType.Unknown;
            }
        }
    }
}