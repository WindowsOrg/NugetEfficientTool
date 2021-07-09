using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// 小黑框提示窗口
    /// </summary>
    public partial class NotificationTipWindow : Window, INotificationWindow
    {
        public NotificationTipWindow()
        {
            InitializeComponent();
        }

        public void SetText(string text, string title)
        {
            NotificationTextBlock.Text = text;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Title = title;
            }
        }

        public void SetText(string text, string subText, string title)
        {
            throw new NotSupportedException();
        }

        public void SetView(UIElement view, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                Title = title;
            }
        }

        public void SetOwner(Window window)
        {
            if (window!=null)
            {
                Owner = window;
            }
        }

        public void SetNotificationContent(NotificationContent content)
        {
            throw new NotImplementedException();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public new async void Show()
        {
            base.Show();
            await Task.Delay(TimeSpan.FromSeconds(2));
            this.Close();
        }

        public new bool? ShowDialog()
        {
            var result = base.ShowDialog();
            this.Close();
            return result;
        }
    }
}
