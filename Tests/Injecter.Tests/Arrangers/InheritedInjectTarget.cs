using Injecter.Tests.Arrangers.Services;

namespace Injecter.Tests.Arrangers
{
    public sealed class InheritedInjectTarget : AbstractInjectTarget
    {
        [Inject] private readonly ISimpleService? _simpleService = default;

        public bool IsServiceNotNull => BaseSimpleService is not null && _simpleService is not null;
    }
}
