using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Injecter.Unity.Editor
{
    public delegate bool FilterGameObject(GameObject gameObject);
    public delegate void PatchGameObject(GameObject gameObject, string location);

    internal static class GameObjectPatcher
    {
        public static void AddComponentsToEveryPrefab(FilterGameObject filter, PatchGameObject patch)
        {
            foreach (var prefabPath in AssetDatabase.GetAllAssetPaths().Where(s => s.EndsWith(".prefab")))
            {
                var prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath);

                if (prefab is GameObject prefabObject)
                {
                    AddScriptsToGameObject(prefabObject, filter, patch, $"prefab: {prefabPath}");
                    AssetDatabase.SaveAssets();
                }
            }
        }

        public static void AddComponentsToCurrentScene(FilterGameObject filter, PatchGameObject patch)
        {
            var sceneName = SceneManager.GetActiveScene().name;

            var rootObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            foreach (var gameObject in rootObjects)
            {
                AddScriptsToGameObject(gameObject, filter, patch, $"scene: {sceneName}");
            }
        }

        public static void AddComponentsToEveryScene(FilterGameObject filter, PatchGameObject patch)
        {
            var dataPathFull = Path.GetFullPath(Application.dataPath);
            var scenes = Directory
                .EnumerateFiles(dataPathFull, "*.unity", SearchOption.AllDirectories)
                .Select(s => s.Replace(dataPathFull, string.Empty))
                .Select(s => $"Assets{s}")
                .ToArray();

            var originalScenePath = SceneManager.GetActiveScene().path;
            EditorSceneManager.SaveOpenScenes();

            foreach (var scene in scenes)
            {
                EditorSceneManager.OpenScene(scene);
                AddComponentsToCurrentScene(filter, patch);
                EditorSceneManager.SaveOpenScenes();
            }

            EditorSceneManager.OpenScene(originalScenePath);
        }

        public static void AddComponentsToEverything(FilterGameObject filter, PatchGameObject patch)
        {
            AddComponentsToEveryPrefab(filter, patch);
            AddComponentsToEveryScene(filter, patch);
        }

        public static void EnsureComponentSafe<T>(MonoBehaviour instance, GameObject holder, string location) where T : Component
        {
            Debug.Log($"Adding script: {typeof(T).Name} to GameObjec: {holder.name} at {location}", holder);

            if (instance.TryGetComponent<T>(out _)) return;

            var component = Undo.AddComponent<T>(holder);

            if (component == null) return;

            if (PrefabUtility.IsAddedComponentOverride(component))
            {
                Undo.DestroyObjectImmediate(component);
            }
        }

        private static void AddScriptsToGameObject(GameObject gameObject, FilterGameObject filter, PatchGameObject patch, string location)
        {
            var instances = gameObject
                .GetComponentsInChildren<Transform>(true)
                .Where(t => t != null && filter(t.gameObject))
                .Select(t => t.gameObject)
                .ToArray();

            for (var i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                if (instance == null) continue;
                patch(instance, location);
            }
        }
    }
}
