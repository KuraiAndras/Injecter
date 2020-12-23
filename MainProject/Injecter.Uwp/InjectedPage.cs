#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Injecter.Uwp
{
    public abstract class InjectedPage : Page
    {
        protected InjectedPage()
        {
            Scope = CompositionRoot.ServiceProvider is not null
                ? CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this)
                : null;

            Unloaded += OnUnloaded;
        }

        protected IServiceScope? Scope { get; }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Scope?.Dispose();

            Unloaded -= OnUnloaded;
        }
    }

    public abstract class InjectedPage<TViewModel> : InjectedPage
    {
        protected InjectedPage() => Loaded += OnLoaded;

        [Inject] protected TViewModel ViewModel { get; } = default!;

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ViewModel;

            Loaded -= OnLoaded;
        }
    }
}
