using UnityEngine;

namespace Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy
{
    public sealed class ListenToDispose : MonoBehaviour, IListenToDispose
    {
        public bool DisposeCalled { get; set; } = false;
    }
}
