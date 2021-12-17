using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Injecter.Hosting.Unity
{
    public static class UnityHostBuilderExtensions
    {
        /// <summary>
        /// Listens for Unity.Application.quit <see cref="UnityEngine.Application"/> to start the shutdown process.
        /// This will unblock extensions like RunAsync and WaitForShutdownAsync.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to configure.</param>
        /// <param name="configureOptions">The delegate for configuring the <see cref="UnityLifetime"/>.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder UseUnityLifetime(this IHostBuilder hostBuilder, Action<UnityLifeTimeOptions> configureOptions) =>
            hostBuilder.ConfigureServices((_, collection) =>
            {
                collection.AddSingleton<IHostLifetime, UnityLifetime>();
                collection.Configure(configureOptions);
            });

        /// <summary>
        /// Listens for Unity.Application.quit <see cref="UnityEngine.Application"/> to start the shutdown process.
        /// This will unblock extensions like RunAsync and WaitForShutdownAsync.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to configure.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder UseUnityLifetime(this IHostBuilder hostBuilder) =>
            hostBuilder.ConfigureServices((_, collection) => collection.AddSingleton<IHostLifetime, UnityLifetime>());

        /// <summary>
        /// Configure <see cref="IHost"/> to sensible defaults for Unity
        /// </summary>
        /// <param name="configureOptions">The delegate for configuring the <see cref="UnityLifetime"/>.</param>
        /// <param name="ignoreCommandLineArgs">Don't add command line parameters. Defaults to false</param>
        /// <param name="ignoreEnvironmentVariables">Don't add environment variables. Defaults to false</param>
        /// <param name="jsonConfigurations">Text assets which contain json data. Load the files using <see cref="Resources.Load(string)"/> or use the new Addressables package.</param>
        /// <returns>The original builder</returns>
        public static IHostBuilder UseUnity
        (
            this IHostBuilder builder,
            Action<UnityLifeTimeOptions> configureOptions,
            bool ignoreCommandLineArgs = false,
            bool ignoreEnvironmentVariables = false,
            params TextAsset[] jsonConfigurations
        )
        {
            var resourcesFolder = Path.GetFullPath(Application.dataPath);

            if (Application.isEditor)
            {
                builder.UseContentRoot(resourcesFolder);

                builder.UseEnvironment("Development");
            }

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                if (!ignoreCommandLineArgs) configurationBuilder.AddCommandLine(Environment.GetCommandLineArgs());
                if (!ignoreEnvironmentVariables) configurationBuilder.AddEnvironmentVariables();

                foreach (var jsonConfiguration in jsonConfigurations.Select(c => c.text.ToStream()))
                {
                    configurationBuilder.AddJsonStream(jsonConfiguration);
                }
            });

            builder.UseUnityLifetime(configureOptions);

            return builder;
        }

        public static void RegisterInjectionsOnSceneLoad(this IHost host) => InjectionHelper.RegisterInjectionsOnSceneLoad(host.Services);

        private static MemoryStream ToStream(this string @string)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@string);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
