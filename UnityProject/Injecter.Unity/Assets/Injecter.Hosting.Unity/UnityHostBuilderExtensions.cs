using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
    }
}
