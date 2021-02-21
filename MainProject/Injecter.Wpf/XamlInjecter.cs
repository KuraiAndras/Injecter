using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Windows;

namespace Injecter.Wpf
{
    public static class XamlInjecter
    {
        public static readonly DependencyProperty InjectProperty = DependencyProperty.RegisterAttached("Inject", typeof(bool?), typeof(XamlInjecter), new PropertyMetadata(null, DoInjection));

        public static bool? GetInject(DependencyObject obj) => (bool?)obj.GetValue(InjectProperty);
        public static void SetInject(DependencyObject obj, bool? value) => obj.SetValue(InjectProperty, value);

        private static void DoInjection(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(d.GetType(), d, false);
        }

        public static readonly DependencyProperty InjectScopedProperty = DependencyProperty.RegisterAttached("InjectScoped", typeof(DisposeBehaviour?), typeof(XamlInjecter), new PropertyMetadata(null, DoInjectionScoped));

        public static DisposeBehaviour? GetInjectScoped(DependencyObject obj) => (DisposeBehaviour?)obj.GetValue(InjectScopedProperty);
        public static void SetInjectScoped(DependencyObject obj, DisposeBehaviour value) => obj.SetValue(InjectScopedProperty, value);

        private static void DoInjectionScoped(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            CompositionRoot.ServiceProvider
                .GetRequiredService<IInjecter>()
                .InjectIntoType(d.GetType(), d, true);

            var behavior = (DisposeBehaviour?)e.NewValue;

            var owner = (FrameworkElement)d;

            switch (behavior)
            {
                case DisposeBehaviour.OnWindowClose:
                    {
                        void OnLoaded(object sender, EventArgs eventArgs)
                        {
                            owner.Loaded -= OnLoaded;
                            owner = null!;

                            var window = Window.GetWindow(d);

                            void OnWindowClosed(object? _, EventArgs __)
                            {
                                window.Closed -= OnWindowClosed;
                                window = null;

                                CleanUp(ref owner);
                            }

                            window!.Closed += OnWindowClosed;
                        }

                        owner.Loaded += OnLoaded;

                        break;
                    }
                case DisposeBehaviour.OnDispatcherShutdown:
                    {
                        void OnControlShutdown(object? sender, EventArgs __)
                        {
                            Application.Current.Dispatcher.ShutdownFinished -= OnControlShutdown;

                            CleanUp(ref owner);
                        }

                        Application.Current.Dispatcher.ShutdownStarted += OnControlShutdown;

                        break;
                    }
                case DisposeBehaviour.OnUnloaded:
                    {
                        void OnControlUnloaded(object? _, RoutedEventArgs __)
                        {
                            owner.Unloaded -= OnControlUnloaded;

                            CleanUp(ref owner);
                        }

                        owner.Unloaded += OnControlUnloaded;

                        break;
                    }
                case DisposeBehaviour.Manual: break;
                default: throw new ArgumentOutOfRangeException(behavior.ToString(), behavior, "Dispose behaviour not found");
            }
        }

        public static void CleanUp(ref FrameworkElement owner)
        {
            CompositionRoot.ServiceProvider
                .GetRequiredService<IScopeStore>()
                .DisposeScope(owner);

            if (owner is IDisposable disposable) disposable.Dispose();
            owner = null!;
        }
    }
}
