using Injecter;
using Injecter.Hosting.Wpf;
using Microsoft.Extensions.Hosting;
using SampleLogic;
using System.Windows;

namespace WpfSample
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = new HostBuilder()
                .UseWpfLifetime()
                .ConfigureServices(s => s.AddSharedLogic())
                .Build();

            CompositionRoot.ServiceProvider = _host.Services;

            _host.Start();

            Exit += OnExit;
        }

        private void OnExit(object _, ExitEventArgs __) => _host.Dispose();
    }
}
