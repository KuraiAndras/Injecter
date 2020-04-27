using MediatR;

namespace Injecter.FastMediatR.Tests
{
    public sealed class Add : IRequest<int>
    {
        public int A { get; set; }

        public int B { get; set; }
    }
}
