using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Kybs0.Csproj.Analyzer;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget替换记录恢复操作
    /// </summary>
    internal static class NugetReplacedRevert
    {
        /// <summary>
        /// 替换Nuget引用
        /// </summary>
        /// <param name="document"></param>
        /// <param name="lastReplacedRecord"></param>
        public static void RevertReference(XDocument document, ReplacedFileRecord lastReplacedRecord)
        {
            var csprojFileType = CsProj.GetCsprojFileType(document);
            switch (csprojFileType)
            {
                case CsprojFileType.NetCore:
                    {
                        RevertNetCoreReference(document, lastReplacedRecord);
                    }
                    break;
                case CsprojFileType.NetFramework:
                    {
                        RevertNetFrameworkReference(document, lastReplacedRecord);
                    }
                    break;
            }
        }
        private static void RevertNetFrameworkReference(XDocument document, ReplacedFileRecord replacedRecord)
        {
            var references = CsProj.GetNugetReferences(document).ToList();
            //添加package引用
            var referenceElement = new XElement(replacedRecord.ReferenceType);
            var version = replacedRecord.Version;
            if (replacedRecord.ReferenceType == CsProjConst.PackageReferenceName)
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
            //因NugetReference列表不存在，处理边界情况
            if (references.Count == 0)
            {
                //找到一个空的ItemGroup
                var itemGroups = CsProj.GetItemGroups(document);
                var emptyItemGroup = itemGroups.FirstOrDefault(i => !i.HasElements);
                if (emptyItemGroup != null)
                {
                    emptyItemGroup.Add(referenceElement);
                    return;
                }
                //找Reference列表
                var referenceElements = CsProj.GetReferences(document).ToList();
                if (referenceElements.Count > 0)
                {
                    referenceElements[referenceElements.Count - 1].AddAfterSelf(referenceElement);
                    return;
                }
                //直接在Project内插入ItemGroup
                var itemGroup = new XElement("ItemGroup");
                itemGroup.Add(referenceElement);
                document.Root?.Add(itemGroup);
                return;
            }
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
        private static void RevertNetCoreReference(XDocument document, ReplacedFileRecord replacedRecord)
        {
            var references = CsProj.GetReferences(document).ToList();
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
