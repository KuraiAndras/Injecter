using NUnit.Framework;

namespace Injecter.Unity.Tests.Arrange.ComplexSceneInjectedProperly
{
    public sealed class ComplexConcrete : ComplexBase
    {
        private TestService _testService1;

        [Inject]
        public void Initialize(TestService testService1) => _testService1 = testService1;

        public void AssertConcreteInjected() => Assert.NotNull(_testService1);
    }
}
