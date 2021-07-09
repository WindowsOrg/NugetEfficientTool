using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public class DownloadStateInfo
    {
        public DownloadState ProgressState { get; set; } = DownloadState.Initial;
        public DownloadProgess Progess { get; set; }=new DownloadProgess();

        public int Maximum
        {
            get => Progess.Maximum;
            set => Progess.Maximum = value;
        }
        public int Minimun
        {
            get => Progess.Minimun;
            set => Progess.Minimun = value;
        }
        public int Value
        {
            get => Progess.Value;
            set => Progess.Value = value;
        }
    }

    public enum DownloadState
    {
        Initial,
        Downloading,
        DownloadCompleted,
        DownloadError
    }

    public class DownloadProgess
    {
        public int Value { get; set; } = 0;
        public int Maximum { get; set; } = 100;
        public int Minimun { get; set; } = 0;
    }
}
