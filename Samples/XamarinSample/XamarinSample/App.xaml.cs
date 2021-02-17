using Injecter;
using Microsoft.Extensions.DependencyInjection;
using SampleLogic;

namespace XamarinSample
{
    public partial class App
    {
        public App()
        {
            CompositionRoot.ServiceProvider = new ServiceCollection()
                .AddSharedLogic()
                .BuildServiceProvider();

            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Start
        }

        protected override void OnSleep()
        {
            // Sleep
        }

        protected override void OnResume()
        {
            // Resume
        }
    }
}
