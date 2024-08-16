namespace NugetEfficientTool.Nuget
{
    public abstract class CodeModule
    {
        /// <summary>
        /// 模块类型
        /// </summary>
        public virtual ModuleType ModuleType { get; }
        /// <summary>
        /// 项目名
        /// </summary>
        public virtual string Name { get;protected set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 依赖的项目名列表
        /// </summary>
        public List<ModuleDependency> ModuleDependencies { get; } = new List<ModuleDependency>();
    }
}
