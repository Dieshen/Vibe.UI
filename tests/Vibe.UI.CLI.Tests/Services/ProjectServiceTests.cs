using FluentAssertions;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Services;

public class ProjectServiceTests : IDisposable
{
    private readonly string _testProjectPath;
    private readonly ProjectService _projectService;

    public ProjectServiceTests()
    {
        _testProjectPath = Path.Combine(Path.GetTempPath(), $"vibe-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testProjectPath);
        _projectService = new ProjectService();
    }

    [Fact]
    public async Task DetectProjectTypeAsync_ReturnsUnknown_WhenNoCsprojExists()
    {
        // Act
        var projectType = await _projectService.DetectProjectTypeAsync(_testProjectPath);

        // Assert
        projectType.Should().Be("Unknown");
    }

    [Fact]
    public async Task DetectProjectTypeAsync_ReturnsBlazorWebAssembly_ForWasmProject()
    {
        // Arrange
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk.BlazorWebAssembly"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly"" Version=""8.0.0"" />
  </ItemGroup>
</Project>";
        await File.WriteAllTextAsync(Path.Combine(_testProjectPath, "Test.csproj"), csprojContent);

        // Act
        var projectType = await _projectService.DetectProjectTypeAsync(_testProjectPath);

        // Assert
        projectType.Should().Be("Blazor WebAssembly");
    }

    [Fact]
    public async Task DetectProjectTypeAsync_ReturnsBlazorServer_ForServerProject()
    {
        // Arrange
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.Web"" Version=""8.0.0"" />
  </ItemGroup>
</Project>";
        await File.WriteAllTextAsync(Path.Combine(_testProjectPath, "Test.csproj"), csprojContent);

        // Act
        var projectType = await _projectService.DetectProjectTypeAsync(_testProjectPath);

        // Assert
        projectType.Should().Be("Blazor Server");
    }

    [Fact]
    public async Task DetectProjectTypeAsync_ReturnsBlazor_ForGenericBlazorProject()
    {
        // Arrange
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk.Razor"">
  <ItemGroup>
    <PackageReference Include=""SomeOtherPackage"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";
        await File.WriteAllTextAsync(Path.Combine(_testProjectPath, "Test.csproj"), csprojContent);

        // Act
        var projectType = await _projectService.DetectProjectTypeAsync(_testProjectPath);

        // Assert
        projectType.Should().Be("Blazor");
    }

    [Fact]
    public async Task AddPackageReferenceAsync_AddsPackageToProject()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>";
        await File.WriteAllTextAsync(csprojPath, csprojContent);

        // Act
        await _projectService.AddPackageReferenceAsync(_testProjectPath, "Vibe.UI");

        // Assert
        var updatedContent = await File.ReadAllTextAsync(csprojPath);
        updatedContent.Should().Contain("PackageReference");
        updatedContent.Should().Contain("Include=\"Vibe.UI\"");
        updatedContent.Should().Contain("Version=\"1.0.0\"");
    }

    [Fact]
    public async Task AddPackageReferenceAsync_DoesNotDuplicate_WhenPackageExists()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <PackageReference Include=""Vibe.UI"" Version=""1.0.0"" />
  </ItemGroup>
</Project>";
        await File.WriteAllTextAsync(csprojPath, csprojContent);

        // Act
        await _projectService.AddPackageReferenceAsync(_testProjectPath, "Vibe.UI");

        // Assert
        var updatedContent = await File.ReadAllTextAsync(csprojPath);
        var occurrences = System.Text.RegularExpressions.Regex.Matches(updatedContent, "Include=\"Vibe\\.UI\"").Count;
        occurrences.Should().Be(1);
    }

    [Fact]
    public async Task AddPackageReferenceAsync_DoesNothing_WhenNoCsprojExists()
    {
        // Act
        await _projectService.AddPackageReferenceAsync(_testProjectPath, "Vibe.UI");

        // Assert - No exception thrown and no files created
        Directory.GetFiles(_testProjectPath).Should().BeEmpty();
    }

    [Theory]
    [InlineData("light")]
    [InlineData("dark")]
    [InlineData("both")]
    public async Task CopyThemeFilesAsync_CreatesVibeCSS(string theme)
    {
        // Act
        await _projectService.CopyThemeFilesAsync(_testProjectPath, theme);

        // Assert
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "vibe.css");
        File.Exists(cssPath).Should().BeTrue();
    }

    [Fact]
    public async Task CopyThemeFilesAsync_LightTheme_ContainsLightVariables()
    {
        // Act
        await _projectService.CopyThemeFilesAsync(_testProjectPath, "light");

        // Assert
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "vibe.css");
        var content = await File.ReadAllTextAsync(cssPath);
        content.Should().Contain("--vibe-background: #ffffff");
        content.Should().Contain("--vibe-foreground: #111111");
    }

    [Fact]
    public async Task CopyThemeFilesAsync_DarkTheme_ContainsDarkVariables()
    {
        // Act
        await _projectService.CopyThemeFilesAsync(_testProjectPath, "dark");

        // Assert
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "vibe.css");
        var content = await File.ReadAllTextAsync(cssPath);
        content.Should().Contain("--vibe-background: #1a1a1a");
        content.Should().Contain("--vibe-foreground: #ffffff");
    }

    [Fact]
    public async Task CopyThemeFilesAsync_BothThemes_ContainsBothVariables()
    {
        // Act
        await _projectService.CopyThemeFilesAsync(_testProjectPath, "both");

        // Assert
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "vibe.css");
        var content = await File.ReadAllTextAsync(cssPath);
        content.Should().Contain("--vibe-background: #ffffff");
        content.Should().Contain("--vibe-background: #1a1a1a");
        content.Should().Contain(".dark");
    }

    [Fact]
    public async Task CopyThemeFilesAsync_DefaultsToLight_ForInvalidTheme()
    {
        // Act
        await _projectService.CopyThemeFilesAsync(_testProjectPath, "invalid");

        // Assert
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "vibe.css");
        var content = await File.ReadAllTextAsync(cssPath);
        content.Should().Contain("--vibe-background: #ffffff");
        content.Should().NotContain(".dark");
    }

    [Fact]
    public async Task CopyThemeFilesAsync_CreatesWwwrootDirectory()
    {
        // Act
        await _projectService.CopyThemeFilesAsync(_testProjectPath, "light");

        // Assert
        var wwwrootPath = Path.Combine(_testProjectPath, "wwwroot");
        Directory.Exists(wwwrootPath).Should().BeTrue();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            Directory.Delete(_testProjectPath, true);
        }
    }
}
