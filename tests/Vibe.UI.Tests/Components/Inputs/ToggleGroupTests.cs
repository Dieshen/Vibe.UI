using Microsoft.AspNetCore.Components.Web;

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
    public void ToggleGroup_SingleMode_ClearsPreviouslyPressedSibling()
    {
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent(BuildItems("left", "right")));

        cut.FindAll("button")[0].Click();

        cut.FindAll("button")[0].GetAttribute("aria-pressed")!.ShouldBe("true");
        cut.FindAll("button")[1].GetAttribute("aria-pressed")!.ShouldBe("false");

        cut.FindAll("button")[1].Click();

        cut.FindAll("button")[0].GetAttribute("aria-pressed")!.ShouldBe("false");
        cut.FindAll("button")[1].GetAttribute("aria-pressed")!.ShouldBe("true");
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

    [Fact]
    public void ToggleGroup_UsesSelectedItemAsOnlyInitialTabStop()
    {
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Value, "center")
            .AddChildContent(BuildItems("left", "center", "right")));

        var buttons = cut.FindAll("button");

        buttons.Count(button => button.GetAttribute("tabindex") == "0").ShouldBe(1);
        buttons[0].GetAttribute("tabindex")!.ShouldBe("-1");
        buttons[1].GetAttribute("tabindex")!.ShouldBe("0");
        buttons[2].GetAttribute("tabindex")!.ShouldBe("-1");
    }

    [Fact]
    public void ToggleGroup_UsesFirstEnabledItemWhenNothingIsSelected()
    {
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent(BuildItems(("left", true), ("center", false), ("right", false))));

        var buttons = cut.FindAll("button");

        buttons[0].GetAttribute("tabindex")!.ShouldBe("-1");
        buttons[1].GetAttribute("tabindex")!.ShouldBe("0");
        buttons[2].GetAttribute("tabindex")!.ShouldBe("-1");
    }

    [Fact]
    public void ToggleGroup_HorizontalArrowsMoveFocusWrapAndPreserveSelection()
    {
        JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-dom.js");
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Value, "left")
            .AddChildContent(BuildItems("left", "center", "right")));

        cut.FindAll("button")[0].KeyDown("ArrowLeft");

        var buttons = cut.FindAll("button");
        buttons[2].GetAttribute("tabindex")!.ShouldBe("0");
        buttons[0].GetAttribute("aria-pressed")!.ShouldBe("true");
        buttons[2].GetAttribute("aria-pressed")!.ShouldBe("false");
        JSInterop.Invocations.Last().Identifier.ShouldBe("focusElement");
        JSInterop.Invocations.Last().Arguments[0].ShouldBe(buttons[2].Id);

        buttons[2].KeyDown("ArrowRight");

        cut.FindAll("button")[0].GetAttribute("tabindex")!.ShouldBe("0");
    }

    [Fact]
    public void ToggleGroup_HorizontalNavigationSkipsDisabledItems()
    {
        JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-dom.js");
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent(BuildItems(("left", false), ("center", true), ("right", false))));

        cut.FindAll("button")[0].KeyDown("ArrowRight");

        var buttons = cut.FindAll("button");
        buttons[1].GetAttribute("tabindex")!.ShouldBe("-1");
        buttons[2].GetAttribute("tabindex")!.ShouldBe("0");
    }

    [Fact]
    public void ToggleGroup_VerticalArrowsUseVerticalAxisOnly()
    {
        JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-dom.js");
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Orientation, ToggleGroup.ToggleGroupOrientation.Vertical)
            .AddChildContent(BuildItems("top", "middle", "bottom")));

        cut.FindAll("button")[0].KeyDown("ArrowRight");
        cut.FindAll("button")[0].GetAttribute("tabindex")!.ShouldBe("0");

        cut.FindAll("button")[0].KeyDown("ArrowDown");
        cut.FindAll("button")[1].GetAttribute("tabindex")!.ShouldBe("0");

        cut.FindAll("button")[1].KeyDown("ArrowUp");
        cut.FindAll("button")[0].GetAttribute("tabindex")!.ShouldBe("0");
    }

    [Fact]
    public void ToggleGroup_HomeAndEndMoveToEnabledBounds()
    {
        JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-dom.js");
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent(BuildItems(("first", true), ("second", false), ("third", false))));

        cut.FindAll("button")[1].KeyDown("End");
        cut.FindAll("button")[2].GetAttribute("tabindex")!.ShouldBe("0");

        cut.FindAll("button")[2].KeyDown("Home");
        cut.FindAll("button")[1].GetAttribute("tabindex")!.ShouldBe("0");
    }

    [Fact]
    public void ToggleGroup_FocusUpdatesTheSingleRovingTabStop()
    {
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent(BuildItems("left", "center", "right")));

        cut.FindAll("button")[2].TriggerEvent("onfocus", new FocusEventArgs());

        var buttons = cut.FindAll("button");
        buttons.Count(button => button.GetAttribute("tabindex") == "0").ShouldBe(1);
        buttons[2].GetAttribute("tabindex")!.ShouldBe("0");
    }

    [Fact]
    public void ToggleGroup_DisabledGroupRemovesEveryItemFromTabOrder()
    {
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent(BuildItems("left", "center", "right")));

        cut.FindAll("button").ShouldAllBe(button => button.GetAttribute("tabindex") == "-1");
    }

    [Fact]
    public void ToggleGroup_MultipleModeKeepsOnlyFirstSelectedItemInTabOrder()
    {
        var cut = Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, ToggleGroup.ToggleGroupType.Multiple)
            .Add(p => p.Values, new List<string> { "right", "center" })
            .AddChildContent(BuildItems("left", "center", "right")));

        var buttons = cut.FindAll("button");
        buttons.Count(button => button.GetAttribute("tabindex") == "0").ShouldBe(1);
        buttons[1].GetAttribute("tabindex")!.ShouldBe("0");
    }

    [Fact]
    public void ToggleGroup_ReassignsTabStopWhenCurrentItemBecomesDisabled()
    {
        var cut = Render<ToggleGroup>(parameters => parameters
            .AddChildContent(BuildItems(("left", false), ("right", false))));

        cut.FindAll("button")[0].GetAttribute("tabindex")!.ShouldBe("0");

        cut.Render(parameters => parameters
            .AddChildContent(BuildItems(("left", true), ("right", false))));

        var buttons = cut.FindAll("button");
        buttons[0].GetAttribute("tabindex")!.ShouldBe("-1");
        buttons[1].GetAttribute("tabindex")!.ShouldBe("0");
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

    private static RenderFragment BuildItems(params (string Value, bool Disabled)[] items)
    {
        return builder =>
        {
            var sequence = 0;
            foreach (var item in items)
            {
                builder.OpenComponent<ToggleGroupItem>(sequence++);
                builder.AddAttribute(sequence++, nameof(ToggleGroupItem.Value), item.Value);
                builder.AddAttribute(sequence++, nameof(ToggleGroupItem.Disabled), item.Disabled);
                builder.AddAttribute(sequence++, nameof(ToggleGroupItem.ChildContent), (RenderFragment)(childBuilder =>
                {
                    childBuilder.AddContent(0, item.Value);
                }));
                builder.CloseComponent();
            }
        };
    }
}
