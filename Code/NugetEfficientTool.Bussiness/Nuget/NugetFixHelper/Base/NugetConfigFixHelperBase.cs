using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    public abstract class NugetConfigFixHelperBase : INugetConfigFixHelper
    {
        #region 构造函数

        protected NugetConfigFixHelperBase( XDocument xDocument,
             IEnumerable<NugetFixStrategy> nugetFixStrategies)
        {
            Document = xDocument;
            NugetFixStrategies = nugetFixStrategies ?? throw new ArgumentNullException(nameof(nugetFixStrategies));
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 执行修复
        /// </summary>
        /// <returns>返回修复后的文档内容</returns>
        public XDocument Fix()
        {
            foreach (var nugetFixStrategy in NugetFixStrategies)
            {
                if (FixDocumentByStrategy(nugetFixStrategy))
                {
                    _succeedNugetFixStrategyList.Add(nugetFixStrategy);
                }
                else
                {
                    _ignoredNugetFixStrategyList.Add(nugetFixStrategy);
                }
            }

            return Document;
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 使用指定的策略进行修复
        /// </summary>
        /// <param name="nugetFixStrategy">修复策略</param>
        /// <returns>策略是否有效执行</returns>
        protected abstract bool FixDocumentByStrategy(NugetFixStrategy nugetFixStrategy);

        #endregion

        #region 公共字段

        public string Log { get; protected set; }

        public IEnumerable<NugetFixStrategy> SucceedStrategies => _succeedNugetFixStrategyList;

        public IEnumerable<NugetFixStrategy> IgnoredStrategies => _ignoredNugetFixStrategyList;

        #endregion

        #region 内部变量

        protected readonly XDocument Document;

        protected readonly IEnumerable<NugetFixStrategy> NugetFixStrategies;

        private readonly List<NugetFixStrategy> _succeedNugetFixStrategyList = new List<NugetFixStrategy>();

        private readonly List<NugetFixStrategy> _ignoredNugetFixStrategyList = new List<NugetFixStrategy>();

        #endregion
    }
}