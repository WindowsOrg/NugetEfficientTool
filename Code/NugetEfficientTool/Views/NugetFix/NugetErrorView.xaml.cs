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
    public partial class NugetErrorView : UserControl
    {
        public NugetErrorView()
        {
            InitializeComponent();
            Loaded += NugetFixView_Loaded;
        }

        private void NugetFixView_Loaded(object sender, RoutedEventArgs e)
        {
            var firstSolution = UserOperationConfigHelper.GetSolutions().FirstOrDefault();
            if (firstSolution != null)
            {
                SolutionTextBox.Text = firstSolution.SolutionFile;
            }
        }
        private async void SolutionTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            SolutionTextBox.Text = SolutionTextBox.Text.Trim('"');
            var solutionFile = SolutionTextBox.Text;
            // 其实输入的可能是文件夹
            try
            {
                if (!File.Exists(solutionFile) && Directory.Exists(solutionFile))
                {
                    if (SolutionFileHelper.TryGetSlnFile(solutionFile, out var slnFile))
                    {
                        SolutionTextBox.Text = slnFile;
                    }
                }
            }
            catch (Exception exception)
            {
                CustomText.Log.Error(exception);
            }

            SolutionTextBox.SelectionStart = SolutionTextBox.Text.Length;
        }
        private bool CheckInputText(out string solutionFile)
        {
            solutionFile = SolutionTextBox.Text;
            if (string.IsNullOrWhiteSpace(solutionFile))
            {
                CustomText.Notification.ShowInfo(Window.GetWindow(this), "解决方案路径不能为空…… 心急吃不了热豆腐……");
                return false;
            }
            if (!File.Exists(solutionFile))
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
        private void CheckNugetButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!CheckInputText(out var solutionFile))
            {
                return;
            }
            //检测Nuget版本
            _nugetVersionChecker = new NugetVersionChecker(solutionFile);
            _nugetVersionChecker.Check();
            TextBoxErrorMessage.Text = _nugetVersionChecker.Message;
            ButtonFixVersion.IsEnabled = _nugetVersionChecker.MismatchVersionNugetInfoExs.Any() &&
                                         !_nugetVersionChecker.ErrorFormatNugetConfigs.Any();
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

                        if (toReparingFiles.Any(i=>i==fileNugetInfo.ConfigPath))
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

        #region private fields

        private NugetVersionChecker _nugetVersionChecker;

        #endregion
    }
}
