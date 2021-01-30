using Injecter;
using SampleLogic;
using System;
using System.Windows;

namespace WpfSample
{
    public partial class HelloControl
    {
        [Inject] private ICounter ViewModel { get; } = default!;

        public HelloControl() => InitializeComponent();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            DataContext = ViewModel;
        }

        private void OnIncrementClicked(object sender, RoutedEventArgs e) => ViewModel.Increment();
    }
}
