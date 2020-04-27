using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Injecter.FastMediatR.Tests
{
    public sealed class NoResponseHandler : ISyncHandler<NoResponse, Unit>
    {
        public Task<Unit> Handle(NoResponse request, CancellationToken cancellationToken) => Task.FromResult(HandleSync(request));

        public Unit HandleSync(NoResponse request) => Unit.Value;
    }
}