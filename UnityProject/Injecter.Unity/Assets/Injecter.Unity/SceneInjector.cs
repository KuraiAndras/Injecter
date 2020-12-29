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
        private SceneInjectorOptions _options;

        private IServiceProvider _serviceProvider;
        private IInjecter _injecter;

        public void InitializeScene(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _injecter = _serviceProvider.GetRequiredService<IInjecter>();

            foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                InjectIntoGameObject(rootGameObject);
            }

            _options = _serviceProvider.GetRequiredService<SceneInjectorOptions>();

            if (_options.DontDestroyOnLoad) DontDestroyOnLoad(gameObject);
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
            if (_serviceProvider is IDisposable disposable) disposable.Dispose();
        }
    }
}
