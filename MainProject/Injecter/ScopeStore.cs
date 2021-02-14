using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Injecter
{
    public sealed class ScopeStore : IScopeStore
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<object, IServiceScope> _scopes = new();

        public ScopeStore(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IServiceScope CreateScope(object owner)
        {
            var scope = _serviceProvider.CreateScope();
            _scopes.Add(owner, scope);

            return scope;
        }

        public IServiceScope GetScope(object owner) => _scopes[owner];

        public void DisposeScope(object owner)
        {
            var scope = _scopes[owner];

            scope.Dispose();

            _scopes.Remove(owner);
        }
    }
}
