namespace Kybs0.Project.Architecture
{
    /// <summary>
    /// 项目文件
    /// </summary>
    public class ProjectModule : CodeModule
    {
        public ProjectModule(string csprojFile)
        {
            CsprojFile = csprojFile;
        }
        /// <summary>
        /// Csproj文件路径
        /// </summary>
        public string CsprojFile { get; }
        /// <summary>
        /// 项目名
        /// </summary>
        public override string Name => Path.GetFileNameWithoutExtension(CsprojFile);

        /// <summary>
        /// 模块类型
        /// </summary>
        public override ModuleType ModuleType => ModuleType.Project;
    }
}
