using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        /// <summary>
        /// nuget修复版本选择窗口
        /// </summary>
        /// <param name="mismatchVersionNugetGroups">存在版本不匹配的Nuget列表</param>
        public FixingVersionSelectWindow(IEnumerable<FileNugetInfoGroup> mismatchVersionNugetGroups)
        {
            if (ReferenceEquals(mismatchVersionNugetGroups, null))
                throw new ArgumentNullException(nameof(mismatchVersionNugetGroups));

            InitializeComponent();
            _mismatchVersionNugetInfoExs = mismatchVersionNugetGroups.OrderBy(i=>i.NugetName);
            foreach (var mismatchVersionNugetInfoEx in _mismatchVersionNugetInfoExs)
            {
                var nugetName = mismatchVersionNugetInfoEx.NugetName;
                var repeatNugetVersions = mismatchVersionNugetInfoEx.FileNugetInfos.Select(x => x.Version).Distinct();
                var versionSelectControl = new NugetVersionSelectorControl(nugetName, repeatNugetVersions);
                NugetVersionsPanel.Children.Add(versionSelectControl);
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
                foreach (var child in NugetVersionsPanel.Children)
                {
                    if (!(child is NugetVersionSelectorControl nugetVersionSelectorUserControl))
                    {
                        continue;
                    }

                    var nugetName = nugetVersionSelectorUserControl.NugetName;
                    var selectedVersion = nugetVersionSelectorUserControl.SelectedVersion;
                    //过滤掉“忽略修复”
                    if (selectedVersion== NugetVersion.IgnoreFix)
                    {
                        continue;
                    }
                    var fixNugetStrategy = CreateVersionFixStrategy(nugetName, selectedVersion);
                    if (fixNugetStrategy == null)
                    {
                        continue;
                    }
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
                CustomText.Notification.ShowInfo(null,exception.Message);
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
            var nugetDllInfos = GetNugetDllInfos(selectedVersionNugetInfos);
            ////检测DllPath是否存在多个
            //if (!CheckMultiDllError(nugetDllInfos)) return null;
            //选择一个nugetDll路径，创建修复策略
            var nugetDllInfo = nugetDllInfos.Any(i => string.IsNullOrEmpty(i.DllPath)) ? nugetDllInfos.FirstOrDefault(i => string.IsNullOrEmpty(i.DllPath)) :
                nugetDllInfos.FirstOrDefault();
            if (nugetDllInfo != null)
            {
                return new NugetFixStrategy(nugetName, selectedVersion, nugetDllInfo);
            }
            //选择对应版本，来创建修复策略
            var targetFramework = GetTargetFramework(selectedVersionNugetInfos);
            if (targetFramework == null)
            {
                return new NugetFixStrategy(nugetName, selectedVersion, new NugetDllInfo("", ""));
            }

            string nugetDllPath = string.Empty;
            if (FindReplacingDllPath(nugetName, selectedVersion, targetFramework, out var dllFilePath))
            {
                nugetDllPath = dllFilePath;
            }
            return new NugetFixStrategy(nugetName, selectedVersion, targetFramework, nugetDllPath);
        }

        private bool CheckMultiDllError(List<NugetDllInfo> nugetDllInfos)
        {
            if (nugetDllInfos.Count > 1)
            {
                //如果是2个DLL路径，且其中有一个空值（空值一般是PackageReference），可以继续修复
                if (nugetDllInfos.Count == 2 && nugetDllInfos.Any(i => string.IsNullOrEmpty(i.DllPath)))
                {
                    return true;
                }
                //检测异常
                var multiDllReferenceError = string.Empty;
                foreach (var nugetDllInfo in nugetDllInfos)
                {
                    var lineMessage = nugetDllInfo.DllPath;
                    if (string.IsNullOrEmpty(lineMessage))
                    {
                        lineMessage = $"PackageReference {nugetDllInfo.DllFullName}";
                    }
                    multiDllReferenceError = StringSplicer.SpliceWithNewLine(multiDllReferenceError, lineMessage);
                }
                var errorMessage = "指定的修复策略存在多个 Dll 路径，修复工具无法确定应该使用哪一个。请保留现场并联系开发者。";
                errorMessage = StringSplicer.SpliceWithDoubleNewLine(errorMessage, multiDllReferenceError);
                CustomText.Notification.ShowInfo(null, errorMessage);
                return false;
            }

            return true;
        }

        private static string GetTargetFramework(List<FileNugetInfo> selectedVersionNugetInfos)
        {
            var targetFrameworks = selectedVersionNugetInfos.Where(x => x.TargetFramework != null)
                .Select(x => x.TargetFramework).Distinct().ToList();
            targetFrameworks.Sort();
            targetFrameworks.Reverse();
            var targetFramework = targetFrameworks.FirstOrDefault();
            return targetFramework;
        }

        private static List<NugetDllInfo> GetNugetDllInfos(List<FileNugetInfo> selectedVersionNugetInfos)
        {
            var nugetDllInfos = new List<NugetDllInfo>();
            foreach (var versionNugetInfo in selectedVersionNugetInfos)
            {
                if (versionNugetInfo.NugetDllInfo == null)
                {
                    continue;
                }
                
                var dllPath = versionNugetInfo.NugetDllInfo.DllPath ?? string.Empty;
                if (dllPath.Contains("packages"))
                {
                    var list = dllPath.Split(new[] { "\\packages\\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (list.Count > 1)
                    {
                        dllPath = $"\\packages\\{list.Last()}";
                    }
                }
                if (nugetDllInfos.Any(i => i.DllPath == dllPath || i.DllPath.EndsWith(dllPath)))
                {
                    continue;
                }

                nugetDllInfos.Add(versionNugetInfo.NugetDllInfo);
            }

            return nugetDllInfos;
        }

        private bool FindReplacingDllPath(string nugetName, string nugetVersion, string targetFramework, out string dllFilePath)
        {
            var userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var folder = Path.Combine(userProfileFolder, ".nuget", "packages", nugetName, nugetVersion, "lib",
                targetFramework);
            dllFilePath = Path.Combine(folder, $"{nugetName}.dll");
            // 不一定使用 nuget name 命名
            if (!File.Exists(dllFilePath))
            {
                string[] dllFileList;
                if (!Directory.Exists(folder))
                {
                    dllFileList = new string[0];
                }
                else
                {
                    dllFileList = Directory.GetFiles(folder, "*.dll");
                }
                if (dllFileList.Length == 0)
                {
                    Console.Error.WriteLine($"找不到 {dllFilePath}，可能无法进行正常修复。先试着编译一下，还原下 Nuget 包");
                    return false;
                }
                if (dllFileList.Length == 1)
                {
                    dllFilePath = dllFileList[0];
                }
                else
                {
                    var file = dllFileList.FirstOrDefault(temp => temp.ToLower().Contains(nugetName.ToLower()));
                    dllFilePath = file ?? dllFileList[0];
                }
            }
            return true;
        }
    }
}