using Injecter.Tests.Arrangers.Services;

namespace Injecter.Tests.Arrangers
{
    public abstract class AbstractInjectTarget
    {
        [Inject] protected ISimpleService? BaseSimpleService { get; set; }
    }
}
