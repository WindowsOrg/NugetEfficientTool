using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Kybs0.Project;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Business
{
    class PackageConfigReferenceWayUpdater
    {
        private readonly string _packageFile;

        private readonly XDocument _xDocument;

        public PackageConfigReferenceWayUpdater(string csProjFile)
        {
            _packageFile = csProjFile;
            _xDocument = new CodeXmlReader(csProjFile).Document;
        }

        public string Log { get; private set; }

        public bool TryUpgrade()
        {
            var rootElement = _xDocument.Root;
            if (rootElement == null)
            {
                return false;
            }

            var packageElementList = rootElement.Elements().ToList();
            for (var i = 0; i < packageElementList.Count; i++)
            {
                var packageElement = packageElementList[i];
                var referenceContent = packageElement.ToString();
                packageElement.Remove();
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 删除 {referenceContent}");
            }
            _xDocument.Save(_packageFile);
            //空引用，删除Packages.config
            if (_xDocument.Root?.HasElements == false)
            {
                FileHelper.DeleteFile(_packageFile);
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 删除 {_packageFile}");
            }
            return true;
        }

        public bool CanUpgrade()
        {
            var rootElement = _xDocument.Root;
            if (rootElement == null)
            {
                return false;
            }

            var packageElementList = rootElement.Elements().ToList();
            return packageElementList.Any();
        }
    }
}
