﻿using System.Drawing;
using System.Windows;
using System.Windows.Interop;

namespace NugetEfficientTool.Utils
{
    public static class DpiHelper
    {
        public static double GetDpiRatio(DependencyObject dependencyObject)
        {
            var window = Window.GetWindow(dependencyObject);
            if (window==null)
            {
                return 1;
            }
            Graphics currentGraphics = Graphics.FromHwnd(new WindowInteropHelper(window).Handle);
            double dpixRatio = currentGraphics.DpiX / 96;
            return dpixRatio;
        }
    }
}
