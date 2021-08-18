using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NugetEfficientTool.Utils
{
    public abstract class BindingPropertyBase:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
