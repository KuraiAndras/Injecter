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

        private void OnIncrementClicked(object _, RoutedEventArgs __) => ViewModel.Increment();
    }
}
