using System.Text.RegularExpressions;
using NuGet.Versioning;

namespace NugetEfficientTool.Nuget
{
    /// <summary>
    /// NuGetVersion扩展方法
    /// </summary>
    public static class NugetVersions
    {
        public const string IgnoreFix = "忽略修复";
        private static readonly Regex NumberVersionRegex = new Regex(@"[0-9]+");
        /// <summary>
        /// 升级版本号
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string AddVersion(this NuGetVersion version)
        {
            string newVersion;
            if (version.Revision == 0 && string.IsNullOrEmpty(version.Release))
            {
                //正式版本且无构建号的，增加构建号
                newVersion = $"{version.Major}.{version.Minor}.{version.Patch}.{1}";
            }
            else
            {
                var componentVersion = version.ToString();
                //获取版本中的数字
                var versionNumbers = NumberVersionRegex.Matches(componentVersion);
                //判断版本是否以数字结尾
                var isVersionNumberEnd = versionNumbers.Count > 0 &&
                                         componentVersion.EndsWith(versionNumbers[versionNumbers.Count - 1].Value);
                if (isVersionNumberEnd)
                {
                    var lastVersion=versionNumbers[versionNumbers.Count - 1];
                    var versionStart = componentVersion.Substring(0, componentVersion.Length - lastVersion.Length);
                    //数字结尾，版本+1
                    var versionEndNumber = versionNumbers[versionNumbers.Count - 1].Value;
                    var newVersionEnd = Convert.ToInt32(versionEndNumber) + 1;
                    newVersion = $"{versionStart}{newVersionEnd}";
                }
                else
                {
                    //补充数字版本号1
                    newVersion = $"{componentVersion}1";
                }
            }

            return newVersion;
        }
    }
}
