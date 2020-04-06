using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;

namespace Injecter.Unity.Tests.Arrange.InjectorAwakeCalledFirst
{
    [DefaultExecutionOrder(-999)]
    public sealed class AwakeCalledFirstInjector : InjectStarter
    {
#pragma warning disable 649
        [SerializeField] private AwakeLogger _logger;
#pragma warning restore 649

        protected override IServiceProvider CreateServiceProvider()
        {
            _logger.CallerTypes.Add(GetType());

            return new ServiceCollection().AddSceneInjector().BuildServiceProvider();
        }
    }
}
