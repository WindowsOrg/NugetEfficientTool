﻿using System;
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
            TextBoxErrorMessage.Text = _versionChecker.Message;
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
            var nugetVersionFixWindow = new FixingVersionSelectWindow(_versionChecker.MismatchVersionNugets)
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
                //修复版本
                var repairLog = RepairVersionErrors(nugetFixStrategies);
                TextBoxErrorMessage.Text = repairLog;
                FixVersionButton.IsEnabled = false;
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

        /// <summary>
        /// 根据版本策略修复版本
        /// </summary>
        /// <param name="nugetFixStrategies"></param>
        /// <returns></returns>
        private string RepairVersionErrors(List<NugetFixStrategy> nugetFixStrategies)
        {
            var repairLog = string.Empty;
            var toReparingFiles = new List<string>();
            foreach (var nugetFileGroups in _versionChecker.MismatchVersionNugets)
            {
                foreach (var nugetFile in nugetFileGroups.FileNugetInfos)
                {
                    if (nugetFixStrategies.All(i => i.NugetName != nugetFile.Name))
                    {
                        continue;
                    }

                    //如果文件已经满足当前修复策略，则跳过
                    if (nugetFixStrategies.All(i => $"{i.NugetName}_{i.NugetVersion}" ==
                                                    $"{nugetFile.Name}_{nugetFile.Version}"))
                    {
                        continue;
                    }

                    if (toReparingFiles.Any(i => i == nugetFile.ConfigPath))
                    {
                        continue;
                    }

                    toReparingFiles.Add(nugetFile.ConfigPath);
                }
            }

            //对文件进行修复
            foreach (var configFile in toReparingFiles)
            {
                var nugetConfigRepairer = new FileNugetVersionRepairer(configFile, nugetFixStrategies);
                nugetConfigRepairer.Repair();
                if (!string.IsNullOrEmpty(nugetConfigRepairer.Log))
                {
                    repairLog = StringSplicer.SpliceWithDoubleNewLine(repairLog, nugetConfigRepairer.Log);
                }
            }

            return repairLog;
        }

        #endregion

        #region 升级Nuget引用方式

        private void UpgradeReferenceButton_OnClick(object sender, RoutedEventArgs e)
        {
            //修复文件引用格式
            var fileReferenceWayUpdater = new FileReferenceWayRepairer(SolutionTextBox.Text);
            fileReferenceWayUpdater.Fix();
            var log = fileReferenceWayUpdater.Log;
            TextBoxErrorMessage.Text = string.IsNullOrEmpty(log) ? "没有发现能升级PackageReference的引用" : log;
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
