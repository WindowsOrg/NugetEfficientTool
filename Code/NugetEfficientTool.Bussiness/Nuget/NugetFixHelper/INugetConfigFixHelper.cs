using System.Collections.Generic;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    public interface INugetConfigFixHelper
    {
        /// <summary>
        /// 修复日志
        /// </summary>
        string Log { get; }

        /// <summary>
        /// 执行成功的策略
        /// </summary>
        IEnumerable<NugetFixStrategy> SucceedStrategies { get; }

        /// <summary>
        /// 忽略执行的策略
        /// </summary>
        IEnumerable<NugetFixStrategy> IgnoredStrategies { get; }

        /// <summary>
        /// 执行修复
        /// </summary>
        /// <returns>返回修复后的文档内容</returns>
        XDocument Fix();
    }
}