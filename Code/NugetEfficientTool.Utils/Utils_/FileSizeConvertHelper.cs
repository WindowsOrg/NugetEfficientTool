using System;
using System.Text.RegularExpressions;

namespace NugetEfficientTool.Utils
{
    public static class FileSizeConvertHelper
    {
        /// <summary>
        /// 将文件大小转换为用于显示的大小
        /// </summary>
        /// <param name="value">文件大小，单位：B</param>
        /// <param name="isRound">是否四舍五入</param>
        /// <param name="separators">数字和单位间的分隔符</param>
        /// <returns>用于显示的字符串，只保留GB和TB的小数</returns>
        public static string ConvertSize(long value, bool isRound = true, string separators = "")
        {
            var result = Convert(value, 1024, 1, isRound, separators, "B", "KB", "MB", "GB", "TB");

            //只保留GB和TB的小数
            if (!result.Contains("GB") && !result.Contains("TB"))
            {
                result = new Regex("\\.[\\d]+").Replace(result, "");
            }

            return result;
        }

        /// <summary>
        /// 将数值转换为用于显示的数值
        /// </summary>
        /// <param name="value">需要转换的值</param>
        /// <param name="interval">进制</param>
        /// <param name="digits">小数位数</param>
        /// <param name="isRound">是否四舍五入</param>
        /// <param name="separators">数字和单位间的分隔符</param>
        /// <param name="units">单位</param>
        /// <returns></returns>
        public static string Convert(long value, long interval, int digits, bool isRound, string separators, params string[] units)
        {
            var current = 0;
            var temp = value;

            while (current < units.Length - 1 && temp >= interval)
            {
                current++;
                temp /= interval;
            }
            var result = value * 1.0 / Math.Pow(interval, current);

            if (!isRound)
            {
                var pow = Math.Pow(10, digits);
                return $"{Math.Truncate(result * pow) / pow}{units[current]}";
            }

            if (string.IsNullOrEmpty(separators))
            {
                return $"{Math.Round(result, digits)}{units[current]}";
            }

            return $"{Math.Round(result, digits)}{separators}{units[current]}";
        }
    }
}
