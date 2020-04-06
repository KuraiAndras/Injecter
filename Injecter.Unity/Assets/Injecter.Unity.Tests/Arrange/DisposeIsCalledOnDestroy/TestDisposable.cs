namespace Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy
{
    public sealed class TestDisposable : ITestDisposable
    {
        private readonly IListenToDispose _listenToDispose;

        public TestDisposable(IListenToDispose listenToDispose) => _listenToDispose = listenToDispose;

        public void Dispose() => _listenToDispose.DisposeCalled = true;
    }
}
