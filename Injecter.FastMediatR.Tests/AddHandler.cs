using System.Threading;
using System.Threading.Tasks;

namespace Injecter.FastMediatR.Tests
{
    public sealed class AddHandler : ISyncHandler<Add, int>
    {
        public Task<int> Handle(Add request, CancellationToken cancellationToken) => Task.FromResult(HandleSync(request));

        public int HandleSync(Add request) => request.A + request.B;
    }
}
