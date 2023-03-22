using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NugetEfficientTool.Business;

namespace NugetEfficientTool
{
    /// <summary>
    /// NugetVersionSelectorUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class VersionSelectorControl : UserControl
    {
        public VersionSelectorControl(string nugetName, IEnumerable<string> nugetVersions)
        {
            InitializeComponent();
            NugetName = nugetName;
            TextBlockNugetName.Text = nugetName;
            var nugetVersionList = nugetVersions.ToList();
            nugetVersionList.Sort(NugetVersionDescendingComparison);
            nugetVersionList.Add(NugetVersion.IgnoreFix);
            ComboBoxNugetVersion.ItemsSource = nugetVersionList;
            ComboBoxNugetVersion.SelectedIndex = 0;
        }

        public string NugetName { get; }

        public string SelectedVersion => ComboBoxNugetVersion.Text;

        /// <summary>
        /// 版本号降序排序器
        /// </summary>
        /// <param name="x">版本号 X</param>
        /// <param name="y">版本号 Y</param>
        /// <returns>比较结果</returns>
        private int NugetVersionDescendingComparison(string x, string y)
        {
            if (x == y)
            {
                return 0;
            }

            var xMainVersion = GetMainVersion(x);
            var yMainVersion = GetMainVersion(y);
            var mainVersionCompareResult = -xMainVersion.CompareTo(yMainVersion);
            if (mainVersionCompareResult != 0)
            {
                return mainVersionCompareResult;
            }

            var xSubVersion = GetSubVersion(x);
            var ySubVersion = GetSubVersion(y);
            var subVersionCompareResult = -StringComparer.InvariantCultureIgnoreCase.Compare(xSubVersion, ySubVersion);
            return subVersionCompareResult;
        }

        /// <summary>
        /// 获取主版本号部分
        /// </summary>
        /// <param name="version">版本号字符串</param>
        /// <returns>版本号对象</returns>
        private Version GetMainVersion(string version)
        {
            var index = version.IndexOf('-');
            try
            {
                if (index < 0)
                {
                    return new Version(version);
                }

                return new Version(version.Substring(0, index));
            }
            catch (Exception)
            {
                CustomText.Notification.ShowInfo(Window.GetWindow(this), $"无法从 {version} 构造版本号对象。请保留现场，并联系开发者。");
                return new Version(0, 0);
            }
        }

        private string GetSubVersion(string version)
        {
            var index = version.IndexOf('-');
            return version.Substring(index + 1);
        }

        public void SelectHighVersion()
        {
            ComboBoxNugetVersion.SelectedIndex = 0;
        }
        public void SelectIgnoreVersion()
        {
            ComboBoxNugetVersion.SelectedIndex = ComboBoxNugetVersion.Items.Count - 1;
        }
    }
}