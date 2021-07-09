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

namespace TransYeekit.Resources
{
    /// <summary>
    /// NotificationTipWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotificationTipWindow : Window, INotificationWindow
    {
        public NotificationTipWindow()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            NotificationTextBlock.Text = text;
        }

        public void SetView(UIElement view)
        {
            
        }

        public void SetOwner(Window window)
        {
            Owner = window;
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
    }
}
