using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    public class PackageFileNugetReplacer : XmlFileNugetReplacer, INugetFileReplacer
    {
        private readonly ReplacedFileRecord _replacedFileRecord;
        private readonly string _nugetName;

        public PackageFileNugetReplacer(string packageFile, string nugetName) : base(packageFile)
        {
            _nugetName = nugetName;
        }

        public PackageFileNugetReplacer(string packageFile, ReplacedFileRecord replacedFileRecord) : base(packageFile)
        {
            _replacedFileRecord = replacedFileRecord;
        }

        public ReplacedFileRecord ReplaceNuget()
        {
            var document = Document;
            var documentRoot = document.Root;
            var packageElements = documentRoot.Elements().ToList();
            var packageElement = packageElements.Where(x => x.Attribute(PackagesConfig.IdAttribute).Value == _nugetName).ToList().FirstOrDefault();
            if (packageElement != null)
            {
                var modifiedRecord = new ReplacedFileRecord() { FileName = File, NugetName = _nugetName, };
                modifiedRecord.ModifiedLineIndex = packageElements.IndexOf(packageElement);
                modifiedRecord.Version = packageElement.Attribute(PackagesConfig.VersionAttribute).Value;
                modifiedRecord.TargetFramework = packageElement.Attribute(PackagesConfig.TargetFrameworkAttribute).Value;
                packageElement.Remove();
                SaveFile();
                return modifiedRecord;
            }
            return null;
        }

        public void RevertNuget()
        {
            var rootElement = Document.Root;
            var packageElements = rootElement.Elements().ToList();
            var packageElement = new XElement("package");
            packageElement.SetAttributeValue("id", _replacedFileRecord.NugetName);
            packageElement.SetAttributeValue("version", _replacedFileRecord.Version);
            packageElement.SetAttributeValue("targetFramework", _replacedFileRecord.TargetFramework);
            packageElements[_replacedFileRecord.ModifiedLineIndex].AddBeforeSelf(packageElement);

            SaveFile();
        }
    }
}
