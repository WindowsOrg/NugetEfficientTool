using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// CsProj文件Nuget引用方式
    /// </summary>
    public class CsProjReferenceWayUpdater
    {
        private readonly string _csProjFile;
        private readonly XDocument _xDocument;

        public CsProjReferenceWayUpdater(string csProjFile)
        {
            _csProjFile = csProjFile;
            _xDocument = new XmlReader(csProjFile).Document;
        }
        /// <summary>
        /// 升级
        /// </summary>
        /// <returns></returns>
        public bool TryUpgrade()
        {
            var xDocument = _xDocument;
            var references = CsProj.GetNugetReferences(xDocument).ToList();
            if (!references.Any())
            {
                return false;
            }

            foreach (var reference in references)
            {
                if (!CanUpgrade(reference)) continue;
                var nugetInfo = CsProj.GetNugetInfo(reference);
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetInfo.Name} 改为 PackageReference");
                UpgradeToPackageReference(reference, nugetInfo.Name, nugetInfo.Version);
            }
            //删除重复节点
            var newInfoReferences = CsProj.GetNugetReferences(xDocument).ToList();
            var elements = new List<XElement>();
            foreach (var referenceItem in newInfoReferences)
            {
                if (elements.Any(i => i.ToString() == referenceItem.ToString()))
                {
                    //删除重复的节点，解决转PackageReference后导致的重复引用
                    referenceItem.Remove();
                    continue;
                }
                elements.Add(referenceItem);
            }
            //删除package.config
            var nonePackagesFileElement = _xDocument.Root?.Elements().Where(i=>i.Name.LocalName=="ItemGroup").
                SelectMany(i=>i.Elements().Where(i=>i.Name.LocalName=="None"))
                .Where(i => i.HasAttributes && i.Attribute("Include")?.Value == "packages.config")?.FirstOrDefault();
            if (nonePackagesFileElement!=null)
            {
                nonePackagesFileElement.Remove();
            }
            _xDocument.Save(_csProjFile);
            return true;
        }
        /// <summary>
        /// 是否可以升级
        /// </summary>
        /// <returns></returns>
        public bool CanUpgrade()
        {
            var xDocument = _xDocument;
            var references = CsProj.GetNugetReferences(xDocument).ToList();
            if (!references.Any())
            {
                return false;
            }

            return references.Exists(CanUpgrade);
        }
        private bool CanUpgrade(XElement reference)
        {
            var hintPathElement = reference.Elements()
                .FirstOrDefault(elem => elem.Name.LocalName == CsProjConst.HintPathElementName);
            if (hintPathElement == null)
            {
                return false;
            }

            //规避本地引用dll
            if (!hintPathElement.Value.Contains(CsProjConst.HintPathPackagePiece) ||
                !hintPathElement.Value.Contains(CsProjConst.HintPathLibPiece))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 将Reference替换为PackageReference
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="nugetName"></param>
        /// <param name="nugetVersion"></param>
        private void UpgradeToPackageReference(XElement reference, string nugetName, string nugetVersion)
        {
            //创建元素时，添加xmlns，用于修复xmlns=""空值的问题
            var xmlns = CsProj.GetXmlns(_xDocument);
            var xElement = new XElement(xmlns + CsProjConst.PackageReferenceName);
            xElement.SetAttributeValue(CsProjConst.IncludeAttribute, nugetName);
            xElement.SetAttributeValue(CsProjConst.VersionAttribute, nugetVersion);
            if (reference.NextNode is XElement nextElement)
            {
                reference.Remove();
                nextElement.AddBeforeSelf(xElement);
            }
            else if (reference.PreviousNode is XElement previousElement)
            {
                reference.Remove();
                previousElement.AddAfterSelf(xElement);
            }
            else if (reference.Parent is XElement parentElement)
            {
                reference.Remove();
                parentElement.AddFirst(xElement);
            }
            else
            {
                throw new InvalidOperationException($"{reference}未能完成替换到目标{nugetName}-{nugetVersion}");
            }
            xElement.SetAttributeValue(CsProjConst.XmlnsAttribute, null);
        }
        public string Log { get; set; }
    }
}
