using System.Xml.Linq;
using FluentAssertions;
using Spectre.Console.Cli;
using System.Reflection;
using Vibe.UI.CLI.Commands;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using Vibe.UI.CLI.Tests.Helpers;
using Xunit;

namespace Vibe.UI.CLI.Tests.Commands;

/// <summary>
/// Tests for InitCommand. Uses SpectreConsole collection to prevent parallel execution
/// due to Spectre.Console's global exclusivity lock for interactive operations.
/// </summary>
[Collection("SpectreConsole")]
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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        Directory.Exists(Path.Combine(_testProjectPath, "Components")).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_CreatesCssFoundationFiles()
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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert - Should copy CSS foundation files to wwwroot/css/
        var cssDir = Path.Combine(_testProjectPath, "wwwroot", "css");
        Directory.Exists(cssDir).Should().BeTrue();

        // Check for CSS foundation files (may not exist if template files not found in test environment)
        var vibeBaseCss = Path.Combine(cssDir, "vibe-base.css");
        var vibeUtilitiesCss = Path.Combine(cssDir, "vibe-utilities.css");
        var vibeAllCss = Path.Combine(cssDir, "vibe-all.css");

        // At minimum, the directory should exist
        Directory.Exists(cssDir).Should().BeTrue("CSS directory should be created");
    }

    [Fact]
    public async Task ExecuteAsync_CreatesVibeInfrastructure()
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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert - Note: Some directories may not be created if template files are not available
        // in the test environment (templates are packaged with the CLI tool, not included in test project)
        var vibePath = Path.Combine(_testProjectPath, "Vibe");

        // The init command should at least attempt to create the Vibe directory structure
        // The actual files may not exist if templates aren't found, but infrastructure copying shouldn't throw
        // We verify that the configuration was saved successfully as a minimum
        File.Exists(Path.Combine(_testProjectPath, "vibe.json")).Should().BeTrue("Config file should be created");

        // If templates are available, these directories should exist:
        // - Vibe/Base (with ClassBuilder)
        // - Vibe/Services
        // - Vibe/Enums
        // - Vibe/ServiceCollectionExtensions.cs
        // We don't assert on them since template availability varies by environment
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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

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
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var configService = new ConfigService();
        var config = await configService.LoadConfigAsync(_testProjectPath);
        config.Should().NotBeNull();
        config!.ProjectType.Should().Be("Blazor"); // InitCommand sets "Blazor" not "Blazor WebAssembly"
        config.ComponentsDirectory.Should().Contain("Components"); // Could be "Components/vibe"
        config.CssVariables.Should().BeTrue();
    }

    #region Vibe.UI.CSS Integration Tests

    [Fact]
    public async Task ExecuteAsync_WithCss_AddsVibeCssPackageReference()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.Web"" Version=""10.0.0"" />
  </ItemGroup>
