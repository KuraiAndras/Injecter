using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter
{
    public interface IInjecter
    {
        /// <summary>
        /// Injects services into members marked with <see cref="InjectAttribute"/>. If the type has no injections, the return is null
        /// </summary>
        /// <param name="type">Type of the object to inject into.</param>
        /// <param name="instance">The instance which receives injection.</param>
        /// <param name="createScope">When true, creates a new scope for the instance.</param>
        /// <returns><see cref="IServiceScope"/> that gets created during injection, if any.</returns>
        IServiceScope? InjectIntoType(Type type, object instance, bool createScope);

        /// <summary>
        /// <seealso cref="InjectIntoType"/>
        /// </summary>
        /// <typeparam name="T">Instance type.</typeparam>
        /// <param name="instance">The instance which receives injection.</param>
        /// <param name="createScope">When true, creates a new scope for the instance.</param>
        /// <returns><see cref="IServiceScope"/> that gets created during injection, if any.</returns>
        IServiceScope? InjectIntoType<T>(T instance, bool createScope) where T : notnull;
    }
}
