using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter.Unity
{
    public abstract class InjectedMonoBehavior<TViewModel> : InjectedMonoBehavior
    {
        protected TViewModel ViewModel { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            ViewModel = Scope.ServiceProvider.GetRequiredService<TViewModel>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }
    }
}
