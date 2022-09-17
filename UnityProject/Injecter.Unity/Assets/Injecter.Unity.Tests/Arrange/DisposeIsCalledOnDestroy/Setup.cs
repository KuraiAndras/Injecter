using System;

namespace Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy
{
    public sealed class ListenToDispose
    {
        public bool DisposeCalled { get; set; } = false;
    }

    public sealed class TestDisposable : IDisposable
    {
        private readonly ListenToDispose _listenToDispose;

        public TestDisposable(ListenToDispose listenToDispose) => _listenToDispose = listenToDispose;

        public void Dispose() => _listenToDispose.DisposeCalled = true;
    }
}
