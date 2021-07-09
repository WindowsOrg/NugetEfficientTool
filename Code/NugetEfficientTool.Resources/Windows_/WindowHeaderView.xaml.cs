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

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// 通用窗口的标题栏控件
    /// </summary>
    public partial class WindowHeaderView : UserControl
    {
        public WindowHeaderView()
        {
            InitializeComponent();
            Loaded += WindowHeaderView_Loaded;
        }

        private void WindowHeaderView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WindowHeaderView_Loaded;
            var window = Window.GetWindow(this);
            if (window != null) window.StateChanged += Window_OnStateChanged;
        }

        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register(
            "HeaderBackground", typeof(SolidColorBrush), typeof(WindowHeaderView), new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#FFF0F0F0")));

        public SolidColorBrush HeaderBackground
        {
            get { return (SolidColorBrush) GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MinimizeButtonVisibilityProperty = DependencyProperty.Register(
            "MinimizeButtonVisibility", typeof(Visibility), typeof(WindowHeaderView), new PropertyMetadata(default(Visibility)));

        public Visibility MinimizeButtonVisibility
        {
            get { return (Visibility) GetValue(MinimizeButtonVisibilityProperty); }
            set { SetValue(MinimizeButtonVisibilityProperty, value); }
        }

        public WindowHeaderView(DefinedWindowType definedWindowType):this()
        {
            if (definedWindowType==DefinedWindowType.GrayTitleDialog)
            {
                MinimizeButton.Visibility = Visibility.Collapsed;
            }
        }
        private void HeaderGridOnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (args.ButtonState == MouseButtonState.Pressed)
            {
                var window = Window.GetWindow(this);
                window?.DragMove();
            }
        }
        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null) window.WindowState = WindowState.Minimized;
        }

        public event EventHandler Closed;
        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Closed!=null)
            {
                Closed.Invoke(null,EventArgs.Empty);
            }
            else
            {
                var window = Window.GetWindow(this);
                window?.Close();
            }
        }

        private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null) window.WindowState = WindowState.Maximized;
        }

        private void RestoreNormalButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null) window.WindowState = WindowState.Normal;
        }

        private void Window_OnStateChanged(object sender, EventArgs e)
        {
            //TODO 待添加依赖属性，提供简化窗口样式给小窗口
            if (Window.GetWindow(this)?.WindowState == WindowState.Normal)
            {
                RestoreNormalButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
            else if (Window.GetWindow(this)?.WindowState == WindowState.Maximized)
            {
                RestoreNormalButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
