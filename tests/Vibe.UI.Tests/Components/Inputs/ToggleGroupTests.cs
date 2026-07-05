namespace Vibe.UI.Tests.Components.Inputs;

public class ToggleGroupTests : TestBase
{
    [Fact]
    public void ToggleGroup_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ShouldNotBeNull();
    }

    [Fact]
    public void ToggleGroup_Has_GroupRole()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        var group = cut.Find(".vibe-toggle-group");
        group.GetAttribute("role")!.ShouldBe("group");
    }

    [Fact]
    public void ToggleGroup_Has_SingleType_ByDefault()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Instance.Type.ShouldBe(ToggleGroup.ToggleGroupType.Single);
    }

    [Fact]
    public void ToggleGroup_Accepts_MultipleType()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, ToggleGroup.ToggleGroupType.Multiple)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Instance.Type.ShouldBe(ToggleGroup.ToggleGroupType.Multiple);
    }

    [Fact]
    public void ToggleGroup_Has_HorizontalOrientation_ByDefault()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("vibe-toggle-group-horizontal");
    }

    [Fact]
    public void ToggleGroup_Applies_VerticalOrientation()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Orientation, ToggleGroup.ToggleGroupOrientation.Vertical)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("vibe-toggle-group-vertical");
    }

    [Fact]
    public void ToggleGroup_Applies_SizeClass()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Size, ToggleGroup.ToggleGroupSize.Large)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("vibe-toggle-group-large");
    }

    [Fact]
    public void ToggleGroup_Applies_DisabledClass()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("vibe-toggle-group-disabled");
    }

    [Fact]
    public void ToggleGroup_Has_AriaLabel()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.AriaLabel, "Toggle options")
            .AddChildContent("<div>Items</div>"));

        // Assert
        var group = cut.Find(".vibe-toggle-group");
        group.GetAttribute("aria-label")!.ShouldBe("Toggle options");
    }

    [Fact]
    public void ToggleGroup_Accepts_Value_InSingleMode()
    {
        // Arrange & Act
        var cut = Render<ToggleGroup>(parameters => parameters
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
        var cut = Render<ToggleGroup>(parameters => parameters
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
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.CssClass, "custom-toggle-group")
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-toggle-group").ClassList.ShouldContain("custom-toggle-group");
    }

    [Fact]
    public void ToggleGroup_ForwardsAdditionalAttributesClassAndAriaOrientation()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Class, "editor-formatting")
            .Add(p => p.Orientation, ToggleGroup.ToggleGroupOrientation.Vertical)
            .Add(p => p.Disabled, true)
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "toggle-group" }
            })
            .AddChildContent("<div>Items</div>"));

        // Assert
        var group = cut.Find(".vibe-toggle-group");
        group.ClassList.ShouldContain("editor-formatting");
        group.GetAttribute("data-testid")!.ShouldBe("toggle-group");
        group.GetAttribute("aria-orientation")!.ShouldBe("vertical");
        group.GetAttribute("aria-disabled")!.ShouldBe("true");
    }

    [Fact]
    public void ToggleGroup_InvalidEnumValues_FallBackToDefaultClasses()
    {
        // Act
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, (ToggleGroup.ToggleGroupType)999)
            .Add(p => p.Orientation, (ToggleGroup.ToggleGroupOrientation)999)
            .Add(p => p.Size, (ToggleGroup.ToggleGroupSize)999)
            .AddChildContent(BuildItems("left", "right")));

        // Assert
        var group = cut.Find(".vibe-toggle-group");
        group.ClassList.ShouldContain("vibe-toggle-group-horizontal");
        group.ClassList.ShouldContain("vibe-toggle-group-default");
        group.ClassList.ShouldNotContain("vibe-toggle-group-999");
    }

    [Fact]
    public void ToggleGroup_SingleMode_TogglesValueAndUpdatesItemState()
    {
        // Arrange
        string? changedValue = "unchanged";
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.ValueChanged, value => changedValue = value)
            .AddChildContent(BuildItems("left", "right")));

        // Act
        cut.FindAll("button")[1].Click();

        // Assert
        changedValue.ShouldBe("right");
        cut.FindAll("button")[1].ClassList.ShouldContain("vibe-toggle-group-item-pressed");
        cut.FindAll("button")[1].GetAttribute("aria-pressed")!.ShouldBe("true");

        // Act
        cut.FindAll("button")[1].Click();

        // Assert
        changedValue.ShouldBeNull();
        cut.FindAll("button")[1].ClassList.ShouldNotContain("vibe-toggle-group-item-pressed");
    }

    [Fact]
    public void ToggleGroup_MultipleMode_DoesNotMutateCallerOwnedValuesList()
    {
        // Arrange
        var originalValues = new List<string> { "left" };
        List<string>? changedValues = null;
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, ToggleGroup.ToggleGroupType.Multiple)
            .Add(p => p.Values, originalValues)
            .Add(p => p.ValuesChanged, values => changedValues = values)
            .AddChildContent(BuildItems("left", "right")));

        // Act
        cut.FindAll("button")[1].Click();

        // Assert
        originalValues.ShouldBe(new List<string> { "left" });
        changedValues.ShouldNotBeNull();
        changedValues.ShouldBe(new List<string> { "left", "right" });
        changedValues.ShouldNotBeSameAs(originalValues);
        cut.FindAll("button")[1].ClassList.ShouldContain("vibe-toggle-group-item-pressed");
    }

    [Fact]
    public void ToggleGroup_WhenDisabled_DisablesChildItemsAndIgnoresSyntheticClick()
    {
        // Arrange
        string? changedValue = "unchanged";
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ValueChanged, value => changedValue = value)
            .AddChildContent(BuildItems("left", "right")));

        // Act
        cut.FindAll("button")[0].Click();

        // Assert
        changedValue.ShouldBe("unchanged");
        var button = cut.FindAll("button")[0];
        button.HasAttribute("disabled").ShouldBeTrue();
        button.GetAttribute("aria-disabled")!.ShouldBe("true");
    }

    private static RenderFragment BuildItems(params string[] values)
    {
        return builder =>
        {
            var sequence = 0;
            foreach (var value in values)
            {
                builder.OpenComponent<ToggleGroupItem>(sequence++);
                builder.AddAttribute(sequence++, nameof(ToggleGroupItem.Value), value);
                builder.AddAttribute(sequence++, nameof(ToggleGroupItem.ChildContent), (RenderFragment)(childBuilder =>
                {
                    childBuilder.AddContent(0, value);
                }));
                builder.CloseComponent();
            }
        };
    }
}
