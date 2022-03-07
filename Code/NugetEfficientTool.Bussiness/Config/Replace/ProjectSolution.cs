using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Business
{
    [DataContract]
    public class ProjectSolution
    {
        /// <summary>
        /// 解决方案
        /// </summary>
        [DataMember]
        public string SolutionFile { get; set; }
        /// <summary>
        /// 项目Id
        /// </summary>
        [DataMember]
        public string Id { get; set; }
    }
}
