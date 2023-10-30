using System.Xml.Linq;

namespace Kybs0.Project
{
    internal class CsProjFileService : ICsProjFileService
    {
        public List<XElement> GetReferences(XDocument xDocument)
        {
            return GetCsProjService(xDocument).GetReferences(xDocument);
        }

        public List<XElement> GetProjectReferences(XDocument xDocument)
        {
            return GetCsProjService(xDocument).GetProjectReferences(xDocument);
        }

        public List<XElement> GetPackageReferences(XDocument xDocument)
        {
            return GetCsProjService(xDocument).GetPackageReferences(xDocument);
        }

        /// <summary>
        /// 获取所有Nuget-Reference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public List<XElement> GetNugetReferences(XDocument xDocument)
        {
            return GetCsProjService(xDocument).GetNugetReferences(xDocument);
        }
        public bool IsNugetReference(XElement xElement)
        {
            return GetCsProjService(xElement.Document).IsNugetReference(xElement);
        }
        public NugetInfo GetNugetInfo(XElement xElement)
        {
            return GetCsProjService(xElement.Document).GetNugetInfo(xElement);
        }

        #region 引用类型

        private static readonly ICsProjFileService NetCoreCsProj = new NetCoreCsprojService();
        private static readonly ICsProjFileService NetFrameworkCsProj = new NetFrameworkCsprojService();

        private ICsProjFileService GetCsProjService(XDocument xDocument)
        {
            var csprojFileType = GetCsprojFileType(xDocument);
            switch (csprojFileType)
            {
                case CsprojFileType.NetFramework:
                    {
                        return NetFrameworkCsProj;
                    }
                case CsprojFileType.NetCore:
                    {
                        return NetCoreCsProj;
                    }
            }
            return null;
        }
        public CsprojFileType GetCsprojFileType(XDocument xDocument)
        {
            if (xDocument == null)
            {
                throw new ArgumentNullException(nameof(xDocument));
            }
            var rootElement = xDocument.Root;
            if (rootElement == null)
            {
                throw new ArgumentNullException(nameof(xDocument.Root));
            }
            var xElementName = rootElement.Name.LocalName;
            if (xElementName != CsProjConst.RootName)
            {
                throw new InvalidOperationException("顶级Root，不是Project类型！");
            }
            var xAttribute = rootElement.Attribute(CsProjConst.SdkAttribute);
            if (xAttribute != null && xAttribute.Value == CsProjConst.SdkValue)
            {
                return CsprojFileType.NetCore;
            }
            return CsprojFileType.NetFramework;
        }

        #endregion
    }
}
