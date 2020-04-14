using Injecter.Tests.Arrangers.Services;

namespace Injecter.Tests.Arrangers
{
    public sealed class SimpleInjectTarget
    {
        [Inject] private readonly ISimpleService? _simpleService = default;

        [Inject] public ISimpleService? SimpleService { get; } = default;

        public bool IsServiceNotNull => !(_simpleService is null);
    }
}
