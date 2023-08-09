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
            return GetXElementsByNameInItemGroups(xDocument, CsProjConst.PackageReferenceName);
        }

        public List<XElement> GetPackageReferences(XDocument xDocument)
        {
            return GetReferences(xDocument);
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

            if (xElement.Name.LocalName != CsProjConst.PackageReferenceName)
            {
                return false;
            }

            var includeAttribute = xElement.Attribute(CsProjConst.IncludeAttribute);
            if (includeAttribute == null)
            {
                return false;
            }
            return !string.IsNullOrWhiteSpace(includeAttribute.Value);
        }
        public NugetInfo GetNugetInfo(XElement xElement)
        {
            var nugetName = xElement.Attribute(CsProjConst.IncludeAttribute).Value;
            var versionElements = xElement.Elements().Where(x => x.Name.LocalName == CsProjConst.VersionElementName).ToList();
            if (versionElements.Count != 0)
            {
                var nugetVersion = versionElements.First().Value;
                return new NugetInfo(nugetName, nugetVersion);
            }
            //PackageReference的Version,可能是以属性形式存在
            var versionAttribute = xElement.Attributes(CsProjConst.VersionElementName).FirstOrDefault();
            if (versionAttribute != null)
            {
                return new NugetInfo(nugetName, versionAttribute.Value);
            }
            return new NugetInfo(nugetName, string.Empty);
        }
        public void RevertReference(XDocument document, ReplacedFileRecord replacedRecord)
        {
            var references = GetReferences(document);
            //添加package引用
            var referenceElement = new XElement(CsProjConst.PackageReferenceName);
            referenceElement.SetAttributeValue(CsProjConst.IncludeAttribute, replacedRecord.NugetName);
            referenceElement.SetAttributeValue(CsProjConst.VersionAttribute, replacedRecord.Version);

            //在之前位置插入Reference引用
            if (replacedRecord.ModifiedLineIndex >= references.Count)
            {
                references[references.Count - 1].AddAfterSelf(referenceElement);
            }
            else
            {
                references[replacedRecord.ModifiedLineIndex].AddBeforeSelf(referenceElement);
            }
        }
    }
}
