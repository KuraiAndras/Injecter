using MediatR;

namespace Injecter.FastMediatR
{
    public interface ISyncHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        TResponse HandleSync(TRequest request);
    }
}