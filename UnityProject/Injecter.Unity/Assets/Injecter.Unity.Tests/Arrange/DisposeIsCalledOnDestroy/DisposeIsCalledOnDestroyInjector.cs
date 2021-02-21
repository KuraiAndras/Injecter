using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;

namespace Injecter.Unity.Tests.Arrange.DisposeIsCalledOnDestroy
{
    [DefaultExecutionOrder(-999)]
    public sealed class DisposeIsCalledOnDestroyInjector : InjectStarter
    {
        protected override IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSceneInjector(sceneInjectorOptions: o => o.CreateScopes = true);

            services.AddSingleton<IListenToDispose, ListenToDispose>(_ => FindObjectOfType<ListenToDispose>());
            services.AddTransient<ITestDisposable, TestDisposable>();

            return services.BuildServiceProvider();
        }
    }
}
