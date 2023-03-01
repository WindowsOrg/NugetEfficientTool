using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    class NetFrameworkCsprojService : CsProjFileBase, ICsProjFileService
    {
        /// <summary>
        /// 获取Reference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public List<XElement> GetReferences(XDocument xDocument)
        {
            var xElements = new List<XElement>();
            xElements.AddRange(GetXElementsByNameInItemGroups(xDocument, ReferenceName));
            xElements.AddRange(GetXElementsByNameInItemGroups(xDocument, PackageReferenceName));
            return xElements;
        }
        /// <summary>
        /// 获取所有Nuget-Reference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public List<XElement> GetNugetReferences(XDocument xDocument)
        {
            var references = GetReferences(xDocument);
            var nugetReferences = references.Where(IsNugetReference).ToList();
            return nugetReferences;
        }
        /// <summary>
        /// 是否为Nuget引用
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public bool IsNugetReference(XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            if (xElement.Name.LocalName != ReferenceName && xElement.Name.LocalName != PackageReferenceName)
            {
                return false;
            }

            if (xElement.Attribute(IncludeAttribute) == null)
            {
                return false;
            }

            var includeValue = xElement.Attribute(IncludeAttribute)?.Value;
            if (string.IsNullOrEmpty(includeValue))
            {
                return false;
            }

            var versionElements = xElement.Elements().Where(x => x.Name.LocalName == VersionElementName).ToList();
            if (versionElements.Any())
            {
                return true;
            }

            var hintPathChildElements = xElement.Elements().Where(x => x.Name.LocalName == HintPathElementName).ToList();

            if (!hintPathChildElements.Any(x => x.Value.Contains(@"\packages\")))
            {
                return false;
            }

            return IncludeValueRegex.IsMatch(includeValue);
        }
        public NugetInfo GetNugetInfo(XElement xElement)
        {
            var includeValue = xElement.Attribute(IncludeAttribute).Value;
            string nugetVersion;
            //PackageReference
            if (PackageReferenceName.Equals(xElement.Name.LocalName))
            {
                var versionElements = xElement.Elements().Where(x => x.Name.LocalName == VersionElementName).ToList();
                if (versionElements.Count != 0)
                {
                    nugetVersion = versionElements.First().Value;
                    return new NugetInfo(includeValue, nugetVersion);
                }
                //PackageReference的Version,可能是以属性形式存在
                var versionAttribute = xElement.Attributes(VersionElementName).FirstOrDefault();
                if (versionAttribute != null)
                {
                    return new NugetInfo(includeValue, versionAttribute.Value);
                }
            }
            //Reference
            var nugetName = NugetNameRegex.Match(includeValue).Value;
            nugetVersion = NugetVersionRegex.Match(includeValue).Value;
            //如果内部有HintPath，则获取DLL路径信息中的Nuget名称和版本号。HintPath下的信息才是准确的
            if (xElement.Elements().FirstOrDefault(x => x.Name.LocalName == CsProjConst.HintPathElementName) is XElement hintPathElement)
            {
                var nugetInfo = HintPathElements.GetNugetInfo(hintPathElement);
                if (nugetInfo != null)
                {
                    nugetName = nugetInfo.Name;
                    nugetVersion = nugetInfo.Version;
                }
            }

            return new NugetInfo(nugetName, nugetVersion);
        }
        public void RevertReference(XDocument document, ReplacedFileRecord replacedRecord)
        {
            var references = GetNugetReferences(document);
            //添加package引用
            var referenceElement = new XElement(replacedRecord.ReferenceType);
            var version = replacedRecord.Version;
            if (replacedRecord.ReferenceType == PackageReferenceName)
            {
                referenceElement.SetAttributeValue(CsProjConst.IncludeAttribute, $"{replacedRecord.NugetName}");
                var versionElement = new XElement(CsProjConst.VersionElementName);
                versionElement.SetValue(replacedRecord.Version);
                referenceElement.Add(versionElement);
            }
            else
            {
                //补充.0
                if (version.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                {
                    version = $"{version}.0";
                }
                referenceElement.SetAttributeValue(CsProjConst.IncludeAttribute, $"{replacedRecord.NugetName}, Version={version}, Culture=neutral, processorArchitecture=MSIL");
                var hintPathElement = new XElement(CsProjConst.HintPathElementName);
                hintPathElement.SetValue(replacedRecord.NugetDllPath);
                referenceElement.Add(hintPathElement);
            }
            references[replacedRecord.ModifiedLineIndex].AddBeforeSelf(referenceElement);
        }

        private static readonly Regex NugetNameRegex = new Regex(@".+(?=,\s*Version)");

        private static readonly Regex NugetVersionRegex = new Regex(@"(?<=Version=).+(?=,\s*Culture)");
    }
}
