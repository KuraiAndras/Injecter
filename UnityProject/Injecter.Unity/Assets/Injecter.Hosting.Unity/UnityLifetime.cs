using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Injecter.Hosting.Unity
{
    /// <summary>
    /// Listens for Unity Application.quit and EditorApplication.playModeStateChanged
    /// </summary>
    public sealed class UnityLifetime : IHostLifetime, IDisposable
    {
        private readonly ManualResetEvent _shutdownBlock = new ManualResetEvent(false);
        private CancellationTokenRegistration _applicationStartedRegistration;
        private CancellationTokenRegistration _applicationStoppingRegistration;

        public UnityLifetime(
            IOptions<UnityLifeTimeOptions> options,
            IHostEnvironment environment,
            IHostApplicationLifetime applicationLifetime,
            IOptions<HostOptions> hostOptions)
            : this(options, environment, applicationLifetime, hostOptions, NullLoggerFactory.Instance)
        {
        }

        public UnityLifetime(
            IOptions<UnityLifeTimeOptions> options,
            IHostEnvironment environment,
            IHostApplicationLifetime applicationLifetime,
            IOptions<HostOptions> hostOptions,
            ILoggerFactory loggerFactory)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            HostOptions = hostOptions?.Value ?? throw new ArgumentNullException(nameof(hostOptions));
            Logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
        }

        private UnityLifeTimeOptions Options { get; }

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
                    .Register(state => ((UnityLifetime)state).OnApplicationStarted(), this);

                _applicationStoppingRegistration = ApplicationLifetime
                    .ApplicationStopping
                    .Register(state => ((UnityLifetime)state).OnApplicationStopping(), this);
            }

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Application.quitting += OnUnityQuitting;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnUnityPlayModeStateChanged;
#endif

            // Console applications start immediately.
            return Task.CompletedTask;
        }

        private void OnApplicationStarted()
        {
            Logger.LogInformation("Unity application started");
            Logger.LogInformation("Hosting environment: {EnvName}", Environment.EnvironmentName);
            Logger.LogInformation("Content root path: {ContentRoot}", Environment.ContentRootPath);
        }

        private void OnApplicationStopping() => Logger.LogInformation("Unity application is shutting down...");

        private void OnProcessExit(object sender, EventArgs e)
        {
            ApplicationLifetime.StopApplication();
            if (!_shutdownBlock.WaitOne(HostOptions.ShutdownTimeout))
            {
                Logger.LogInformation("Waiting for the host to be disposed, ensure all 'IHost' instances are wrapped in 'using' blocks");
            }

            _shutdownBlock.WaitOne();
            // On Linux if the shutdown is triggered by SIGTERM then that's signaled with the 143 exit code.
            // Suppress that since we shut down gracefully. https://github.com/aspnet/AspNetCore/issues/6526
            System.Environment.ExitCode = 0;
        }

        private void OnUnityQuitting() => ApplicationLifetime.StopApplication();

#if UNITY_EDITOR
        private void OnUnityPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode) OnUnityQuitting();
        }
#endif

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose()
        {
            _shutdownBlock.Set();

            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            Application.quitting -= OnUnityQuitting;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnUnityPlayModeStateChanged;
#endif

            _applicationStartedRegistration.Dispose();
            _applicationStoppingRegistration.Dispose();
        }
    }
}
