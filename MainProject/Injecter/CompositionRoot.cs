using System;
using System.Diagnostics.CodeAnalysis;

namespace Injecter
{
    public static class CompositionRoot
    {
        /// <summary>
        /// The root service provider used by framework implementations.
        /// </summary>
        [MaybeNull] public static IServiceProvider? ServiceProvider { get; set; }
    }
}
