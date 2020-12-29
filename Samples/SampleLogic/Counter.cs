namespace SampleLogic
{
    public sealed class Counter : ViewModelBase, ICounter
    {
        private int _count;

        public int Count
        {
            get => _count;
            private set => SetAndNotifyProperty(ref _count, value);
        }

        public void Increment() => Count++;
    }
}
