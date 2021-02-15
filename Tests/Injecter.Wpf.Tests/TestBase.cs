using Microsoft.Extensions.DependencyInjection;
using SampleLogic;
using System;

namespace Injecter.Wpf.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected TestBase() =>
            CompositionRoot.ServiceProvider = new ServiceCollection()
                .AddSharedLogic()
                .BuildServiceProvider();

        public void Dispose()
        {
            ((IDisposable)CompositionRoot.ServiceProvider!).Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
