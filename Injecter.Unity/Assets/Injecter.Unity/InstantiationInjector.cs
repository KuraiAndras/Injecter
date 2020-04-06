using Microsoft.Extensions.DependencyInjection;
using System;
using Object = UnityEngine.Object;

namespace Injecter.Unity
{
    public static class InstantiationInjector
    {
        public static IServiceCollection AddInjectorServices(this IServiceCollection services, Action<InjecterOptions> optionsBuilder = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IGameObjectFactory, DefaultGameObjectFactory>();
            services.AddSingleton<ISceneInjector, SceneInjector>(_ => Object.FindObjectOfType<SceneInjector>());
            services.AddInjecter(optionsBuilder);

            return services;
        }
    }
}
