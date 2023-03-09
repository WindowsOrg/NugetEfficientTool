using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NugetEfficientTool.Business;
using NugetEfficientTool.Utils;
using Path = System.IO.Path;

namespace NugetEfficientTool
{
    /// <summary>
    /// 操作项目替换<see cref="NugetReplaceView"/>的容器
    /// </summary>
    public partial class ReplaceViewContainer : UserControl
    {
        public ReplaceViewContainer()
        {
            InitializeComponent();
            Loaded += ReplaceViewContainer_Loaded;
        }

        private void ReplaceViewContainer_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ReplaceViewContainer_Loaded;
            //加载初始化
            var projectSolutions = NugetReplaceConfigs.GetSolutions();
            if (projectSolutions.Count == 0)
            {
                var nugetReplaceView = new NugetReplaceView();
                var replaceViewItem = new ReplaceProjectMode(nugetReplaceView);
                var tabItem = new TabItem() { DataContext = replaceViewItem };
                ProjectTabs.Items.Add(tabItem);
            }
            else
            {
                foreach (var projectSolution in projectSolutions)
                {
                    var solutionFile = projectSolution.SolutionFile;
                    var nugetReplaceView = new NugetReplaceView();
                    var replaceViewItem = new ReplaceProjectMode(solutionFile, projectSolution.Id, nugetReplaceView);
                    var tabItem = new TabItem() { DataContext = replaceViewItem };
                    ProjectTabs.Items.Add(tabItem);
                }

                AddProjectButton.Visibility = Visibility.Visible;
            }
            var projectTabsItem = ProjectTabs.Items[0];
            if (projectTabsItem is FrameworkElement element && element.DataContext is ReplaceProjectMode lastView)
            {
                ProjectContent.Child = lastView.ReplaceView;
            }
        }

        private void TabItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ReplaceProjectMode viewItem)
            {
                ProjectContent.Child = viewItem.ReplaceView;
            }
        }
        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProjectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var nugetReplaceView = new NugetReplaceView();
            var replaceViewItem = new ReplaceProjectMode(nugetReplaceView);
            var tabItem = new TabItem() { DataContext = replaceViewItem };
            ProjectTabs.Items.Add(tabItem);
            ProjectTabs.SelectedItem = tabItem;

            ProjectContent.Child = nugetReplaceView;
        }
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ProjectTabs.Items.Count == 1)
            {
                return;
            }
            if (!(sender is Button button))
            {
                return;
            }
            //删除
            var tabItem = button.VisualAncestorByInterface<TabItem>();
            ProjectTabs.Items.Remove(tabItem);
            if (tabItem.DataContext is ReplaceProjectMode projectMode)
            {
                var solutions = NugetReplaceConfigs.GetSolutions();
                var solution = solutions.FirstOrDefault(i => i.Id == projectMode.Id);
                if (solution != null)
                {
                    solutions.Remove(solution);
                    NugetReplaceConfigs.SaveSolutions(solutions);
                }
            }
            //显示下一个
            var projectTabsItem = ProjectTabs.Items[ProjectTabs.Items.Count - 1];
            if (projectTabsItem is FrameworkElement element && element.DataContext is ReplaceProjectMode lastView)
            {
                ProjectContent.Child = lastView.ReplaceView;
            }
        }
    }
}
