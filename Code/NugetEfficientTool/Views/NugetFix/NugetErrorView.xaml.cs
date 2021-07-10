using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            UserOperationConfigHelper.SolutionFileUpdated += UserOperationConfigHelper_SolutionFileUpdated;
        }

        private void UserOperationConfigHelper_SolutionFileUpdated(object sender, string currentSolution)
        {
            SolutionTextBox.Text = currentSolution;
        }

        private void NugetFixView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= NugetFixView_Loaded;
            SolutionTextBox.Text = UserOperationConfigHelper.GetSolutionFile();
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
                MessageBox.Show("解决方案路径不能为空…… 心急吃不了热豆腐……");
                return false;
            }
            if (!File.Exists(solutionFile))
            {
                MessageBox.Show("找不到指定的解决方案，这是啥情况？？？");
                return false;
            }
            UserOperationConfigHelper.SaveSolutionFile(solutionFile);
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

        //private void ButtonFixFormat_OnClick(object sender, RoutedEventArgs e)
        //{
        //    var idePath = TextBoxIdePath.Text;
        //    if (string.IsNullOrWhiteSpace(idePath))
        //    {
        //        MessageBox.Show("大佬，IDE 路径都还没配置，你这样我很难帮你办事啊……");
        //        return;
        //    }

        //    if (!File.Exists(idePath))
        //    {
        //        MessageBox.Show("找不到配置的 IDE，可能离家出走了吧……");
        //        return;
        //    }

        //    OpenFilesByIde(idePath, _nugetVersionChecker.ErrorFormatNugetConfigs.Select(x => x.FilePath).Distinct());
        //}

        private void OpenFilesByIde(string idePath, IEnumerable<string> filePaths)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("cmd.exe")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            try
            {
                process.Start();
                foreach (var filePath in filePaths)
                {
                    process.StandardInput.WriteLine($"\"{idePath}\" \"{filePath}\"");
                }
            }
            finally
            {
                process.Close();
            }
        }
        /// <summary>
        /// 修复Nuget版本问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixNugetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var nugetVersionFixWindow = new NugetVersionFixWindow(_nugetVersionChecker.MismatchVersionNugetInfoExs)
            {
                Owner = Window.GetWindow(this)
            };
            nugetVersionFixWindow.NugetFixStrategiesSelected += (o, args) =>
            {
                var nugetFixStrategies = args.NugetFixStrategies;
                if (nugetFixStrategies == null || !nugetFixStrategies.Any())
                {
                    return;
                }

                var repairLog = string.Empty;
                foreach (var mismatchVersionNugetInfoEx in _nugetVersionChecker.MismatchVersionNugetInfoExs)
                {
                    foreach (var nugetInfoEx in mismatchVersionNugetInfoEx.VersionUnusualNugetInfoExs)
                    {
                        var nugetConfigRepairer = new NugetConfigRepairer(nugetInfoEx.ConfigPath, nugetFixStrategies);
                        nugetConfigRepairer.Repair();
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
                MessageBox.Show(exception.Message);
            }
        }

        #region private fields

        private NugetVersionChecker _nugetVersionChecker;

        #endregion
    }
}
