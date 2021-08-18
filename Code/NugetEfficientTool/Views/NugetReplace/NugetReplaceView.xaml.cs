using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NugetEfficientTool.Business;

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
            UserOperationConfigHelper.SolutionFileUpdated += UserOperationConfigHelper_SolutionFileUpdated;
            SolutionTextBox.TextChanged += SolutionTextBox_OnTextChanged;
        }

        private void UserOperationConfigHelper_SolutionFileUpdated(object sender, string currentSolution)
        {
            SolutionTextBox.Text = currentSolution;
        }

        private void NugetFixView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= NugetFixView_Loaded;
            ViewModel.Initialize(this);
        }

        private NugetReplaceViewModel ViewModel => DataContext as NugetReplaceViewModel;

        private async void SourceProjectTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
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
                    if (SolutionFileHelper.TryGetSlnFile(solutionFile, out var slnFile))
                    {
                        solutionFile = slnFile;
                    }
                }
            }
            catch (Exception exception)
            {
                CustomText.Log.Error(exception);
            }
            if (sourceText != solutionFile)
            {
                SolutionTextBox.TextChanged -= SolutionTextBox_OnTextChanged;
                SolutionTextBox.Text = solutionFile;
                SolutionTextBox.TextChanged += SolutionTextBox_OnTextChanged;
                SolutionTextBox.SelectionStart = solutionFile.Length;
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
    }

    public interface INugetReplaceView
    {
        Window Window { get; }
    }
}
