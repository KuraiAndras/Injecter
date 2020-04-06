using System;

namespace Injecter
{
    public static class CompositionRoot
    {
        public static IServiceProvider ServiceProvider { get; set; } = default;
    }
}
