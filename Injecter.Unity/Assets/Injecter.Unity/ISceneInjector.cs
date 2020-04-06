using System;
using UnityEngine;

namespace Injecter.Unity
{
    public interface ISceneInjector
    {
        /// <summary>
        /// Inject into all Game Objects in the current scene and set service provider
        /// </summary>
        /// <param name="serviceProvider"></param>
        void InitializeScene(IServiceProvider serviceProvider);

        /// <summary>
        /// Sets up usages of the InjectAttribute
        /// </summary>
        /// <param name="gameObjectInstance">GameObject instance to inject into</param>
        /// <returns>Original instance</returns>
        GameObject InjectIntoGameObject(GameObject gameObjectInstance);
    }
}
