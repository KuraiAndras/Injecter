using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter
{
    public static class InjecterExtensions
    {
        public static IServiceCollection AddInjecter(this IServiceCollection services, Action<InjecterOptions> optionsBuilder)
        {
            var options = new InjecterOptions();

            optionsBuilder?.Invoke(options);

            services.AddSingleton<InjecterOptions>();
            services.AddSingleton<IInjecter, Injecter>();

            return services;
        }

        public static IServiceScope InjectIntoType<T>(this IInjecter injecter, object instance)
        {
            if (injecter is null) throw new ArgumentNullException(nameof(injecter));

            return injecter.InjectIntoType(typeof(T), instance);
        }
    }
}
