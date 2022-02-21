#nullable enable
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

namespace Injecter.Unity
{
    public abstract class InjectedMonoBehavior : MonoBehaviour
    {
        private readonly bool _createScopes;

        protected InjectedMonoBehavior(bool createScopes = false) => _createScopes = createScopes;

        protected IServiceScope? Scope { get; private set; }

        protected virtual void Awake()
        {
            if (CompositionRoot.ServiceProvider is null)
            {
                Debug.LogError($"Trying to access the {nameof(CompositionRoot.ServiceProvider)} on {nameof(CompositionRoot)} before it has value");
                return;
            }

            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this, _createScopes);
        }

        protected virtual void OnDestroy()
        {
            if (Scope is null) return;

            Scope.ServiceProvider.GetRequiredService<IScopeStore>().DisposeScope(this);

            Scope = null;
        }
    }
}
