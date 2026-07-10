using FluentAssertions;
using Spectre.Console.Cli;
using Vibe.UI.CLI.Commands;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using Vibe.UI.CLI.Tests.Helpers;
using Xunit;

namespace Vibe.UI.CLI.Tests.Commands;

/// <summary>
/// Tests for UpdateCommand. Uses SpectreConsole collection to prevent parallel execution
/// due to Spectre.Console's global exclusivity lock for interactive operations.
/// </summary>
[Collection("SpectreConsole")]
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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "update",
            null);

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

        var componentPath = Path.Combine(_testProjectPath, "Components", "Button.razor");
        await File.WriteAllTextAsync(componentPath, "old version");

        var settings = new UpdateCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "update",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().NotBe("old version");
        content.Should().Contain("vibe-button");
        (await File.ReadAllTextAsync(Path.Combine(_testProjectPath, "_Imports.razor")))
            .Should().Contain("@using global::Vibe.UI.Components");
    }

    [Fact]
    public async Task ExecuteAsync_FromWebAppRoot_UpdatesInitializedClientProject()
    {
        // Arrange
        var clientPath = await CreateWebAppProjectsAsync();
        await InitializeProjectWithComponent("button", clientPath);
        var componentPath = Path.Combine(clientPath, "Components", "Button.razor");
        await File.WriteAllTextAsync(componentPath, "old version");

        var settings = new UpdateCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "update",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        (await File.ReadAllTextAsync(componentPath)).Should().Contain("vibe-button");
    }

    [Fact]
    public async Task ExecuteAsync_WithSkipPrompts_UpdatesAllComponents()
    {
        // Arrange
        await InitializeProjectWithMultipleComponents();

        var buttonPath = Path.Combine(_testProjectPath, "Components", "Button.razor");
        var checkboxPath = Path.Combine(_testProjectPath, "Components", "Checkbox.razor");
        await File.WriteAllTextAsync(buttonPath, "old button");
        await File.WriteAllTextAsync(checkboxPath, "old checkbox");

        var settings = new UpdateCommand.Settings
        {
            Component = null,
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "update",
            null);

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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "update",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        var buttonPath = Path.Combine(_testProjectPath, "Components", "Button.razor");
        File.Exists(buttonPath).Should().BeTrue();

        // Checkbox should not be created as it wasn't installed
        var checkboxPath = Path.Combine(_testProjectPath, "Components", "Checkbox.razor");
        File.Exists(checkboxPath).Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_OverwritesExistingFiles()
    {
        // Arrange
        await InitializeProjectWithComponent("button");

        var componentPath = Path.Combine(_testProjectPath, "Components", "Button.razor");
        await File.WriteAllTextAsync(componentPath, "custom modifications");

        var settings = new UpdateCommand.Settings
        {
            Component = "button",
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "update",
            null);

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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "update",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0, "command should complete successfully even with no components to update");
    }

    private async Task InitializeProject(string? projectPath = null)
    {
        projectPath ??= _testProjectPath;
        var configService = new ConfigService();
        await configService.SaveConfigAsync(projectPath, new VibeConfig
        {
            ProjectType = "Blazor WebAssembly",
            Theme = "light",
            ComponentsDirectory = "Components",
            CssVariables = true
        });
    }

    private async Task InitializeProjectWithComponent(string componentName, string? projectPath = null)
    {
        projectPath ??= _testProjectPath;
        await InitializeProject(projectPath);
        var componentService = new ComponentService();
        await componentService.InstallComponentAsync(projectPath, "Components", componentName, false);
    }

    private async Task InitializeProjectWithMultipleComponents()
    {
        await InitializeProject();
        var componentService = new ComponentService();
        await componentService.InstallComponentAsync(_testProjectPath, "Components", "button", false);
        await componentService.InstallComponentAsync(_testProjectPath, "Components", "checkbox", false);
    }

    private async Task<string> CreateWebAppProjectsAsync()
    {
        var serverPath = Path.Combine(_testProjectPath, "Sample");
        var clientPath = Path.Combine(_testProjectPath, "Sample.Client");
        Directory.CreateDirectory(serverPath);
        Directory.CreateDirectory(clientPath);

        await File.WriteAllTextAsync(
            Path.Combine(serverPath, "Sample.csproj"),
            @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <ItemGroup>
    <ProjectReference Include=""..\Sample.Client\Sample.Client.csproj"" />
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly.Server"" Version=""10.0.0"" />
  </ItemGroup>
</Project>");
        await File.WriteAllTextAsync(
            Path.Combine(serverPath, "Program.cs"),
            "builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();");
        await File.WriteAllTextAsync(
            Path.Combine(clientPath, "Sample.Client.csproj"),
            @"<Project Sdk=""Microsoft.NET.Sdk.BlazorWebAssembly"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly"" Version=""10.0.0"" />
  </ItemGroup>
</Project>");

        return clientPath;
    }

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            Directory.Delete(_testProjectPath, true);
        }
    }
}
