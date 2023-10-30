using System.Text;
using System.Xml.Linq;
using Kybs0.Project;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// 文件nuget替换（抽象类）
    /// </summary>
    public abstract class XmlFileNugetReplacer
    {
        protected readonly string XmlFile;
        protected XmlFileNugetReplacer(string xmlFile)
        {
            XmlFile = xmlFile;
            Document = new CodeXmlReader(xmlFile).Document;
        }

        protected string File => XmlFile;
        public XDocument Document { get; }

        public void SaveFile()
        {
            Document.Save(XmlFile);
            //修复自动生成xmlns的问题
            var allText = System.IO.File.ReadAllText(XmlFile);
            var removedXmlnsText = allText.Replace(" xmlns=\"\"", string.Empty);
            System.IO.File.WriteAllText(XmlFile, removedXmlnsText, Encoding.UTF8);
        }
    }
}
