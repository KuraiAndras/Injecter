using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Injecter.Unity
{
    public sealed class SceneInjector : ISceneInjector
    {
        private readonly SceneInjectorOptions _options;
        private readonly IInjecter _injecter;

        public SceneInjector(SceneInjectorOptions options, IInjecter injecter)
        {
            _options = options;
            _injecter = injecter;
        }

        public void InitializeScene(InjectStarter injectStarter, bool createScopes)
        {
            foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                InjectIntoGameObject(rootGameObject, createScopes);
            }

            if (_options.DontDestroyOnLoad) UnityEngine.Object.DontDestroyOnLoad(injectStarter);
        }

        public GameObject InjectIntoGameObject(GameObject gameObjectInstance, bool createScopes)
        {
            if (gameObjectInstance == null) throw new ArgumentNullException(nameof(gameObjectInstance));

            var componentsToInject = gameObjectInstance
                .GetComponentsInChildren(typeof(MonoBehaviour), true)
                .Select(c =>
                {
                    if (c == null) throw new ComponentMissingException(gameObjectInstance.name);
                    return (c.GetType(), (object)c, c.gameObject);
                });

            var destroyDictionary = new Dictionary<GameObject, List<IDisposable>>();

            foreach (var (type, instance, componentGameObject) in componentsToInject)
            {
                var instanceScope = _injecter.InjectIntoType(type, instance, createScopes);

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
    }
}
