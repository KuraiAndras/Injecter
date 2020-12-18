using UnityEngine;

namespace Injecter.Unity
{
    public interface IGameObjectFactory
    {
        /// <summary>
        /// Calls Object.Instantiate with the appropriate parameters
        /// Injects into created instance.
        /// </summary>
        /// <returns>Instantiated GameObject instance</returns>
        GameObject Instantiate(GameObject original);

        /// <summary>
        /// Calls Object.Instantiate with the appropriate parameters
        /// Injects into created instance.
        /// </summary>
        /// <returns>Instantiated GameObject instance</returns>
        GameObject Instantiate(GameObject original, Transform parent);

        /// <summary>
        /// Calls Object.Instantiate with the appropriate parameters
        /// Injects into created instance.
        /// </summary>
        /// <returns>Instantiated GameObject instance</returns>
        GameObject Instantiate(GameObject original, Transform parent, bool instantiateInWorldSpace);

        /// <summary>
        /// Calls Object.Instantiate with the appropriate parameters
        /// Injects into created instance.
        /// </summary>
        /// <returns>Instantiated GameObject instance</returns>
        GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation);

        /// <summary>
        /// Calls Object.Instantiate with the appropriate parameters
        /// Injects into created instance.
        /// </summary>
        /// <returns>Instantiated GameObject instance</returns>
        GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent);
    }
}
