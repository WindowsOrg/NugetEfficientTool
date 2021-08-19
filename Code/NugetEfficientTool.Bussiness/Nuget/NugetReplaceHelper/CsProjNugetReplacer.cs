using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// 项目文件Nuget替换类
    /// </summary>
    public class CsProjNugetReplacer : XmlFileNugetReplacer, INugetFileReplacer
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
            var references = CsProj.GetReferences(Document).ToList();
            var nugetInfoReferences = references.Where(CsProj.IsNugetReference).ToList();
            var referenceElement = nugetInfoReferences.FirstOrDefault(x => CsProj.GetNugetInfo(x).Name == _nugetName);
            if (referenceElement == null)
            {
                return null;
            }
            //获取Nuget引用信息
            var replacedFileRecord = GetNugetReferenceInfo(referenceElement, references.IndexOf(referenceElement));
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
                NugetDllPath = referenceElement.Value
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

        public void RevertNuget()
        {
            CsProj.RevertReference(Document, _lastReplacedRecord);
            //删除源代码引用
            var projectReferences = CsProj.GetProjectReferences(Document);
            var sourceProjectReferences = projectReferences.Where(i => i.Attribute(CsProjConst.IncludeAttribute).Value.Contains(_sourceProjectFile)).ToList();
            foreach (var sourceProjectReference in sourceProjectReferences)
            {
                sourceProjectReference.Remove();
            }
            SaveFile();
        }
    }
}
