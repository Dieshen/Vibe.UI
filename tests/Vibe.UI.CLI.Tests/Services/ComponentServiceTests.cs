using FluentAssertions;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Services;

public class ComponentServiceTests : IDisposable
{
    private readonly string _testProjectPath;
    private readonly ComponentService _componentService;

    public ComponentServiceTests()
    {
        _testProjectPath = Path.Combine(Path.GetTempPath(), $"vibe-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testProjectPath);
        _componentService = new ComponentService();
    }

    [Fact]
    public void GetAvailableComponents_ReturnsNonEmptyList()
    {
        // Act
        var components = _componentService.GetAvailableComponents();

        // Assert
        components.Should().NotBeEmpty();
        components.Count.Should().BeGreaterThan(100); // We have 109 components
    }

    [Fact]
    public void GetAvailableComponents_ReturnsSortedComponents()
    {
        // Act
        var components = _componentService.GetAvailableComponents();

        // Assert
        var categorySorted = components.Select(c => c.Category).ToList();
        categorySorted.Should().BeInAscendingOrder();
    }

    [Theory]
    [InlineData("button", "Button", "Inputs")]
    [InlineData("checkbox", "Checkbox", "Inputs")]
    [InlineData("card", "Card", "Layout")]
    [InlineData("dialog", "Dialog", "Overlay")]
    [InlineData("tabs", "Tabs", "Navigation")]
    [InlineData("alert", "Alert", "Feedback")]
    [InlineData("accordion", "Accordion", "Disclosure")]
    [InlineData("calendar", "Calendar", "DateTime")]
    [InlineData("treeview", "TreeView", "Advanced")]
    [InlineData("command", "Command", "Utility")]
    [InlineData("themetoggle", "ThemeToggle", "Theme")]
    public void GetComponent_ReturnsCorrectComponent(string name, string expectedName, string expectedCategory)
    {
        // Act
        var component = _componentService.GetComponent(name);

        // Assert
        component.Should().NotBeNull();
        component!.Name.Should().Be(expectedName);
        component.Category.Should().Be(expectedCategory);
    }

    [Fact]
    public void GetComponent_IsCaseInsensitive()
    {
        // Act
        var lowerCase = _componentService.GetComponent("button");
        var upperCase = _componentService.GetComponent("BUTTON");
        var mixedCase = _componentService.GetComponent("BuTtOn");

        // Assert
        lowerCase.Should().NotBeNull();
        upperCase.Should().NotBeNull();
        mixedCase.Should().NotBeNull();
        lowerCase!.Name.Should().Be(upperCase!.Name).And.Be(mixedCase!.Name);
    }

    [Fact]
    public void GetComponent_ReturnsNull_ForNonExistentComponent()
    {
        // Act
        var component = _componentService.GetComponent("nonexistent");

        // Assert
        component.Should().BeNull();
    }

    [Theory]
    [InlineData("radiogroup", "RadioGroupItem")]
    [InlineData("togglegroup", "ToggleGroupItem")]
    [InlineData("tabs", "TabItem")]
    [InlineData("breadcrumb", "BreadcrumbItem")]
    [InlineData("accordion", "AccordionItem")]
    [InlineData("carousel", "CarouselItem")]
    [InlineData("menu", "MenuItem")]
    [InlineData("grid", "GridItem")]
    [InlineData("contextmenu", "ContextMenuItem")]
    public void GetComponent_HasCorrectDependencies(string componentName, string expectedDependency)
    {
        // Act
        var component = _componentService.GetComponent(componentName);

        // Assert
        component.Should().NotBeNull();
        component!.Dependencies.Should().Contain(expectedDependency);
    }

    [Fact]
    public void GetInstalledComponents_ReturnsEmptyList_WhenDirectoryDoesNotExist()
    {
        // Act
        var installed = _componentService.GetInstalledComponents(_testProjectPath, "Components");

        // Assert
        installed.Should().BeEmpty();
    }

    [Fact]
    public void GetInstalledComponents_ReturnsInstalledComponents()
    {
        // Arrange - Flat structure: components go directly to Components/
        var componentsDir = Path.Combine(_testProjectPath, "Components");
        Directory.CreateDirectory(componentsDir);
        File.WriteAllText(Path.Combine(componentsDir, "Button.razor"), "<button></button>");
        File.WriteAllText(Path.Combine(componentsDir, "Checkbox.razor"), "<input type='checkbox' />");

        // Act
        var installed = _componentService.GetInstalledComponents(_testProjectPath, "Components");

        // Assert
        installed.Should().HaveCount(2);
        installed.Should().Contain("Button");
        installed.Should().Contain("Checkbox");
    }

    [Fact]
    public async Task InstallComponentAsync_CreatesComponentFile()
    {
        // Act
        await _componentService.InstallComponentAsync(_testProjectPath, "Components", "button", false);

        // Assert
        var componentPath = Path.Combine(_testProjectPath, "Components", "Button.razor");
        File.Exists(componentPath).Should().BeTrue();
    }

    [Fact]
    public async Task InstallComponentAsync_CreatesComponentInFlatStructure()
    {
        // Act
        await _componentService.InstallComponentAsync(_testProjectPath, "Components", "dialog", false);

        // Assert - Flat structure: component goes directly to Components/Dialog.razor (no category subdirectory)
        var componentPath = Path.Combine(_testProjectPath, "Components", "Dialog.razor");
        File.Exists(componentPath).Should().BeTrue();
    }

    [Theory]
    [InlineData("dialog", "Dialog", "vibe-dialog.js")]
    [InlineData("richtexteditor", "RichTextEditor", "vibe-richtext.js")]
    [InlineData("resizable", "Resizable", "vibe-resizable.js")]
    [InlineData("tabs", "Tabs", "vibe-dom.js")]
    [InlineData("themetoggle", "ThemeToggle", "vibe-theme.js")]
    public async Task InstallComponentAsync_RewritesPackageModulePathForSourceMode(
        string componentName,
        string outputName,
        string moduleFile)
    {
        // Act
        await _componentService.InstallComponentAsync(
            _testProjectPath,
            "Components",
            componentName,
            overwrite: false);

        // Assert
        var componentPath = Path.Combine(_testProjectPath, "Components", $"{outputName}.razor");
        var content = await File.ReadAllTextAsync(componentPath);

        content.Should().Contain($"./js/{moduleFile}");
        content.Should().NotContain("./_content/Vibe.UI/js/");
    }

    [Fact]
    public async Task InstallComponentAsync_ThrowsException_ForNonExistentComponent()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _componentService.InstallComponentAsync(_testProjectPath, "Components", "nonexistent", false));
    }

    [Fact]
    public async Task InstallComponentAsync_ThrowsException_WhenComponentsDirIsRooted()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _componentService.InstallComponentAsync(_testProjectPath, Path.GetTempPath(), "button", false));
    }

    [Fact]
    public async Task InstallComponentAsync_ThrowsException_WhenComponentsDirEscapesProject()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _componentService.InstallComponentAsync(_testProjectPath, "..\\..\\outside", "button", false));
    }

    [Fact]
    public async Task InstallComponentAsync_ThrowsException_WhenCustomOutputDirEscapesProject()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _componentService.InstallComponentAsync(_testProjectPath, "Components", "button", false, customName: null, customOutputDir: "..\\..\\outside"));
    }

    [Fact]
    public async Task InstallComponentAsync_DoesNotOverwrite_WhenOverwriteIsFalse()
    {
        // Arrange - Flat structure: component goes directly to Components/Button.razor
        var componentDir = Path.Combine(_testProjectPath, "Components");
        Directory.CreateDirectory(componentDir);
        var componentPath = Path.Combine(componentDir, "Button.razor");
        await File.WriteAllTextAsync(componentPath, "original content");

        // Act
        await _componentService.InstallComponentAsync(_testProjectPath, "Components", "button", false);

        // Assert
        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().Be("original content");
    }

    [Fact]
    public async Task InstallComponentAsync_Overwrites_WhenOverwriteIsTrue()
    {
        // Arrange - Flat structure: component goes directly to Components/Button.razor
        var componentDir = Path.Combine(_testProjectPath, "Components");
        Directory.CreateDirectory(componentDir);
        var componentPath = Path.Combine(componentDir, "Button.razor");
        await File.WriteAllTextAsync(componentPath, "original content");

        // Act
        await _componentService.InstallComponentAsync(_testProjectPath, "Components", "button", true);

        // Assert
        var content = await File.ReadAllTextAsync(componentPath);
        content.Should().NotBe("original content");
        content.Should().Contain("vibe-button");
    }

    [Fact]
    public void GetAvailableComponents_IncludesAllCategories()
    {
        // Act
        var components = _componentService.GetAvailableComponents();
        var categories = components.Select(c => c.Category).Distinct().ToList();

        // Assert - 12 categories total
        categories.Should().Contain("Inputs");
        categories.Should().Contain("Form");
        categories.Should().Contain("DataDisplay");
        categories.Should().Contain("Layout");
        categories.Should().Contain("Navigation");
        categories.Should().Contain("Overlay");
        categories.Should().Contain("Feedback");
        categories.Should().Contain("Disclosure");
        categories.Should().Contain("DateTime");
        categories.Should().Contain("Utility");
        categories.Should().Contain("Advanced");
        categories.Should().Contain("Theme");
    }

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            Directory.Delete(_testProjectPath, true);
        }
    }
}
