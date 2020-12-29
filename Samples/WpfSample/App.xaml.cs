﻿using Injecter;
using Injecter.Hosting.Wpf;
using Microsoft.Extensions.Hosting;
using SampleLogic;
using System.Windows;

namespace WpfSample
{
    public partial class App : Application
    {
        private IHost? _host;

        private void Initialize(object sender, StartupEventArgs e)
        {
            _host = new HostBuilder()
                .UseWpfLifetime()
                .ConfigureServices(s => s.AddSharedLogic())
                .Build();

            CompositionRoot.ServiceProvider = _host.Services;

            _host.Start();
        }

        private void App_OnExit(object sender, ExitEventArgs e) => _host?.Dispose();
    }
}
