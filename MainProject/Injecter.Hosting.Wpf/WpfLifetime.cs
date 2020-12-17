using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Injecter.Hosting.Wpf
{
    /// <summary>
    /// Listens for Unity Application.Current.Exit.
    /// </summary>
    public sealed class WpfLifetime : IHostLifetime, IDisposable
    {
        private readonly ManualResetEvent _shutdownBlock = new(false);
        private CancellationTokenRegistration _applicationStartedRegistration;
        private CancellationTokenRegistration _applicationStoppingRegistration;

        public WpfLifetime(
            IOptions<WpfLifeTimeOptions> options,
            IHostEnvironment environment,
            IHostApplicationLifetime applicationLifetime,
            IOptions<HostOptions> hostOptions)
            : this(options, environment, applicationLifetime, hostOptions, NullLoggerFactory.Instance)
        {
        }

        public WpfLifetime(
            IOptions<WpfLifeTimeOptions> options,
            IHostEnvironment environment,
            IHostApplicationLifetime applicationLifetime,
            IOptions<HostOptions> hostOptions,
            ILoggerFactory loggerFactory)
        {
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            HostOptions = hostOptions.Value ?? throw new ArgumentNullException(nameof(hostOptions));
            Logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
        }

        private WpfLifeTimeOptions Options { get; }

        private IHostEnvironment Environment { get; }

        private IHostApplicationLifetime ApplicationLifetime { get; }

        private HostOptions HostOptions { get; }

        private ILogger Logger { get; }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            if (!Options.SuppressStatusMessages)
            {
                _applicationStartedRegistration = ApplicationLifetime
                    .ApplicationStarted
                    .Register(state => ((WpfLifetime)state!).OnApplicationStarted(), this);

                _applicationStoppingRegistration = ApplicationLifetime
                    .ApplicationStopping
                    .Register(state => ((WpfLifetime)state!).OnApplicationStopping(), this);
            }

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Application.Current.Exit += OnWpfExiting;

            // Console applications start immediately.
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose()
        {
            _shutdownBlock.Set();

            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            Application.Current.Exit -= OnWpfExiting;

            _applicationStartedRegistration.Dispose();
            _applicationStoppingRegistration.Dispose();
        }

        private void OnApplicationStarted()
        {
            Logger.LogInformation("Wpf application started");
            Logger.LogInformation("Hosting environment: {EnvName}", Environment.EnvironmentName);
            Logger.LogInformation("Content root path: {ContentRoot}", Environment.ContentRootPath);
        }

        private void OnApplicationStopping() => Logger.LogInformation("Unity application is shutting down...");

        private void OnProcessExit(object? sender, EventArgs e)
        {
            ApplicationLifetime.StopApplication();
            if (!_shutdownBlock.WaitOne(HostOptions.ShutdownTimeout))
            {
                Logger.LogInformation("Waiting for the host to be disposed. Ensure all 'IHost' instances are wrapped in 'using' blocks.");
            }

            _shutdownBlock.WaitOne();

            // On Linux if the shutdown is triggered by SIGTERM then that's signaled with the 143 exit code.
            // Suppress that since we shut down gracefully. https://github.com/aspnet/AspNetCore/issues/6526
            System.Environment.ExitCode = 0;
        }

        private void OnWpfExiting(object? sender, ExitEventArgs e) => ApplicationLifetime.StopApplication();
    }
}
