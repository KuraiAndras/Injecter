using NUnit.Framework;
using UnityEngine;

namespace Injecter.Unity.Tests.Arrange.ComplexSceneInjectedProperly
{
    public abstract class ComplexBase : MonoBehaviour
    {
        [Inject] private readonly ITestService1 _testService1 = default;

        [Inject] protected ITestService1 TestService1 { get; } = default;

        public void AssertBaseInjected()
        {
            Assert.NotNull(_testService1);
            Assert.NotNull(TestService1);

            Assert.True(!ReferenceEquals(_testService1, TestService1));
        }
    }
}
