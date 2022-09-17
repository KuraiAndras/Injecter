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
        [MenuItem("Assets / Create / Injecter / AppInstaller", false, 2)]
        private static void CreateAppInstaller() => ProjectWindowUtil.CreateAssetWithContent(
            "AppInstaller.cs",
@"#nullable enable
using Injecter;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public static class AppInstaller
{
    /// <summary>
    /// Set this from test assembly to disable
    /// </summary>
    public static bool Run { get; set; } = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void Install()
    {
        if (!Run) return;

        var serviceProvider = new ServiceCollection()
            .Configure()
            .BuildServiceProvider(true);

        CompositionRoot.ServiceProvider = serviceProvider;

        Application.quitting += OnQuitting;

        async void OnQuitting()
        {
            Application.quitting -= OnQuitting;

            await serviceProvider.DisposeAsync().ConfigureAwait(false);
        }
    }

    public static IServiceCollection Configure(this IServiceCollection services)
    {
        services.AddInjecter(o => o.UseCaching = true);
        // TODO: Add services

        return services;
    }
}
");
        [MenuItem("Assets / Create / Injecter / AppInstaller with Serilog", false, 2)]
        private static void CreateAppInstallerWithSerilog() => ProjectWindowUtil.CreateAssetWithContent(
           "AppInstaller.cs",
@"#nullable enable
using Injecter;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Unity3D;
using System;
using System.IO;
using UnityEngine;

public static class AppInstaller
{
    /// <summary>
    /// Set this from test assembly to disable
    /// </summary>
    public static bool Run { get; set; } = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void Install()
    {
        if (!Run) return;

        var logFilePath = Path.Combine(Application.persistentDataPath, Application.productName + ""_application.log"");

        var logger = new LoggerConfiguration()
            .WriteTo.Unity3D()
            .WriteTo.Async(a => a.File(logFilePath, fileSizeLimitBytes: 10 * 1024 * 1024, rollOnFileSizeLimit: true, retainedFileCountLimit: 10))
            .CreateLogger();

        try
        {
            var serviceProvider = new ServiceCollection()
                .Configure()
                .BuildServiceProvider(true);

            CompositionRoot.ServiceProvider = serviceProvider;

            Application.quitting += OnQuitting;

            async void OnQuitting()
            {
                Application.quitting -= OnQuitting;

                await serviceProvider.DisposeAsync().ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            logger.Fatal(e, ""Host terminated unexpectedly"");
            throw;
        }
    }

    public static IServiceCollection Configure(this IServiceCollection services, Serilog.ILogger logger)
    {
        services.AddInjecter(o => o.UseCaching = true);
        services.AddLogging(b => b.AddSerilog(logger));

        // TODO: Add services

        return services;
    }
}
");
    }
}
