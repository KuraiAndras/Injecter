#nullable enable

namespace Injecter.Unity
{
    /// <summary>
    /// Inherit from this class to easily enable injection
    /// </summary>
    public abstract class MonoBehaviourInjected : InjectedMonoBehaviour
    {
        protected MonoBehaviourInjected() : base(false) { }
    }
}
