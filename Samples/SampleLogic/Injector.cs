using Injecter;
using Microsoft.Extensions.DependencyInjection;

namespace SampleLogic
{
    public static class Injector
    {
        public static IServiceCollection AddSharedLogic(this IServiceCollection services)
        {
            services.AddInjecter();
            services.AddSingleton<ICounter, Counter>();

            return services;
        }
    }
}
