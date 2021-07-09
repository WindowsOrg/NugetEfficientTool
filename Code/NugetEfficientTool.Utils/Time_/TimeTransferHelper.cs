using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public static class TimeTransferHelper
    {
        /// <summary>
        /// 秒转换小时
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static string SecondToHour(int second)
        {
            int day = 0;
            int hour = 0;
            int minute = 0;

            if (second >= 60)
            {
                minute = second / 60;
                second = second % 60;
            }
            if (minute >= 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }

            if (hour >= 8)
            {
                day = hour / 8;
                hour = hour % 8;
            }

            var result = string.Empty;
            if (day > 0)
            {
                result += day + "d";
            }
            if (hour > 0)
            {
                result += hour + "h";
            }
            if (minute > 0)
            {
                result += minute + "min";
            }
            return result;
        }
    }
}
