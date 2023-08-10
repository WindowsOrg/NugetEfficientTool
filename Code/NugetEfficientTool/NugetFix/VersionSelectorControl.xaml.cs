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
            nugetVersionList.Sort(NugetVersionContrast.DescendingCompare);
            nugetVersionList.Add(NugetVersion.IgnoreFix);
            ComboBoxNugetVersion.ItemsSource = nugetVersionList;
            ComboBoxNugetVersion.SelectedIndex = 0;
        }

        public string NugetName { get; }

        public string SelectedVersion => ComboBoxNugetVersion.Text;

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