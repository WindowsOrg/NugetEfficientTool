using System.Xml.Linq;

namespace Kybs0.Csproj.Analyzer
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
    }
}
