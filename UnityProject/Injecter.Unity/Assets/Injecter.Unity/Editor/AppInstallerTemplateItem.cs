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
using System.Threading.Tasks;
using UnityEngine;

public static class AppInstaller
{
    /// <summary>
    /// Set this from test assembly to disable
    /// </summary>
    public static bool Run { get; set; } = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static async Task Install()
    {
        if (!Run) return;

        var logFilePath = Path.Combine(Application.persistentDataPath, Application.productName + ""Application.log"");

        var logger = new LoggerConfiguration()
            .WriteTo.Unity3D()
            .WriteTo.Async(a => a.File(logFilePath, fileSizeLimitBytes: 10 * 1024 * 1024, rollOnFileSizeLimit: true, retainedFileCountLimit: 10))
            .CreateLogger();

        try
        {
            var serviceProvider = new ServiceCollection()
                .Configure(logger)
                .BuildServiceProvider(true);

            CompositionRoot.ServiceProvider = serviceProvider;

            Application.quitting += OnQuitting;

            logger.Information(""Application started"");

            async void OnQuitting()
            {
                try
                {
                    Application.quitting -= OnQuitting;

                    logger.Information(""Application is quitting"");

                    await serviceProvider.DisposeAsync().ConfigureAwait(false);
                    CompositionRoot.ServiceProvider = null;
                }
                catch (Exception e)
                {
                    logger.Error(e, ""Disposing of coposition root failed"");
                    throw e;
                }
                finally
                {
                    try
                    {
                        logger.Information(""Application quit"");
                        await logger.DisposeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.Fatal(e, ""Host terminated unexpectedly"");
            await logger.DisposeAsync().ConfigureAwait(false);
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
