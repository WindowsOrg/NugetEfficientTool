using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    internal interface INugetFileReplacer
    {
        ReplacedFileRecord ReplaceNuget();
        void RevertNuget();
    }
    public class XmlFileNugetReplacer
    {
        private string _xmlFile;
        protected XmlFileNugetReplacer(string xmlFile)
        {
            _xmlFile = xmlFile;
            Document = new XmlReader(xmlFile).Document;
        }

        protected string File => _xmlFile;
        public XDocument Document { get; }

        public void SaveFile()
        {
            Document.Save(_xmlFile);
            //修复自动生成xmlns的问题
            var allText = System.IO.File.ReadAllText(_xmlFile);
            var removedXmlnsText = allText.Replace(" xmlns=\"\"", string.Empty);
            System.IO.File.WriteAllText(_xmlFile, removedXmlnsText, Encoding.UTF8);
        }
    }
}
