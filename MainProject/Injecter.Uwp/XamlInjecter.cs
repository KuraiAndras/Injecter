﻿using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.ApplicationModel;
using Windows.UI.Core;
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

            CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(d, false);
        }

        public static readonly DependencyProperty InjectScopedProperty = DependencyProperty.RegisterAttached(
            "InjectScoped",
            typeof(DisposeBehaviour?),
            typeof(XamlInjecter),
            new PropertyMetadata(null, DoInjectionScoped));

        public static void SetInjectScoped(UIElement element, DisposeBehaviour value) => element.SetValue(InjectScopedProperty, value);
        public static DisposeBehaviour GetInjectScoped(UIElement element) => (DisposeBehaviour)element.GetValue(InjectScopedProperty);

        private static void DoInjectionScoped(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignMode2Enabled || DesignMode.DesignModeEnabled) return;

            CompositionRoot.ServiceProvider
                .GetRequiredService<IInjecter>()
                .InjectIntoType(d, true);

            var behavior = (DisposeBehaviour?)e.NewValue;

            var owner = (FrameworkElement)d;

            switch (behavior)
            {
                case DisposeBehaviour.OnWindowClose:
                {
                    void OnLoaded(object _, RoutedEventArgs __)
                    {
                        owner.Loaded -= OnLoaded;

                        var window = Window.Current;

                        void OnWindowClosed(object ___, CoreWindowEventArgs ____)
                        {
                            // ReSharper disable once AccessToModifiedClosure
                            window.Closed -= OnWindowClosed;
                            window = null;

                            // ReSharper disable once AccessToModifiedClosure
                            CleanUp(ref owner);
                        }

                        window!.Closed += OnWindowClosed;
                    }

                    owner.Loaded += OnLoaded;

                    break;
                }
                case DisposeBehaviour.OnUnloaded:
                {
                    // ReSharper disable AccessToModifiedClosure
                    void OnControlUnloaded(object _, RoutedEventArgs __)
                    {
                        owner.Unloaded -= OnControlUnloaded;

                        CleanUp(ref owner);
                    }
                    // ReSharper restore AccessToModifiedClosure

                    owner.Unloaded += OnControlUnloaded;

                    break;
                }
                case DisposeBehaviour.Manual: break;
                default: throw new ArgumentOutOfRangeException(behavior.ToString(), behavior, "Dispose behaviour not found");
            }
        }

        private static void CleanUp(ref FrameworkElement owner)
        {
            CompositionRoot.ServiceProvider
                .GetRequiredService<IScopeStore>()
                .DisposeScope(owner);

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (owner is IDisposable disposable) disposable.Dispose();
            owner = null!;
        }
    }
}
