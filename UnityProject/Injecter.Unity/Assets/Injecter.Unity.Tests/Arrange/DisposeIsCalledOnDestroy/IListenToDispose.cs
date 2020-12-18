namespace Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy
{
    public interface IListenToDispose
    {
        bool DisposeCalled { get; set; }
    }
}
