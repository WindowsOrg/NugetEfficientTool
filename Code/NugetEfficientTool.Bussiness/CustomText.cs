using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kybs0.Log;

namespace NugetEfficientTool.Business
{
    public static class CustomText
    {
        public static UserPath Path { get; } = new UserPath();
        public static Logger Log { get; set; }

        public const string ProjectName = "NugetEfficientTool";
        public const string PackagesConfigSearchPattern = "packages.config";
        public const string CsProjSearchPattern = "*.csproj";
    }
}
