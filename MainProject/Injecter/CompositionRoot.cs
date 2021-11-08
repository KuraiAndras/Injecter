using System;

namespace Injecter
{
    public static class CompositionRoot
    {
        /// <summary>
        /// The root service provider used by framework implementations.
        /// </summary>
        public static IServiceProvider? ServiceProvider { get; set; }
    }
}
