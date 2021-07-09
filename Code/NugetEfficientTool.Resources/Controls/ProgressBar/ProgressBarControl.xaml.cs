using System.Windows;
using System.Windows.Controls;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// ProgressBarControl.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBarControl : UserControl
    {
        public ProgressBarControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ProgressingStringFormatTipProperty = DependencyProperty.Register(
            "ProgressingStringFormatTip", typeof(string), typeof(ProgressBarControl), new PropertyMetadata("下载中…{0}%"));

        public string ProgressingStringFormatTip
        {
            get { return (string) GetValue(ProgressingStringFormatTipProperty); }
            set { SetValue(ProgressingStringFormatTipProperty, value); }
        }

        public static readonly DependencyProperty ProgressedTipProperty = DependencyProperty.Register(
            "ProgressedTip", typeof(string), typeof(ProgressBarControl), new PropertyMetadata("下载完成！"));

        public string ProgressedTip
        {
            get { return (string) GetValue(ProgressedTipProperty); }
            set { SetValue(ProgressedTipProperty, value); }
        }

        public static readonly DependencyProperty ProgressErrorTipProperty = DependencyProperty.Register(
            "ProgressErrorTip", typeof(string), typeof(ProgressBarControl), new PropertyMetadata("下载失败！"));

        public string ProgressErrorTip
        {
            get { return (string) GetValue(ProgressErrorTipProperty); }
            set { SetValue(ProgressErrorTipProperty, value); }
        }

        public static readonly DependencyProperty ProgressStateProperty = DependencyProperty.Register(
            "ProgressState", typeof(ProgressState), typeof(ProgressBarControl), new PropertyMetadata(default(ProgressState)));

        public ProgressState ProgressState
        {
            get { return (ProgressState) GetValue(ProgressStateProperty); }
            set { SetValue(ProgressStateProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(ProgressBarControl), new PropertyMetadata(default(double)));
        /// <summary>
        /// 进度值
        /// </summary>
        public double Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(int), typeof(ProgressBarControl), new PropertyMetadata(100));
        /// <summary>
        /// 进度条显示的最大值
        /// </summary>
        public int Maximum
        {
            get { return (int) GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(int), typeof(ProgressBarControl), new PropertyMetadata(0));
        /// <summary>
        /// 进度条显示的最小值
        /// </summary>
        public int Minimum
        {
            get { return (int) GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
    }

    public enum ProgressState
    {
        Initial,
        Progressing,
        Completed,
        Error
    }
    public static class DownloadStateToProgressState{
        public static ProgressState ToProgressState(this DownloadState downloadState)
        {
            switch (downloadState)
            {
                case DownloadState.Downloading: return ProgressState.Progressing;
                case DownloadState.DownloadCompleted: return ProgressState.Completed;
                case DownloadState.DownloadError: return ProgressState.Error;
            }
            return ProgressState.Initial;
        }
    }
}
