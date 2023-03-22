using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NugetEfficientTool.Business
{
    public class NugetVersionContrast
    {
        /// <summary>
        /// 版本号降序排序器
        /// </summary>
        /// <param name="x">版本号 X</param>
        /// <param name="y">版本号 Y</param>
        /// <returns>比较结果</returns>
        public static int VersionDescendingComparison(string x, string y)
        {
            if (x == y)
            {
                return 0;
            }

            var xMainVersion = GetMainVersion(x);
            var yMainVersion = GetMainVersion(y);
            var mainVersionCompareResult = -xMainVersion.CompareTo(yMainVersion);
            if (mainVersionCompareResult != 0)
            {
                return mainVersionCompareResult;
            }

            var xSubVersion = GetSubVersion(x);
            var ySubVersion = GetSubVersion(y);
            var subVersionCompareResult = -StringComparer.InvariantCultureIgnoreCase.Compare(xSubVersion, ySubVersion);
            return subVersionCompareResult;
        }

        /// <summary>
        /// 获取主版本号部分
        /// </summary>
        /// <param name="version">版本号字符串</param>
        /// <returns>版本号对象</returns>
        private static Version GetMainVersion(string version)
        {
            var index = version.IndexOf('-');
            try
            {
                if (index < 0)
                {
                    return new Version(version);
                }

                return new Version(version.Substring(0, index));
            }
            catch (Exception)
            {
                CustomText.Notification.ShowInfo(null, $"无法从 {version} 构造版本号对象。请保留现场，并联系开发者。");
                return new Version(0, 0);
            }
        }

        private static string GetSubVersion(string version)
        {
            var index = version.IndexOf('-');
            return version.Substring(index + 1);
        }
    }
}
