using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    class CsProjFileService : ICsProjFileService
    {
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

        public void RevertReference(XDocument document, ReplacedFileRecord replacedRecord)
        {
            GetCsProjService(document).RevertReference(document, replacedRecord);
        }

        #region 引用类型

        private static readonly ICsProjFileService NetCoreCsProj = new NetCoreCsprojService();
        private static readonly ICsProjFileService NetFrameworkCsProj = new NetFrameworkCsprojService();

        private static ICsProjFileService GetCsProjService(XDocument xDocument)
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
        private static CsprojFileType GetCsprojFileType(XDocument xDocument)
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
