using Injecter;
using Microsoft.Extensions.DependencyInjection;

namespace SampleLogic
{
    public static class Injector
    {
        public static void AddSharedLogic(this IServiceCollection services)
        {
            services.AddInjecter();
            services.AddSingleton<ICounter, Counter>();
        }
    }
}
