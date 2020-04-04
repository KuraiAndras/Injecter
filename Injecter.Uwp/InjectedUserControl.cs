using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Injecter.Uwp
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedUserControl : UserControl
    {
        protected InjectedUserControl()
        {
            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

            Unloaded += UnloadHandler;
        }

        protected IServiceScope Scope { get; }

        protected virtual void UnloadHandler(object o, RoutedEventArgs rea)
        {
            Scope?.Dispose();

            Unloaded -= UnloadHandler;
        }
    }

    public abstract class InjectedUserControl<TViewModel> : InjectedUserControl
    {
        protected InjectedUserControl() => Loaded += OnLoaded;

        [Inject] protected TViewModel ViewModel { get; } = default;

        protected override void UnloadHandler(object o, RoutedEventArgs rea)
        {
            base.UnloadHandler(o, rea);

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }

        protected virtual void OnLoaded(object _, RoutedEventArgs __)
        {
            DataContext = ViewModel;

            Loaded -= OnLoaded;
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
