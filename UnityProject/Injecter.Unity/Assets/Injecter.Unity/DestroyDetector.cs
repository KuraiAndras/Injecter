#nullable enable
using System;
using UnityEngine;

namespace Injecter.Unity
{
    internal sealed class DestroyDetector : MonoBehaviour
    {
        private IDisposable[]? _disposables;

        internal void RegisterDisposables(IDisposable[] disposables)
        {
            DisposeAll();

            _disposables = disposables;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "UnityFunction")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "FalsePositive")]
        private void Awake() => hideFlags = HideFlags.HideInInspector;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "UnityFunction")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "FalsePositive")]
        private void OnDestroy() => DisposeAll();

        private void DisposeAll()
        {
            if (_disposables is null) return;

            for (var i = 0; i < _disposables.Length; i++)
            {
                _disposables[i].Dispose();
            }
        }
    }
}
