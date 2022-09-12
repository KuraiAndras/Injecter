using System.Collections.Generic;
using UnityEngine;

namespace Injecter.Unity
{
    [DefaultExecutionOrder(int.MaxValue)]
    public sealed class MonoDisposer : MonoBehaviour
    {
        private List<MonoBehaviour> _owners;
        private IScopeStore _store;

        public void Initialize(List<MonoBehaviour> owners, IScopeStore store)
        {
            _owners = owners;
            _store = store;
        }

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
