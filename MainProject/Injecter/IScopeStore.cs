using Microsoft.Extensions.DependencyInjection;

namespace Injecter
{
    public interface IScopeStore
    {
        IServiceScope CreateScope(object owner);
        void DisposeScope(object owner);
        IServiceScope GetScope(object owner);
    }
}
