using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter
{
    public static class CompositionRoot
    {
        public static IServiceProvider ServiceProvider { get; set; } = new ServiceCollection().AddInjecter().BuildServiceProvider();
    }
}
