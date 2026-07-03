using FluentAssertions;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Integration;

/// <summary>
/// End-to-end integration tests that verify the complete 'vibe add' flow.
/// These tests use actual file system operations to validate that component installation
/// produces working, installable components.
/// </summary>
public class EndToEndInstallationTests : IDisposable
{
    private readonly string _testProjectPath;
    private readonly ComponentService _componentService;
    private readonly string _componentsDir = "Components";

    public EndToEndInstallationTests()
    {
        // Create unique temp directory for this test run
        _testProjectPath = Path.Combine(Path.GetTempPath(), $"VibeTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testProjectPath);

        // Initialize test project structure
        InitializeTestProject();

        // Create service instance
        _componentService = new ComponentService();
    }

    #region Test 1: Add Simple Component Creates File

    [Theory]
    [InlineData("button")]
    [InlineData("alert")]
    [InlineData("card")]
    [InlineData("dialog")]
    [InlineData("accordion")]
    [InlineData("calendar")]
    [InlineData("badge")]
    [InlineData("breadcrumb")]
    [InlineData("form")]
    [InlineData("command")]
    [InlineData("treeview")]
    public async Task AddCommand_ShouldCreateComponentFile(string componentName)
    {
        // Act
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: .razor file exists in flat directory structure (like shadcn/ui)
        var componentInfo = _componentService.GetComponent(componentName);
        componentInfo.Should().NotBeNull();

        var expectedPath = Path.Combine(_testProjectPath, _componentsDir, $"{componentInfo!.Name}.razor");
        File.Exists(expectedPath).Should().BeTrue($"Component file should exist at {expectedPath}");

        // Verify file has valid content
        var content = await File.ReadAllTextAsync(expectedPath);
        content.Should().NotBeNullOrWhiteSpace("Component file should have content");
        // Check for basic Razor component structure
        content.Should().Contain("@", "Component should contain Razor directives");
        content.Should().Contain("vibe-", "Component file should contain vibe CSS class prefix");
    }

    #endregion

    #region Test 2: Add Component With Dependencies

    [Theory]
    [InlineData("radiogroup", "RadioGroupItem")]
    [InlineData("togglegroup", "ToggleGroupItem")]
    [InlineData("tabs", "TabItem")]
    [InlineData("breadcrumb", "BreadcrumbItem")]
    [InlineData("accordion", "AccordionItem")]
    [InlineData("carousel", "CarouselItem")]
    [InlineData("contextmenu", "ContextMenuItem")]
    [InlineData("navigationmenu", "NavigationMenuItem")]
    [InlineData("toast", "ToastContainer")]
    public async Task AddCommand_ShouldInstallDependencies(string parentComponent, string dependencyComponent)
    {
        // Arrange
        var parent = _componentService.GetComponent(parentComponent);
        parent.Should().NotBeNull();
        parent!.Dependencies.Should().Contain(dependencyComponent, "Parent component should list dependency");

        // Act: Install parent component (Note: Current implementation doesn't auto-install dependencies,
        // but we verify the dependency information is correct)
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, parentComponent, overwrite: false);

        // Assert: Parent component created in flat directory structure
        var parentPath = Path.Combine(_testProjectPath, _componentsDir, $"{parent.Name}.razor");
        File.Exists(parentPath).Should().BeTrue("Parent component should be created");

        // Now install dependency explicitly
        var dependency = _componentService.GetComponent(dependencyComponent.ToLowerInvariant());
        dependency.Should().NotBeNull("Dependency component should exist in service");

        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, dependencyComponent.ToLowerInvariant(), overwrite: false);

        // Assert: Dependency also created in flat directory structure
        var dependencyPath = Path.Combine(_testProjectPath, _componentsDir, $"{dependency!.Name}.razor");
        File.Exists(dependencyPath).Should().BeTrue("Dependency component should be created");

