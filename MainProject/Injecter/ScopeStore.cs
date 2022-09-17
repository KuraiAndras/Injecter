using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Injecter
{
    public sealed class ScopeStore : IScopeStore, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<object, IServiceScope> _scopes = new();

        public int NumberOfScopes => _scopes.Count;

        public ScopeStore(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IServiceScope CreateScope(object owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            var scope = _serviceProvider.CreateScope();
            _scopes.TryAdd(owner, scope);

            return scope;
        }

        public IServiceScope? GetScope(object owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            _scopes.TryGetValue(owner, out var scope);

            return scope;
        }

        public void DisposeScope(object owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            if (!_scopes.TryGetValue(owner, out var scope)) return;

            scope.Dispose();

            _scopes.TryRemove(owner, out _);
        }

        public void ClearAllScopes()
        {
            foreach (var scope in _scopes)
            {
                scope.Value.Dispose();
            }

            _scopes.Clear();
        }

        public void Dispose() => ClearAllScopes();
    }
}
