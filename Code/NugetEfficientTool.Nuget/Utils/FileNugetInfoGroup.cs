namespace NugetEfficientTool.Nuget
{
    public class FileNugetInfoGroup
    {
        public FileNugetInfoGroup(string nugetName, List<FileNugetInfo> fileNugetInfos)
        {
            if (fileNugetInfos.Any(x => x.Name != nugetName))
            {
                throw new InvalidDataException("传入的 Nuget 信息数组存在与声明的 Nuget 名称不匹配的项目");
            }

            NugetName = nugetName;
            FileNugetInfos = fileNugetInfos;
        }


        public FileNugetInfoGroup(IGrouping<string, FileNugetInfo> fileNugetInfoGroup)
            : this(fileNugetInfoGroup.Key, fileNugetInfoGroup.ToList())
        {
        }

        public FileNugetInfoGroup(List<FileNugetInfo> fileNugetInfos)
            : this(fileNugetInfos.FirstOrDefault()?.Name, fileNugetInfos)
        {
        }

        public string NugetName { get; }

        public List<FileNugetInfo> FileNugetInfos { get; }
    }
}