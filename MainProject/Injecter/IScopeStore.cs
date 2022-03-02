using Microsoft.Extensions.DependencyInjection;

namespace Injecter
{
    public interface IScopeStore
    {
        IServiceScope CreateScope<T>(T owner) where T : notnull;
        void DisposeScope<T>(T owner) where T : notnull;
        IServiceScope? GetScope<T>(T owner) where T : notnull;
        void ClearAllScopes();
    }
}