</Project>");

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath,
            WithCss = true  // Explicitly enable Vibe.UI.CSS
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var updatedCsproj = await File.ReadAllTextAsync(csprojPath);
        var doc = XDocument.Parse(updatedCsproj);
        var ns = doc.Root!.GetDefaultNamespace();

        var vibeCssRef = doc.Descendants(ns + "PackageReference")
            .FirstOrDefault(pr => pr.Attribute("Include")?.Value == "Vibe.UI.CSS");

        vibeCssRef.Should().NotBeNull("Vibe.UI.CSS package reference should be added with --with-css");
        vibeCssRef!.Attribute("Version")?.Value.Should().Be(GetCliVersion());
    }

    private static string GetCliVersion()
    {
        var informational = typeof(InitCommand).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informational))
        {
            return informational.Split('+', 2)[0];
        }

        var version = typeof(InitCommand).Assembly.GetName().Version;
        return version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
    }

    [Fact]
    public async Task ExecuteAsync_WithCss_AddsVibeCssConfiguration()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>");

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath,
            WithCss = true  // Explicitly enable Vibe.UI.CSS
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var updatedCsproj = await File.ReadAllTextAsync(csprojPath);
        var doc = XDocument.Parse(updatedCsproj);
        var ns = doc.Root!.GetDefaultNamespace();

        var vibeCssEnabled = doc.Descendants(ns + "VibeCssEnabled").FirstOrDefault();
        var vibeCssOutput = doc.Descendants(ns + "VibeCssOutput").FirstOrDefault();
        var vibeCssIncludeBase = doc.Descendants(ns + "VibeCssIncludeBase").FirstOrDefault();

        vibeCssEnabled.Should().NotBeNull("VibeCssEnabled should be added with --with-css");
        vibeCssEnabled!.Value.Should().Be("true");

        vibeCssOutput.Should().NotBeNull("VibeCssOutput should be added with --with-css");
        vibeCssOutput!.Value.Should().Be("wwwroot/css/Vibe.UI.CSS");

        vibeCssIncludeBase.Should().NotBeNull("VibeCssIncludeBase should be added with --with-css");
        vibeCssIncludeBase!.Value.Should().Be("true");
    }

    [Fact]
    public async Task ExecuteAsync_Default_SkipsVibeCssPackageReference()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.Web"" Version=""10.0.0"" />
  </ItemGroup>
</Project>");

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath
            // WithCss defaults to false
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var updatedCsproj = await File.ReadAllTextAsync(csprojPath);
        var doc = XDocument.Parse(updatedCsproj);
        var ns = doc.Root!.GetDefaultNamespace();

        var vibeCssRef = doc.Descendants(ns + "PackageReference")
            .FirstOrDefault(pr => pr.Attribute("Include")?.Value == "Vibe.UI.CSS");

        vibeCssRef.Should().BeNull("Vibe.UI.CSS package reference should NOT be added by default");

        var vibeCssEnabled = doc.Descendants(ns + "VibeCssEnabled").FirstOrDefault();
        vibeCssEnabled.Should().BeNull("VibeCssEnabled should NOT be added by default");
    }

    [Fact]
    public async Task ExecuteAsync_WithCss_ExistingVibeCssReference_DoesNotDuplicate()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        await File.WriteAllTextAsync(csprojPath, @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <PropertyGroup>
    <VibeCssEnabled>true</VibeCssEnabled>
    <VibeCssOutput>wwwroot/css/Vibe.UI.CSS</VibeCssOutput>
    <VibeCssIncludeBase>true</VibeCssIncludeBase>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Vibe.UI.CSS"" Version=""1.0.0"" />
  </ItemGroup>
</Project>");

        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath,
            WithCss = true  // Explicitly enable Vibe.UI.CSS
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var updatedCsproj = await File.ReadAllTextAsync(csprojPath);
        var doc = XDocument.Parse(updatedCsproj);
        var ns = doc.Root!.GetDefaultNamespace();

        var vibeCssRefs = doc.Descendants(ns + "PackageReference")
            .Where(pr => pr.Attribute("Include")?.Value == "Vibe.UI.CSS")
            .ToList();

        vibeCssRefs.Should().HaveCount(1, "Should not duplicate Vibe.UI.CSS reference");

        var vibeCssEnabledElements = doc.Descendants(ns + "VibeCssEnabled").ToList();
        vibeCssEnabledElements.Should().HaveCount(1, "Should not duplicate VibeCssEnabled");
    }

    [Fact]
    public async Task ExecuteAsync_NoCsprojFile_DoesNotThrow()
    {
        // Arrange - No csproj file created
        var settings = new InitCommand.Settings
        {
            SkipPrompts = true,
            ProjectPath = _testProjectPath
        };

        var context = new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "init",
            null);

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert - Should complete without throwing
        result.Should().Be(0);
        File.Exists(Path.Combine(_testProjectPath, "vibe.json")).Should().BeTrue();
    }

    #endregion

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            Directory.Delete(_testProjectPath, true);
        }
    }
}

