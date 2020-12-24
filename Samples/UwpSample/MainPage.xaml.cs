using Injecter.Uwp;
using Microsoft.UI.Xaml;

namespace UwpSample
{
    public sealed partial class MainPage
    {
        public MainPage() => InitializeComponent();

        private void OnClickButton(object sender, RoutedEventArgs e) => ButtonText.Text = "Clicked";
    }

    public sealed class asd : InjectedPage
    {

    }
}
