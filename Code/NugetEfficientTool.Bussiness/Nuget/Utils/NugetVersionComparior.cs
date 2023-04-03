using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NuGet.Versioning;

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
            //主版本号
            var xVersion = new NuGetVersion(x);
            var yVersion = new NuGetVersion(y);
            var compareResult = -xVersion.CompareTo(yVersion);
            return compareResult;
        }
    }
}
