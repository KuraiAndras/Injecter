using System;
using UnityEngine;

namespace Injecter.Unity
{
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
}
