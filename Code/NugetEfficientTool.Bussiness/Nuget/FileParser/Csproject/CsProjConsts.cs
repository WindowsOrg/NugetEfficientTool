using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    public static class CsProjConst
    {
        public const string RootName = "Project";
        public const string ItemGroupName = "ItemGroup";

        public const string PackageReferenceName = "PackageReference";
        public const string ReferenceName = "Reference";
        public const string ProjectReferenceName = "ProjectReference";

        public const string XmlnsAttribute = "xmlns";
        public const string IncludeAttribute = "Include";
        public const string UpdateAttribute = "Update";
        public const string VersionAttribute = "Version";

        public const string VersionElementName = VersionAttribute;
        public const string HintPathElementName = "HintPath";

        //路径引用路径中package片段。注：要关注sln和项目是否在同一路径下
        public const string HintPathPackagePiece = "packages\\";
        public const string HintPathLibPiece = "lib\\";

        public static string SdkAttribute = "Sdk";
        public static string SdkValue = "Microsoft.NET.Sdk.WindowsDesktop";
    }
}
