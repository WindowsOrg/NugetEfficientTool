using System.IO;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool
{
    public class NugetReplaceItem : BindingPropertyBase
    {
        public NugetReplaceItem(string argName, string argSourceCsprojPath)
        {
            _nugetName = argName;
            _sourceCsprojFile = argSourceCsprojPath;
        }

        public NugetReplaceItem()
        {
           
        }
        /// <summary>
        /// Nuget名称
        /// </summary>
        public string NugetName
        {
            get => _nugetName;
            set
            {
                _nugetName = value;
                OnPropertyChanged();
            }
        }

        private string _nugetName;
        /// <summary>
        /// Nuget对应的源代码引用
        /// </summary>
        public string SourceCsprojFile
        {
            get => _sourceCsprojFile;
            set
            {
                _sourceCsprojFile = value;
                if (!string.IsNullOrEmpty(_sourceCsprojFile) && File.Exists(value))
                {
                    NugetName = Path.GetFileNameWithoutExtension(value);
                }
                OnPropertyChanged();
            }
        }
        private string _sourceCsprojFile;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        private bool _isSelected = true;
        /// <summary>
        /// 是否已经替换
        /// </summary>
        public bool HasReplaced
        {
            get => _hasReplaced;
            set
            {
                _hasReplaced = value;
                OnPropertyChanged();
            }
        }
        private bool _hasReplaced;

    }
}
