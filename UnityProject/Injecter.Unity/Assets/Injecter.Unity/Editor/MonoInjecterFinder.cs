#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Injecter.Unity.Editor
{
    public static class MonoInjecterFinder
    {
        private static readonly Dictionary<Type, bool> _typeCache = new Dictionary<Type, bool>();

        [MenuItem("Tools / Injecter / Ensure injection scripts in every prefab")]
        public static void AddComponentsToEveryPrefab()
        {
            foreach (var prefabPath in AssetDatabase.GetAllAssetPaths().Where(s => s.EndsWith(".prefab")))
            {
                var prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath);

                if (prefab is GameObject prefabObject)
                {
                    AddScriptsToGameObject(prefabObject, $"prefab: {prefabPath}");
                    AssetDatabase.SaveAssets();
                }
            }
        }

        [MenuItem("Tools / Injecter / Ensure injection scripts in current scene")]
        public static void AddComponentsToCurrentScene()
        {
            var sceneName = SceneManager.GetActiveScene().name;

            var rootObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            foreach (var gameObject in rootObjects)
            {
                AddScriptsToGameObject(gameObject, $"scene: {sceneName}");
            }
        }

        [MenuItem("Tools / Injecter / Ensure injection scripts in every scene (will load all scenes)")]
        public static void AddComponentsToEveryScene()
        {
            var dataPathFull = Path.GetFullPath(Application.dataPath);
            var scenes = Directory
                .EnumerateFiles(dataPathFull, "*.unity", SearchOption.AllDirectories)
                .Select(s => s.Replace(dataPathFull, string.Empty))
                .Select(s => $"Assets{s}")
                .ToArray();

            OfferSaveIfSceneIsDirty();

            foreach (var scene in scenes)
            {
                EditorSceneManager.OpenScene(scene);
                AddComponentsToCurrentScene();
                OfferSaveIfSceneIsDirty();
            }
        }

        [MenuItem("Tools / Injecter / Ensure injection scripts on everyting (will load all scenes)")]
        public static void AddComponentsToEverything()
        {
            AddComponentsToEveryPrefab();
            AddComponentsToEveryScene();
        }

        private static void AddScriptsToGameObject(GameObject gameObject, string location)
        {
            var instances = gameObject
                .GetComponentsInChildren<MonoBehaviour>(true)
                .Where(b => b != null)
                .Where(b =>
                {
                    var type = b.GetType();

                    if (!_typeCache.TryGetValue(type, out var isInjectable))
                    {
                        var members = TypeHelpers.GetInjectableMembers(type);
                        isInjectable = members.Count != 0;
                        _typeCache.Add(type, isInjectable);
                    }

                    return isInjectable;
                })
                .ToArray();

            for (var i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                EnsureComponent<MonoInjector>(instance, location);
                EnsureComponent<MonoDisposer>(instance, location);
            }
        }

        private static void OfferSaveIfSceneIsDirty()
        {
            if (SceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }
        }

        private static void EnsureComponent<T>(MonoBehaviour instance, string location) where T : Component
        {
            if (!instance.gameObject.TryGetComponent<T>(out var _))
            {
                Debug.Log($"Adding script: {typeof(T).Name} to GameObjec: {instance.gameObject.name} at {location}");
                Undo.AddComponent<T>(instance.gameObject);
            }
        }
    }
}
