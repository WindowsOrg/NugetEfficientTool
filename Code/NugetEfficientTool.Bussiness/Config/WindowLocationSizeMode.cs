using System.Windows;

namespace NugetEfficientTool.Business
{
    public class WindowLocationSizeMode
    {
        public double Left { get; }
        public double Top { get; }
        public double ActualWidth { get; }
        public double ActualHeight { get; }
        public WindowState WindowState { get; set; }
        public WindowLocationSizeMode(double left, double top, double actualWidth, double actualHeight, WindowState windowState)
        {
            Left = left;
            Top = top;
            ActualWidth = actualWidth;
            ActualHeight = actualHeight;
            WindowState = windowState;
        }
    }
}
