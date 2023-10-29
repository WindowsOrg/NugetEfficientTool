using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Kybs0.Csproj.Analyzer;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// 项目文件Nuget替换类
    /// </summary>
    public class CsProjNugetReplacer : XmlFileNugetReplacer
    {
        private readonly string _nugetName;
        private readonly string _newProjectId;
        private readonly ReplacedFileRecord _lastReplacedRecord;
        private readonly string _sourceProjectFile;

        public CsProjNugetReplacer(string projectFile, string nugetName, string newProjectId, string sourceProjectFile) : base(projectFile)
        {
            _newProjectId = newProjectId;
            _nugetName = nugetName;
            _sourceProjectFile = sourceProjectFile;
        }

        public CsProjNugetReplacer(string projectFile, ReplacedFileRecord lastReplacedRecord, string sourceProjectFile) : base(projectFile)
        {
            _lastReplacedRecord = lastReplacedRecord;
            _sourceProjectFile = sourceProjectFile;
        }

        #region 替换Nuget

        /// <summary>
        /// 替换Nuget
        /// </summary>
        /// <returns></returns>
        public ReplacedFileRecord ReplaceNuget()
        {
            var nugetInfoReferences = CsProj.GetNugetReferences(Document).ToList();
            var referenceElement = nugetInfoReferences.FirstOrDefault(x => CsProj.GetNugetInfo(x).Name == _nugetName);
            if (referenceElement == null)
            {
                return null;
            }
            //获取Nuget引用信息
            var replacedFileRecord = GetNugetReferenceInfo(referenceElement, nugetInfoReferences.IndexOf(referenceElement));
            //删除Nuget的引用
            referenceElement.Remove();
            //添加源项目的引用
            AddSourceReference();
            SaveFile();
            return replacedFileRecord;
        }

        private ReplacedFileRecord GetNugetReferenceInfo(XElement referenceElement, int referenceLineOrder)
        {
            var nugetInfo = CsProj.GetNugetInfo(referenceElement);
            var version = nugetInfo.Version;
            var replacedFileRecord = new ReplacedFileRecord()
            {
                NugetName = _nugetName,
                FileName = File,
                ModifiedLineIndex = referenceLineOrder,
                Version = version,
                NugetDllPath = referenceElement.Value,
                ReferenceType = referenceElement.Name.LocalName
            };

            return replacedFileRecord;
        }

        private void AddSourceReference()
        {
            var xElement = new XElement("ProjectReference");
            xElement.SetAttributeValue("Include", _sourceProjectFile);
            xElement.Add(new XElement("Project", $"{{{_newProjectId}}}"));
            xElement.Add(new XElement("Name", _nugetName));
            var projectReferences = CsProj.GetProjectReferences(Document).ToList();
            if (projectReferences.Any())
            {
                projectReferences[0].AddBeforeSelf(xElement);
            }
            else
            {
                var documentRoot = Document.Root;
                if (documentRoot == null)
                {
                    throw new InvalidOperationException($"document.Root是空的,{XmlFile}");
                }
                var itemGroup = documentRoot.Elements().Where(i => i.Name.LocalName == CsProjConst.ItemGroupName).ToList()[0];
                itemGroup.Add(xElement);
            }
        }

        #endregion

        #region 还原Nuget

        public void RevertNuget()
        {
            RevertReference(Document, _lastReplacedRecord);
            //删除源代码引用
            var projectReferences = CsProj.GetProjectReferences(Document);
            var sourceProjectReferences = projectReferences.Where(i => i.Attribute(CsProjConst.IncludeAttribute).Value.Contains(_sourceProjectFile)).ToList();
            foreach (var sourceProjectReference in sourceProjectReferences)
            {
                sourceProjectReference.Remove();
            }
            SaveFile();
        }
        /// <summary>
        /// 替换Nuget引用
        /// </summary>
        /// <param name="document"></param>
        /// <param name="lastReplacedRecord"></param>
        private void RevertReference(XDocument document, ReplacedFileRecord lastReplacedRecord)
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
        private void RevertNetFrameworkReference(XDocument document, ReplacedFileRecord replacedRecord)
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
        private void RevertNetCoreReference(XDocument document, ReplacedFileRecord replacedRecord)
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

        #endregion

    }
}
