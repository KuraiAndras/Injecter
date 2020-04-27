using MediatR;
using System;

namespace Injecter.FastMediatR
{
    public interface IFastMediator : IDisposable
    {
        void SendSync(IRequest request);

        TResponse SendSync<TResponse>(IRequest<TResponse> request);
    }
}
