#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Injecter.Unity.Editor
{
    public static class MonoInjecterFinder
    {
        private static readonly Dictionary<Type, bool> _typeCache = new Dictionary<Type, bool>();

        [MenuItem("Tools / Injecter / Ensure injection scripts in every prefab")]
        public static void AddComponentsToEveryPrefab() => GameObjectPatcher.AddComponentsToEveryPrefab(FilterInjectables, AddScriptsToGameObject);

        [MenuItem("Tools / Injecter / Ensure injection scripts in current scene")]
        public static void AddComponentsToCurrentScene() => GameObjectPatcher.AddComponentsToCurrentScene(FilterInjectables, AddScriptsToGameObject);

        [MenuItem("Tools / Injecter / Ensure injection scripts in every scene")]
        public static void AddComponentsToEveryScene() => GameObjectPatcher.AddComponentsToEveryScene(FilterInjectables, AddScriptsToGameObject);

        [MenuItem("Tools / Injecter / Ensure injection scripts on everyting")]
        public static void AddComponentsToEverything() => GameObjectPatcher.AddComponentsToEverything(FilterInjectables, AddScriptsToGameObject);

        private static bool FilterInjectables(GameObject gameObject) => gameObject
            .GetComponents<MonoBehaviour>()
            .Any(b => CanBeInjected(b));

        private static bool CanBeInjected(MonoBehaviour component)
        {
            if (component == null) return false;

            var type = component.GetType();

            if (!_typeCache.TryGetValue(type, out var isInjectable))
            {
                var members = TypeHelpers.GetInjectableMembers(type);
                isInjectable = members.Count != 0;
                _typeCache.Add(type, isInjectable);
            }

            return isInjectable;
        }

        private static void AddScriptsToGameObject(GameObject gameObject, string location)
        {
            var instances = gameObject
                .GetComponentsInChildren<MonoBehaviour>(true)
                .Where(b => CanBeInjected(b))
                .ToArray();

            for (var i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                GameObjectPatcher.EnsureComponentSafe<MonoInjector>(instance, gameObject, location);
                GameObjectPatcher.EnsureComponentSafe<MonoDisposer>(instance, gameObject, location);
            }
        }
    }
}
