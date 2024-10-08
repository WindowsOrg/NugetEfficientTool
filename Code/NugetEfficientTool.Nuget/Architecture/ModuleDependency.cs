﻿namespace NugetEfficientTool.Nuget
{
    public class ModuleDependency
    {
        public string Name { get; }
        public ModuleType ModuleType { get; }

        public ModuleDependency(string name, ModuleType moduleType)
        {
            Name = name;
            ModuleType = moduleType;
        }
    }
}
