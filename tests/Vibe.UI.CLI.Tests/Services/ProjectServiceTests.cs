using FluentAssertions;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using System.Reflection;
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
    public async Task DetectProjectTopologyAsync_ReturnsStandaloneWasm_ForSingleWasmProject()
    {
        // Arrange
        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk.BlazorWebAssembly"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly"" Version=""10.0.0"" />
  </ItemGroup>
</Project>";
        await File.WriteAllTextAsync(Path.Combine(_testProjectPath, "Test.csproj"), csprojContent);

        // Act
        var topology = await _projectService.DetectProjectTopologyAsync(_testProjectPath);

        // Assert
        topology.Kind.Should().Be(BlazorProjectKind.BlazorWebAssembly);
        topology.DisplayName.Should().Be("Blazor WebAssembly");
        topology.HasClientProject.Should().BeTrue();
        topology.HasServerProject.Should().BeFalse();
        topology.ClientProjectPath.Should().Be(_testProjectPath);
    }

    [Fact]
    public async Task DetectProjectTopologyAsync_ReturnsWebApp_ForServerAndClientRoot()
    {
        // Arrange
        var (serverPath, clientPath) = await CreateWebAppProjectsAsync();

        // Act
        var topology = await _projectService.DetectProjectTopologyAsync(_testProjectPath);

        // Assert
        topology.Kind.Should().Be(BlazorProjectKind.BlazorWebApp);
        topology.DisplayName.Should().Be("Blazor Web App");
        topology.ServerProjectPath.Should().Be(serverPath);
        topology.ClientProjectPath.Should().Be(clientPath);
        topology.ClientNamespace.Should().Be("TestApp.Client");
    }

    [Fact]
    public async Task DetectProjectTopologyAsync_ReturnsWebAppClient_WhenRunFromClientProject()
    {
        // Arrange
        var (serverPath, clientPath) = await CreateWebAppProjectsAsync();

        // Act
        var topology = await _projectService.DetectProjectTopologyAsync(clientPath);

        // Assert
        topology.Kind.Should().Be(BlazorProjectKind.BlazorWebAppClient);
        topology.DisplayName.Should().Be("Blazor Web App Client");
        topology.ProjectPath.Should().Be(clientPath);
        topology.ServerProjectPath.Should().Be(serverPath);
        topology.ClientProjectPath.Should().Be(clientPath);
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
        updatedContent.Should().Contain($"Version=\"{GetCliVersion()}\"");
    }

    [Fact]
    public async Task AddPackageReferenceAsync_DoesNotDuplicate_WhenPackageExists()
    {
        // Arrange
        var csprojPath = Path.Combine(_testProjectPath, "Test.csproj");
        var csprojContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <PackageReference Include=""Vibe.UI"" Version=""{GetCliVersion()}"" />
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
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "Vibe.UI.CSS");
        File.Exists(cssPath).Should().BeTrue();
    }

    [Fact]
    public async Task CopyThemeFilesAsync_LightTheme_ContainsLightVariables()
    {
        // Act
        await _projectService.CopyThemeFilesAsync(_testProjectPath, "light");

        // Assert
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "Vibe.UI.CSS");
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
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "Vibe.UI.CSS");
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
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "Vibe.UI.CSS");
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
        var cssPath = Path.Combine(_testProjectPath, "wwwroot", "Vibe.UI.CSS");
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

    private static string GetCliVersion()
    {
        var informational = typeof(ProjectService).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informational))
        {
            return informational.Split('+', 2)[0];
        }

        var version = typeof(ProjectService).Assembly.GetName().Version;
        return version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
    }

    private async Task<(string ServerPath, string ClientPath)> CreateWebAppProjectsAsync()
    {
        var serverPath = Path.Combine(_testProjectPath, "TestApp");
        var clientPath = Path.Combine(_testProjectPath, "TestApp.Client");
        Directory.CreateDirectory(serverPath);
        Directory.CreateDirectory(clientPath);

        await File.WriteAllTextAsync(
            Path.Combine(serverPath, "TestApp.csproj"),
            @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <ItemGroup>
    <ProjectReference Include=""..\TestApp.Client\TestApp.Client.csproj"" />
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly.Server"" Version=""10.0.0"" />
  </ItemGroup>
</Project>");

        await File.WriteAllTextAsync(
            Path.Combine(serverPath, "Program.cs"),
            @"builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TestApp.Client._Imports).Assembly);");

        await File.WriteAllTextAsync(
            Path.Combine(clientPath, "TestApp.Client.csproj"),
            @"<Project Sdk=""Microsoft.NET.Sdk.BlazorWebAssembly"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly"" Version=""10.0.0"" />
  </ItemGroup>
</Project>");

        return (serverPath, clientPath);
    }
}

