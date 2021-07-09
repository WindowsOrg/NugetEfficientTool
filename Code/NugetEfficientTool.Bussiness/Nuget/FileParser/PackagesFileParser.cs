using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NugetEfficientTool.Business;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget 解析器
    /// 用于解析 PackagesConfig.xml
    /// </summary>
    internal class PackagesFileParser : INugetFileParser
    {
        public PackagesFileParser(XDocument xDocument)
        {
            _xDocument = xDocument;
        }

        /// <summary>
        /// 解析时的异常信息
        /// </summary>
        public string ExceptionMessage { get; private set; }

        /// <summary>
        /// 是否格式正常
        /// </summary>
        /// <returns>是否格式正常</returns>
        public bool IsGoodFormat()
        {
            if (_isGoodFormat.HasValue)
            {
                return _isGoodFormat.Value;
            }

            _isGoodFormat = false;
            var root = _xDocument.Root;
            if (root.Name.LocalName != PackagesConfig.RootName)
            {
                ExceptionMessage = $"packages.config 根节点名称不为 ${PackagesConfig.RootName}";
                return false;
            }

            var xElements = root.Elements();
            for (var i = 0; i < xElements.Count(); i++)
            {
                var element = xElements.ElementAt(i);
                if (element.Name.LocalName != PackagesConfig.ElementName)
                {
                    ExceptionMessage = $"第 {i} 个节点异常：{element.Name} 不是合法的 packages.config 子节点。";
                    return false;
                }

                if (!CheckElementAttribute(element, PackagesConfig.IdAttribute, out var errorMessage)
                    || !CheckElementAttribute(element, PackagesConfig.VersionAttribute, out errorMessage)
                    || !CheckElementAttribute(element, PackagesConfig.TargetFrameworkAttribute, out errorMessage))
                {
                    ExceptionMessage = $"第 {i} 个节点异常：{errorMessage}";
                    return false;
                }
            }

            _isGoodFormat = true;
            return true;
        }

        /// <summary>
        /// 获取 Nuget 包信息列表
        /// </summary>
        /// <returns>Nuget 包信息列表</returns>
        public IEnumerable<NugetInfo> GetNugetInfos()
        {
            if (!IsGoodFormat())
            {
                throw new InvalidOperationException("无法在格式异常的配置文件中读取 Nuget 信息。");
            }

            var nugetInfoList = new List<NugetInfo>();
            var root = _xDocument.Root;
            var xElements = root.Elements();
            foreach (var element in xElements)
            {
                var packageInfo = new NugetInfo(
                    element.Attribute(PackagesConfig.IdAttribute).Value,
                    element.Attribute(PackagesConfig.VersionAttribute).Value,
                    element.Attribute(PackagesConfig.TargetFrameworkAttribute).Value);
                nugetInfoList.Add(packageInfo);
            }

            return nugetInfoList;
        }

        private bool CheckElementAttribute(XElement xElement, string attributeName, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (xElement.Attribute(PackagesConfig.TargetFrameworkAttribute) == null)
            {
                errorMessage = $"缺少 {PackagesConfig.TargetFrameworkAttribute} 属性。";
                return false;
            }

            return true;
        }

        #region 私有变量

        private readonly XDocument _xDocument;

        private bool? _isGoodFormat;

        #endregion
    }
}