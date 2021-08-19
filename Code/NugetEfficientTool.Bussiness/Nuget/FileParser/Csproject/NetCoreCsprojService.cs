using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    class NetCoreCsprojService : CsProjFileBase, ICsProjFileService
    {
        /// <summary>
        /// 获取Reference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public List<XElement> GetReferences(XDocument xDocument)
        {
            return GetXElementsByNameInItemGroups(xDocument, PackageReferenceName);
        }
        public List<XElement> GetNugetReferences(XDocument xDocument)
        {
            var references = GetReferences(xDocument);
            var nugetReferences = references.Where(IsNugetReference).ToList();
            return nugetReferences;
        }
        public bool IsNugetReference(XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            if (xElement.Name.LocalName != PackageReferenceName)
            {
                return false;
            }

            var includeAttribute = xElement.Attribute(IncludeAttribute);
            if (includeAttribute == null)
            {
                return false;
            }
            return !string.IsNullOrWhiteSpace(includeAttribute.Value);
        }
        public NugetInfo GetNugetInfo(XElement xElement)
        {
            var nugetName = xElement.Attribute(IncludeAttribute).Value;
            var nugetVersion = xElement.Attribute(VersionAttribute).Value;
            return new NugetInfo(nugetName, nugetVersion);
        }
        public void RevertReference(XDocument document, ReplacedFileRecord replacedRecord)
        {
            var references = GetReferences(document);
            //添加package引用
            var referenceElement = new XElement(CsProjConst.PackageReferenceName);
            referenceElement.SetAttributeValue(CsProjConst.IncludeAttribute, $"{replacedRecord.NugetName}, Version={replacedRecord.Version}");
            references[replacedRecord.ModifiedLineIndex].AddBeforeSelf(referenceElement);
        }
    }
}
