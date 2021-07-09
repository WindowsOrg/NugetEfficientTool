using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NugetEfficientTool.Business;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            StateChanged += MainWindow_StateChanged;
        }

        #region 窗口事件

        private void HeaderGridOnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (args.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Hide(sender, e);
        }

        public void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;
        }

        private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void RestoreNormalButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                RestoreNormalButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
            else if (WindowState == WindowState.Maximized)
            {
                RestoreNormalButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region 辅助操作

        //取消触摸拖动窗口抖动
        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
            // Do nothing
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            //显示窗口位置及大小
            AdjustLocationAndSize();
            //禁用系统菜单弹出方向
            MenuDropAlignmentHelper.DisableSystemMenuAlignment();
        }

        private void AdjustLocationAndSize()
        {
            var locationSizeMode = UiHabitsConfigHelper.GetWindowLocation();
            if (locationSizeMode == null)
            {
                return;
            }

            var windowState = locationSizeMode.WindowState;
            if (windowState == WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
                return;
            }

            Left = locationSizeMode.Left;
            Top = locationSizeMode.Top;
            Width = locationSizeMode.ActualWidth;
            Height = locationSizeMode.ActualHeight;
        }

        #endregion
    }
}
