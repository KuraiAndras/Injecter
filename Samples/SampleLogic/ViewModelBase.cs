using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SampleLogic
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetAndNotifyProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
        {
            if (backingField?.Equals(value) == true) return;

            backingField = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void InvokePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
