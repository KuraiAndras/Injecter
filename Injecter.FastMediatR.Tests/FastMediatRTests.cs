using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Xunit;

namespace Injecter.FastMediatR.Tests
{
    public sealed class FastMediatRTests : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        public FastMediatRTests() =>
            _serviceProvider = new ServiceCollection()
                .AddFastMediatR()
                .AddMediatR(typeof(FastMediatRTests).Assembly)
                .BuildServiceProvider();

        [Fact]
        public void FastMediatRWorks()
        {
            // Arrange
            var fastMediator = _serviceProvider.GetRequiredService<IFastMediator>();
            var request = new Add { A = 1, B = 1 };

            // Act
            var result = fastMediator.SendSync(request);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void SendNoResponseWorks()
        {
            // Arrange
            var fastMediator = _serviceProvider.GetRequiredService<IFastMediator>();
            var request = new NoResponse();

            // Act
            fastMediator.SendSync(request);

            // Assert
            Assert.True(true);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void FastMediatRMightBeFasterThanSyncing(int iterationCount)
        {
            // Arrange
            var fastMediator = _serviceProvider.GetRequiredService<IFastMediator>();
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            var request = new Add { A = 1, B = 1 };

            // Act
            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < iterationCount; i++)
            {
                mediator.Send(request).GetAwaiter().GetResult();
            }

            stopwatch.Stop();

            var mediatorTime = stopwatch.Elapsed;

            stopwatch.Restart();

            for (var i = 0; i < iterationCount; i++)
            {
                fastMediator.SendSync(request);
            }

            stopwatch.Stop();
            var fastMediatorTime = stopwatch.Elapsed;

            // Assert
            Assert.True(mediatorTime > fastMediatorTime);
        }

        public void Dispose()
        {
            if (_serviceProvider is IDisposable d) d.Dispose();
        }
    }
}
