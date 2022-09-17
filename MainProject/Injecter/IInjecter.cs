using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter
{
    public interface IInjecter
    {
        /// <summary>
        /// Injects services into members marked with <see cref="InjectAttribute"/>. If the type has no injections, the return is null
        /// </summary>
        /// <param name="instance">The instance which receives injection.</param>
        /// <param name="createScope">When true, creates a new scope for the instance.</param>
        /// <returns><see cref="IServiceScope"/> that gets created during injection, if any.</returns>
        IServiceScope? InjectIntoType(object instance, bool createScope);
    }
}
