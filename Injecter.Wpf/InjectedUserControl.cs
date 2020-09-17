using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Injecter.Wpf
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedUserControl : UserControl, IDisposable
    {
        protected InjectedUserControl()
        {
            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

            Dispatcher.ShutdownStarted += OnControlShutdown;
        }

        protected IServiceScope Scope { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;

            Scope?.Dispose();

            Dispatcher.ShutdownStarted -= OnControlShutdown;
        }

        private void OnControlShutdown(object sender, EventArgs e) => Dispose();
    }

    public abstract class InjectedUserControl<TViewModel> : InjectedUserControl
    {
        protected InjectedUserControl() => Loaded += OnLoadedHandler;

        [Inject] protected TViewModel ViewModel { get; } = default;

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing && ViewModel is IDisposable disposable) disposable.Dispose();

            base.Dispose(isDisposing);
        }

        protected virtual void OnLoadedHandler(object o, RoutedEventArgs rea)
        {
            DataContext = ViewModel;

            Loaded -= OnLoadedHandler;
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
