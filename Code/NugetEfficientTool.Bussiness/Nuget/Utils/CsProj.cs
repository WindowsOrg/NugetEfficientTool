using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// CsProject
    /// </summary>
    public class CsProj
    {

        #region 获取节点

        /// <summary>
        /// 获取PackageReference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetPackageReferences(XDocument xDocument)
        {
            return GetXElementsByNameInItemGroups(xDocument, PackageReferenceName);
        }
        /// <summary>
        /// 获取Reference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetReferences(XDocument xDocument)
        {
            return GetXElementsByNameInItemGroups(xDocument, ReferenceName);
        }
        /// <summary>
        /// 获取ProjectReference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetProjectReferences(XDocument xDocument)
        {
            return GetXElementsByNameInItemGroups(xDocument, ProjectReferenceName);
        }
        /// <summary>
        /// 获取所有Nuget引用节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetNugetInfoReferences(XDocument xDocument)
        {
            return GetReferences(xDocument).Where(IsNugetInfoReference);
        }

        /// <summary>
        /// 获取Element名称对应的节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <param name="xElementName"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetXElementsByNameInItemGroups(XDocument xDocument,
             string xElementName)
        {
            if (xDocument == null)
            {
                throw new ArgumentNullException(nameof(xDocument));
            }

            if (xElementName == null)
            {
                throw new ArgumentNullException(nameof(xElementName));
            }

            var xElementList = new List<XElement>();
            var itemGroupElements = xDocument.Root.Elements().Where(x => x.Name.LocalName == ItemGroupName);
            foreach (var itemGroupElement in itemGroupElements)
            {
                xElementList.AddRange(itemGroupElement.Elements().Where(x => x.Name.LocalName == xElementName));
            }

            return xElementList;
        }

        #endregion

        #region Nuget相关

        /// <summary>
        /// 是否为Nuget引用
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static bool IsNugetInfoReference(XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            if (xElement.Name.LocalName != ReferenceName)
            {
                throw new InvalidOperationException($"传入的键不是 {ReferenceName}，详情：{xElement}");
            }

            if (xElement.Attribute(IncludeAttribute) == null)
            {
                return false;
            }

            var hintPathChildElements = xElement.Elements().Where(x => x.Name.LocalName == HintPathElementName);
            if (!hintPathChildElements.Any())
            {
                return false;
            }

            if (!hintPathChildElements.Any(x => x.Value.Contains(@"\packages\")))
            {
                return false;
            }

            var includeValue = xElement.Attribute(IncludeAttribute).Value;
            return IncludeValueRegex.IsMatch(includeValue);
        }
        /// <summary>
        /// 从Nuget引用节点获取Nuget信息
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public static NugetInfo GetNugetInfoFromNugetInfoReference(XElement xElement,
             string sourceFilePath = null)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            if (!IsNugetInfoReference(xElement))
            {
                throw new InvalidOperationException($"传入的键不含 Nuget 信息，详情：{xElement}");
            }

            var includeValue = xElement.Attribute(IncludeAttribute).Value;
            var nugetName = NugetNameRegex.Match(includeValue).Value;
            var nugetVersion = NugetVersionRegex.Match(includeValue).Value;
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                return new NugetInfo(nugetName, nugetVersion);
            }

            var dllPath = GetDllPath(xElement, Path.GetDirectoryName(sourceFilePath));
            var nugetDllInfo = new NugetDllInfo(dllPath, includeValue);
            return new NugetInfo(nugetName, nugetVersion, nugetDllInfo);
        }

        /// <summary>
        /// 获取DLL的绝对路径
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="csProjDirectory"></param>
        /// <returns></returns>
        private static string GetDllPath(XElement xElement, string csProjDirectory)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            if (csProjDirectory == null)
            {
                throw new ArgumentNullException(nameof(csProjDirectory));
            }

            if (!IsNugetInfoReference(xElement))
            {
                throw new InvalidOperationException($"传入的键不含 Nuget 信息，详情：{xElement}");
            }

            if (!Directory.Exists(csProjDirectory))
            {
                throw new DirectoryNotFoundException(csProjDirectory);
            }

            var dllRelativePath = xElement.Elements().First(x => x.Name.LocalName == HintPathElementName).Value;
            var dllAbsolutePath = Path.GetFullPath(Path.Combine(csProjDirectory, dllRelativePath));
            return dllAbsolutePath;
        }
        /// <summary>
        /// 从Nuget引用DLL地址，获取.NET版本
        /// </summary>
        /// <param name="dllFilePath"></param>
        /// <returns></returns>
        public static string GetTargetFrameworkOfDll(string dllFilePath)
        {
            var matchCollection = NugetTargetFrameworkRegex.Matches(dllFilePath);
            return matchCollection[matchCollection.Count - 1].Value;
        }

        #endregion

        #region CsProject属性

        public const string RootName = "Project";

        public const string ItemGroupName = "ItemGroup";

        public const string PackageReferenceName = "PackageReference";

        public const string ReferenceName = "Reference";

        public const string ProjectReferenceName = "ProjectReference";

        public const string IncludeAttribute = "Include";

        public const string UpdateAttribute = "Update";

        public const string VersionAttribute = "Version";

        public const string VersionElementName = VersionAttribute;

        public const string HintPathElementName = "HintPath";

        #endregion

        #region private fields

        private static readonly Regex IncludeValueRegex = new Regex(@".+,\s*Version=.+,\s*Culture=.+,\s*processorArchitecture=.+");

        private static readonly Regex NugetNameRegex = new Regex(@".+(?=,\s*Version)");

        private static readonly Regex NugetVersionRegex = new Regex(@"(?<=Version=).+(?=,\s*Culture)");

        private static readonly Regex NugetTargetFrameworkRegex = new Regex(@"(?<=lib\\).*(?=\\)");

        #endregion

    }
}