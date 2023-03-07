using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// CsProj文件版本修复器
    /// </summary>
    public class CsProjReferenceFixer : NugetReferenceFixerBase
    {
        public CsProjReferenceFixer(XDocument xDocument, string csProjPath, IEnumerable<NugetFixStrategy> nugetFixStrategies)
            : base(xDocument, nugetFixStrategies)
        {
            _csProjPath = csProjPath;
        }

        private readonly string _csProjPath;

        protected override bool FixDocumentByStrategy(NugetFixStrategy nugetFixStrategy)
        {
            //以PackageReference为主
            var references = CsProj.GetReferences(Document).Where(x =>
            {
                var nugetInfo = CsProj.GetNugetInfo(x);
                return nugetInfo.Name == nugetFixStrategy.NugetName &&
                       nugetInfo.Version != nugetFixStrategy.NugetVersion;
            }).ToList();
            if (!references.Any())
            {
                return false;
            }
            //忽略修复
            if (nugetFixStrategy.NugetVersion == NugetVersion.IgnoreFix)
            {
                return true;
            }

            for (int i = 0; i < references.Count; i++)
            {
                var reference = references[i];
                if (i != 0)
                {
                    reference.Remove();
                    continue;
                }
                if (reference.Name.LocalName == CsProjConst.PackageReferenceName)
                {
                    FixPackageReference(reference, nugetFixStrategy);
                }
                else
                {
                    FixNugetReference(reference, nugetFixStrategy);
                }
            }
            return true;
        }

        /// <summary>
        /// 修复PackageReference
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="nugetFixStrategy"></param>
        private void FixPackageReference(XElement reference, NugetFixStrategy nugetFixStrategy)
        {
            var removeAttributeList = new[] { "Private", "HintPath" };
            //删除元素
            if (reference.Elements().Any())
            {
                foreach (var attribute in removeAttributeList)
                {
                    reference.Element(attribute)?.Remove();
                }
            }
            //删除属性
            foreach (var attribute in removeAttributeList)
            {
                // 设置为 null 将删除属性
                reference.SetAttributeValue(attribute, null);
            }

            Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
            reference.SetAttributeValue(CsProjConst.IncludeAttribute, nugetFixStrategy.NugetName);
            var versionElement = reference.Elements().FirstOrDefault(element => element.Name.LocalName == CsProjConst.VersionElementName);
            if (versionElement != null)
            {
                versionElement.SetValue(nugetFixStrategy.NugetVersion);
            }
            else
            {
                reference.SetAttributeValue(CsProjConst.VersionAttribute, nugetFixStrategy.NugetVersion);
            }
        }

        private void FixNugetReference(XElement reference, NugetFixStrategy nugetFixStrategy)
        {
            if (nugetFixStrategy.NugetDllInfo == null)
            {
                return;
            }
            Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
            ReplaceReferenceToPackageReference(reference, nugetFixStrategy.NugetName, nugetFixStrategy.NugetVersion);
        }
        /// <summary>
        /// 升级Nuget引用
        /// </summary>
        public override void UpgradeNugetReference()
        {
            var nugetInfoReferences = CsProj.GetReferences(Document).ToList();
            foreach (var nugetReference in nugetInfoReferences)
            {
                var hintPathElement = nugetReference.Elements().FirstOrDefault(elem => elem.Name.LocalName == CsProjConst.HintPathElementName);
                if (hintPathElement == null)
                {
                    continue;
                }
                //规避本地引用dll
                if (!hintPathElement.Value.Contains(@"\packages\") || !hintPathElement.Value.Contains(@"\lib\"))
                {
                    continue;
                }
                var nugetInfo = CsProj.GetNugetInfo(nugetReference);
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetInfo.Name} 改为 PackageReference");
                ReplaceReferenceToPackageReference(nugetReference, nugetInfo.Name, nugetInfo.Version);
            }
        }
        /// <summary>
        /// 将Reference替换为PackageReference
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="nugetName"></param>
        /// <param name="nugetVersion"></param>
        private void ReplaceReferenceToPackageReference(XElement reference, string nugetName, string nugetVersion)
        {
            //创建元素时，添加xmlns，用于修复xmlns=""空值的问题
            var xmlns = CsProj.GetXmlns(Document);
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

    }
}