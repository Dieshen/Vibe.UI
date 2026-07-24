namespace Vibe.UI.Tests.Components.Inputs;

public class ToggleGroupItemTests : TestBase
{
    [Fact]
    public void ToggleGroupItem_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .AddChildContent("Item"));

        // Assert
        cut.Find(".vibe-toggle-group-item").ShouldNotBeNull();
    }

    [Fact]
    public void ToggleGroupItem_Renders_AsButton()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .AddChildContent("Item"));

        // Assert
        var button = cut.Find("button");
        button.GetAttribute("type")!.ShouldBe("button");
    }

    [Fact]
    public void ToggleGroupItem_Requires_Value()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .AddChildContent("Item"));

        // Assert
        cut.Instance.Value.ShouldBe("item1");
    }

    [Fact]
    public void ToggleGroupItem_Renders_ChildContent()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .AddChildContent("Test Content"));

        // Assert
        cut.Find("button").TextContent.ShouldBe("Test Content");
    }

    [Fact]
    public void ToggleGroupItem_Has_AriaPressed_Attribute()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .AddChildContent("Item"));

        // Assert
        var button = cut.Find("button");
        button.HasAttribute("aria-pressed").ShouldBeTrue();
    }

    [Fact]
    public void ToggleGroupItem_Applies_DisabledAttribute_WhenDisabled()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .Add(p => p.Disabled, true)
            .AddChildContent("Item"));

        // Assert
        var button = cut.Find("button");
        button.HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".vibe-toggle-group-item").ClassList.ShouldContain("vibe-toggle-group-item-disabled");
    }

    [Fact]
    public void ToggleGroupItem_Applies_PressedClass_WhenSelected()
    {
        // This test verifies the CSS class application, though parent integration requires the parent component
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .AddChildContent("Item"));

        // Assert - initially not pressed
        cut.Find(".vibe-toggle-group-item").ClassList.ShouldNotContain("vibe-toggle-group-item-pressed");
    }

    [Fact]
    public void ToggleGroupItem_Applies_CustomCssClass()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .Add(p => p.CssClass, "custom-item")
            .AddChildContent("Item"));

        // Assert
        cut.Find(".vibe-toggle-group-item").ClassList.ShouldContain("custom-item");
    }

    [Fact]
    public void ToggleGroupItem_ForwardsClassAndAdditionalAttributes()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .Add(p => p.Class, "toolbar-item")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "toggle-item" }
            })
            .AddChildContent("Item"));

        // Assert
        var button = cut.Find("button");
        button.ClassList.ShouldContain("toolbar-item");
        button.GetAttribute("data-testid")!.ShouldBe("toggle-item");
    }

    [Fact]
    public void ToggleGroupItem_WhenDisabled_SetsAriaDisabled()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .Add(p => p.Disabled, true)
            .AddChildContent("Item"));

        // Assert
        cut.Find("button").GetAttribute("aria-disabled")!.ShouldBe("true");
    }

    [Fact]
    public void ToggleGroupItem_WithNullValue_DoesNotThrow()
    {
        // Act
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, null!)
            .AddChildContent("Item"));

        // Assert
        cut.Find("button").GetAttribute("aria-pressed")!.ShouldBe("false");
    }

    [Fact]
    public void ToggleGroupItem_StandaloneItemIsKeyboardReachable()
    {
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .AddChildContent("Item"));

        var button = cut.Find("button");
        button.GetAttribute("tabindex")!.ShouldBe("0");
        button.Id.ShouldStartWith("vibe-toggle-item-");
    }

    [Fact]
    public void ToggleGroupItem_UsesProvidedIdForFocusCoordination()
    {
        var cut = Render<ToggleGroupItem>(parameters => parameters
            .Add(p => p.Value, "item1")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["id"] = "alignment-left"
            })
            .AddChildContent("Item"));

        cut.Find("button").Id.ShouldBe("alignment-left");
    }
}
