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

        [MenuItem("Tools / Injecter / Ensure injection scripts on current scene and all prefabs")]
        public static void AddComponents()
        {
            var instances = Resources.FindObjectsOfTypeAll<MonoBehaviour>().Where(b =>
            {
                var type = b.GetType();

                if (!_typeCache.TryGetValue(type, out var isInjectable))
                {
                    var members = TypeHelpers.GetInjectableMembers(type);
                    isInjectable = members.Count != 0;
                    _typeCache.Add(type, isInjectable);
                }

                return isInjectable;
            }).ToArray();

            for (var i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                EnsureComponent<MonoInjector>(instance);
                EnsureComponent<MonoDisposer>(instance);
            }
        }

        private static void EnsureComponent<T>(MonoBehaviour instance) where T : Component
        {
            if (!instance.gameObject.TryGetComponent<T>(out var _))
            {
                Undo.AddComponent<T>(instance.gameObject);
            }
        }
    }
}
