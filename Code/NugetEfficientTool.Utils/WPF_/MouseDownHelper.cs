using System;
using System.Windows;

namespace NugetEfficientTool.Utils
{
    public class MouseDownHelper
    {
        private static DateTime _dateTime = new DateTime();
        private static Point _prePoint = new Point();

        /// <summary>
        /// 判断是否双击
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static bool IsDoubleClick(Point point, DateTime dataTime)
        {
            var duration = dataTime.Subtract(_dateTime).Duration();

            if (duration.TotalMilliseconds < 500 &&
                Math.Abs(point.X - _prePoint.X) < 2 && Math.Abs(point.Y - _prePoint.Y) < 2)
                return true;

            _dateTime = dataTime;
            _prePoint = point;
            return false;
        }
    }
}
