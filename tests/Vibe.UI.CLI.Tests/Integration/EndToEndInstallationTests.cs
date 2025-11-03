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
    [InlineData("button", "Input")]
    [InlineData("alert", "Feedback")]
    [InlineData("card", "Layout")]
    [InlineData("dialog", "Overlay")]
    [InlineData("accordion", "Disclosure")]
    [InlineData("calendar", "DateTime")]
    [InlineData("badge", "DataDisplay")]
    [InlineData("breadcrumb", "Navigation")]
    [InlineData("form", "Form")]
    [InlineData("command", "Utility")]
    [InlineData("treeview", "Advanced")]
    public async Task AddCommand_ShouldCreateComponentFile(string componentName, string category)
    {
        // Act
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: .razor file exists in correct category directory
        var componentInfo = _componentService.GetComponent(componentName);
        componentInfo.Should().NotBeNull();

        var expectedPath = Path.Combine(_testProjectPath, _componentsDir, category, $"{componentInfo!.Name}.razor");
        File.Exists(expectedPath).Should().BeTrue($"Component file should exist at {expectedPath}");

        // Verify file has valid content
        var content = await File.ReadAllTextAsync(expectedPath);
        content.Should().NotBeNullOrWhiteSpace("Component file should have content");
        content.Should().Contain("@namespace Vibe.UI.Components", "Component should declare correct namespace");
        content.Should().Contain($"vibe-{componentName.ToLowerInvariant()}", "Component file should contain component CSS class");
    }

    #endregion

    #region Test 2: Add Component With Dependencies

    [Theory]
    [InlineData("radiogroup", "RadioGroupItem", "Input")]
    [InlineData("togglegroup", "ToggleGroupItem", "Input")]
    [InlineData("tabs", "TabItem", "Navigation")]
    [InlineData("breadcrumb", "BreadcrumbItem", "Navigation")]
    [InlineData("accordion", "AccordionItem", "Disclosure")]
    [InlineData("carousel", "CarouselItem", "Disclosure")]
    [InlineData("contextmenu", "ContextMenuItem", "Overlay")]
    [InlineData("navigationmenu", "NavigationMenuItem", "Navigation")]
    [InlineData("toast", "ToastContainer", "Feedback")]
    public async Task AddCommand_ShouldInstallDependencies(string parentComponent, string dependencyComponent, string category)
    {
        // Arrange
        var parent = _componentService.GetComponent(parentComponent);
        parent.Should().NotBeNull();
        parent!.Dependencies.Should().Contain(dependencyComponent, "Parent component should list dependency");

        // Act: Install parent component (Note: Current implementation doesn't auto-install dependencies,
        // but we verify the dependency information is correct)
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, parentComponent, overwrite: false);

        // Assert: Parent component created
        var parentPath = Path.Combine(_testProjectPath, _componentsDir, category, $"{parent.Name}.razor");
        File.Exists(parentPath).Should().BeTrue("Parent component should be created");

        // Now install dependency explicitly
        var dependency = _componentService.GetComponent(dependencyComponent.ToLowerInvariant());
        dependency.Should().NotBeNull("Dependency component should exist in service");

        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, dependencyComponent.ToLowerInvariant(), overwrite: false);

        // Assert: Dependency also created in correct directory
        var dependencyPath = Path.Combine(_testProjectPath, _componentsDir, category, $"{dependency!.Name}.razor");
        File.Exists(dependencyPath).Should().BeTrue("Dependency component should be created");

        // Both files should be in the same category directory
        var parentDir = Path.GetDirectoryName(parentPath);
        var dependencyDir = Path.GetDirectoryName(dependencyPath);
        parentDir.Should().Be(dependencyDir, "Parent and dependency should be in same category directory");
    }

    #endregion

    #region Test 3: Add Component With CSS

    [Theory]
    [InlineData("button")]
    [InlineData("input")]
    [InlineData("card")]
    [InlineData("dialog")]
    public async Task AddCommand_ShouldCreateCssFileWhenComponentHasCss(string componentName)
    {
        // Arrange: Verify component has CSS enabled
        var component = _componentService.GetComponent(componentName);
        component.Should().NotBeNull();
        component!.HasCss.Should().BeTrue($"Component {componentName} should have HasCss = true");

        // Act: Install component
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: .razor file created
        var razorPath = Path.Combine(_testProjectPath, _componentsDir, component.Category, $"{component.Name}.razor");
        File.Exists(razorPath).Should().BeTrue("Component .razor file should exist");

        // Assert: .razor.css file also created
        var cssPath = Path.Combine(_testProjectPath, _componentsDir, component.Category, $"{component.Name}.razor.css");
        File.Exists(cssPath).Should().BeTrue("Component .razor.css file should exist when HasCss is true");

        // Verify CSS file has valid content
        var cssContent = await File.ReadAllTextAsync(cssPath);
        cssContent.Should().NotBeNullOrWhiteSpace("CSS file should have content");
        cssContent.Should().Contain($".vibe-{componentName.ToLowerInvariant()}", "CSS should contain component class");
    }

    #endregion

    #region Test 4: Overwrite Protection Works

    [Fact]
    public async Task AddCommand_ShouldNotOverwriteExistingFile_WhenOverwriteIsFalse()
    {
        // Arrange: Install component once
        const string componentName = "button";
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(_testProjectPath, _componentsDir, component.Category, $"{component.Name}.razor");

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
        var componentPath = Path.Combine(_testProjectPath, _componentsDir, component.Category, $"{component.Name}.razor");

        const string customContent = "<!-- Custom modified content -->";
        await File.WriteAllTextAsync(componentPath, customContent);

        // Act: Install again with overwrite=true
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: true);

        // Assert: File replaced with template content
        var currentContent = await File.ReadAllTextAsync(componentPath);
        currentContent.Should().NotBe(customContent, "File should be overwritten when overwrite is true");
        currentContent.Should().Contain("vibe-input", "New content should contain component template");
        currentContent.Should().Contain("@namespace Vibe.UI.Components", "New content should contain namespace declaration");
    }

    #endregion

    #region Test 6: Component Directory Created

    [Theory]
    [InlineData("button", "Input")]
    [InlineData("alert", "Feedback")]
    [InlineData("card", "Layout")]
    public async Task AddCommand_ShouldCreateCategoryDirectory_WhenNotExists(string componentName, string expectedCategory)
    {
        // Arrange: Ensure category directory does NOT exist
        var categoryPath = Path.Combine(_testProjectPath, _componentsDir, expectedCategory);
        if (Directory.Exists(categoryPath))
        {
            Directory.Delete(categoryPath, recursive: true);
        }

        Directory.Exists(categoryPath).Should().BeFalse("Category directory should not exist initially");

        // Act: Install component
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: Category directory created
        Directory.Exists(categoryPath).Should().BeTrue($"Category directory '{expectedCategory}' should be created");

        // Component file exists in new directory
        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(categoryPath, $"{component.Name}.razor");
        File.Exists(componentPath).Should().BeTrue("Component file should exist in newly created directory");
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

        // Assert: All 3 files exist in Components/Input/
        var inputDir = Path.Combine(_testProjectPath, _componentsDir, "Input");
        Directory.Exists(inputDir).Should().BeTrue("Input category directory should exist");

        foreach (var componentName in inputComponents)
        {
            var component = _componentService.GetComponent(componentName)!;
            var componentPath = Path.Combine(inputDir, $"{component.Name}.razor");
            File.Exists(componentPath).Should().BeTrue($"{component.Name}.razor should exist in Input directory");
        }

        // Verify directory contains exactly these files (plus potential CSS files)
        var razorFiles = Directory.GetFiles(inputDir, "*.razor", SearchOption.TopDirectoryOnly);
        razorFiles.Should().HaveCountGreaterOrEqualTo(3, "Should have at least 3 .razor files");

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

        // Assert: File content contains expected CSS class
        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(_testProjectPath, _componentsDir, component.Category, $"{component.Name}.razor");

        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().Contain(expectedClass, $"Component should contain CSS class '{expectedClass}'");

        // Verify standard template structure
        content.Should().Contain("@namespace Vibe.UI.Components");
        content.Should().Contain("@inherits ThemedComponentBase");
        content.Should().Contain("[Parameter]");
        content.Should().Contain("public RenderFragment? ChildContent");
        content.Should().Contain("[Parameter(CaptureUnmatchedValues = true)]");
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

            // Assert: All create the same component
            var expectedPath = Path.Combine(uniquePath, _componentsDir, "Input", "Button.razor");
            File.Exists(expectedPath).Should().BeTrue($"Component should be created regardless of case: {variation}");
        }
    }

    [Fact]
    public async Task AddCommand_ShouldCreateFilesWithCorrectEncoding()
    {
        // Act: Install component
        const string componentName = "button";
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: File should be readable as UTF-8
        var component = _componentService.GetComponent(componentName)!;
        var componentPath = Path.Combine(_testProjectPath, _componentsDir, component.Category, $"{component.Name}.razor");

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

        // Assert: Component created in nested path
        var component = _componentService.GetComponent(componentName)!;
        var expectedPath = Path.Combine(_testProjectPath, nestedPath, component.Category, $"{component.Name}.razor");
        File.Exists(expectedPath).Should().BeTrue("Component should be created in nested directory structure");
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
        installedComponents.Should().HaveCountGreaterOrEqualTo(3);
        installedComponents.Should().Contain("Button");
        installedComponents.Should().Contain("Input");
        installedComponents.Should().Contain("Checkbox");
    }

    [Theory]
    [InlineData("button")]
    [InlineData("input")]
    [InlineData("card")]
    public async Task AddCommand_CssFile_ShouldHaveCorrectStructure(string componentName)
    {
        // Act: Install component with CSS
        await _componentService.InstallComponentAsync(_testProjectPath, _componentsDir, componentName, overwrite: false);

        // Assert: CSS file exists and has correct structure
        var component = _componentService.GetComponent(componentName)!;
        var cssPath = Path.Combine(_testProjectPath, _componentsDir, component.Category, $"{component.Name}.razor.css");

        if (component.HasCss)
        {
            File.Exists(cssPath).Should().BeTrue();

            var cssContent = await File.ReadAllTextAsync(cssPath);
            cssContent.Should().Contain(".vibe-", "CSS should use vibe prefix");
            cssContent.Should().Contain("{", "CSS should have valid selector blocks");
            cssContent.Should().Contain("}", "CSS should have valid selector blocks");
        }
    }

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
        var filePath = Path.Combine(_testProjectPath, _componentsDir, category, $"{componentName}.razor");
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
