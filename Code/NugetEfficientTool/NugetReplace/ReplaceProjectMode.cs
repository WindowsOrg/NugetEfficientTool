using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NugetEfficientTool.Annotations;

namespace NugetEfficientTool
{
    internal class ReplaceProjectMode : INotifyPropertyChanged
    {
        public ReplaceProjectMode()
        {
        }
        public ReplaceProjectMode(NugetReplaceView nugetReplaceView) : this(string.Empty, Guid.NewGuid().ToString(), nugetReplaceView)
        {
        }
        public ReplaceProjectMode(string solutionFile, string id, NugetReplaceView nugetReplaceView)
        {
            SolutionFile = solutionFile;
            ReplaceView = nugetReplaceView;
            Id = id;
            ReplaceView.Init(Id, solutionFile);
            nugetReplaceView.SolutionFileUpdated += NugetReplaceView_SolutionFileUpdated;
        }
        private void NugetReplaceView_SolutionFileUpdated(object sender, string solutionFile)
        {
            SolutionFile = solutionFile;
        }

        public string Id { get; }

        public string SolutionFile
        {
            get => _solutionFile;
            set
            {
                _solutionFile = value;
                SolutionName = string.IsNullOrEmpty(value)
                    ? "Project"
                    : Path.GetFileNameWithoutExtension(value);
                OnPropertyChanged();
            }
        }
        private string _solutionName = "Project";

        public string SolutionName
        {
            get => _solutionName;
            set
            {
                _solutionName = value;
                OnPropertyChanged();
            }
        }

        public NugetReplaceView ReplaceView { get; }

        private string _solutionFile = string.Empty;
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
