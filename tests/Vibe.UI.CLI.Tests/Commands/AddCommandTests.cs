using FluentAssertions;
using Spectre.Console.Cli;
using Vibe.UI.CLI.Commands;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using Vibe.UI.CLI.Tests.Helpers;
using Xunit;

namespace Vibe.UI.CLI.Tests.Commands;

/// <summary>
/// Tests for AddCommand. Uses SpectreConsole collection to prevent parallel execution
/// due to Spectre.Console's global exclusivity lock for interactive operations.
/// </summary>
[Collection("SpectreConsole")]
public class AddCommandTests : IDisposable
{
    private readonly string _testProjectPath;
    private readonly AddCommand _command;

    public AddCommandTests()
    {
        _testProjectPath = Path.Combine(Path.GetTempPath(), $"vibe-add-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testProjectPath);
        _command = new AddCommand();
    }

    [Fact]
    public async Task ExecuteAsync_WithoutConfig_ReturnsError()
    {
        // Arrange
        var settings = new AddCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "add",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(1, "command should fail when config doesn't exist");
    }

    [Fact]
    public async Task ExecuteAsync_WithValidComponent_InstallsComponent()
    {
        // Arrange
        await InitializeProject();

        var settings = new AddCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "add",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var componentPath = Path.Combine(_testProjectPath, "Components", "Input", "Button.razor");
        File.Exists(componentPath).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidComponent_ReturnsError()
    {
        // Arrange
        await InitializeProject();

        var settings = new AddCommand.Settings
        {
            Component = "nonexistent",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "add",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(1, "command should fail for non-existent component");
    }

    [Fact]
    public async Task ExecuteAsync_WithDependencies_InstallsDependencies()
    {
        // Arrange
        await InitializeProject();

        var settings = new AddCommand.Settings
        {
            Component = "tabs",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "add",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var tabsPath = Path.Combine(_testProjectPath, "Components", "Navigation", "Tabs.razor");
        var tabItemPath = Path.Combine(_testProjectPath, "Components", "Navigation", "TabItem.razor");
        File.Exists(tabsPath).Should().BeTrue();
        File.Exists(tabItemPath).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithOverwrite_OverwritesExistingComponent()
    {
        // Arrange
        await InitializeProject();

        var componentDir = Path.Combine(_testProjectPath, "Components", "Input");
        Directory.CreateDirectory(componentDir);
        var componentPath = Path.Combine(componentDir, "Button.razor");
        await File.WriteAllTextAsync(componentPath, "old content");

        var settings = new AddCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            Overwrite = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "add",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().NotBe("old content");
        content.Should().Contain("vibe-button");
    }

    [Fact]
    public async Task ExecuteAsync_WithoutOverwrite_PreservesExistingComponent()
    {
        // Arrange
        await InitializeProject();

        var componentDir = Path.Combine(_testProjectPath, "Components", "Input");
        Directory.CreateDirectory(componentDir);
        var componentPath = Path.Combine(componentDir, "Button.razor");
        await File.WriteAllTextAsync(componentPath, "old content");

        var settings = new AddCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            Overwrite = false,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "add",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().Be("old content");
    }

    [Theory]
    [InlineData("button", "Input")]
    [InlineData("card", "Layout")]
    [InlineData("dialog", "Overlay")]
    [InlineData("alert", "Feedback")]
    [InlineData("accordion", "Disclosure")]
    public async Task ExecuteAsync_InstallsComponentInCorrectCategory(string component, string category)
    {
        // Arrange
        await InitializeProject();

        var settings = new AddCommand.Settings
        {
            Component = component,
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "add",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var categoryPath = Path.Combine(_testProjectPath, "Components", category);
        Directory.Exists(categoryPath).Should().BeTrue();
    }

    private async Task InitializeProject()
    {
        var configService = new ConfigService();
        await configService.SaveConfigAsync(_testProjectPath, new VibeConfig
        {
            ProjectType = "Blazor WebAssembly",
            Theme = "light",
            ComponentsDirectory = "Components",
            CssVariables = true
        });
    }

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            Directory.Delete(_testProjectPath, true);
        }
    }
}
