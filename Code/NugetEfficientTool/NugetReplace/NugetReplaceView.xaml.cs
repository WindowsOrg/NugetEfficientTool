using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kybs0.Project;
using Microsoft.Win32;
using NugetEfficientTool.Business;
using static NuGet.Packaging.PackagingConstants;
using FolderHelper = NugetEfficientTool.Utils.FolderHelper;

namespace NugetEfficientTool
{
    /// <summary>
    /// Nuget包替换
    /// </summary>
    public partial class NugetReplaceView : UserControl, INugetReplaceView
    {
        public NugetReplaceView()
        {
            InitializeComponent();
            Loaded += NugetFixView_Loaded;
        }

        public void Init(string id, string solutionFile)
        {
            _id = id;
            SolutionTextBox.Text = solutionFile;
        }

        private void NugetFixView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= NugetFixView_Loaded;
            ViewModel.Initialize(this, _id, SolutionTextBox.Text);
            SolutionTextBox.TextChanged += SolutionTextBox_OnTextChanged;
        }

        private NugetReplaceViewModel ViewModel => DataContext as NugetReplaceViewModel;

        private async void SourceProjectTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is NugetReplaceItem replaceItem)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                var sourceCsprojFile = textBox.Text.Trim('"');
                // 其实输入的可能是文件夹
                try
                {
                    if (!File.Exists(sourceCsprojFile) && Directory.Exists(sourceCsprojFile))
                    {
                        var csProjList =
                            FolderHelper.GetFilesFromDirectory(sourceCsprojFile, "*.csproj").ToList();
                        //如果是只存在一个csproj文件，则直接显示出来
                        if (csProjList.Count == 1)
                        {
                            sourceCsprojFile = csProjList[0];
                        }
                    }
                }
                catch (Exception exception)
                {
                    NugetTools.Log.Error(exception);
                }
                textBox.Text = sourceCsprojFile;
                textBox.SelectionStart = textBox.Text.Length;
                //填充Nuget显示
                if (!string.IsNullOrEmpty(sourceCsprojFile) && File.Exists(sourceCsprojFile))
                {
                    replaceItem.NugetName = Path.GetFileNameWithoutExtension(sourceCsprojFile);
                }
            }
        }

        private async void NugetNameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.Text.Contains('"'))
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(10));
                textBox.Text = textBox.Text.Trim('"');
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private async void SolutionTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            var sourceText = SolutionTextBox.Text;
            var solutionFile = sourceText.Trim('"');
            // 其实输入的可能是文件夹
            try
            {
                if (!File.Exists(solutionFile) && Directory.Exists(solutionFile))
                {
                    if (SolutionFiles.TryGetSlnFiles(solutionFile, out var slnFiles)&&
                        slnFiles.Count==1)
                    {
                        solutionFile = slnFiles[0];
                    }
                }
            }
            catch (Exception exception)
            {
                NugetTools.Log.Error(exception);
            }
            //判断输入的解决方案，是否已添加
            var projectSolutions = NugetReplaceConfigs.GetSolutions();
            if (projectSolutions.Any(i => i.SolutionFile == solutionFile))
            {
                //已存在解决方案，则置空
                solutionFile = string.Empty;
                NugetTools.Notification.ShowInfo(Window.GetWindow(this), "此解决方案已添加");
            }
            if (sourceText != solutionFile)
            {
                SolutionTextBox.TextChanged -= SolutionTextBox_OnTextChanged;
                SolutionTextBox.Text = solutionFile;
                SolutionTextBox.TextChanged += SolutionTextBox_OnTextChanged;
                SolutionTextBox.SelectionStart = solutionFile.Length;
            }
            //保存
            if (!string.IsNullOrEmpty(solutionFile))
            {
                NugetReplaceCacheManager.SaveOrUpdateSolution(_id, solutionFile);
                SolutionFileUpdated?.Invoke(this, solutionFile);
            }
        }

        private void ReplacingItem_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement uiElement && uiElement.DataContext is NugetReplaceItem item)
            {
                if (e.OriginalSource is CheckBox || e.OriginalSource is TextBox || e.OriginalSource is Button)
                {
                    return;
                }
                item.IsSelected = !item.IsSelected;
                ViewModel.UpdateOperationStatus();
            }
        }
        public Window Window => Window.GetWindow(this);
        private void NugetCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateOperationStatus();
        }
        private string _id = string.Empty;
        public event EventHandler<string> SolutionFileUpdated;
    }

    public interface INugetReplaceView
    {
        Window Window { get; }
    }
}
