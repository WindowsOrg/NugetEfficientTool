using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NugetEfficientTool.Nuget
{
    /// <summary>
    /// CsProject数据辅助类
    /// </summary>
    public static class CsProj
    {
        /// <summary>
        /// 获取Csproj文件平台类型
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static CsprojFileType GetCsprojFileType(XDocument xDocument)
        {
            var csprojFileType = ((CsProjFileService)CsProjService).GetCsprojFileType(xDocument);
            return csprojFileType;
        }

        #region 获取节点
        /// <summary>
        /// 获取所有Nuget-Reference节点
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
        public static IEnumerable<XElement> GetNugetReferences(XDocument xDocument)
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

        /// <summary>
        /// 获取Xmlns值
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static XNamespace GetXmlns(XDocument document)
        {
            var rootElement = document.Root;
            var xmlnsAttribute = rootElement?.Attribute(CsProjConst.XmlnsAttribute);
            //"http://schemas.microsoft.com/developer/msbuild/2003"
            XNamespace xmlns = xmlnsAttribute?.Value;
            return xmlns;
        }

        public static List<XElement> GetItemGroups(XDocument xDocument)
        {
            var rootElement = xDocument.Root;
            var itemGroupElements = rootElement?.Elements().Where(x => x.Name.LocalName == CsProjConst.ItemGroupName).ToList();
            return itemGroupElements ?? new List<XElement>();
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
            if (matchCollection.Count == 0)
            {
                return string.Empty;
            }
            return matchCollection[matchCollection.Count - 1].Value;
        }

        /// <summary>
        /// 获取所有Nuget信息
        /// </summary>
        /// <param name="xDocument"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<NugetInfo> GetNugetInfos(XDocument xDocument, string filePath)
        {
            var csProjFileParser = new CsProjFileParser(xDocument, filePath);
            var nugetInfos = csProjFileParser.GetNugetInfos();
            //对集成打包的Nuget，合并
            var results = new List<NugetInfo>();
            foreach (var nugetInfo in nugetInfos)
            {
                if (results.Any(i => i.Name == nugetInfo.Name))
                {
                    continue;
                }
                results.Add(nugetInfo);
            }
            return results;
        }

        #endregion

        #region 组件项目相关

        /// <summary>
        /// 判断是否组件类型的项目
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static bool IsComponent(XDocument xDocument)
        {
            var rootElement = xDocument.Root;
            var sdkAttribute = rootElement?.Attribute("Sdk");
            return sdkAttribute != null;
        }

        public static string GetComponentVersion(XDocument xDocument)
        {
            var rootElement = xDocument.Root;
            var propertyGroups = rootElement?.Elements("PropertyGroup").ToList();
            var componentVersionElement = propertyGroups?.SelectMany(i => i.Elements("Version")).FirstOrDefault();
            var componentVersion = componentVersionElement?.Value;
            return componentVersion;
        }
        private static readonly Regex NumberVersionRegex = new Regex(@"[0-9]");

        #endregion

        #region private fields

        private static readonly ICsProjFileService CsProjService = new CsProjFileService();

        private static readonly Regex NugetTargetFrameworkRegex = new Regex(@"(?<=lib\\).*(?=\\)");

        #endregion

    }
}