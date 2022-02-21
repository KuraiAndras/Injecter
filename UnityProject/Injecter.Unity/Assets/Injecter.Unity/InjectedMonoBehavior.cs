#nullable enable
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

namespace Injecter.Unity
{
    public abstract class InjectedMonoBehavior : MonoBehaviour
    {
        private readonly bool _createScopes;

        protected InjectedMonoBehavior(bool createScopes = false) => _createScopes = createScopes;

        protected IServiceScope Scope { get; private set; }

        protected virtual void Awake() =>
            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this, _createScopes);

        protected virtual void OnDestroy() => Scope?.Dispose();
    }
}
