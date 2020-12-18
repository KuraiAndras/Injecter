using UnityEngine;

namespace Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy
{
    public sealed class DisposingScript : MonoBehaviour
    {
        [Inject] private readonly ITestDisposable _testDisposable;
    }
}
