using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter
{
    public interface IInjecter
    {
        /// <summary>
        /// Injects services into members marked with <see cref="InjectAttribute"/>.
        /// </summary>
        /// <param name="type">Type of the object to inject into.</param>
        /// <param name="instance">The instance which receives injection.</param>
        /// <returns><see cref="IServiceScope"/> that gets created during injection.</returns>
        IServiceScope InjectIntoType(Type type, object instance);
    }
}
