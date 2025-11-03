using FluentAssertions;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Services;

/// <summary>
/// Validates that the CLI component registry stays synchronized with the actual codebase.
/// These tests prevent drift between the registry and actual component files.
/// </summary>
public class ComponentRegistryValidationTests
{
    private readonly ComponentService _componentService;
    private readonly string _projectRoot;
    private readonly string _componentsPath;

    public ComponentRegistryValidationTests()
    {
        _componentService = new ComponentService();

        // Navigate from test directory to project root
        // The test runs in: tests/Vibe.UI.CLI.Tests/bin/Debug/net9.0
        // We need to go up to project root: ../../../../..
        var testDirectory = Directory.GetCurrentDirectory();
        _projectRoot = Path.GetFullPath(Path.Combine(testDirectory, "..", "..", "..", "..", ".."));
        _componentsPath = Path.Combine(_projectRoot, "src", "Vibe.UI", "Components");
    }

    [Fact]
    public void AllRegistryComponents_ShouldExistAsFiles()
    {
        // Arrange
        var registryComponents = GetRegistryComponents();
        var missingComponents = new List<string>();

        // Act
        foreach (var component in registryComponents.Values)
        {
            var expectedPath = Path.Combine(
                _componentsPath,
                component.Category,
                $"{component.Name}.razor");

            if (!File.Exists(expectedPath))
            {
                missingComponents.Add($"{component.Name} (expected at: {expectedPath})");
            }
        }

        // Assert
        missingComponents.Should().BeEmpty(
            $"All components in the CLI registry must exist as .razor files. Missing components:\n{string.Join("\n", missingComponents)}");
    }

    [Fact]
    public void AllComponentFiles_ShouldBeInRegistry()
    {
        // Arrange
        var actualFiles = GetActualComponentFiles();
        var registryComponents = GetRegistryComponents();
        var unregisteredComponents = new List<string>();

        // Act
        foreach (var (componentName, componentPath) in actualFiles)
        {
            var key = componentName.ToLowerInvariant();
            if (!registryComponents.ContainsKey(key))
            {
                var category = GetCategoryFromPath(componentPath);
                unregisteredComponents.Add($"{componentName} (Category: {category}, Path: {componentPath})");
            }
        }

        // Assert
        unregisteredComponents.Should().BeEmpty(
            $"All .razor component files must be registered in the CLI. Unregistered components:\n{string.Join("\n", unregisteredComponents)}");
    }

    [Fact]
    public void ComponentDependencies_ShouldExist()
    {
        // Arrange
        var registryComponents = GetRegistryComponents();
        var invalidDependencies = new List<string>();

        // Act
        foreach (var component in registryComponents.Values)
        {
            if (component.Dependencies == null || !component.Dependencies.Any())
                continue;

            foreach (var dependency in component.Dependencies)
            {
                var dependencyKey = dependency.ToLowerInvariant();
                if (!registryComponents.ContainsKey(dependencyKey))
                {
                    invalidDependencies.Add(
                        $"{component.Name} depends on '{dependency}' which is not in the registry");
                }
            }
        }

        // Assert
        invalidDependencies.Should().BeEmpty(
            $"All component dependencies must exist in the registry. Invalid dependencies:\n{string.Join("\n", invalidDependencies)}");
    }

    [Fact]
    public void ComponentCategories_ShouldMatchDirectories()
    {
        // Arrange
        var registryComponents = GetRegistryComponents();
        var categoryMismatches = new List<string>();

        // Act
        foreach (var component in registryComponents.Values)
        {
            var expectedPath = Path.Combine(
                _componentsPath,
                component.Category,
                $"{component.Name}.razor");

            if (!File.Exists(expectedPath))
            {
                // Find where the file actually is
                var actualPath = FindComponentFile(component.Name);
                if (actualPath != null)
                {
                    var actualCategory = GetCategoryFromPath(actualPath);
                    categoryMismatches.Add(
                        $"{component.Name}: Registry says '{component.Category}' but file is in '{actualCategory}'");
                }
            }
        }

        // Assert
        categoryMismatches.Should().BeEmpty(
            $"Component categories in registry must match their directory structure. Mismatches:\n{string.Join("\n", categoryMismatches)}");
    }

