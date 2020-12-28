using Injecter.Uwp;
using SampleLogic;
using Windows.UI.Xaml;

namespace UwpSample
{
    public abstract class HelloControlBase : InjectedUserControl<ICounter>
    {
    }

    public sealed partial class HelloControl
    {
        public HelloControl() => InitializeComponent();

        private void OnIncrementClicked(object _, RoutedEventArgs __) => ViewModel.Increment();
    }
}
