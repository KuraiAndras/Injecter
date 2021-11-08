using SampleLogic;
using System;

namespace Injecter.Wpf.Tests
{
    public sealed partial class ScopedHelloControl : IDisposable
    {
        [Inject] private readonly ICounter _counter = default!;

        public ScopedHelloControl()
        {
            InitializeComponent();
            DataContext = _counter;
        }

        public void Dispose() => _counter.Increment();
    }
}
