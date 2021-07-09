using System.Windows;

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// ComfirmWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotificationWindow : Window, INotificationWindow
    {
        public NotificationWindow()
        {
            InitializeComponent();
        }

        public void SetText(string text, string title)
        {
            ContentTextBlock.Text = text;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Title = title;
            }
        }

        public void SetText(string text, string subText, string title)
        {
            ContentTextBlock.Text = text;
            SubContentTextBlock.Text = subText;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Title = title;
            }
        }

        public void SetView(UIElement view, string title)
        {
            ContentBorder.Child = view;
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
            if (!string.IsNullOrEmpty(content.Content))
            {
                ContentTextBlock.Text = content.Content;
            }
            else
            {
                ContentBorder.Child = content.View;
            }

            if (content.Owner != null)
            {
                Owner = content.Owner;
            }
            if (content.Title != null)
            {
                Title = content.Title;
            }

            if (content.CancelButton != null)
            {
                OkCancelBorder.Visibility = Visibility.Visible;
                OkBorder.Visibility = Visibility.Collapsed;
                CancelButton.Content = content.CancelButton.Content;
                if (content.OkButton != null)
                {
                    OkButton.Content = content.OkButton.Content;
                }
            }else
            {
                if (content.OkButton != null)
                {
                    OkButton.Content = content.OkButton.Content;
                }
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        public new bool? ShowDialog()
        {
            var result = base.ShowDialog();
            this.Close();
            return result;
        }
    }
}
