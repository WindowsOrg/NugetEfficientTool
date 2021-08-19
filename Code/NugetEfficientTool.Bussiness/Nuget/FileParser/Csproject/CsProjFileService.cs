using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    class CsProjFileService : CsProjFileBase, ICsProjFileService
    {
        /// <summary>
        /// 获取Reference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public List<XElement> GetReferences(XDocument xDocument)
        {
            return GetCsProjService(xDocument).GetReferences(xDocument);
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
            return GetCsProjService(xElement).IsNugetReference(xElement);
        }
        public NugetInfo GetNugetInfo(XElement xElement)
        {
            return GetCsProjService(xElement).GetNugetInfo(xElement);
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
                        return NetCoreCsProj;
                    }
                case CsprojFileType.NetCore:
                    {
                        return NetFrameworkCsProj;
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
        /// <summary>
        /// todo 临时处理
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        private static ICsProjFileService GetCsProjService(XElement xElement)
        {
            var csprojFileType = GetCsprojFileType(xElement);
            switch (csprojFileType)
            {
                case CsprojFileType.NetFramework:
                    {
                        return NetCoreCsProj;
                    }
                case CsprojFileType.NetCore:
                    {
                        return NetFrameworkCsProj;
                    }
            }
            return null;
        }
        private static CsprojFileType GetCsprojFileType(XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }
            var xElementName = xElement.Name.LocalName;
            if (xElementName == ReferenceName)
            {
                return CsprojFileType.NetFramework;
            }
            if (xElementName == PackageReferenceName)
            {
                return CsprojFileType.NetCore;
            }
            throw new InvalidOperationException($"未知Csproj引用类型：{xElementName}");
        }

        #endregion
    }
}
