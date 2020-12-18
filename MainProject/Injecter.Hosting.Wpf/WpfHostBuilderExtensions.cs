using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Injecter.Hosting.Wpf
{
    public static class WpfHostBuilderExtensions
    {
        /// <summary>
        /// Listens for Application.Current.Exit to start the shutdown process.
        /// This will unblock extensions like RunAsync and WaitForShutdownAsync.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to configure.</param>
        /// <param name="configureOptions">The delegate for configuring the <see cref="WpfLifetime"/>.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder UseWpfLifetime(this IHostBuilder hostBuilder, Action<WpfLifeTimeOptions> configureOptions) =>
            hostBuilder.ConfigureServices((_, collection) =>
            {
                collection.AddSingleton<IHostLifetime, WpfLifetime>();
                collection.Configure(configureOptions);
            });

        /// <summary>
        /// Listens for Application.Current.Exit to start the shutdown process.
        /// This will unblock extensions like RunAsync and WaitForShutdownAsync.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to configure.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder UseWpfLifetime(this IHostBuilder hostBuilder) =>
            hostBuilder.ConfigureServices((_, collection) => collection.AddSingleton<IHostLifetime, WpfLifetime>());
    }
}
