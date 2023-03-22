using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using NugetEfficientTool.Business;

namespace NugetEfficientTool
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class NugetFixView : UserControl
    {
        public NugetFixView()
        {
            InitializeComponent();
            Loaded += NugetFixView_Loaded;
        }

        private void NugetFixView_Loaded(object sender, RoutedEventArgs e)
        {
            var nugetFixPath = NugetFixConfigs.GetNugetFixPath();
            SolutionTextBox.Text = nugetFixPath;
        }

        private async void SolutionTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            SolutionTextBox.Text = SolutionTextBox.Text.Trim('"');
            //整理输入光标
            SolutionTextBox.SelectionStart = SolutionTextBox.Text.Length;
        }
        private bool CheckInputText(string solutionText, out string solutionFile)
        {
            solutionFile = solutionText;
            if (string.IsNullOrWhiteSpace(solutionFile))
            {
                CustomText.Notification.ShowInfo(Window.GetWindow(this), "解决方案路径不能为空…… 心急吃不了热豆腐……");
                return false;
            }
            var isFolder = !File.Exists(solutionFile) && Directory.Exists(solutionFile);
            if (isFolder && SolutionFileHelper.TryGetSlnFiles(solutionFile, out var slnFiles) &&
                slnFiles.Count == 0)
            {
                CustomText.Notification.ShowInfo(Window.GetWindow(this), "找不到指定的解决方案，这是啥情况？？？");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检查Nuget版本问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckNugetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var solutionText = SolutionTextBox.Text;
            if (!CheckInputText(solutionText, out var solutionFile) ||
                IsChecking)
            {
                return;
            }
            //检测Nuget错误
            bool canReferenceWayUpgrade = false;
            try
            {
                IsChecking = true;
                await Task.Run(() =>
                {
                    _versionChecker = new VersionErrorChecker(solutionFile);
                    _versionChecker.Check();
                    var referenceWayChecker = new ReferenceWayChecker(solutionFile);
                    referenceWayChecker.Check();
                    canReferenceWayUpgrade = referenceWayChecker.NeedFix;
                });
            }
            finally
            {
                IsChecking = false;
            }
            //设置检测结果
            var message = _versionChecker.Message;
            if (string.IsNullOrEmpty(message))
            {
                if (canReferenceWayUpgrade)
                {
                    message = "存在Reference可升级";
                }
                else
                {
                    message = "完美无瑕！";
                }
            }
            TextBoxErrorMessage.Text = message;
            FixVersionButton.IsEnabled = _versionChecker.MismatchVersionNugets.Any() &&
                                         !_versionChecker.ErrorFormatNugetFiles.Any();
            UpgradeReferenceButton.Visibility = canReferenceWayUpgrade ? Visibility.Visible : Visibility.Collapsed;
            //更新到配置文件
            NugetFixConfigs.SaveNugetFixPath(solutionText);
        }

        #region 修复Nuget版本问题

        /// <summary>
        /// 修复Nuget版本问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixVersionButton_OnClick(object sender, RoutedEventArgs e)
        {
            var nugetVersionFixWindow = new VersionSelectWindow(_versionChecker.MismatchVersionNugets)
            {
                Owner = Window.GetWindow(this)
            };
            nugetVersionFixWindow.NugetFixStrategiesSelected += (o, args) =>
            {
                var nugetFixStrategies = args.NugetFixStrategies.ToList();
                if (!nugetFixStrategies.Any())
                {
                    return;
                }
                // 根据版本策略修复版本
                var repairLog = new NugetMismatchVersionGroupFix(_versionChecker.MismatchVersionNugets,nugetFixStrategies).Fix();
                TextBoxErrorMessage.Text = repairLog;
                FixVersionButton.IsEnabled = false;
                UpgradeReferenceButton.Visibility = Visibility.Collapsed;
                nugetVersionFixWindow.Close();
            };

            try
            {
                nugetVersionFixWindow.ShowDialog();
            }
            catch (Exception exception)
            {
                CustomText.Notification.ShowInfo(Window.GetWindow(this), exception.Message);
                CustomText.Log.Error(exception);
            }
        }

        #endregion

        #region 升级Nuget引用方式

        private void UpgradeReferenceButton_OnClick(object sender, RoutedEventArgs e)
        {
            //修复文件引用格式
            var fileReferenceWayUpdater = new FileReferenceWayRepairer(SolutionTextBox.Text);
            fileReferenceWayUpdater.Fix();
            var log = fileReferenceWayUpdater.Log;
            TextBoxErrorMessage.Text = string.IsNullOrEmpty(log) ? "emmm...未找到Reference可升级" : log;
            UpgradeReferenceButton.Visibility = Visibility.Collapsed;
            FixVersionButton.IsEnabled = false;
        }

        #endregion

        #region 属性

        public static readonly DependencyProperty IsCheckingProperty = DependencyProperty.Register(
            "IsChecking", typeof(bool), typeof(NugetFixView), new PropertyMetadata(default(bool)));
        /// <summary>
        /// 检查中
        /// </summary>
        public bool IsChecking
        {
            get => (bool)GetValue(IsCheckingProperty);
            set => SetValue(IsCheckingProperty, value);
        }

        #endregion

        #region private fields

        private VersionErrorChecker _versionChecker;

        #endregion
    }
}
