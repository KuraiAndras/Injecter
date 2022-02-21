#nullable enable
using System;
using UnityEngine;

namespace Injecter.Unity
{
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable RCS1213 // Remove unused member declaration.
    internal sealed class DestroyDetector : MonoBehaviour
    {
        private IDisposable[] _disposables;

        internal void RegisterDisposables(IDisposable[] disposables)
        {
            DisposeAll();

            _disposables = disposables;
        }

        private void Awake() => hideFlags = HideFlags.HideInInspector;
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
#pragma warning restore RCS1213 // Remove unused member declaration.
#pragma warning restore IDE0051 // Remove unused private members
}
