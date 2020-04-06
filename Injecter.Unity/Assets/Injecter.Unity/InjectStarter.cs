using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;

namespace Injecter.Unity
{
    /// <summary>
    /// Sets <see cref="CompositionRoot"/>
    /// Set Script Execution Order to early.
    /// </summary>
    public abstract class InjectStarter : MonoBehaviour
    {
        protected virtual void Awake()
        {
            var serviceProvider = CreateServiceProvider();

            var options = serviceProvider.GetRequiredService<SceneInjectorOptions>();

            switch (options.InjectionBehavior)
            {
                case SceneInjectorOptions.Behavior.Factory:
                    gameObject.AddComponent<SceneInjector>().InitializeScene(serviceProvider);
                    break;
                case SceneInjectorOptions.Behavior.CompositionRoot:
                    CompositionRoot.ServiceProvider = CreateServiceProvider();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(options.InjectionBehavior.ToString());
            }
        }

        protected abstract IServiceProvider CreateServiceProvider();
    }
}