        // Both files should be in the same Components directory
        var parentDir = Path.GetDirectoryName(parentPath);
        var dependencyDir = Path.GetDirectoryName(dependencyPath);
        parentDir.Should().Be(dependencyDir, "Parent and dependency should be in same Components directory");
    }

    #endregion

    #region Test 3: Add Component With CSS

    // CSS file creation tests removed - Vibe.UI now uses a single global vibe-components.css file
    // instead of individual component CSS files, following shadcn/ui patterns

    #endregion

    #region Test 4: Overwrite Protection Works

    [Fact]
    public async Task AddCommand_ShouldNotOverwriteExistingFile_WhenOverwriteIsFalse()
    {
        // Arrange: Install component once
        const string componentName = "button";
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(_testProjectPath, _componentsDir, $"{component.Name}.razor");

        // Modify the file with custom content
        const string customContent = "<!-- Custom modified content -->";
        await File.WriteAllTextAsync(componentPath, customContent);

        // Act: Try to install again with overwrite=false
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: Original file content preserved
        var currentContent = await File.ReadAllTextAsync(componentPath);
        currentContent.Should().Be(customContent, "File should not be overwritten when overwrite is false");
    }

    #endregion

    #region Test 5: Overwrite Works When Enabled

    [Fact]
    public async Task AddCommand_ShouldOverwriteExistingFile_WhenOverwriteIsTrue()
    {
        // Arrange: Install component and modify file
        const string componentName = "input";
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(_testProjectPath, _componentsDir, $"{component.Name}.razor");

        const string customContent = "<!-- Custom modified content -->";
        await File.WriteAllTextAsync(componentPath, customContent);

        // Act: Install again with overwrite=true
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: true);

        // Assert: File replaced with template content
        var currentContent = await File.ReadAllTextAsync(componentPath);
        currentContent.Should().NotBe(customContent, "File should be overwritten when overwrite is true");
        currentContent.Should().Contain("vibe-input", "New content should contain component template");
        // Verify it contains either @namespace or @using directives
        (currentContent.Contains("@namespace") || currentContent.Contains("@using")).Should().BeTrue("New content should contain namespace or using directives");
    }

    #endregion

    #region Test 6: Component Directory Created

    [Theory]
    [InlineData("button")]
    [InlineData("alert")]
    [InlineData("card")]
    public async Task AddCommand_ShouldCreateComponentsDirectory_WhenNotExists(string componentName)
    {
        // Arrange: Ensure Components directory does NOT exist
        var componentsPath = Path.Combine(_testProjectPath, _componentsDir);
        if (Directory.Exists(componentsPath))
        {
            Directory.Delete(componentsPath, recursive: true);
        }

        Directory.Exists(componentsPath).Should().BeFalse("Components directory should not exist initially");

        // Act: Install component
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: Components directory created
        Directory.Exists(componentsPath).Should().BeTrue("Components directory should be created");

        // Component file exists in flat directory structure
        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(componentsPath, $"{component.Name}.razor");
        File.Exists(componentPath).Should().BeTrue("Component file should exist in Components directory");
    }

    #endregion

    #region Test 7: Multiple Components in Same Category

    [Fact]
    public async Task AddCommand_ShouldSupportMultipleComponentsInSameCategory()
    {
        // Arrange: Three Input components
        string[] inputComponents = { "button", "input", "checkbox" };

        // Act: Install all three components
        foreach (var componentName in inputComponents)
        {
            await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);
        }

        // Assert: All 3 files exist in flat Components/ directory
        var componentsPath = Path.Combine(_testProjectPath, _componentsDir);
        Directory.Exists(componentsPath).Should().BeTrue("Components directory should exist");

        foreach (var componentName in inputComponents)
        {
            var component = _componentService.GetComponent(componentName)!;
            var componentPath = Path.Combine(componentsPath, $"{component.Name}.razor");
            File.Exists(componentPath).Should().BeTrue($"{component.Name}.razor should exist in Components directory");
        }

        // Verify directory contains at least these 3 razor files
        var razorFiles = Directory.GetFiles(componentsPath, "*.razor", SearchOption.TopDirectoryOnly);
        razorFiles.Should().HaveCount(count => count >= 3, "Should have at least 3 .razor files");

        var componentNames = razorFiles.Select(Path.GetFileNameWithoutExtension).ToList();
        componentNames.Should().Contain("Button");
        componentNames.Should().Contain("Input");
        componentNames.Should().Contain("Checkbox");
    }

    #endregion

    #region Test 8: Template Contains Expected Content

    [Theory]
    [InlineData("button", "vibe-button")]
    [InlineData("dialog", "vibe-dialog")]
    [InlineData("card", "vibe-card")]
    [InlineData("alert", "vibe-alert")]
    [InlineData("checkbox", "vibe-checkbox")]
    public async Task CreatedComponent_ShouldContainExpectedClassName(string componentName, string expectedClass)
    {
        // Act: Install component
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: File content contains expected CSS class in flat directory structure
        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(_testProjectPath, _componentsDir, $"{component.Name}.razor");

        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().Contain(expectedClass, $"Component should contain CSS class '{expectedClass}'");

        // Verify standard template structure
        // Components may use explicit @namespace or rely on implicit namespace from folder structure
        (content.Contains("@namespace") || content.Contains("@using")).Should().BeTrue("Component should have namespace or using directive");
        content.Should().Contain("@inherits");
        content.Should().Contain("[Parameter]");
        content.Should().Contain("ChildContent");
    }

    #endregion

    #region Additional Tests: Validation and Edge Cases

    [Fact]
    public async Task AddCommand_ShouldThrowException_ForNonExistentComponent()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, "nonexistentcomponent", overwrite: false)
        );

        exception.Message.Should().Contain("not found", "Exception should indicate component was not found");
    }

    [Fact]
    public async Task AddCommand_ShouldHandleCaseInsensitiveComponentNames()
    {
        // Arrange: Test various case combinations
        string[] caseVariations = { "button", "BUTTON", "Button", "BuTtOn" };

        // Act: Install using different cases
        foreach (var variation in caseVariations)
        {
            var uniquePath = Path.Combine(_testProjectPath, $"test_{variation}");
            Directory.CreateDirectory(uniquePath);

            await _componentService.InstallComponentAsync(uniquePath, _componentsDir, variation, overwrite: false);

            // Assert: All create the same component in flat directory structure
            var expectedPath = Path.Combine(uniquePath, _componentsDir, "Button.razor");
            File.Exists(expectedPath).Should().BeTrue($"Component should be created regardless of case: {variation}");
        }
    }

    [Fact]
    public async Task AddCommand_ShouldCreateFilesWithCorrectEncoding()
    {
        // Act: Install component
        const string componentName = "button";
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: File should be readable as UTF-8 in flat directory structure
        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(_testProjectPath, _componentsDir, $"{component.Name}.razor");

        var content = await File.ReadAllTextAsync(componentPath, System.Text.Encoding.UTF8);
        content.Should().NotBeNullOrWhiteSpace();

        // Should not throw encoding exceptions
        content.Should().NotContain("�", "File should not contain encoding errors");
    }

    [Fact]
    public async Task AddCommand_ShouldWorkWithNestedComponentDirectories()
    {
        // Arrange: Use nested components directory path
        const string nestedPath = "src/MyApp/Components";

        // Act: Install component
        const string componentName = "dialog";
        await _componentService.InstallComponentAsync(_testProjectPath, nestedPath, componentName, overwrite: false);

        // Assert: Component created in nested path with flat structure
        var component = _componentService.GetComponent(componentName)!;
        var expectedPath = Path.Combine(_testProjectPath, nestedPath, $"{component.Name}.razor");
        File.Exists(expectedPath).Should().BeTrue("Component should be created in nested directory with flat structure");
    }

    [Fact]
    public async Task GetInstalledComponents_ShouldReturnComponentsAfterInstallation()
    {
        // Arrange: Install several components
        string[] componentsToInstall = { "button", "input", "checkbox" };

        foreach (var componentName in componentsToInstall)
        {
            await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);
        }

        // Act: Get installed components
        var installedComponents = _componentService.GetInstalledComponents(_testProjectPath, _componentsDir);

        // Assert: All installed components are detected
        installedComponents.Should().HaveCount(count => count >= 3);
        installedComponents.Should().Contain("Button");
        installedComponents.Should().Contain("Input");
        installedComponents.Should().Contain("Checkbox");
    }

    // CSS structure tests removed - Vibe.UI now uses a single global vibe-components.css file
    // instead of individual component CSS files, following shadcn/ui patterns

    #endregion

    #region Helper Methods

    private void InitializeTestProject()
    {
        // Create basic project structure
        var componentsPath = Path.Combine(_testProjectPath, _componentsDir);
        Directory.CreateDirectory(componentsPath);

        // Create minimal vibe.json config
        var configPath = Path.Combine(_testProjectPath, "vibe.json");
        var configContent = @"{
  ""ProjectType"": ""Blazor"",
  ""Theme"": ""light"",
  ""ComponentsDirectory"": ""Components"",
  ""CssVariables"": true
}";
        File.WriteAllText(configPath, configContent);
    }

    private string ReadComponentFile(string category, string componentName)
    {
        // Flat directory structure - category parameter is ignored
        var filePath = Path.Combine(_testProjectPath, _componentsDir, $"{componentName}.razor");
        return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
    }

    #endregion

    #region Cleanup

    public void Dispose()
    {
        // Clean up test directory
        if (Directory.Exists(_testProjectPath))
        {
            try
            {
                Directory.Delete(_testProjectPath, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors - temp directory will be cleaned by OS eventually
            }
        }
    }

    #endregion
}
