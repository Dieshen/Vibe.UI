using FluentAssertions;
using Vibe.UI.CLI.Models;
using Xunit;

namespace Vibe.UI.CLI.Tests.Models;

public class ComponentInfoTests
{
    [Fact]
    public void ComponentInfo_CanBeInstantiated()
    {
        // Act
        var component = new ComponentInfo();

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public void ComponentInfo_AllowsSettingAllProperties()
    {
        // Arrange
        var component = new ComponentInfo();

        // Act
        component.Name = "Button";
        component.Category = "Input";
        component.Description = "A button component";
        component.Dependencies = new List<string> { "Icon" };
        component.HasCss = true;
        component.Example = "<Button>Click me</Button>";

        // Assert
        component.Name.Should().Be("Button");
        component.Category.Should().Be("Input");
        component.Description.Should().Be("A button component");
        component.Dependencies.Should().ContainSingle().Which.Should().Be("Icon");
        component.HasCss.Should().BeTrue();
        component.Example.Should().Be("<Button>Click me</Button>");
    }

    [Fact]
    public void ComponentInfo_Dependencies_CanBeNull()
    {
        // Arrange
        var component = new ComponentInfo
        {
            Name = "Button",
            Category = "Input",
            Description = "A button component"
        };

        // Assert
        component.Dependencies.Should().BeNull();
    }

    [Fact]
    public void ComponentInfo_Dependencies_CanBeEmptyList()
    {
        // Arrange
        var component = new ComponentInfo
        {
            Name = "Button",
            Category = "Input",
            Description = "A button component",
            Dependencies = new List<string>()
        };

        // Assert
        component.Dependencies.Should().BeEmpty();
    }

    [Fact]
    public void ComponentInfo_Dependencies_CanContainMultipleItems()
    {
        // Arrange
        var component = new ComponentInfo
        {
            Name = "DataTable",
            Category = "DataDisplay",
            Description = "A data table component",
            Dependencies = new List<string> { "Table", "Pagination", "Button" }
        };

        // Assert
        component.Dependencies.Should().HaveCount(3);
        component.Dependencies.Should().Contain(new[] { "Table", "Pagination", "Button" });
    }
}
