#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Injecter.Uwp
{
    public abstract class InjectedUserControl : UserControl
    {
        protected InjectedUserControl()
        {
            Scope = CompositionRoot.ServiceProvider is not null
                ? CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this)
                : null;

            Unloaded += UnloadHandler;
        }

        protected IServiceScope? Scope { get; }

        protected virtual void UnloadHandler(object o, RoutedEventArgs rea)
        {
            Scope?.Dispose();

            Unloaded -= UnloadHandler;
        }
    }

    public abstract class InjectedUserControl<TViewModel> : InjectedUserControl
    {
        protected InjectedUserControl() => Loaded += OnLoaded;

        [Inject] protected TViewModel ViewModel { get; } = default!;

        protected virtual void OnLoaded(object o, RoutedEventArgs rea)
        {
            DataContext = ViewModel;

            Loaded -= OnLoaded;
        }
    }
}
