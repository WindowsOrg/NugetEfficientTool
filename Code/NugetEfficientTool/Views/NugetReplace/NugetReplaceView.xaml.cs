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
        private readonly UserOperationConfig _operationConfig;
        public NugetReplaceView()
        {
            InitializeComponent();
            _operationConfig = new UserOperationConfig();
            Loaded += NugetFixView_Loaded;
        }

        private void NugetFixView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= NugetFixView_Loaded;
            var currentSolution = _operationConfig.GetSolutionFile();
            SolutionTextBox.Text = currentSolution;
        }
        /// <summary>
        /// 替换源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplaceButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!GetInputSolutionFile(out var solutionFile)) return;

            try
            {
                _nugetReplaceService.Replace(solutionFile, NugetNameTextBox.Text,SourceProjectTextBox.Text);
                ReplaceButton.IsEnabled = false;
                RevertButton.IsEnabled = true;
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
            if (!GetInputSolutionFile(out var solutionFile)) return;
            try
            {
                _nugetReplaceService.Revert(solutionFile, NugetNameTextBox.Text, SourceProjectTextBox.Text);
            }
            catch (Exception exception)
            {
                NugetReplaceCacheManager.ClearReplacedNugetInfo(solutionFile, NugetNameTextBox.Text);
                MessageBox.Show(exception.Message);
                CustomText.Log.Error(exception);
            }
            ReplaceButton.IsEnabled = true;
            RevertButton.IsEnabled = false;
        }
        private bool GetInputSolutionFile(out string solutionFile)
        {
            SolutionTextBox.Text = SolutionTextBox.Text.Trim('"');
            solutionFile = SolutionTextBox.Text;

            if (string.IsNullOrWhiteSpace(solutionFile))
            {
                MessageBox.Show("源代码路径不能为空…… 心急吃不了热豆腐……");
                return true;
            }

            if (!File.Exists(solutionFile))
            {
                // 其实输入的可能是文件夹
                if (SolutionFileHelper.TryGetSlnFile(solutionFile, out var slnFile))
                {
                    solutionFile = slnFile;
                }
                else
                {
                    MessageBox.Show("找不到指定的解决方案，这是啥情况？？？");
                    return false;
                }
            }

            _operationConfig.SaveSolutionFile(solutionFile);
            return true;
        }
    }
}
