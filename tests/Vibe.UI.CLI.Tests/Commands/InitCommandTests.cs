using FluentAssertions;
using Spectre.Console.Cli;
using Vibe.UI.CLI.Commands;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Commands;

public class InitCommandTests : IDisposable
{
    private readonly string _testProjectPath;
    private readonly InitCommand _command;

    public InitCommandTests()
    {
        _testProjectPath = Path.Combine(Path.GetTempPath(), $"vibe-init-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testProjectPath);
        _command = new InitCommand();
    }

    [Fact]
    public async Task ExecuteAsync_WithSkipPrompts_CreatesConfiguration()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.Web"" Version=""8.0.0"" />
  </ItemGroup>
</Project>");

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "init",
            data: null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        File.Exists(Path.Combine(_testProjectPath, "vibe.json")).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_CreatesComponentsDirectory()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk"" />");

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "init",
            data: null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        Directory.Exists(Path.Combine(_testProjectPath, "Components")).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_CreatesWwwrootWithThemeFiles()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk"" />");

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "init",
            data: null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var wwwrootPath = Path.Combine(_testProjectPath, "wwwroot");
        Directory.Exists(wwwrootPath).Should().BeTrue();
        File.Exists(Path.Combine(wwwrootPath, "vibe.css")).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_AddsPackageReference()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        var initialContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>";
        await File.WriteAllTextAsync(csprojPath, initialContent);

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "init",
            data: null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(csprojPath);
        updatedContent.Should().Contain("PackageReference");
        updatedContent.Should().Contain("Vibe.UI");
    }

    [Fact]
    public async Task ExecuteAsync_AlreadyInitialized_WithSkipPrompts_Reconfigures()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk"" />");

        var configService = new ConfigService();
        await configService.SaveConfigAsync(_testProjectPath, new VibeConfig
        {
            ProjectType = "Blazor",
            Theme = "dark",
            ComponentsDirectory = "OldComponents",
            CssVariables = false
        });

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "init",
            data: null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        File.Exists(Path.Combine(_testProjectPath, "vibe.json")).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_SavesCorrectConfiguration()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly"" Version=""8.0.0"" />
  </ItemGroup>
</Project>");

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "init",
            data: null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var configService = new ConfigService();
        var config = await configService.LoadConfigAsync(_testProjectPath);
        config.Should().NotBeNull();
        config!.ProjectType.Should().Be("Blazor WebAssembly");
        config.ComponentsDirectory.Should().Be("Components");
        config.CssVariables.Should().BeTrue();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            Directory.Delete(_testProjectPath, true);
        }
    }
}
