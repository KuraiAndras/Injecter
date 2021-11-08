using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter.Unity
{
    public static class InstantiationInjector
    {
        public static IServiceCollection AddSceneInjector(
            this IServiceCollection services,
            Action<InjecterOptions> injecterOptionsBuilder = null,
            Action<SceneInjectorOptions> sceneInjectorOptions = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            var options = new SceneInjectorOptions();

            sceneInjectorOptions?.Invoke(options);

            services.AddSingleton<IGameObjectFactory, DefaultGameObjectFactory>();
            services.AddSingleton<ISceneInjector, SceneInjector>();
            services.AddSingleton(options);

            services.AddInjecter(injecterOptionsBuilder);

            return services;
        }
    }
}
