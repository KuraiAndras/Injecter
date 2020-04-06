using System;
using UnityEngine;

namespace Injecter.Unity
{
    public interface ISceneInjector
    {
        void InitializeScene(IServiceProvider serviceProvider, Action<SceneInjectorOptions> optionsBuilder = null);

        /// <summary>
        /// Sets up usages of the InjectAttribute
        /// </summary>
        /// <param name="gameObjectInstance">GameObject instance to inject into</param>
        /// <returns>Original instance</returns>
        GameObject InjectIntoGameObject(GameObject gameObjectInstance);
    }
}
