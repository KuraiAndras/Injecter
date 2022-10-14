using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

namespace Injecter.Unity
{
    /// <summary>
    /// Inheriting from this class will inject everything marked with the <see cref="InjectAttribute"/>
    /// </summary>
    public abstract class MonoBehaviourInjected : MonoBehaviour
    {
        [Inject] private IScopeStore _store;

        protected IServiceScope Scope { get; private set; }

        /// <summary>
        /// Call the base function before accessing injected members
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Unity method")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Unity method")]
        protected virtual void Awake() => Scope = CompositionRoot.ServiceProvider.GetRequiredService<IInjecter>().InjectIntoType(this, true);

        /// <summary>
        /// Call the base function after accessing injected members
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Unity method")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Unity method")]
        protected virtual void OnDestroy()
        {
            _store.DisposeScope(this);

            Scope = null;
            _store = null;
        }
    }
}
