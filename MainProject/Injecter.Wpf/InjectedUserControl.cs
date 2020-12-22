using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Injecter.Wpf
{
    public abstract class InjectedUserControl : UserControl, IDisposable
    {
        private Window? _window;

        protected InjectedUserControl(DisposeBehaviour behavior = DisposeBehaviour.OnDispatcherShutdown)
        {
            Scope = CompositionRoot.ServiceProvider?.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);
            Behavior = behavior;
            Loaded += OnControlLoaded;
        }

        public DisposeBehaviour Behavior { get; }

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
        }

        private void OnControlLoaded(object _, RoutedEventArgs __)
        {
            Loaded -= OnControlLoaded;

            switch (Behavior)
            {
                case DisposeBehaviour.OnWindowClose:

                    _window = Window.GetWindow(this);

                    _window!.Closed += OnWindowClosed;

                    break;
                case DisposeBehaviour.OnDispatcherShutdown:

                    Dispatcher.ShutdownStarted += OnControlShutdown;

                    break;
                case DisposeBehaviour.OnUnloaded:

                    Unloaded += OnControlUnloaded;

                    break;
                case DisposeBehaviour.Manual: break;
                default: throw new ArgumentOutOfRangeException(Behavior.ToString(), Behavior, "Dispose behaviour not found");
            }
        }

        private void OnWindowClosed(object? _, EventArgs __)
        {
            if (_window is not null)
            {
                _window.Closed -= OnWindowClosed;
                _window = null;
            }

            Dispose();
        }

        private void OnControlUnloaded(object? _, EventArgs __)
        {
            Unloaded -= OnControlUnloaded;

            Dispose();
        }

        private void OnControlShutdown(object? _, EventArgs __)
        {
            Dispatcher.ShutdownStarted -= OnControlShutdown;

            Dispose();
        }
    }

    public abstract class InjectedUserControl<TViewModel> : InjectedUserControl
    {
        protected InjectedUserControl(DisposeBehaviour behavior = DisposeBehaviour.OnDispatcherShutdown)
            : base(behavior) => Loaded += OnLoadedHandler;

        [Inject] protected TViewModel ViewModel { get; } = default!;

        protected virtual void OnLoadedHandler(object o, RoutedEventArgs rea)
        {
            DataContext = ViewModel;

            Loaded -= OnLoadedHandler;
        }
    }
}
