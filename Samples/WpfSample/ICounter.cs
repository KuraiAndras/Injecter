using System.ComponentModel;

namespace WpfSample
{
    public interface ICounter : INotifyPropertyChanged
    {
        int Count { get; }
        void Increment();
    }
}
