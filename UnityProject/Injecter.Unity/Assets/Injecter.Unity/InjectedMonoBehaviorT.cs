#nullable enable
using System;

namespace Injecter.Unity
{
    public abstract class InjectedMonoBehaviour<TViewModel> : InjectedMonoBehaviour
    {
        protected InjectedMonoBehaviour(bool createScopes) : base(createScopes)
        {
        }

        [Inject] protected TViewModel ViewModel { get; } = default!;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (ViewModel is IDisposable disposable) disposable.Dispose();
        }
    }
}
