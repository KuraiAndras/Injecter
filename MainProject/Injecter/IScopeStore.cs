using Microsoft.Extensions.DependencyInjection;

namespace Injecter
{
    public interface IScopeStore
    {
        int NumberOfScopes { get; }

        IServiceScope CreateScope(object owner);
        void DisposeScope(object owner);
        IServiceScope? GetScope(object owner);
        void ClearAllScopes();
    }
}
