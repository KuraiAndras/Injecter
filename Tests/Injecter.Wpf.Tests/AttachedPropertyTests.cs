using FluentAssertions;
using SampleLogic;
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
    }
}
