using System.Windows;
using System.Windows.Controls;

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// LoadingControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingControl : UserControl
    {
        public LoadingControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register(
            "IsSearching", typeof(bool), typeof(LoadingControl), new PropertyMetadata(default(bool)));

        public bool IsSearching
        {
            get { return (bool) GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }

        public static readonly DependencyProperty LoadingTextProperty = DependencyProperty.Register(
            "LoadingText", typeof(string), typeof(LoadingControl), new PropertyMetadata("加载中..."));

        public string LoadingText
        {
            get { return (string) GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }
    }
}
