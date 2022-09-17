using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using UnityEngine;

namespace Injecter.Unity
{
    /// <summary>
    /// Will inject all injectable components on the current <see cref="GameObject"/>. Has execution order of <see cref="int.MinValue"/>
    /// </summary>
    [DefaultExecutionOrder(int.MinValue)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MonoDisposer))]
    public sealed class MonoInjector : MonoBehaviour
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Unity method")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Unity method")]
        private void Awake()
        {
            var injecter = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>();

            var components = GetComponents<MonoBehaviour>();

            var owners = new List<MonoBehaviour>(components.Length);
            for (var i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (component == null || component == this) continue;

                injecter.InjectIntoType(component, true);
                owners.Add(component);
            }

            gameObject.GetComponent<MonoDisposer>().Initialize(owners, CompositionRoot.ServiceProvider.GetRequiredService<IScopeStore>());
        }
    }
}
