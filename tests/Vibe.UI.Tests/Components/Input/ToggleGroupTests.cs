namespace Vibe.UI.Tests.Components.Input;

public class ToggleGroupTests : TestBase
{
    [Fact]
    public void ToggleGroup_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ShouldNotBeNull();
    }

    [Fact]
    public void ToggleGroup_Has_GroupRole()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        var group = cut.Find(".vibe-toggle-group");
        group.GetAttribute("role").ShouldBe("group");
    }

    [Fact]
    public void ToggleGroup_Has_SingleType_ByDefault()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Instance.Type.ShouldBe(ToggleGroup.ToggleGroupType.Single);
    }

    [Fact]
    public void ToggleGroup_Accepts_MultipleType()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, ToggleGroup.ToggleGroupType.Multiple)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Instance.Type.ShouldBe(ToggleGroup.ToggleGroupType.Multiple);
    }

    [Fact]
    public void ToggleGroup_Has_HorizontalOrientation_ByDefault()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("vibe-toggle-group-horizontal");
    }

    [Fact]
    public void ToggleGroup_Applies_VerticalOrientation()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .Add(p => p.Orientation, ToggleGroup.ToggleGroupOrientation.Vertical)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("vibe-toggle-group-vertical");
    }

    [Fact]
    public void ToggleGroup_Applies_SizeClass()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .Add(p => p.Size, ToggleGroup.ToggleGroupSize.Large)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("vibe-toggle-group-large");
    }

    [Fact]
    public void ToggleGroup_Applies_DisabledClass()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("vibe-toggle-group-disabled");
    }

    [Fact]
    public void ToggleGroup_Has_AriaLabel()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .Add(p => p.AriaLabel, "Toggle options")
            .AddChildContent("<div>Items</div>"));

        // Assert
        var group = cut.Find(".vibe-toggle-group");
        group.GetAttribute("aria-label").ShouldBe("Toggle options");
    }

    [Fact]
    public void ToggleGroup_Accepts_Value_InSingleMode()
    {
        // Arrange & Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, ToggleGroup.ToggleGroupType.Single)
            .Add(p => p.Value, "item1")
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Instance.Value.ShouldBe("item1");
    }

    [Fact]
    public void ToggleGroup_Accepts_Values_InMultipleMode()
    {
        // Arrange
        var values = new List<string> { "item1", "item2" };

        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, ToggleGroup.ToggleGroupType.Multiple)
            .Add(p => p.Values, values)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Instance.Values.ShouldContain("item1");
        cut.Instance.Values.ShouldContain("item2");
    }

    [Fact]
    public void ToggleGroup_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<ToggleGroup>(parameters => parameters
            .Add(p => p.CssClass, "custom-toggle-group")
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("custom-toggle-group");
    }
}
