using FluentAssertions;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Services;

public class ConfigServiceTests : IDisposable
{
    private readonly string _testProjectPath;
    private readonly ConfigService _configService;

    public ConfigServiceTests()
    {
        _testProjectPath = Path.Combine(Path.GetTempPath(), $"vibe-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testProjectPath);
        _configService = new ConfigService();
    }

    [Fact]
    public void ConfigExists_ReturnsFalse_WhenConfigDoesNotExist()
    {
        // Act
        var exists = _configService.ConfigExists(_testProjectPath);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ConfigExists_ReturnsTrue_AfterConfigIsSaved()
    {
        // Arrange
        var config = new VibeConfig
        {
            ProjectType = "Blazor WebAssembly",
            Theme = "light",
            ComponentsDirectory = "Components",
            CssVariables = true
        };

        // Act
        await _configService.SaveConfigAsync(_testProjectPath, config);
        var exists = _configService.ConfigExists(_testProjectPath);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task SaveConfigAsync_CreatesVibeJsonFile()
    {
        // Arrange
        var config = new VibeConfig
        {
            ProjectType = "Blazor Server",
            Theme = "dark",
            ComponentsDirectory = "UI/Components",
            CssVariables = true
        };

        // Act
        await _configService.SaveConfigAsync(_testProjectPath, config);

        // Assert
        var configPath = Path.Combine(_testProjectPath, "vibe.json");
        File.Exists(configPath).Should().BeTrue();
    }

    [Fact]
    public async Task SaveConfigAsync_WritesCorrectJson()
    {
        // Arrange
        var config = new VibeConfig
        {
            ProjectType = "Blazor WebAssembly",
            Theme = "both",
            ComponentsDirectory = "Components",
            CssVariables = false
        };

        // Act
        await _configService.SaveConfigAsync(_testProjectPath, config);
        var json = await File.ReadAllTextAsync(Path.Combine(_testProjectPath, "vibe.json"));

        // Assert
        json.Should().Contain("\"projectType\": \"Blazor WebAssembly\"");
        json.Should().Contain("\"theme\": \"both\"");
        json.Should().Contain("\"componentsDirectory\": \"Components\"");
        json.Should().Contain("\"cssVariables\": false");
    }

    [Fact]
    public async Task LoadConfigAsync_ReturnsNull_WhenConfigDoesNotExist()
    {
        // Act
        var config = await _configService.LoadConfigAsync(_testProjectPath);

        // Assert
        config.Should().BeNull();
    }

    [Fact]
    public async Task LoadConfigAsync_ReturnsConfig_WhenConfigExists()
    {
        // Arrange
        var originalConfig = new VibeConfig
        {
            ProjectType = "Blazor WebAssembly",
            Theme = "light",
            ComponentsDirectory = "Components",
            CssVariables = true
        };
        await _configService.SaveConfigAsync(_testProjectPath, originalConfig);

        // Act
        var loadedConfig = await _configService.LoadConfigAsync(_testProjectPath);

        // Assert
        loadedConfig.Should().NotBeNull();
        loadedConfig!.ProjectType.Should().Be("Blazor WebAssembly");
        loadedConfig.Theme.Should().Be("light");
        loadedConfig.ComponentsDirectory.Should().Be("Components");
        loadedConfig.CssVariables.Should().BeTrue();
    }

    [Fact]
    public async Task SaveAndLoad_RoundTrip_PreservesAllProperties()
    {
        // Arrange
        var originalConfig = new VibeConfig
        {
            ProjectType = "Blazor Server",
            Theme = "dark",
            ComponentsDirectory = "UI/Components",
            CssVariables = false
        };

        // Act
        await _configService.SaveConfigAsync(_testProjectPath, originalConfig);
        var loadedConfig = await _configService.LoadConfigAsync(_testProjectPath);

        // Assert
        loadedConfig.Should().BeEquivalentTo(originalConfig);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            Directory.Delete(_testProjectPath, true);
        }
    }
}
