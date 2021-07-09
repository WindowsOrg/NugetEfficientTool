using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Business
{
    public class ReplacedNugetInfo
    {
        /// <summary>
        /// 解决方案
        /// </summary>
        public string SolutionFile { get; set; }
        /// <summary>
        /// Nuget名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 源代码csproj文件路径(可能是相对路径，也可能是绝对路径)
        /// </summary>
        public string SourceCsprojPath { get; set; }
        /// <summary>
        /// 替换记录
        /// </summary>
        public List<ReplacedFileRecord> Records { get; set; }=new List<ReplacedFileRecord>();
    }
    /// <summary>
    /// 一个文件变更记录
    /// </summary>
    public class ReplacedFileRecord
    {
        /// <summary>
        /// Nuget名称
        /// </summary>
        public string NugetName { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 变更位置
        /// </summary>
        public int ModifiedLineIndex { get; set; }
        /// <summary>
        /// Nuget版本
        /// </summary>
        public string Version { get; set; }
        [CanBeNull]
        public string TargetFramework { get; set; }
        [CanBeNull]
        public string NugetDllPath { get; set; }
    }
}
