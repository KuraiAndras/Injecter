using Injecter.Tests.Arrangers;
using Injecter.Tests.Arrangers.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Injecter.Tests
{
    public class InjecterTests
    {
        [Fact]
        public void TargetObjectGetsInjection()
        {
            // Arrange
            var (injecter, serviceProvider) = CreateInjecter(services => services.AddSingleton<ISimpleService, SimpleService>());

            // Act
            var target = new SimpleInjectTarget();
            var scope = injecter.InjectIntoType(target, false);

            // Assert
            Assert.True(target.IsServiceNotNull && target.SimpleService is not null);

            DisposeServices(serviceProvider);
            DisposeServices(scope);
        }

        [Fact]
        public void InheritedClassesReceiveInjection()
        {
            // Arrange
            var (injecter, serviceProvider) = CreateInjecter(services => services.AddSingleton<ISimpleService, SimpleService>());

            // Act
            var target = new InheritedInjectTarget();
            var scope = injecter.InjectIntoType(target, false);

            // Assert
            Assert.True(target.IsServiceNotNull);

            DisposeServices(serviceProvider);
            DisposeServices(scope);
        }

        [Fact]
        public void NotUsingCachingWorks()
        {
            // Arrange
            var (injecter, serviceProvider) = CreateInjecter(
                services => services.AddSingleton<ISimpleService, SimpleService>(),
                options => options.UseCaching = false);

            // Act
            var target = new SimpleInjectTarget();
            var scope = injecter.InjectIntoType(target, false);

            // Assert
            Assert.True(target.IsServiceNotNull && target.SimpleService is not null);

            DisposeServices(serviceProvider);
            DisposeServices(scope);
        }

        [Fact]
        public void AddInjecterThrowsAneWhenServicesNull()
        {
            // Arrange
            IServiceCollection? services = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            void Act() => services!.AddInjecter();

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void InjecterValidatesArguments()
        {
            // Arrange
            var (injecter, serviceProvider) = CreateInjecter(services => services.AddSingleton<ISimpleService, SimpleService>());

            // Act
            void Act1() => injecter.InjectIntoType(null!, null!, false);
            void Act2() => injecter.InjectIntoType(typeof(object), null!, false);

            // Assert
            Assert.Throws<ArgumentNullException>(Act1);
            Assert.Throws<ArgumentNullException>(Act2);
            DisposeServices(serviceProvider);
        }

        [Fact]
        public void CompositionRootRetainsServiceProvider()
        {
            // Arrange
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            // Act
            CompositionRoot.ServiceProvider = serviceProvider;

            // Assert
            Assert.NotNull(CompositionRoot.ServiceProvider);

            DisposeServices(CompositionRoot.ServiceProvider);
            DisposeServices(serviceProvider);
            CompositionRoot.ServiceProvider = null;
        }

        [Fact]
        public void NoScopeIsCreated()
        {
            // Arrange
            var (injecter, serviceProvider) = CreateInjecter(services => services.AddSingleton<ISimpleService, SimpleService>());

            // Act
            var target = new SimpleInjectTarget();
            var scope = injecter.InjectIntoType(target, false);

            // Assert
            Assert.True(target.IsServiceNotNull && target.SimpleService is not null);
            Assert.Null(scope);

            DisposeServices(serviceProvider);
        }

        private static (IInjecter injecter, ServiceProvider serviceProvider) CreateInjecter(Action<IServiceCollection> configureServices, Action<InjecterOptions>? optionsBuilder = default)
        {
            var services = new ServiceCollection();

            configureServices(services);

            services.AddInjecter(optionsBuilder);

            var serviceProvider = services.BuildServiceProvider();

            return (serviceProvider.GetRequiredService<IInjecter>(), serviceProvider);
        }

        private static void DisposeServices(IServiceProvider? serviceProvider)
        {
            if (serviceProvider is IDisposable disposable) disposable.Dispose();
        }

        private static void DisposeServices(IServiceScope? serviceScope)
        {
            if (serviceScope is IDisposable disposable) disposable.Dispose();
        }
    }
}
