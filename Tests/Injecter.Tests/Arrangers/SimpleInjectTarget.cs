using Injecter.Tests.Arrangers.Services;

namespace Injecter.Tests.Arrangers
{
    public sealed class SimpleInjectTarget
    {
        [Inject] private readonly ISimpleService _simpleService = default!;

        private ISimpleService? _simpleService2;

        [Inject] public ISimpleService? SimpleService { get; } = default;

        public bool IsServiceNotNull => _simpleService is not null && _simpleService2 is not null;

        [Inject]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by Injecter")]
        private void Construct(ISimpleService simpleService) => _simpleService2 = simpleService;
    }
}
