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
            if (!CheckInputText(solutionText, out var solutionFile))
            {
                return;
            }
            //检测Nuget版本
            try
            {
                IsChecking = true;
                await Task.Run(() =>
                {
                    _nugetVersionChecker = new NugetVersionChecker(solutionFile);
                    _nugetVersionChecker.Check();
                });
            }
            finally
            {
                IsChecking = false;
            }
            //设置检测结果
            TextBoxErrorMessage.Text = _nugetVersionChecker.Message;
            ButtonFixVersion.IsEnabled = _nugetVersionChecker.MismatchVersionNugetInfoExs.Any() &&
                                         !_nugetVersionChecker.ErrorFormatNugetConfigs.Any();
            //更新到配置文件
            NugetFixConfigs.SaveNugetFixPath(solutionText);
        }
        /// <summary>
        /// 修复Nuget版本问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixNugetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var nugetVersionFixWindow = new FixingVersionSelectWindow(_nugetVersionChecker.MismatchVersionNugetInfoExs)
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

                var repairLog = string.Empty;
                var toReparingFiles = new List<string>();
                foreach (var mismatchVersionNugetInfoEx in _nugetVersionChecker.MismatchVersionNugetInfoExs)
                {
                    foreach (var fileNugetInfo in mismatchVersionNugetInfoEx.FileNugetInfos)
                    {
                        if (nugetFixStrategies.All(i => i.NugetName != fileNugetInfo.Name))
                        {
                            continue;
                        }
                        //如果文件已经满足当前修复策略，则跳过
                        if (nugetFixStrategies.All(i => $"{i.NugetName}_{i.NugetVersion}" ==
                                                      $"{fileNugetInfo.Name}_{fileNugetInfo.Version}"))
                        {
                            continue;
                        }

                        if (toReparingFiles.Any(i => i == fileNugetInfo.ConfigPath))
                        {
                            continue;
                        }
                        toReparingFiles.Add(fileNugetInfo.ConfigPath);
                    }
                }
                //对文件进行修复
                foreach (var configFile in toReparingFiles)
                {
                    var nugetConfigRepairer = new NugetConfigRepairer(configFile, nugetFixStrategies);
                    nugetConfigRepairer.Repair();
                    if (!string.IsNullOrEmpty(nugetConfigRepairer.Log))
                    {
                        repairLog = StringSplicer.SpliceWithDoubleNewLine(repairLog, nugetConfigRepairer.Log);
                    }
                }
                TextBoxErrorMessage.Text = repairLog;
                ButtonFixVersion.IsEnabled = false;
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

        #region private fields

        private NugetVersionChecker _nugetVersionChecker;

        #endregion
    }
}
