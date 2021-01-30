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

            if (CompositionRoot.ServiceProvider is null) return;

            CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(d.GetType(), d, false);
        }

        public static readonly DependencyProperty InjectScopedProperty = DependencyProperty.RegisterAttached("InjectScoped", typeof(DisposeBehaviour?), typeof(XamlInjecter), new PropertyMetadata(null, DoInjectionScoped));

        public static DisposeBehaviour? GetInjectScoped(DependencyObject obj) => (DisposeBehaviour?)obj.GetValue(InjectScopedProperty);
        public static void SetInjectScoped(DependencyObject obj, DisposeBehaviour value) => obj.SetValue(InjectScopedProperty, value);

        private static void DoInjectionScoped(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            if (CompositionRoot.ServiceProvider is null) return;

            var scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(d.GetType(), d);

            var behavior = (DisposeBehaviour?)e.NewValue;

            switch (behavior)
            {
                case DisposeBehaviour.OnWindowClose:
                    {
                        if (d is not FrameworkElement f) throw new InvalidOperationException($"{d} is not of type {nameof(FrameworkElement)}");

                        void OnLoaded(object sender, EventArgs eventArgs)
                        {
                            f.Loaded -= OnLoaded;
                            f = null!;

                            var window = Window.GetWindow(d);

                            void OnWindowClosed(object _, EventArgs __)
                            {
                                window.Closed -= OnWindowClosed;
                                window = null;

                                scope!.Dispose();
                                scope = null;

                                if (d is IDisposable disposable) disposable.Dispose();
                                d = null!;
                            }

                            window!.Closed += OnWindowClosed;
                        }

                        f.Loaded += OnLoaded;

                        break;
                    }
                case DisposeBehaviour.OnDispatcherShutdown:
                    {
                        void OnControlShutdown(object sender, EventArgs eventArgs)
                        {
                            Application.Current.Dispatcher.ShutdownFinished -= OnControlShutdown;

                            scope!.Dispose();
                            scope = null;

                            if (d is IDisposable disposable) disposable.Dispose();
                            d = null!;
                        }

                        Application.Current.Dispatcher.ShutdownStarted += OnControlShutdown;

                        break;
                    }
                case DisposeBehaviour.OnUnloaded:
                    {
                        if (d is not FrameworkElement f) throw new InvalidOperationException($"{d} is not of type {nameof(FrameworkElement)}");

                        void OnControlUnloaded(object sender, RoutedEventArgs routedEventArgs)
                        {
                            f.Unloaded -= OnControlUnloaded;
                            f = null!;

                            scope!.Dispose();
                            scope = null;

                            if (d is IDisposable disposable) disposable.Dispose();
                            d = null!;
                        }

                        f.Unloaded += OnControlUnloaded;

                        break;
                    }
                case DisposeBehaviour.Manual: break;
                default: throw new ArgumentOutOfRangeException(behavior.ToString(), behavior, "Dispose behaviour not found");
            }
        }
    }
}
