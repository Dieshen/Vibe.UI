using FluentAssertions;
using Spectre.Console.Cli;
using Vibe.UI.CLI.Commands;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Commands;

public class UpdateCommandTests : IDisposable
{
    private readonly string _testProjectPath;
    private readonly UpdateCommand _command;

    public UpdateCommandTests()
    {
        _testProjectPath = Path.Combine(Path.GetTempPath(), $"vibe-update-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testProjectPath);
        _command = new UpdateCommand();
    }

    [Fact]
    public async Task ExecuteAsync_WithoutConfig_ReturnsError()
    {
        // Arrange
        var settings = new UpdateCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "update",
            data: null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(1, "command should fail when config doesn't exist");
    }

    [Fact]
    public async Task ExecuteAsync_WithSpecificComponent_UpdatesComponent()
    {
        // Arrange
        await InitializeProjectWithComponent("button");

        var componentPath = Path.Combine(_testProjectPath, "Components", "Input", "Button.razor");
        await File.WriteAllTextAsync(componentPath, "old version");

        var settings = new UpdateCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "update",
            data: null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().NotBe("old version");
        content.Should().Contain("vibe-button");
    }

    [Fact]
    public async Task ExecuteAsync_WithSkipPrompts_UpdatesAllComponents()
    {
        // Arrange
        await InitializeProjectWithMultipleComponents();

        var buttonPath = Path.Combine(_testProjectPath, "Components", "Input", "Button.razor");
        var checkboxPath = Path.Combine(_testProjectPath, "Components", "Input", "Checkbox.razor");
        await File.WriteAllTextAsync(buttonPath, "old button");
        await File.WriteAllTextAsync(checkboxPath, "old checkbox");

        var settings = new UpdateCommand.Settings
        {
            Component = null,
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "update",
            data: null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var buttonContent = await File.ReadAllTextAsync(buttonPath);
        var checkboxContent = await File.ReadAllTextAsync(checkboxPath);
        buttonContent.Should().NotBe("old button");
        checkboxContent.Should().NotBe("old checkbox");
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesOnlyInstalledComponents()
    {
        // Arrange
        await InitializeProjectWithComponent("button");

        var settings = new UpdateCommand.Settings
        {
            Component = null,
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "update",
            data: null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var buttonPath = Path.Combine(_testProjectPath, "Components", "Input", "Button.razor");
        File.Exists(buttonPath).Should().BeTrue();

        // Checkbox should not be created as it wasn't installed
        var checkboxPath = Path.Combine(_testProjectPath, "Components", "Input", "Checkbox.razor");
        File.Exists(checkboxPath).Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_OverwritesExistingFiles()
    {
        // Arrange
        await InitializeProjectWithComponent("button");

        var componentPath = Path.Combine(_testProjectPath, "Components", "Input", "Button.razor");
        await File.WriteAllTextAsync(componentPath, "custom modifications");

        var settings = new UpdateCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "update",
            data: null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().NotContain("custom modifications");
        content.Should().Contain("vibe-button");
    }

    [Fact]
    public async Task ExecuteAsync_WithNoInstalledComponents_CompletesSuccessfully()
    {
        // Arrange
        await InitializeProject();

        var settings = new UpdateCommand.Settings
        {
            Component = null,
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "update",
            data: null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0, "command should complete successfully even with no components to update");
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

    private async Task InitializeProjectWithComponent(string componentName)
    {
        await InitializeProject();
        var componentService = new ComponentService();
        await componentService.InstallComponentAsync(_testProjectPath, "Components", componentName, false);
    }

    private async Task InitializeProjectWithMultipleComponents()
    {
        await InitializeProject();
        var componentService = new ComponentService();
        await componentService.InstallComponentAsync(_testProjectPath, "Components", "button", false);
        await componentService.InstallComponentAsync(_testProjectPath, "Components", "checkbox", false);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            Directory.Delete(_testProjectPath, true);
        }
    }
}
