using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Injecter.Uwp
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedPage : Page
    {
        protected InjectedPage()
        {
            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

            Unloaded += OnUnloaded;
        }

        protected IServiceScope Scope { get; }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Scope?.Dispose();

            Unloaded -= OnUnloaded;
        }
    }

    public abstract class InjectedPage<TViewModel> : InjectedPage
    {
        protected InjectedPage() => Loaded += OnLoaded;

        [Inject] protected TViewModel ViewModel { get; } = default;

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ViewModel;

            Loaded -= OnLoaded;
        }

        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            base.OnUnloaded(sender, e);

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
