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

        public IServiceScope CreateScope<T>(T owner) where T : notnull
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            var scope = _serviceProvider.CreateScope();
            _scopes.Add(owner, scope);

            return scope;
        }

        public IServiceScope? GetScope<T>(T owner) where T : notnull
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            _scopes.TryGetValue(owner, out var scope);

            return scope;
        }

        public void DisposeScope<T>(T owner) where T : notnull
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            if (!_scopes.TryGetValue(owner, out var scope)) return;

            scope.Dispose();

            _scopes.Remove(owner);
        }

        public void ClearAllScopes()
        {
            foreach (var scope in _scopes)
            {
                scope.Value.Dispose();
            }

            _scopes.Clear();
        }
    }
}
