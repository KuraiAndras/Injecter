#nullable enable
using UnityEngine;

namespace Injecter.Unity
{
    public sealed class DefaultGameObjectFactory : IGameObjectFactory
    {
        private readonly ISceneInjector _sceneInjector;

        public DefaultGameObjectFactory(ISceneInjector sceneInjector) => _sceneInjector = sceneInjector;

        public GameObject Instantiate(GameObject original, bool createScopes) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original), createScopes);

        public GameObject Instantiate(GameObject original, Transform parent, bool createScopes) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original, parent), createScopes);

        public GameObject Instantiate(GameObject original, Transform parent, bool instantiateInWorldSpace, bool createScopes) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original, parent, instantiateInWorldSpace), createScopes);

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, bool createScopes) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original, position, rotation), createScopes);

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent, bool createScopes) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original, position, rotation, parent), createScopes);
    }
}
