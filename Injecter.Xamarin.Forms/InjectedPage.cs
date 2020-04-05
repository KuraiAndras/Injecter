using Microsoft.Extensions.DependencyInjection;
using System;
using Xamarin.Forms;

namespace Injecter.Xamarin.Forms
{
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class InjectedPage : Page
    {
        protected InjectedPage() => Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

        protected IServiceScope Scope { get; }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Scope?.Dispose();
        }
    }

    public abstract class InjectedPage<TViewModel> : InjectedPage
    {
        [Inject] protected TViewModel ViewModel { get; } = default;

        protected override void OnAppearing()
        {
            BindingContext = ViewModel;

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
