using UnityEngine;

namespace Injecter.Unity
{
    public interface ISceneInjector
    {
        /// <summary>
        /// Inject into all Game Objects in the current scene and set service provider
        /// </summary>
        /// <param name="injectStarter">The inject Starter instance</param>
        /// <param name="createScopes">Create a new scope for each injection</param>
        void InitializeScene(InjectStarter injectStarter, bool createScopes);

        /// <summary>
        /// Sets up usages of the InjectAttribute
        /// </summary>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="ComponentMissingException"/>
        /// <param name="gameObjectInstance">GameObject instance to inject into</param>
        /// <param name="createScopes">Create new scopes</param>
        /// <returns>Original instance</returns>
        GameObject InjectIntoGameObject(GameObject gameObjectInstance, bool createScopes);
    }
}
