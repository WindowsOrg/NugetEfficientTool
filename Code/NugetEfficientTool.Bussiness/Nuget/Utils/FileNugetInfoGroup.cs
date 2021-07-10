using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NugetEfficientTool.Business
{
    public class FileNugetInfoGroup
    {
        public FileNugetInfoGroup(string nugetName, IEnumerable<FileNugetInfo> fileNugetInfos)
        {
            if (fileNugetInfos.Any(x => x.Name != nugetName))
            {
                throw new InvalidDataException("传入的 Nuget 信息数组存在与声明的 Nuget 名称不匹配的项目");
            }

            NugetName = nugetName;
            FileNugetInfos = fileNugetInfos;
        }


        public FileNugetInfoGroup(IGrouping<string, FileNugetInfo> fileNugetInfoGroup)
            : this(fileNugetInfoGroup.Key, fileNugetInfoGroup)
        {
        }

        public FileNugetInfoGroup(IEnumerable<FileNugetInfo> fileNugetInfos)
            : this(fileNugetInfos.FirstOrDefault()?.Name, fileNugetInfos)
        {
        }

        public string NugetName { get; }

        public IEnumerable<FileNugetInfo> FileNugetInfos { get; }
    }
}