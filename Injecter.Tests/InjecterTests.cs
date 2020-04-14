using Injecter.Tests.Arrangers;
using Injecter.Tests.Arrangers.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var scope = injecter.InjectIntoType(target);

            // Assert
            Assert.True(target.IsServiceNotNull && !(target.SimpleService is null));

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
            var scope = injecter.InjectIntoType(target);

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
            var scope = injecter.InjectIntoType(target);

            // Assert
            Assert.True(target.IsServiceNotNull && !(target.SimpleService is null));

            DisposeServices(serviceProvider);
            DisposeServices(scope);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(4000)]
        public void CachingIsFasterAfterAround3000Iterations(int iterationCount)
        {
            // Arrange
            var stopwatch = Stopwatch.StartNew();

            var (injecter, serviceProvider) = CreateInjecter(
                services => services.AddSingleton<ISimpleService, SimpleService>());

            // Act
            RunInjections(injecter, serviceProvider, iterationCount);

            stopwatch.Stop();

            var time1 = stopwatch.ElapsedTicks;

            // Arrange
            stopwatch.Restart();

            (injecter, serviceProvider) = CreateInjecter(
                services => services.AddSingleton<ISimpleService, SimpleService>(),
                options => options.UseCaching = false);

            // Act
            RunInjections(injecter, serviceProvider, iterationCount);

            stopwatch.Stop();

            var time2 = stopwatch.ElapsedTicks;

            // Assert
            var condition = time1 < time2;
            if (iterationCount >= 3000)
            {
                Assert.True(condition);
            }
            else
            {
                Assert.False(condition);
            }

            static void RunInjections(IInjecter injecter, IServiceProvider serviceProvider, int iterationCount)
            {
                var scopesToDispose = new List<IServiceScope>();
                for (var i = 0; i < iterationCount; i++)
                {
                    var target = new SimpleInjectTarget();
                    var scope = injecter.InjectIntoType(target);
                    scopesToDispose.Add(scope);

                    Assert.True(target.IsServiceNotNull && !(target.SimpleService is null));
                }

                foreach (var serviceScope in scopesToDispose)
                {
                    DisposeServices(serviceScope);
                }

                DisposeServices(serviceProvider);

                scopesToDispose.Clear();
            }
        }

        [Fact]
        public void AddInjecterThrowsAneWhenServicesNull()
        {
            // Arrange
            IServiceCollection? services = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            void Act() => services.AddInjecter();

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void InjecterValidatesArguments()
        {
            // Arrange
            var (injecter, serviceProvider) = CreateInjecter(services => services.AddSingleton<ISimpleService, SimpleService>());

            // Act
            void Act1() => injecter.InjectIntoType(null, null);
            void Act2() => injecter.InjectIntoType(typeof(object), null);

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

        private static (IInjecter injecter, ServiceProvider serviceProvider) CreateInjecter(Action<IServiceCollection> configureServices, Action<InjecterOptions>? optionsBuilder = default)
        {
            var services = new ServiceCollection();

            configureServices(services);

            services.AddInjecter(optionsBuilder);

            var serviceProvider = services.BuildServiceProvider();

            return (serviceProvider.GetRequiredService<IInjecter>(), serviceProvider);
        }

        private static void DisposeServices(IServiceProvider serviceProvider)
        {
            if (serviceProvider is IDisposable disposable) disposable.Dispose();
        }

        private static void DisposeServices(IServiceScope serviceScope)
        {
            if (serviceScope is IDisposable disposable) disposable.Dispose();
        }
    }
}
