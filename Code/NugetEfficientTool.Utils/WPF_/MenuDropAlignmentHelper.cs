using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace NugetEfficientTool.Utils
{
    public static class MenuDropAlignmentHelper
    {
        private static FieldInfo _menuDropAlignmentField;

        /// <summary>
        /// 禁用系统的菜单弹出方向设置，取消对应用程序的Popup弹出方向的影响
        /// </summary>
        public static void DisableSystemMenuAlignment()
        {
            _menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(_menuDropAlignmentField != null);

            EnsureStandardPopupAlignment();
            SystemParameters.StaticPropertyChanged -= SystemParameters_StaticPropertyChanged;
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
        }

        private static void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            EnsureStandardPopupAlignment();
        }

        private static void EnsureStandardPopupAlignment()
        {
            if (SystemParameters.MenuDropAlignment)
            {
                _menuDropAlignmentField?.SetValue(null, false);
            }
        }
    }
}
