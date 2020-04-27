using MediatR;
using System;

namespace Injecter.FastMediatR
{
    public interface IFastMediatR : IDisposable
    {
        void SendSync(IRequest request);

        TResponse SendSync<TResponse>(IRequest<TResponse> request);
    }
}
