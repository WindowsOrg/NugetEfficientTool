using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NugetEfficientTool.Business;

namespace NugetEfficientTool
{
    /// <summary>
    /// nuget修复版本选择窗口
    /// </summary>
    public partial class FixingVersionSelectWindow : Window
    {
        public FixingVersionSelectWindow(IEnumerable<FileNugetInfoGroup> mismatchVersionNugetInfoExs)
        {
            if (ReferenceEquals(mismatchVersionNugetInfoExs, null))
                throw new ArgumentNullException(nameof(mismatchVersionNugetInfoExs));

            InitializeComponent();
            _mismatchVersionNugetInfoExs = mismatchVersionNugetInfoExs;
            foreach (var mismatchVersionNugetInfoEx in _mismatchVersionNugetInfoExs)
            {
                var nugetName = mismatchVersionNugetInfoEx.NugetName;
                var repeatNugetVersions = mismatchVersionNugetInfoEx.FileNugetInfos.Select(x => x.Version)
                    .Distinct();
                var nugetVersionSelectorUserControl =
                    new NugetVersionSelectorUserControl(nugetName, repeatNugetVersions);
                PanelNugetVersionSelectors.Children.Add(nugetVersionSelectorUserControl);
            }
        }

        public event EventHandler<NugetFixStrategiesEventArgs> NugetFixStrategiesSelected;

        private readonly IEnumerable<FileNugetInfoGroup> _mismatchVersionNugetInfoExs;

        private readonly List<NugetFixStrategy> _nugetFixStrategyList = new List<NugetFixStrategy>();

        /// <summary>
        /// 修复版本异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFix_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _nugetFixStrategyList.Clear();
                foreach (var child in PanelNugetVersionSelectors.Children)
                {
                    if (!(child is NugetVersionSelectorUserControl nugetVersionSelectorUserControl))
                    {
                        continue;
                    }

                    var nugetName = nugetVersionSelectorUserControl.NugetName;
                    var selectedVersion = nugetVersionSelectorUserControl.SelectedVersion;
                    var fixNugetStrategy = CreateVersionFixStrategy(nugetName, selectedVersion);
                    _nugetFixStrategyList.Add(fixNugetStrategy);
                }

                if (!_nugetFixStrategyList.Any())
                {
                    return;
                }

                NugetFixStrategiesSelected?.Invoke(this, new NugetFixStrategiesEventArgs(_nugetFixStrategyList));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        /// <summary>
        /// 创建修复策略
        /// </summary>
        /// <param name="nugetName"></param>
        /// <param name="selectedVersion"></param>
        private NugetFixStrategy CreateVersionFixStrategy(string nugetName, string selectedVersion)
        {
            var nugetInfoExGroup = _mismatchVersionNugetInfoExs.FirstOrDefault(x => x.NugetName == nugetName);
            if (nugetInfoExGroup == null) return null;

            var selectedVersionNugetInfos = nugetInfoExGroup.FileNugetInfos.Where(x => x.Version == selectedVersion).ToList();

            var targetFrameworks = selectedVersionNugetInfos.Where(x => x.TargetFramework != null)
                .Select(x => x.TargetFramework).Distinct().ToList();
            targetFrameworks.Sort();
            targetFrameworks.Reverse();
            var nugetDllInfos = selectedVersionNugetInfos.Where(x => x.NugetDllInfo != null)
                .Select(x => x.NugetDllInfo).Distinct().ToList();

            var dllPaths = nugetDllInfos.Select(x => x.DllPath).Distinct().ToList();
            if (dllPaths.Count > 1)
            {
                var errorMessage = "指定的修复策略存在多个 Dll 路径，修复工具无法确定应该使用哪一个。请保留现场并联系开发者。";
                var dllPathMessage = string.Empty;
                foreach (var dllPath in dllPaths)
                {
                    dllPathMessage = StringSplicer.SpliceWithNewLine(dllPathMessage, dllPath);
                }

                errorMessage = StringSplicer.SpliceWithDoubleNewLine(errorMessage, dllPathMessage);
                CustomText.Notification.ShowInfo(this, errorMessage);
                return null;
            }

            var nugetDllInfo = nugetDllInfos.FirstOrDefault();
            if (nugetDllInfo != null)
            {
                return new NugetFixStrategy(nugetName, selectedVersion, nugetDllInfo);
            }
            var targetFramework = targetFrameworks.FirstOrDefault();
            if (targetFramework == null)
            {
                return new NugetFixStrategy(nugetName, selectedVersion, new NugetDllInfo("", ""));
            }
            return new NugetFixStrategy(nugetName, selectedVersion, targetFramework);
        }
    }
}