    [Fact]
    public void RegistryCount_ShouldMatchActualComponentCount()
    {
        // Arrange
        var actualFiles = GetActualComponentFiles();
        var registryComponents = GetRegistryComponents();

        // Act
        var actualCount = actualFiles.Count;
        var registryCount = registryComponents.Count;

        // Assert
        registryCount.Should().Be(actualCount,
            $"Registry has {registryCount} components but found {actualCount} .razor files. " +
            $"This indicates a drift between the registry and codebase.");
    }

    [Fact]
    public void ComponentService_ShouldReturnAllComponents()
    {
        // Arrange & Act
        var components = _componentService.GetAvailableComponents();

        // Assert
        components.Should().NotBeEmpty("ComponentService should return at least one component");
        components.Should().AllSatisfy(c =>
        {
            c.Name.Should().NotBeNullOrWhiteSpace("Component Name should not be null or empty");
            c.Category.Should().NotBeNullOrWhiteSpace("Component Category should not be null or empty");
        });
    }

    [Fact]
    public void ComponentService_GetComponent_ShouldBeCaseInsensitive()
    {
        // Arrange
        var registryComponents = GetRegistryComponents();
        var firstComponent = registryComponents.Values.First();

        // Act
        var lowerCase = _componentService.GetComponent(firstComponent.Name.ToLowerInvariant());
        var upperCase = _componentService.GetComponent(firstComponent.Name.ToUpperInvariant());
        var mixedCase = _componentService.GetComponent(firstComponent.Name);

        // Assert
        lowerCase.Should().NotBeNull("GetComponent should work with lowercase");
        upperCase.Should().NotBeNull("GetComponent should work with uppercase");
        mixedCase.Should().NotBeNull("GetComponent should work with mixed case");

        lowerCase!.Name.Should().Be(firstComponent.Name);
        upperCase!.Name.Should().Be(firstComponent.Name);
        mixedCase!.Name.Should().Be(firstComponent.Name);
    }

    #region Helper Methods

    /// <summary>
    /// Gets all component files from the actual codebase.
    /// Returns a dictionary mapping component name to full file path.
    /// </summary>
    private Dictionary<string, string> GetActualComponentFiles()
    {
        var components = new Dictionary<string, string>();

        if (!Directory.Exists(_componentsPath))
        {
            throw new DirectoryNotFoundException(
                $"Components directory not found at: {_componentsPath}. " +
                $"Current directory: {Directory.GetCurrentDirectory()}");
        }

        var razorFiles = Directory.GetFiles(_componentsPath, "*.razor", SearchOption.AllDirectories);

        foreach (var filePath in razorFiles)
        {
            var componentName = Path.GetFileNameWithoutExtension(filePath);
            components[componentName] = filePath;
        }

        return components;
    }

    /// <summary>
    /// Gets all components from the CLI registry.
    /// Returns a dictionary mapping lowercase component name to ComponentInfo.
    /// </summary>
    private Dictionary<string, ComponentInfo> GetRegistryComponents()
    {
        var components = new Dictionary<string, ComponentInfo>();
        var allComponents = _componentService.GetAvailableComponents();

        foreach (var component in allComponents)
        {
            var key = component.Name.ToLowerInvariant();
            components[key] = component;
        }

        return components;
    }

    /// <summary>
    /// Extracts the category name from a component file path.
    /// E.g., "D:/Projects/.../Components/Input/Button.razor" -> "Input"
    /// </summary>
    private string GetCategoryFromPath(string filePath)
    {
        var relativePath = Path.GetRelativePath(_componentsPath, filePath);
        var parts = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        // First part should be the category directory
        return parts.Length > 0 ? parts[0] : "Unknown";
    }

    /// <summary>
    /// Finds a component file by name in the codebase.
    /// Returns the full path if found, null otherwise.
    /// </summary>
    private string? FindComponentFile(string componentName)
    {
        if (!Directory.Exists(_componentsPath))
            return null;

        var razorFiles = Directory.GetFiles(_componentsPath, $"{componentName}.razor", SearchOption.AllDirectories);
        return razorFiles.FirstOrDefault();
    }

    #endregion
}
