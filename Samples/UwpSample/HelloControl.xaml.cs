using Injecter;
using SampleLogic;
using Windows.UI.Xaml;

namespace UwpSample
{
    public sealed partial class HelloControl
    {
        [Inject] public ICounter ViewModel { get; } = default!;

        public HelloControl()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private void OnIncrementClicked(object _, RoutedEventArgs __) => ViewModel.Increment();
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members
    }
}
