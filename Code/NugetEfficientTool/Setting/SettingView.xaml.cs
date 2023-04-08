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

namespace NugetEfficientTool.Setting
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
            Loaded += SettingWindow_Loaded;
        }

        private void SettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NugetSourceTextBox.Text = NugetFixConfigs.GetNugetSourceUri();
        }

        private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            NugetFixConfigs.SaveNugetSourceUri(NugetSourceTextBox.Text);
            Window.GetWindow(this).Close();
        }
    }
}
