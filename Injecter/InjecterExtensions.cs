using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injecter
{
    public static class InjecterExtensions
    {
        public static IServiceCollection AddInjecter(this IServiceCollection services, Action<InjecterOptions> optionsBuilder = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            var options = new InjecterOptions();

            optionsBuilder?.Invoke(options);

            services.AddSingleton(options);
            services.AddSingleton<IInjecter, Injecter>();

            return services;
        }
    }
}
