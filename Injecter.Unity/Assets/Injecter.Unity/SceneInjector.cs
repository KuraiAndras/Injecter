using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Injecter.Unity
{
    public sealed class SceneInjector : MonoBehaviour, ISceneInjector
    {
        private readonly SceneInjectorOptions _options = new SceneInjectorOptions();

        private IServiceProvider _serviceProvider;
        private IInjecter _injecter;

        public void InitializeScene(IServiceProvider serviceProvider, Action<SceneInjectorOptions> optionsBuilder = default)
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            if (_serviceProvider is ServiceProvider sp) sp.Dispose();

            _serviceProvider = serviceProvider;

            _injecter = _serviceProvider.GetRequiredService<IInjecter>();

            optionsBuilder?.Invoke(_options);

            foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                InjectIntoGameObject(rootGameObject);
            }

            if (_options.DontDestroyOnLoad) DontDestroyOnLoad(this);
        }

        public GameObject InjectIntoGameObject(GameObject gameObjectInstance)
        {
            if (gameObjectInstance is null) throw new ArgumentNullException(nameof(gameObjectInstance));

            var componentsToInject = gameObjectInstance
                .GetComponentsInChildren(typeof(MonoBehaviour), true)
                .Select(c => (c.GetType(), (object)c, c.gameObject));

            var destroyDictionary = new Dictionary<GameObject, List<IDisposable>>();

            foreach (var (type, instance, componentGameObject) in componentsToInject)
            {
                var instanceScope = _injecter.InjectIntoType(type, instance);

                if (instanceScope is null) continue;

                if (destroyDictionary.TryGetValue(componentGameObject, out var disposables))
                {
                    disposables.Add(instanceScope);
                }
                else
                {
                    destroyDictionary.Add(componentGameObject, new List<IDisposable> { instanceScope });
                }
            }

            foreach (var destroyable in destroyDictionary)
            {
                destroyable.Key.AddComponent<DestroyDetector>().RegisterDisposables(destroyable.Value.ToArray());
            }

            return gameObjectInstance;
        }

        private void OnDestroy()
        {
            if (_serviceProvider is ServiceProvider sp) sp.Dispose();
        }
    }
}
