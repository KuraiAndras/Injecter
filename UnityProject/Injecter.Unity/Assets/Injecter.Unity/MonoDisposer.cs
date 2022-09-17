using System.Collections.Generic;
using UnityEngine;

namespace Injecter.Unity
{
    /// <summary>
    /// Will dispose all scopes created while injecting current <see cref="GameObject"/>. Has execution order of <see cref="int.MaxValue"/>
    /// </summary>
    [DefaultExecutionOrder(int.MaxValue)]
    [DisallowMultipleComponent]
    public sealed class MonoDisposer : MonoBehaviour
    {
        private List<MonoBehaviour> _owners = new List<MonoBehaviour>();
        private IScopeStore _store;

        /// <summary>
        /// Initializes the disposer. Called during the injection by <see cref="MonoInjector"/>
        /// </summary>
        /// <param name="owners">The inejcted members</param>
        /// <param name="store"><see cref="IScopeStore"/> Shoul be the instance retrieved from the service provider</param>
        public void Initialize(List<MonoBehaviour> owners, IScopeStore store)
        {
            _owners = owners;
            _store = store;
        }

        /// <summary>
        /// Add a mew owner to dispose. Should be used when dynamically adding injected components to <see cref="GameObject"/>s
        /// </summary>
        /// <param name="owner">The injected member</param>
        public void AddOwner(MonoBehaviour owner) => _owners.Add(owner);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "Unity method")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Unity method")]
        private void OnDestroy()
        {
            if (_owners == null) return;

            for (var i = 0; i < _owners.Count; i++)
            {
                _store.DisposeScope(_owners[i]);
            }

            _owners.Clear();
            _owners = null;
            _store = null;
        }
    }
}
