using System;

namespace NugetEfficientTool.Utils
{
    public static class CirclePointUtil
    {
        /// <summary>
        /// 根据圆的中心、半径、角度获取另一个点
        /// </summary>
        /// <param name="centerPoint">圆的中心</param>
        /// <param name="r">半径</param>
        /// <param name="arcAngel">弧度角度</param>
        /// <returns></returns>
        public static System.Windows.Point GetPointByAngel(System.Windows.Point centerPoint, double r, double arcAngel)
        {
            var p = new System.Windows.Point
            {
                X = Math.Sin(arcAngel) * r + centerPoint.X,
                Y = centerPoint.Y - Math.Cos(arcAngel) * r
            };
            return p;
        }
    }
}
