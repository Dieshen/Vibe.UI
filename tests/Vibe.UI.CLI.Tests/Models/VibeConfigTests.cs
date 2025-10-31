using FluentAssertions;
using Vibe.UI.CLI.Models;
using Xunit;

namespace Vibe.UI.CLI.Tests.Models;

public class VibeConfigTests
{
    [Fact]
    public void VibeConfig_CanBeInstantiated()
    {
        // Act
        var config = new VibeConfig();

        // Assert
        config.Should().NotBeNull();
    }

    [Fact]
    public void VibeConfig_AllowsSettingProperties()
    {
        // Arrange
        var config = new VibeConfig();

        // Act
        config.ProjectType = "Blazor WebAssembly";
        config.Theme = "dark";
        config.ComponentsDirectory = "UI/Components";
        config.CssVariables = false;

        // Assert
        config.ProjectType.Should().Be("Blazor WebAssembly");
        config.Theme.Should().Be("dark");
        config.ComponentsDirectory.Should().Be("UI/Components");
        config.CssVariables.Should().BeFalse();
    }

    [Fact]
    public void VibeConfig_PropertiesAreNullableOrOptional()
    {
        // Act
        var config = new VibeConfig();

        // Assert - Should not throw exceptions
        config.ProjectType = null!;
        config.Theme = null!;
        config.ComponentsDirectory = null!;
    }
}
