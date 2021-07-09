using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NugetEfficientTool.Resources
{
    public static class FontFamilies
    {
        public const string ArialFamily = "Arial";
        public const string MicrosoftYaHei = "Microsoft YaHei";
        public const string MicrosoftYaHeiBold = "Microsoft YaHei Bold";
        public const string MicrosoftYaHeiUi = "Microsoft YaHei";
        public const string MicrosoftYaHeiUiBold = "Microsoft YaHei Bold";

        public static FontFamily Arial = new FontFamily(ArialFamily);
        public static FontFamily YaHei = new FontFamily(MicrosoftYaHei);
        public static FontFamily YaHeiBold = new FontFamily(MicrosoftYaHeiBold);
        public static FontFamily YaHeiUi = new FontFamily(MicrosoftYaHeiUi);
        public static FontFamily YaHeiUiBold = new FontFamily(MicrosoftYaHeiUiBold);
    }
}
