using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace NugetEfficientTool.Nuget
{
    /// <summary>
    /// 项目架构生成类，用于生成一个可描述当前项目集结构的解决方案
    /// </summary>
    public class ProjectArchitecture
    {
        /// <summary>
        /// 是否只显示组件项目
        /// </summary>
        public bool OnlyComponentCsproj { get; set; } = true;

        /// <summary>
        /// 是否只显示项目中有项目源码的依赖
        /// </summary>
        public bool OnlyShowCsprojDependency { get; set; } = true;
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="codeFolder">源代码目录</param>
        /// <param name="slnFolder">生成的解决方案目录</param>
        /// <returns></returns>
        public void Generate(string codeFolder, string slnFolder)
        {
            var allFiles = FolderHelper.GetAllFiles(codeFolder, "*.csproj");
            var csprojFiles = allFiles.Where(i => !i.Contains("ComponentsArchitecture")).ToList();
            var projectDependencies = GetProjectDependencies(csprojFiles);
            CreateArchitectureSln(projectDependencies, slnFolder);
        }

        private string CreateArchitectureSln(List<CodeModule> codeModules, string slnFolder)
        {
            //创建解决方案文件夹
            FolderHelper.DeleteFolder(slnFolder);
            FolderHelper.CreateFolder(slnFolder);
            //创建项目
            foreach (var projectDependency in codeModules)
            {
                var projectFolder = Path.Combine(slnFolder, projectDependency.Name);
                FolderHelper.CreateFolder(projectFolder);
                CreateProject(projectFolder, projectDependency);
            }
            //创建解决方案
            CreateSln(slnFolder, codeModules);
            return slnFolder;
        }

        private static void CreateProject(string projectFolder, CodeModule codeModule)
        {
            var projectFile = Path.Combine(projectFolder, $"{codeModule.Name}.csproj");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            stringBuilder.AppendLine("\t<PropertyGroup>");
            stringBuilder.AppendLine("\t\t<TargetFramework>net6.0</TargetFramework>");
            stringBuilder.AppendLine("\t\t<ImplicitUsings>enable</ImplicitUsings>");
            stringBuilder.AppendLine("\t</PropertyGroup>");
            stringBuilder.AppendLine("\t<ItemGroup>");
            foreach (var moduleDependency in codeModule.ModuleDependencies)
            {
                var moduleName = moduleDependency.Name;
                stringBuilder.AppendLine($"\t\t<ProjectReference Include=\"..\\{moduleName}\\{moduleName}.csproj\" />");
            }
            stringBuilder.AppendLine("\t</ItemGroup>");
            stringBuilder.AppendLine("</Project>");
            File.WriteAllText(projectFile, stringBuilder.ToString());
        }

        private void CreateSln(string slnFolder, List<CodeModule> modules)
        {
            var stringBuilder = new StringBuilder();
            var headerText =
                "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n# Visual Studio Version 17\r\nVisualStudioVersion = 17.6.33815.320\r\nMinimumVisualStudioVersion = 10.0.40219.1";
            stringBuilder.AppendLine(headerText);
            //添加项目引用
            var slnId = Guid.NewGuid().ToString();
            foreach (var projectValue in modules)
            {
                var projectName = projectValue.Name;
                var projectReference =
                    $"Project(\"{{{slnId}}}\") = \"{projectName}\", \"{projectName}\\{projectName}.csproj\", \"{{{projectValue.Id}}}\"";
                stringBuilder.AppendLine(projectReference);
                stringBuilder.AppendLine("EndProject");
            }
            //添加项目配置
            stringBuilder.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            stringBuilder.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
            stringBuilder.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
            stringBuilder.AppendLine("\tEndGlobalSection");
            stringBuilder.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            foreach (var projectModule in modules)
            {
                var projectId = projectModule.Id;
                stringBuilder.AppendLine($"\t\t{{{projectId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                stringBuilder.AppendLine($"\t\t{{{projectId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                stringBuilder.AppendLine($"\t\t{{{projectId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                stringBuilder.AppendLine($"\t\t{{{projectId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            }
            stringBuilder.AppendLine("\tEndGlobalSection");
            stringBuilder.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
            stringBuilder.AppendLine("\t\tHideSolutionNode = FALSE");
            stringBuilder.AppendLine("\tEndGlobalSection");
            stringBuilder.AppendLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
            stringBuilder.AppendLine($"\t\tSolutionGuid = {{{Guid.NewGuid().ToString()}}}");
            stringBuilder.AppendLine("\tEndGlobalSection");
            stringBuilder.AppendLine("EndGlobal");
            //保存
            var slnFile = Path.Combine(slnFolder, "Architecture.sln");
            File.WriteAllText(slnFile, stringBuilder.ToString());
        }

        /// <summary>
        /// 获取所有项目依赖列表
        /// </summary>
        /// <param name="csprojFiles"></param>
        /// <returns></returns>
        private List<CodeModule> GetProjectDependencies(List<string> csprojFiles)
        {
            var projectModules = new List<ProjectModule>();
            foreach (var csprojFile in csprojFiles)
            {
                if (projectModules.Any(i => i.Name == Path.GetFileNameWithoutExtension(csprojFile)))
                {
                    continue;
                }

                var readAllLines = File.ReadAllLines(csprojFile);
                //暂时只处理组件
                if (OnlyComponentCsproj &&
                    !readAllLines.Any(i => i.Contains("<GeneratePackageOnBuild>true</GeneratePackageOnBuild>") ||
                                                        i.Contains("<GeneratePackageOnBuild>True</GeneratePackageOnBuild>") ||
                                                        i.Contains("<GeneratePackageOnBuild>TRUE</GeneratePackageOnBuild>")))
                {
                    continue;
                }
                Debug.WriteLine(csprojFile);
                projectModules.Add(new ProjectModule(csprojFile));
            }

            var codeModules = new List<CodeModule>(projectModules);
            foreach (var projectModule in projectModules)
            {
                var csprojDocument = new CodeXmlReader((projectModule).CsprojFile).Document;
                //添加项目依赖
                var referenceProjects = GetProjectReferences(csprojDocument);
                foreach (var referenceProject in referenceProjects)
                {
                    var otherProjectModule = projectModules.FirstOrDefault(i => i.Name == referenceProject);
                    if (otherProjectModule == null)
                    {
                        continue;
                    }
                    projectModule.ModuleDependencies.Add(new ModuleDependency(referenceProject, ModuleType.Project));
                }
                //添加Nuget依赖
                var nugetReferences = CsProj.GetNugetInfos(csprojDocument, projectModule.CsprojFile);
                //去除自己
                nugetReferences = nugetReferences.Where(i => i.Name != Path.GetFileNameWithoutExtension(projectModule.CsprojFile)).ToList();
                foreach (var nugetReference in nugetReferences)
                {
                    var otherProjectModule = projectModules.FirstOrDefault(i => i.Name == nugetReference.Name);
                    if (otherProjectModule == null)
                    {
                        if (OnlyShowCsprojDependency)
                        {
                            continue;
                        }
                        if (codeModules.All(i => i.Name != nugetReference.Name))
                        {
                            codeModules.Add(new NugetModule(nugetReference.Name));
                        }
                        projectModule.ModuleDependencies.Add(new ModuleDependency(nugetReference.Name, ModuleType.Nuget));
                        continue;
                    }
                    projectModule.ModuleDependencies.Add(new ModuleDependency(nugetReference.Name, ModuleType.Project));
                }
            }

            return codeModules;
        }

        /// <summary>
        /// 获取指定项目依赖的项目列表
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        private List<string> GetProjectReferences(XDocument xDocument)
        {
            var referenceProjects = new List<string>();
            var projectReferenceElements = CsProj.GetProjectReferences(xDocument);
            foreach (var referenceElement in projectReferenceElements)
            {
                var projectPath = referenceElement.Attribute("Include")?.Value;
                if (!string.IsNullOrEmpty(projectPath))
                {
                    var projectName = Path.GetFileNameWithoutExtension(projectPath);
                    referenceProjects.Add(projectName);
                }
            }
            return referenceProjects;
        }
    }
}
