using UnityEngine;

namespace Injecter.Unity.Tests.Arrange.InjectorAwakeCalledFirst
{
    public sealed class AwakeListener : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private AwakeLogger _logger;
#pragma warning restore 649

        private void Awake() => _logger.CallerTypes.Add(GetType());
    }
}
