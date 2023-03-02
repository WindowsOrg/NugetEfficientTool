using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget引用修复器抽象类
    /// </summary>
    public abstract class NugetReferenceFixerBase
    {
        #region 构造函数

        protected NugetReferenceFixerBase(XDocument xDocument, IEnumerable<NugetFixStrategy> nugetFixStrategies)
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
                    _succeedNugetFixStrategies.Add(nugetFixStrategy);
                }
                else
                {
                    _failedNugetFixStrategies.Add(nugetFixStrategy);
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

        /// <summary>
        /// 修复日志
        /// </summary>
        public string Log { get; protected set; }

        /// <summary>
        /// 执行成功的策略
        /// </summary>
        public IEnumerable<NugetFixStrategy> SucceedStrategies => _succeedNugetFixStrategies;

        /// <summary>
        /// 执行失败的策略
        /// </summary>
        public IEnumerable<NugetFixStrategy> FailedStrategies => _failedNugetFixStrategies;

        #endregion

        #region 内部变量

        protected readonly XDocument Document;

        protected readonly IEnumerable<NugetFixStrategy> NugetFixStrategies;

        private readonly List<NugetFixStrategy> _succeedNugetFixStrategies = new List<NugetFixStrategy>();

        private readonly List<NugetFixStrategy> _failedNugetFixStrategies = new List<NugetFixStrategy>();

        #endregion
    }
}