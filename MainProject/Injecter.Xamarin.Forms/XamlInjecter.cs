using Microsoft.Extensions.DependencyInjection;
using System;
using Xamarin.Forms;

namespace Injecter.Xamarin.Forms
{
    public static class XamlInjecter
    {
        public static readonly BindableProperty InjectProperty =
            BindableProperty.CreateAttached(
                "Inject",
                typeof(bool?),
                typeof(XamlInjecter),
                default,
                propertyChanged: OnInjectChanged);

        public static bool? GetInject(BindableObject view) => (bool?)view.GetValue(InjectProperty);

        public static void SetInject(BindableObject view, bool? value) => view.SetValue(InjectProperty, value);

        private static void OnInjectChanged(BindableObject bindable, object _, object __)
        {
            if (DesignMode.IsDesignModeEnabled) return;

            CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(bindable.GetType(), bindable, false);
        }

        public static readonly BindableProperty InjectScopedProperty =
            BindableProperty.CreateAttached(
                "InjectScoped",
                typeof(DisposeBehaviour?),
                typeof(XamlInjecter),
                default,
                propertyChanged: OnInjectScopedChanged);

        public static DisposeBehaviour? GetInjectScoped(BindableObject view) => (DisposeBehaviour?)view.GetValue(InjectProperty);

        public static void SetInjectScoped(BindableObject view, DisposeBehaviour? value) => view.SetValue(InjectProperty, value);

        private static void OnInjectScopedChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (DesignMode.IsDesignModeEnabled) return;

            CompositionRoot.ServiceProvider
                .GetRequiredService<IInjecter>()
                .InjectIntoType(bindable.GetType(), bindable, true);

            var behavior = (DisposeBehaviour?)newvalue;

            switch (behavior)
            {
                case DisposeBehaviour.OnDisappearing:

                    switch (bindable)
                    {
                        case Page page:
                            {
                                void OnControlDisappearing(object _, EventArgs __)
                                {
                                    page.Disappearing -= OnControlDisappearing;

                                    Cleanup(ref page);
                                }

                                page.Disappearing += OnControlDisappearing;
                                break;
                            }
                        case Cell cell:
                            {
                                void OnControlDisappearing(object _, EventArgs __)
                                {
                                    cell.Disappearing -= OnControlDisappearing;

                                    Cleanup(ref cell);
                                }

                                cell.Disappearing += OnControlDisappearing;
                                break;
                            }
                        case BaseShellItem baseShellItem:
                            {
                                void OnControlDisappearing(object _, EventArgs __)
                                {
                                    baseShellItem.Disappearing -= OnControlDisappearing;

                                    Cleanup(ref baseShellItem);
                                }

                                baseShellItem.Disappearing += OnControlDisappearing;
                                break;
                            }
                        default: throw new InvalidOperationException($"Bindable does not have event OnDisappearing. Type: {bindable.GetType().FullName}");
                    }

                    break;
                case DisposeBehaviour.Manual: break;
                default: throw new ArgumentOutOfRangeException(behavior.ToString(), behavior, "Dispose behaviour not found");
            }
        }

        private static void Cleanup<T>(ref T item)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (item is IDisposable d) d.Dispose();

            item = default!;
        }
    }
}
