using UnityEngine;

namespace Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy
{
    [RequireComponent(typeof(MonoInjector))]
    public sealed class DisposingScript : MonoBehaviour
    {
        [Inject]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used for test")]
        private readonly TestDisposable _testDisposable;
    }
}
