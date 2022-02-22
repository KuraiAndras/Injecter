#nullable enable

namespace Injecter.Unity
{
    /// <summary>
    /// Inherit from this class to easily enable scoped injection
    /// </summary>
    public abstract class MonoBehaviourScoped : InjectedMonoBehaviour
    {
        protected MonoBehaviourScoped() : base(true) { }
    }
}
