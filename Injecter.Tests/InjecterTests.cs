﻿using Injecter.Tests.Arrangers;
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
            var (injecter, _) = CreateInjecter(services => services.AddSingleton<ISimpleService, SimpleService>());

            // Act
            var target = new SimpleInjectTarget();
            _ = injecter.InjectIntoType(target);

            // Assert
            Assert.True(target.IsServiceNotNull);
        }

        private static (IInjecter injecter, ServiceProvider serviceProvider) CreateInjecter(Action<IServiceCollection> configureServices, Action<InjecterOptions>? optionsBuilder = default)
        {
            var services = new ServiceCollection();

            configureServices(services);

            services.AddInjecter(optionsBuilder);

            var serviceProvider = services.BuildServiceProvider();

            return (serviceProvider.GetRequiredService<IInjecter>(), serviceProvider);
        }
    }
}
