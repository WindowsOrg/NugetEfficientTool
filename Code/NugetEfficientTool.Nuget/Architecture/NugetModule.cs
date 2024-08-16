namespace NugetEfficientTool.Nuget
{
    public sealed class NugetModule:CodeModule
    {
        public NugetModule(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 模块类型
        /// </summary>
        public override ModuleType ModuleType => ModuleType.Nuget;
    }
}
