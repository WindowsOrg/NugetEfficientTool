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

        /// <summary>
        /// 修复PackageReference
        /// </summary>
        /// <param name="packageReferences"></param>
        /// <param name="nugetFixStrategy"></param>
        private void FixPackageReferences(IEnumerable<XElement> packageReferences, NugetFixStrategy nugetFixStrategy)
        {
            var packageReferenceList = packageReferences.ToList();
            for (var i = 0; i < packageReferenceList.Count; i++)
            {
                if (i != 0)
                {
                    packageReferenceList[i].Remove();
                    continue;
                }

                var removeAttributeList = new[] { "Private", "HintPath" };
                //删除元素
                var firstPackageReference = packageReferenceList[i];
                if (firstPackageReference.Elements().Any())
                {
                    foreach (var attribute in removeAttributeList)
                    {
                        firstPackageReference.Element(attribute)?.Remove();
                    }
                }
                //删除属性
                foreach (var attribute in removeAttributeList)
                {
                    // 设置为 null 将删除属性
                    firstPackageReference.SetAttributeValue(attribute, null);
                }

                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
                firstPackageReference.SetAttributeValue(CsProjConst.IncludeAttribute, nugetFixStrategy.NugetName);
                var versionElement = firstPackageReference.Elements().FirstOrDefault(element => element.Name.LocalName == CsProjConst.VersionElementName);
                if (versionElement != null)
                {
                    versionElement.SetValue(nugetFixStrategy.NugetVersion);
                }
                else
                {
                    firstPackageReference.SetAttributeValue(CsProjConst.VersionAttribute, nugetFixStrategy.NugetVersion);
                }
            }
        }

        private void FixNugetInfoReferences(IEnumerable<XElement> nugetInfoReferences, NugetFixStrategy nugetFixStrategy)
        {
            var nugetInfoReferenceList = nugetInfoReferences.ToList();
            for (var i = 0; i < nugetInfoReferenceList.Count; i++)
            {
                if (i != 0)
                {
                    nugetInfoReferenceList[i].Remove();
                    continue;
                }

                var currentNugetReference = nugetInfoReferenceList[i];
                if (nugetFixStrategy.NugetDllInfo == null)
                {
                    continue;
                }
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
                ReplaceReferenceToPackageReference(currentNugetReference, nugetFixStrategy.NugetName, nugetFixStrategy.NugetVersion);
                //if (string.IsNullOrEmpty(nugetFixStrategy.NugetDllInfo.DllPath))
                //{
                //var xElement = new XElement(CsProjConst.PackageReferenceName);
                //xElement.SetAttributeValue(CsProjConst.IncludeAttribute, nugetFixStrategy.NugetName);
                //xElement.SetAttributeValue(CsProjConst.VersionAttribute, nugetFixStrategy.NugetVersion);
                //if (currentNugetReference.NextNode is XElement nextElement)
                //{
                //    currentNugetReference.Remove();
                //    nextElement.AddBeforeSelf(xElement);
                //}
                //else if (currentNugetReference.PreviousNode is XElement previousElement)
                //{
                //    currentNugetReference.Remove();
                //    previousElement.AddAfterSelf(xElement);
                //}
                //else if (currentNugetReference.Parent is XElement parentElement)
                //{
                //    currentNugetReference.Remove();
                //    parentElement.AddFirst(xElement);
                //}
                //else
                //{
                //    throw new InvalidOperationException($"{currentNugetReference}未能完成替换到目标{nugetFixStrategy.NugetName}-{nugetFixStrategy.NugetVersion}");
                //}
                //}
                //else
                //{
                //    currentNugetReference.SetAttributeValue(CsProjConst.IncludeAttribute,
                //        nugetFixStrategy.NugetDllInfo.DllFullName);
                //    var hintPathElementList = currentNugetReference.Elements()
                //        .Where(x => x.Name.LocalName == CsProjConst.HintPathElementName).ToList();
                //    for (var j = 0; j < hintPathElementList.Count; j++)
                //    {
                //        if (j != 0)
                //        {
                //            hintPathElementList[j].Remove();
                //            continue;
                //        }
                //        hintPathElementList[j].Value = MakeRelativePath(Path.GetDirectoryName(_csProjPath),
                //            nugetFixStrategy.NugetDllInfo.DllPath);
                //    }
                //    if (hintPathElementList.Count > 1)
                //    {
                //        Log = StringSplicer.SpliceWithNewLine(Log, $"    - 删除了 {nugetFixStrategy.NugetName} 的 {hintPathElementList.Count - 1} 个子健冲突");
                //    }
                //}
            }
        }

        private void ReplaceReferenceToPackageReference(XElement reference, string nugetName, string nugetVersion)
        {
            var xElement = new XElement(CsProjConst.PackageReferenceName);
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
        }

        public override void UpgradeNugetReference()
        {
            var nugetInfoReferences = CsProj.GetReferences(Document).ToList();
            foreach (var nugetReference in nugetInfoReferences)
            {
                var isReferenceNuget = nugetReference.Elements().Any(elem => elem.Name.LocalName == CsProjConst.HintPathElementName);
                if (!isReferenceNuget)
                {
                    continue;
                }
                var nugetInfo = CsProj.GetNugetInfo(nugetReference);
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetInfo.Name} 改为 PackageReference");
                ReplaceReferenceToPackageReference(nugetReference, nugetInfo.Name, nugetInfo.Version);
            }
        }

        protected override bool FixDocumentByStrategy(NugetFixStrategy nugetFixStrategy)
        {
            //以PackageReference为主
            var packageReferences = CsProj.GetPackageReferences(Document).Where(x =>
            {
                var nugetInfo = CsProj.GetNugetInfo(x);
                return nugetInfo.Name == nugetFixStrategy.NugetName &&
                       nugetInfo.Version != nugetFixStrategy.NugetVersion;
            }).ToList();
            var nugetInfoReferences = CsProj.GetReferences(Document).Where(x =>
            {
                var nugetInfo = CsProj.GetNugetInfo(x);
                return nugetInfo.Name == nugetFixStrategy.NugetName &&
                       nugetInfo.Version != nugetFixStrategy.NugetVersion;
            }).ToList();
            if (!packageReferences.Any() && !nugetInfoReferences.Any())
            {
                return false;
            }

            if (nugetFixStrategy.NugetVersion == NugetVersion.IgnoreFix)
            {
                return true;
            }

            if (packageReferences.Any())
            {
                FixPackageReferences(packageReferences, nugetFixStrategy);
                nugetInfoReferences.RemoveAll(i => packageReferences.Any(package => package != i));
            }
            else
            {
                FixNugetInfoReferences(nugetInfoReferences, nugetFixStrategy);
            }

            return true;
        }
    }
}