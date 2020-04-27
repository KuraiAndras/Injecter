using Microsoft.Extensions.DependencyInjection;

namespace Injecter.FastMediatR
{
    public static class Injector
    {
        public static IServiceCollection AddFastMediatR(this IServiceCollection services)
        {
            services.AddTransient<IFastMediator, DefaultFastMediator>();

            return services;
        }
    }
}
