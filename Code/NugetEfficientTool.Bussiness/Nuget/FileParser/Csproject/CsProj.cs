using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// CsProject数据辅助类
    /// </summary>
    public class CsProj
    {
        public static void SetXDocument(XDocument xDocument)
        {
           CsProjFileService.XDocument = xDocument;
        }

        #region 获取节点

        /// <summary>
        /// 获取Reference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetReferences(XDocument xDocument)
        {
            return CsProjService.GetReferences(xDocument);
        }
        /// <summary>
        /// 获取所有Nuget-Reference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetNugetInfoReferences(XDocument xDocument)
        {
            return CsProjService.GetNugetReferences(xDocument);
        }
        /// <summary>
        /// 获取ProjectReference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static List<XElement> GetProjectReferences(XDocument xDocument)
        {
            return CsProjService.GetProjectReferences(xDocument);
        }

        #endregion

        #region Nuget相关

        /// <summary>
        /// 是否为Nuget引用
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static bool IsNugetReference(XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            return CsProjService.IsNugetReference(xElement);
        }

        /// <summary>
        /// 从Nuget引用节点获取Nuget信息
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public static NugetInfo GetNugetInfo(XElement xElement, string sourceFilePath = null)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            var nugetInfo = CsProjService.GetNugetInfo(xElement);
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                return nugetInfo;
            }
            var includeAttribute = xElement.Attribute(CsProjConst.IncludeAttribute);
            if (includeAttribute == null)
            {
                return nugetInfo;
            }
            var dllPath = GetDllPath(xElement, Path.GetDirectoryName(sourceFilePath));
            var nugetDllInfo = new NugetDllInfo(dllPath, includeAttribute.Value);
            nugetInfo.NugetDllInfo = nugetDllInfo;
            return nugetInfo;
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
            if (string.IsNullOrWhiteSpace(csProjDirectory) || !Directory.Exists(csProjDirectory))
            {
                return string.Empty;
            }

            var hintPathElement = xElement.Elements().FirstOrDefault(x => x.Name.LocalName == CsProjConst.HintPathElementName);
            if (hintPathElement == null)
            {
                return string.Empty;
            }
            var dllRelativePath = hintPathElement.Value;
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
        public static void RevertReference(XDocument document, ReplacedFileRecord replacedRecord)
        {
            CsProjService.RevertReference(document, replacedRecord);
        }

        #endregion

        #region private fields

        private static readonly ICsProjFileService CsProjService = new CsProjFileService();

        private static readonly Regex NugetTargetFrameworkRegex = new Regex(@"(?<=lib\\).*(?=\\)");

        #endregion
    }
}