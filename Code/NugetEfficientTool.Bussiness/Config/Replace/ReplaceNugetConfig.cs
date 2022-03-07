using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Business
{
    [DataContract]
    public class ReplaceNugetConfig
    {
        public ReplaceNugetConfig(string nugetName, string sourceCroProjFile)
        {
            Name = nugetName;
            SourceCsprojPath = sourceCroProjFile;
        }

        /// <summary>
        /// Nuget名称
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }
        /// <summary>
        /// 源代码csproj文件路径(可能是相对路径，也可能是绝对路径)
        /// </summary>
        [DataMember(Name = "SourceCsprojPath")]
        public string SourceCsprojPath { get; set; }
    }
}
