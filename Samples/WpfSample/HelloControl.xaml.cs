using Injecter.Wpf;
using System.Windows;

namespace WpfSample
{
    public partial class HelloControl
    {
        public HelloControl() : base(DisposeBehaviour.OnWindowClose) => InitializeComponent();

        private void OnIncrementClicked(object sender, RoutedEventArgs e) => ViewModel.Increment();
    }
}
