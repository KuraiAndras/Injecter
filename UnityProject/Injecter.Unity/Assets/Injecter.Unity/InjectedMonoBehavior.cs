using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

namespace Injecter.Unity
{
    public abstract class InjectedMonoBehavior : MonoBehaviour
    {
        protected IServiceScope Scope { get; private set; }

        protected virtual void Awake() =>
            Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(GetType(), this);

        protected virtual void OnDestroy() => Scope?.Dispose();
    }
}
