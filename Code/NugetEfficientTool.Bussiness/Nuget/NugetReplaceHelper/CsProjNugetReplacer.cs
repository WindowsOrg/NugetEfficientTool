using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
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

        public ReplacedFileRecord ReplaceNuget()
        {
            var references = CsProj.GetReferences(Document).ToList();
            var nugetInfoReferences= references.Where(CsProj.IsNugetInfoReference).ToList();
            var referenceElement = nugetInfoReferences.Where(x =>
                CsProj.GetNugetInfoFromNugetInfoReference(x).Name == _nugetName).FirstOrDefault();
            if (referenceElement != null)
            {
                var includeString = referenceElement.Attribute(CsProj.IncludeAttribute).Value;
                var version = includeString.Replace($"{_nugetName},", string.Empty).Replace("Version=", string.Empty)
                    .Replace("Culture=neutral", string.Empty).Replace("processorArchitecture=MSIL", string.Empty)
                    .Replace(",", string.Empty).Trim();
                var replacedFileRecord = new ReplacedFileRecord()
                {
                    NugetName = _nugetName,
                    FileName = File,
                    ModifiedLineIndex = references.IndexOf(referenceElement),
                    Version = version,
                    NugetDllPath = referenceElement.Value
                };
                //删除Nuget的引用
                referenceElement.Remove();
                //添加源项目的引用
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
                    var itemGroup = Document.Root.Elements().Where(i => i.Name.LocalName == CsProj.ItemGroupName).ToList()[0];
                    itemGroup.Add(xElement);
                }
                SaveFile();
                return replacedFileRecord;
            }
            return null;
        }

        public void RevertNuget()
        {
            var references = CsProj.GetReferences(Document).ToList();
            //添加package引用
            var referenceElement = new XElement(CsProj.ReferenceName);
            referenceElement.SetAttributeValue(CsProj.IncludeAttribute, $"{_lastReplacedRecord.NugetName}, Version={_lastReplacedRecord.Version}, Culture=neutral, processorArchitecture=MSIL");
            var hintPathElement = new XElement(CsProj.HintPathElementName);
            hintPathElement.SetValue(_lastReplacedRecord.NugetDllPath);
            referenceElement.Add(hintPathElement);
            references[_lastReplacedRecord.ModifiedLineIndex].AddBeforeSelf(referenceElement);
            //删除源代码引用
            var projectReferences = CsProj.GetProjectReferences(Document);
            var sourceProjectReferences = projectReferences.Where(i => i.Attribute(CsProj.IncludeAttribute).Value.Contains(_sourceProjectFile)).ToList();
            foreach (var sourceProjectReference in sourceProjectReferences)
            {
                sourceProjectReference.Remove();
            }
            SaveFile();
        }
    }
}
