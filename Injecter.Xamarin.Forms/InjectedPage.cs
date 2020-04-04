using Microsoft.Extensions.DependencyInjection;
using System;
using Xamarin.Forms;

namespace Injecter.Xamarin.Forms
{
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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }
    }
}
