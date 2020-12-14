using System;
using System.Diagnostics.CodeAnalysis;

namespace Injecter
{
    public static class CompositionRoot
    {
        [MaybeNull] public static IServiceProvider? ServiceProvider { get; set; }
    }
}
