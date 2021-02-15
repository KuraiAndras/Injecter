using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Injecter.Uwp
{
    public static class XamlInjecter
    {
        public static readonly DependencyProperty InjectProperty = DependencyProperty.RegisterAttached(
            "Inject",
            typeof(bool?),
            typeof(XamlInjecter),
            new PropertyMetadata(null, DoInjection));

        public static void SetInject(UIElement element, bool value) => element.SetValue(InjectProperty, value);
        public static bool GetInject(UIElement element) => (bool)element.GetValue(InjectProperty);

        private static void DoInjection(DependencyObject d, DependencyPropertyChangedEventArgs _)
        {
            if (DesignMode.DesignMode2Enabled || DesignMode.DesignModeEnabled) return;

            CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(d.GetType(), d, false);
        }
    }
}
