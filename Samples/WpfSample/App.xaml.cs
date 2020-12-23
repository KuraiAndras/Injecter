using Injecter;
using Injecter.Hosting.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                .ConfigureServices(s =>
                {
                    s.AddInjecter();
                    s.AddSingleton<ICounter, Counter>();
                })
                .Build();

            CompositionRoot.ServiceProvider = _host.Services;

            _host.Start();
        }

        private void App_OnExit(object sender, ExitEventArgs e) => _host?.Dispose();
    }
}
