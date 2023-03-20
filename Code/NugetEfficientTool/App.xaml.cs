using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Kybs0.Log;
using NugetEfficientTool.Business;
using NugetEfficientTool.Utils;
using Application = System.Windows.Application;

namespace NugetEfficientTool
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //删除原有进程
            ProcessHelper.KillProcess(System.Windows.Forms.Application.ProductName);
            //日志
            CustomText.Log = new Logger(CustomText.ProjectName);
            //启动项
            Startup += App_Startup;


#if !DEBUG
            //全局异常捕获.主要指的是UI线程。
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            //当某个异常未被捕获时出现。主要指的是非UI线程
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //task线程内未处理捕获 
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

#endif
        }
        private MainWindow _mainWindow;
        private void App_Startup(object sender, StartupEventArgs e)
        {
            var startupArgs = e.Args;
            if (startupArgs.Length == 0)
            {
                //显示窗口
                ShowMainWindow();
                SetNotifyIcon();
            }
            else if (startupArgs.Length == 1 && !string.IsNullOrEmpty(startupArgs[0]))
            {
                var nugetAutoFixService = new NugetAutoFixService(startupArgs[0]);
                nugetAutoFixService.Fix();
                Console.WriteLine(nugetAutoFixService.Message);
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"不支持启动参数[{string.Join(",", startupArgs)}]!");
                Environment.Exit(0);
            }
        }
        private void ShowMainWindow()
        {
            _mainWindow = new MainWindow();
            _mainWindow.Show();
        }

        #region 托盘图标

        private NotifyIcon _notifyIcon;
        private void SetNotifyIcon()
        {
            this._notifyIcon = new NotifyIcon();
            this._notifyIcon.BalloonTipText = "Nuget工具";
            this._notifyIcon.ShowBalloonTip(2000);
            this._notifyIcon.Text = "Nuget工具\r\ncopyright @ Kybs0";
            this._notifyIcon.Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this._notifyIcon.Visible = true;
            //打开菜单项
            MenuItem open = new MenuItem("打开");
            open.Click += new EventHandler(ShowMainWindow);
            //退出菜单项
            MenuItem exit = new MenuItem("退出");
            exit.Click += new EventHandler(Close);
            //关联托盘控件
            MenuItem[] childen = new MenuItem[] { open, exit };
            _notifyIcon.ContextMenu = new ContextMenu(childen);

            this._notifyIcon.MouseDoubleClick += new MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left) ShowMainWindow(o, e);
            });
        }

        private void ShowMainWindow(object sender, EventArgs e)
        {
            _mainWindow.Visibility = Visibility.Visible;
            _mainWindow.ShowInTaskbar = true;
            _mainWindow.Activate();
        }

        private void Close(object sender, EventArgs e)
        {
            Current.Shutdown();
        }

        #endregion

        #region Global Exception

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CustomText.Log.Error(e.Exception);
            CustomText.Notification.ShowInfo(null, e.Exception.Message);
            //表示补救成功
            e.Handled = true;
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                CustomText.Log.Error(exception);
                //通过配置legacyUnhandledExceptionPolicy防止后台线程抛出的异常让程序崩溃退出，
                //e.IsTerminating经过配置，才会变成false

                CustomText.Notification.ShowInfo(null, exception.Message);
            }
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            foreach (Exception item in e.Exception.InnerExceptions)
            {
                CustomText.Log.Error(item);
            }
            //设置该异常已察觉（这样处理后就不会引起程序崩溃）
            e.SetObserved();
            CustomText.Notification.ShowInfo(null, e.Exception.Message);
        }

        #endregion
    }
}
