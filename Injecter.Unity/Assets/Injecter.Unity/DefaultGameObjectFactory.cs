using UnityEngine;

namespace Injecter.Unity
{
    public sealed class DefaultGameObjectFactory : IGameObjectFactory
    {
        private readonly ISceneInjector _sceneInjector;

        public DefaultGameObjectFactory(ISceneInjector sceneInjector) => _sceneInjector = sceneInjector;

        public GameObject Instantiate(GameObject original) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original));

        public GameObject Instantiate(GameObject original, Transform parent) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original, parent));

        public GameObject Instantiate(GameObject original, Transform parent, bool instantiateInWorldSpace) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original, parent, instantiateInWorldSpace));

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original, position, rotation));

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent) =>
            _sceneInjector.InjectIntoGameObject(Object.Instantiate(original, position, rotation, parent));
    }
}
