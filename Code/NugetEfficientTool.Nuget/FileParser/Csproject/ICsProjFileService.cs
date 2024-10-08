﻿using System.Xml.Linq;

namespace NugetEfficientTool.Nuget
{
    internal interface ICsProjFileService
    {
        List<XElement> GetReferences(XDocument xDocument);
        List<XElement> GetProjectReferences(XDocument xDocument);
        List<XElement> GetPackageReferences(XDocument xDocument);
        List<XElement> GetNugetReferences(XDocument xDocument);
        bool IsNugetReference(XElement xElement);
        NugetInfo GetNugetInfo(XElement xElement);
    }
}
