using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;

namespace Injecter.Unity
{
    public abstract class InjectStarter : MonoBehaviour
    {
        protected virtual void Awake()
        {
            var serviceProvider = CreateServiceProvider();

            var options = serviceProvider.GetRequiredService<SceneInjectorOptions>();

            switch (options.InjectionBehavior)
            {
                case SceneInjectorOptions.Behavior.Factory:
                    serviceProvider.GetRequiredService<ISceneInjector>().InitializeScene(this, options.CreateScopes);
                    break;
                case SceneInjectorOptions.Behavior.CompositionRoot:
                    CompositionRoot.ServiceProvider = serviceProvider;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(options.InjectionBehavior.ToString());
            }
        }

        protected abstract IServiceProvider CreateServiceProvider();
    }
}
