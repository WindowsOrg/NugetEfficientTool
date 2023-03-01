using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Reference下HintPath元素的辅助操作
    /// <para>
    /// <Reference Include="Microsoft.Threading.Tasks, Version=1.0.12.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL">
    ///     <HintPath>..\packages\Microsoft.Bcl.Async.1.0.168\lib\net40\Microsoft.Threading.Tasks.dll</HintPath>
    /// </Reference>
    /// </para>
    /// </summary>
    internal static class HintPathElements
    {
        /// <summary>
        /// 获取Nuget信息
        /// </summary>
        /// <returns></returns>
        public static NugetInfo GetNugetInfo(XElement hintPathElement)
        {
            var hintPathVale = hintPathElement.Value;
            if (string.IsNullOrEmpty(hintPathVale))
            {
                return null;
            }
            //System.ValueTuple.4.5.0
            var matchesValue = NugetNameVersionRegex.Match(hintPathVale).Value;
            if (string.IsNullOrEmpty(matchesValue))
            {
                return null;
            }
            //.4.5.0
            var versionMatches = VersionRegex.Match(matchesValue).Value;
            //4.5.0
            var version = versionMatches.Substring(1, versionMatches.Length - 1);
            //System.ValueTuple
            var nugetName = matchesValue.Replace(versionMatches, string.Empty);
            return new NugetInfo(nugetName, version);
        }

        private static readonly Regex NugetNameVersionRegex = new Regex(@"(?<=packages\\).+(?=\\lib)");

        private static readonly Regex VersionRegex = new Regex(@"(?=.*)(.[0-9]+){3}");
    }
}
