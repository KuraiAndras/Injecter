#nullable enable
using Injecter.Unity;
using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Injecter.Hosting.Unity
{
    public static class InjectionHelper
    {
        public static void RegisterInjectionsOnSceneLoad(IServiceProvider serviceProvider)
        {
            void SceneLoaded(Scene scene, LoadSceneMode mode)
            {
                var createScopes = serviceProvider.GetRequiredService<SceneInjectorOptions>().CreateScopes;

                serviceProvider.GetRequiredService<ISceneInjector>().InitializeScene(createScopes, scene, mode);
            }

            void OnQuitting()
            {
                Application.quitting -= OnQuitting;
                SceneManager.sceneLoaded -= SceneLoaded;
            }

            SceneManager.sceneLoaded += SceneLoaded;
            Application.quitting += OnQuitting;
        }
    }
}
