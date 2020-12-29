using System.ComponentModel;

namespace SampleLogic
{
    public interface ICounter : INotifyPropertyChanged
    {
        int Count { get; }
        void Increment();
    }
}
