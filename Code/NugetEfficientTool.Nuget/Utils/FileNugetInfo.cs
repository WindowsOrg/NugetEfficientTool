namespace Kybs0.Project
{
    public class FileNugetInfo : NugetInfo
    {
        /// <summary>
        /// 构造一条 Nuget 包信息（拓展）
        /// </summary>
        /// <param name="nugetInfo">Nuget 包信息类</param>
        /// <param name="configPath">配置文件路径</param>
        public FileNugetInfo(NugetInfo nugetInfo, string configPath) : base(nugetInfo)
        {
            ConfigPath = configPath;
        }

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string ConfigPath { get; }

        /// <summary>
        /// 是否空文件。表示这不是一个配置文件，只是一个虚拟的NugetInfo。比如Nuget源下的Nuget包
        /// </summary>
        public bool IsEmptyFile { get; set; }
    }
}