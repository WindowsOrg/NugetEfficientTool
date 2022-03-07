using System.Collections.Generic;
using System.Runtime.Serialization;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Business
{
    [DataContract]
    public class ReplacedNugetInfo
    {
        /// <summary>
        /// 解决方案
        /// </summary>
        [DataMember]
        public string SolutionFile { get; set; }
        /// <summary>
        /// Nuget名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 源代码csproj文件路径(可能是相对路径，也可能是绝对路径)
        /// </summary>
        [DataMember]
        public string SourceCsprojPath { get; set; }
        /// <summary>
        /// 替换记录
        /// </summary>
        [DataMember]
        public List<ReplacedFileRecord> Records { get; set; }=new List<ReplacedFileRecord>();
    }
    /// <summary>
    /// 一个文件变更记录
    /// </summary>
    [DataContract]
    public class ReplacedFileRecord
    {
        /// <summary>
        /// Nuget名称
        /// </summary>
        [DataMember]
        public string NugetName { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember]
        public string FileName { get; set; }
        /// <summary>
        /// 变更位置
        /// </summary>
        [DataMember]
        public int ModifiedLineIndex { get; set; }
        /// <summary>
        /// Nuget版本
        /// </summary>
        [DataMember]
        public string Version { get; set; }
        [CanBeNull]
        [DataMember]
        public string TargetFramework { get; set; }
        [CanBeNull]
        [DataMember]
        public string NugetDllPath { get; set; }
    }
}
