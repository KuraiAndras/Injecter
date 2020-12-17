using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Windows;

namespace Injecter.Wpf
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedWindow : Window, IDisposable
    {
        protected InjectedWindow()
        {
            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

            Closing += OnClosing;
        }

        protected IServiceScope? Scope { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;

            Scope?.Dispose();

            Closing -= OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Scope?.Dispose();

            Closing -= OnClosing;
        }
    }

    public abstract class InjectedWindow<TViewModel> : InjectedWindow
    {
        protected InjectedWindow() => Loaded += OnLoadedHandler;

        [Inject] protected TViewModel ViewModel { get; } = default!;

        protected virtual void OnLoadedHandler(object o, RoutedEventArgs rea)
        {
            DataContext = ViewModel;

            Loaded -= OnLoadedHandler;
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
