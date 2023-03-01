using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    internal interface ICsProjFileService
    {
        List<XElement> GetProjectReferences(XDocument xDocument);
        List<XElement> GetPackageReferences(XDocument xDocument);
        List<XElement> GetNugetReferences(XDocument xDocument);
        List<XElement> GetReferences(XDocument xDocument);
        void RevertReference(XDocument document, ReplacedFileRecord replacedRecord);
        bool IsNugetReference(XElement xElement);
        NugetInfo GetNugetInfo(XElement xElement);
    }
}
