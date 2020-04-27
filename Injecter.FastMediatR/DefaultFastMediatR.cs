using MediatR;
using System;
using System.Collections.Generic;

namespace Injecter.FastMediatR
{
#pragma warning disable CA1062 // Validate arguments of public methods
    public sealed class DefaultFastMediatR : IFastMediatR
    {
        private const string HandleMethodName = nameof(ISyncHandler<IRequest, Unit>.HandleSync);

        private readonly IServiceProvider _serviceProvider;

        private readonly Type _openHandlerType = typeof(ISyncHandler<,>);
        private readonly Type _openRegularHandlerType = typeof(IRequestHandler<,>);
        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        public DefaultFastMediatR(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public void Dispose()
        {
            if (_serviceProvider is IDisposable d) d.Dispose();
            _handlers.Clear();
        }

        public void SendSync(IRequest request) => SendSyncInternal(request);

        public TResponse SendSync<TResponse>(IRequest<TResponse> request) => SendSyncInternal(request);

        private TResponse SendSyncInternal<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            var handlerType = _openHandlerType.MakeGenericType(requestType, responseType);

            if (!_handlers.TryGetValue(handlerType, out var handler))
            {
                var regularHandlerType = _openRegularHandlerType.MakeGenericType(requestType, responseType);

                // ReSharper disable once AssignNullToNotNullAttribute
                handler = Delegate.CreateDelegate(handlerType, _serviceProvider.GetService(regularHandlerType), handlerType.GetMethod(HandleMethodName));
                _handlers.Add(handlerType, handler);
            }

            // ReSharper disable once PossibleNullReferenceException
            return (TResponse)handler.DynamicInvoke(request);
        }
    }
#pragma warning restore CA1062 // Validate arguments of public methods
}
