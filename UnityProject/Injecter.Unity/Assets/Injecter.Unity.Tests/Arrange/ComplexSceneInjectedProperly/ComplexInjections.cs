using Microsoft.Extensions.DependencyInjection;

namespace Injecter.Unity.Tests.Arrange.ComplexSceneInjectedProperly
{
    public static class ComplexInjections
    {
        public static IServiceCollection AddComplexInjection(this IServiceCollection services)
        {
            services.AddTransient<ITestService1, TestService1>();

            return services;
        }
    }
}
