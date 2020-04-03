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
    }
}
