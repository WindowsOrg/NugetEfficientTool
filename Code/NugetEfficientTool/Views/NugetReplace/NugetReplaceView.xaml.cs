using System;
using System.Collections.Generic;
using System.IO;
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

namespace NugetEfficientTool
{
    /// <summary>
    /// Nuget包替换
    /// </summary>
    public partial class NugetReplaceView : UserControl
    {
        private readonly NugetReplaceService _nugetReplaceService = new NugetReplaceService();
        public NugetReplaceView()
        {
            InitializeComponent();
            Loaded += NugetFixView_Loaded;
            UserOperationConfigHelper.SolutionFileUpdated += UserOperationConfigHelper_SolutionFileUpdated;
        }

        private void UserOperationConfigHelper_SolutionFileUpdated(object sender, string currentSolution)
        {
            SolutionTextBox.Text = currentSolution;
        }

        private void NugetFixView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= NugetFixView_Loaded;
            var currentSolution = UserOperationConfigHelper.GetSolutionFile();
            SolutionTextBox.Text = currentSolution;
            var replaceNugetConfigs = UserOperationConfigHelper.GetNugetReplaceConfig();
            if (replaceNugetConfigs.Any())
            {
                NugetNameTextBox.Text = replaceNugetConfigs[0].Name;
                SourceProjectTextBox.Text = replaceNugetConfigs[0].SourceCsprojPath;
            }
        }
        /// <summary>
        /// 替换源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplaceButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!CheckInputText(out var solutionFile, out var nugetName, out var sourceCroProjFile)) return;
            try
            {
                var result = _nugetReplaceService.Replace(solutionFile, NugetNameTextBox.Text, SourceProjectTextBox.Text);
                if (result)
                {
                    ReplaceButton.IsEnabled = false;
                    RevertButton.IsEnabled = true;
                }
            }
            catch (Exception exception)
            {
                NugetReplaceCacheManager.ClearReplacedNugetInfo(solutionFile, NugetNameTextBox.Text);
                MessageBox.Show(exception.Message);
                CustomText.Log.Error(exception);
            }
        }
        /// <summary>
        /// 撤回原nuget引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RevertButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!CheckInputText(out var solutionFile, out var nugetName, out var sourceCroProjFile)) return;
            try
            {
                _nugetReplaceService.Revert(solutionFile, nugetName, sourceCroProjFile);
            }
            catch (Exception exception)
            {
                NugetReplaceCacheManager.ClearReplacedNugetInfo(solutionFile, nugetName);
                MessageBox.Show(exception.Message);
                CustomText.Log.Error(exception);
            }
            ReplaceButton.IsEnabled = true;
            RevertButton.IsEnabled = false;
        }
        private bool CheckInputText(out string solutionFile, out string nugetName, out string sourceCroProjFile)
        {
            solutionFile = SolutionTextBox.Text;
            nugetName = NugetNameTextBox.Text;
            sourceCroProjFile = SourceProjectTextBox.Text;
            if (string.IsNullOrWhiteSpace(solutionFile))
            {
                MessageBox.Show("解决方案路径不能为空…… 心急吃不了热豆腐……");
                return false;
            }
            if (string.IsNullOrWhiteSpace(nugetName))
            {
                MessageBox.Show("Nuget名称不能为空…… 心急吃不了热豆腐……");
                return false;
            }
            if (string.IsNullOrWhiteSpace(sourceCroProjFile))
            {
                MessageBox.Show("源代码路径不能为空…… 心急吃不了热豆腐……");
                return false;
            }
            if (!File.Exists(solutionFile))
            {
                MessageBox.Show("找不到指定的解决方案，这是啥情况？？？");
                return false;
            }
            UserOperationConfigHelper.SaveSolutionFile(solutionFile);
            UserOperationConfigHelper.SaveNugetReplaceConfig(new List<ReplaceNugetConfig>() { new ReplaceNugetConfig(nugetName, sourceCroProjFile) });
            return true;
        }

        private async void SourceProjectTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!SourceProjectTextBox.Text.Contains('"'))
            {
                return;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            SourceProjectTextBox.Text = SourceProjectTextBox.Text.Trim('"');
            SourceProjectTextBox.SelectionStart = SourceProjectTextBox.Text.Length;
        }

        private async void NugetNameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!NugetNameTextBox.Text.Contains('"'))
            {
                return;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            NugetNameTextBox.Text = NugetNameTextBox.Text.Trim('"');
            NugetNameTextBox.SelectionStart = NugetNameTextBox.Text.Length;
        }

        private async void SolutionTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!SolutionTextBox.Text.Contains('"'))
            {
                return;
            }
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
    }
}
