#nullable enable
using UnityEditor;

namespace Injecter.Unity.Editor
{
    /// <summary>
    /// Buttons that create commonly used classes
    /// </summary>
    public static class AppInstallerTemplateItem
    {
        /// <summary>
        /// Creates a basic AppInstaller.cs file in project root
        /// </summary>
        [MenuItem("Assets/Create/Injecter/AppInstallerCompositionRoot", false, 2)]
        private static void CreateAppInstaller() => ProjectWindowUtil.CreateAssetWithContent(
            "AppInstallerCompositionRoot.cs",
@"#nullable enable
using System;
using Injecter;
using Injecter.Hosting.Unity;
using Injecter.Unity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Unity3D;
using UnityEngine;

public static class AppInstaller
{
    public static bool Run { get; set; } = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void Install()
    {
        if (!Run) return;

        var logger = new LoggerConfiguration()
            .WriteTo.Unity3D()
            .CreateLogger();

        try
        {
            var host = new HostBuilder()
                .ConfigureHost(logger)
                .Build();

            CompositionRoot.ServiceProvider = host.Services;

            host.Start();

            Application.quitting += OnQuitting;

            void OnQuitting()
            {
                Log.CloseAndFlush();

                host.Dispose();
                host = null!;

                Application.quitting -= OnQuitting;
            }
        }
        catch (Exception e)
        {
            logger.Fatal(e, ""Host terminated unexpectedly"");
            throw;
        }
    }

    public static IHostBuilder ConfigureHost(this IHostBuilder builder, Serilog.ILogger logger)
    {
          
        return builder
            .UseUnity(_ => { }, false, false, Array.Empty<TextAsset>())
            .ConfigureServices(ConfigureServices)
            .UseDefaultServiceProvider(o =>
            {
                o.ValidateOnBuild = true;
                o.ValidateScopes = true;
            })
            .UseSerilog(logger);
    }

    public static void ConfigureServices(HostBuilderContext builder, IServiceCollection services)
    {
        var assemblies = new[] { typeof(AppInstaller).Assembly };

        services.AddSceneInjector(
            injecterOptions => injecterOptions.UseCaching = true,
            sceneInjectorOptions =>
            {
                sceneInjectorOptions.DontDestroyOnLoad = true;
                sceneInjectorOptions.InjectionBehavior = SceneInjectorOptions.Behavior.CompositionRoot;
            });
    }
}
");
        [MenuItem("Assets/Create/Injecter/AppInstallerFactory", false, 2)]
    private static void CreateAppInstallerFactory() => ProjectWindowUtil.CreateAssetWithContent(
           "AppInstallerFactory.cs",
@"#nullable enable
using System;
using Injecter;
using Injecter.Hosting.Unity;
using Injecter.Unity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Unity3D;
using UnityEngine;

public static class AppInstaller
{
    public static bool Run { get; set; } = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void Install()
    {
        if (!Run) return;

        var logger = new LoggerConfiguration()
            .WriteTo.Unity3D()
            .CreateLogger();

        try
        {
            var host = new HostBuilder()
                .ConfigureHost(logger)
                .Build();

            host.RegisterInjectionsOnSceneLoad();

            host.Start();

            Application.quitting += OnQuitting;

            void OnQuitting()
            {
                Log.CloseAndFlush();

                host.Dispose();
                host = null!;

                Application.quitting -= OnQuitting;
            }
        }
        catch (Exception e)
        {
            logger.Fatal(e, ""Host terminated unexpectedly"");
            throw;
        }
    }

    public static IHostBuilder ConfigureHost(this IHostBuilder builder, Serilog.ILogger logger)
    {
          
        return builder
            .UseUnity(_ => { }, false, false, Array.Empty<TextAsset>())
            .ConfigureServices(ConfigureServices)
            .UseDefaultServiceProvider(o =>
            {
                o.ValidateOnBuild = true;
                o.ValidateScopes = true;
            })
            .UseSerilog(logger);
    }

    public static void ConfigureServices(HostBuilderContext builder, IServiceCollection services)
    {
        var assemblies = new[] { typeof(AppInstaller).Assembly };

        services.AddSceneInjector(
            injecterOptions => injecterOptions.UseCaching = true,
            sceneInjectorOptions =>
            {
                sceneInjectorOptions.DontDestroyOnLoad = true;
                sceneInjectorOptions.InjectionBehavior = SceneInjectorOptions.Behavior.Factory;
            });
    }
}
");
    }
}
