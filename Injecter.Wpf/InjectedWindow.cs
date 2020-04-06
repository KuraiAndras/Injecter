using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Injecter.Wpf
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedWindow : Window
    {
        protected InjectedWindow()
        {
            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

            Unloaded += OnUnloadedHandler;
        }

        protected IServiceScope Scope { get; }

        protected virtual void OnUnloadedHandler(object o, RoutedEventArgs rea)
        {
            Scope?.Dispose();

            Unloaded -= OnUnloadedHandler;
        }
    }

    public abstract class InjectedWindow<TViewModel> : InjectedWindow
    {
        protected InjectedWindow() => Loaded += OnLoadedHandler;

        [Inject] protected TViewModel ViewModel { get; } = default;

        protected override void OnUnloadedHandler(object o, RoutedEventArgs rea)
        {
            base.OnUnloadedHandler(o, rea);

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }

        protected virtual void OnLoadedHandler(object o, RoutedEventArgs rea)
        {
            DataContext = ViewModel;

            Loaded -= OnLoadedHandler;
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
