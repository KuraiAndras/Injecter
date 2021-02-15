using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SampleLogic;
using System.Windows;
using WpfSample;
using Xunit;

namespace Injecter.Wpf.Tests
{
    public sealed class AttachedPropertyTests : TestBase
    {
        [WpfFact]
        public void UsingAttachedPropertiesInjects()
        {
            // Arrange, Act
            var control = new HelloControl();

            // Assert
            control.DataContext.Should().BeAssignableTo<ICounter>();
        }

        [WpfFact]
        public void UsingAttachedPropertiesInjectsScoped()
        {
            // Arrange
            var control = new ScopedHelloControl();
            var controlAsDo = (FrameworkElement)control;

            // Act, Assert
            CompositionRoot.ServiceProvider!
                .GetRequiredService<IScopeStore>()
                .GetScope(control)
                .Should().NotBeNull();

            XamlInjecter.CleanUp(ref controlAsDo);

            // Assert
            control.DataContext.Should().BeAssignableTo<ICounter>();
            controlAsDo.Should().BeNull();

            CompositionRoot.ServiceProvider!
                .GetRequiredService<ICounter>()
                .Count.Should().Be(1);
        }
    }
}
