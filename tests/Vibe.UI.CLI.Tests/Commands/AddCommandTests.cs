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
        var componentPath = Path.Combine(_testProjectPath, "Components", "Button.razor");
        File.Exists(componentPath).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_FromWebAppRoot_UsesInitializedClientProject()
    {
        // Arrange
        var clientPath = await CreateWebAppProjectsAsync();
        await InitializeProject(clientPath);

        var settings = new AddCommand.Settings
        {
            Component = "dialog",
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
        File.Exists(Path.Combine(clientPath, "Components", "Dialog.razor")).Should().BeTrue();
        File.Exists(Path.Combine(_testProjectPath, "Components", "Dialog.razor")).Should().BeFalse();
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
        // Components use flat structure - all go directly into Components directory
        var tabsPath = Path.Combine(_testProjectPath, "Components", "Tabs.razor");
        var tabItemPath = Path.Combine(_testProjectPath, "Components", "TabItem.razor");
        File.Exists(tabsPath).Should().BeTrue();
        File.Exists(tabItemPath).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithOverwrite_OverwritesExistingComponent()
    {
        // Arrange
        await InitializeProject();

        // Components use flat structure - create directly in Components directory
        var componentDir = Path.Combine(_testProjectPath, "Components");
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

        // Components use flat structure - create directly in Components directory
        var componentDir = Path.Combine(_testProjectPath, "Components");
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
    [InlineData("button", "Button")]
    [InlineData("card", "Card")]
    [InlineData("dialog", "Dialog")]
    [InlineData("alert", "Alert")]
    [InlineData("accordion", "Accordion")]
    public async Task ExecuteAsync_InstallsComponentUsingFlatStructure(string component, string expectedComponentName)
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
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        // Components use flat structure - all go directly into Components directory
        var componentPath = Path.Combine(_testProjectPath, "Components", $"{expectedComponentName}.razor");
        File.Exists(componentPath).Should().BeTrue($"component {expectedComponentName} should be installed in flat structure");
    }

    private async Task InitializeProject(string? projectPath = null)
    {
        projectPath ??= _testProjectPath;

        // Create Vibe infrastructure directory structure
        var vibeBasePath = Path.Combine(projectPath, "Vibe", "Base");
        Directory.CreateDirectory(vibeBasePath);

        // Create required infrastructure files expected by AddCommand
        var classBuilderPath = Path.Combine(vibeBasePath, "ClassBuilder.cs");
        await File.WriteAllTextAsync(classBuilderPath, "// Placeholder for ClassBuilder");

        var vibeComponentPath = Path.Combine(vibeBasePath, "VibeComponent.cs");
        await File.WriteAllTextAsync(vibeComponentPath, "// Placeholder for VibeComponent");

        // Save configuration
        var configService = new ConfigService();
        await configService.SaveConfigAsync(projectPath, new VibeConfig
        {
            ProjectType = "Blazor WebAssembly",
            Theme = "light",
            ComponentsDirectory = "Components",
            CssVariables = true
        });
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
