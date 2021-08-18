using System;
using System.Threading.Tasks;
using System.Windows;

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
