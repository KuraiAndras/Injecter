using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;

namespace Injecter.Unity.Tests.Arrange.ComplexSceneInjectedProperly
{
    [DefaultExecutionOrder(-999)]
    public sealed class ComplexInjector : InjectStarter
    {
        protected override IServiceProvider CreateServiceProvider() => new ServiceCollection().AddSceneInjector().AddComplexInjection().BuildServiceProvider();
    }
}
