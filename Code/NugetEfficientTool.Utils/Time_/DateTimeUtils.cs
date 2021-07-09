using System;
using System.Runtime.InteropServices;

namespace NugetEfficientTool.Utils
{
    public static class DateTimeUtils
    {
        public static int DateDiff(DateTime dateTime1, DateTime dateTime2)
        {
            try
            {
                TimeSpan ts1 = new TimeSpan(dateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(dateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                return ts.Days;
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        /// <summary>
        /// 时间戳计时开始时间
        /// </summary>
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long ToTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime - timeStampStartTime).TotalMilliseconds;
        }

        /// <summary>
        /// 13位时间戳（单位：毫秒）转换为DateTime
        /// </summary>
        /// <param name="longTimeStamp">13位时间戳（单位：毫秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime ToDateTime(long longTimeStamp)
        {
            return timeStampStartTime.AddMilliseconds(longTimeStamp).ToLocalTime();
        }

        /// <summary>
        /// 获取系统鼠标双击间隔
        /// </summary>
        /// <returns></returns>
        public static int GetDoubleClickInterval()
        {
            return GetDoubleClickTime();
        }
        [DllImport("user32.dll", EntryPoint = "GetDoubleClickTime")]
        private static extern int GetDoubleClickTime();
    }
}
