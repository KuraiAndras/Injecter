using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UwpSample
{
    public sealed partial class MainPage : Page
    {
        public MainPage() => InitializeComponent();

        private void myButton_Click(object sender, RoutedEventArgs e) => MyButton.Content = "Clicked";
    }
}
