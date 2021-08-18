using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using Point = System.Drawing.Point;

namespace NugetEfficientTool.Utils
{
    public static class PointTransformHelper
    {
        const int DpiPercent = 96;
        /// <summary>
        /// 从屏幕坐标转换为WPF坐标
        /// </summary>
        /// <param name="point">屏幕坐标</param>
        /// <param name="window">窗口，需要此参数获取所在屏幕的DPI</param>
        /// <returns></returns>
        public static Point TransformToWpf(Point point,Window window)
        {
            var intPtr = new WindowInteropHelper(window).Handle;
            using (Graphics currentGraphics = Graphics.FromHwnd(intPtr))
            {
                double dpiXRatio = currentGraphics.DpiX / DpiPercent;
                double dpiYRatio = currentGraphics.DpiY / DpiPercent;
                return new Point((int)(point.X * dpiXRatio), (int)(point.Y * dpiYRatio));
            }
        }
    }
}
