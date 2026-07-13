namespace Vibe.UI.Tests.Components.Inputs;

public class CheckboxTests : TestBase
{
    [Fact]
    public void Checkbox_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Checkbox>();

        // Assert
        var checkbox = cut.Find("input[type='checkbox']");
        checkbox.ShouldNotBeNull();
        checkbox.GetAttribute("checked")!.ShouldBeNull();
    }

    [Fact]
    public void Checkbox_Renders_WithLabel()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .AddChildContent("Accept terms"));

        // Assert
        var label = cut.Find(".vibe-checkbox-label");
        label.TextContent.ShouldBe("Accept terms");
    }

    [Fact]
    public void Checkbox_Renders_WithPlainTextLabel()
    {
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Label, "Enable notifications"));

        cut.Find(".vibe-checkbox-label").TextContent.ShouldBe("Enable notifications");
    }

    [Fact]
    public void Checkbox_Renders_AsChecked()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, true));

        // Assert
        var checkbox = cut.Find("input[type='checkbox']");
        checkbox.HasAttribute("checked").ShouldBeTrue();
        checkbox.GetAttribute("aria-checked")!.ShouldBe("true");
        cut.Find(".vibe-checkbox-control svg.vibe-icon").ShouldNotBeNull();
    }

    [Fact]
    public void Checkbox_Renders_IndeterminateStateWithMixedSemantics()
    {
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Indeterminate, true)
            .Add(p => p.Label, "Select all"));

        var checkbox = cut.Find("input[type='checkbox']");
        checkbox.GetAttribute("aria-checked")!.ShouldBe("mixed");
        checkbox.GetAttribute("data-indeterminate")!.ShouldBe("true");
        cut.Find("label").ClassList.ShouldContain("vibe-checkbox-indeterminate");
        cut.Find(".vibe-checkbox-control svg.vibe-icon").ShouldNotBeNull();
    }

    [Fact]
    public void Checkbox_InteractionClearsIndeterminateState()
    {
        var indeterminate = true;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Indeterminate, true)
            .Add(p => p.IndeterminateChanged, value => indeterminate = value));

        cut.Find("input[type='checkbox']").Change(true);

        indeterminate.ShouldBeFalse();
        cut.Find("input[type='checkbox']").GetAttribute("aria-checked")!.ShouldBe("true");
    }

    [Fact]
    public void Checkbox_Applies_Disabled_Attribute()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var checkbox = cut.Find("input[type='checkbox']");
        checkbox.HasAttribute("disabled").ShouldBeTrue();
        cut.Find("label").ClassList.ShouldContain("vibe-checkbox-disabled");
    }

    [Fact]
    public void Checkbox_InvokesCheckedChanged_WhenClicked()
    {
        // Arrange
        var checkedValue = false;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.CheckedChanged, newValue => checkedValue = newValue));

        // Act
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        checkedValue.ShouldBeTrue();
    }

    [Fact]
    public void Checkbox_InvokesNativeOnChange_WhenClicked()
    {
        ChangeEventArgs? received = null;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.OnChange, args => received = args));

        cut.Find("input[type='checkbox']").Change(true);

        received.ShouldNotBeNull();
        received.Value.ShouldBe(true);
    }

    [Fact]
    public void Checkbox_DoesNotInvokeCallback_WhenNoDelegate()
    {
        // Arrange
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, false));

        // Act
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        cut.Find("input[type='checkbox']").HasAttribute("checked").ShouldBeTrue();
    }

    // === Edge Cases ===

    [Fact]
    public void Checkbox_WithNullChildContent_RendersWithoutLabel()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment?)null));

        // Assert
        cut.FindAll(".vibe-checkbox-label").ShouldBeEmpty();
    }

    [Fact]
    public void Checkbox_WithEmptyChildContent_RendersWithEmptyLabel()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .AddChildContent(string.Empty));

        // Assert
        var label = cut.Find(".vibe-checkbox-label");
        label.TextContent.ShouldBe(string.Empty);
    }

    [Fact]
    public void Checkbox_WithComplexChildContent_RendersCorrectly()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .AddChildContent("<strong>Bold</strong> label text"));

        // Assert
        var label = cut.Find(".vibe-checkbox-label");
        label.InnerHtml.ShouldContain("<strong>Bold</strong>");
    }

    [Fact]
    public void Checkbox_WithVeryLongLabel_RendersCorrectly()
    {
        // Arrange
        var longLabel = new string('A', 500);

        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .AddChildContent(longLabel));

        // Assert
        cut.Find(".vibe-checkbox-label").TextContent.ShouldBe(longLabel);
    }

    // === State Management ===

    [Fact]
    public void Checkbox_TogglesFromCheckedToUnchecked()
    {
        // Arrange
        var checkedValue = true;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, true)
            .Add(p => p.CheckedChanged, newValue => checkedValue = newValue));

        // Act
        cut.Find("input[type='checkbox']").Change(false);

        // Assert
        checkedValue.ShouldBeFalse();
    }

    [Fact]
    public void Checkbox_TogglesFromUncheckedToChecked()
    {
        // Arrange
        var checkedValue = false;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, false)
            .Add(p => p.CheckedChanged, newValue => checkedValue = newValue));

        // Act
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        checkedValue.ShouldBeTrue();
    }

    // === Disabled State ===

    [Fact]
    public void Checkbox_WhenDisabled_DoesNotToggle()
    {
        // Arrange
        var checkedValue = false;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, false)
            .Add(p => p.Disabled, true)
            .Add(p => p.CheckedChanged, newValue => checkedValue = newValue));

        // Act - bUnit can dispatch directly even when the element is disabled.
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        cut.Find("input[type='checkbox']").HasAttribute("disabled").ShouldBeTrue();
        checkedValue.ShouldBeFalse();
    }

    [Fact]
    public void Checkbox_DisabledWithChecked_RendersCorrectly()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, true)
            .Add(p => p.Disabled, true)
            .AddChildContent("Disabled and Checked"));

        // Assert
        var checkbox = cut.Find("input[type='checkbox']");
        checkbox.HasAttribute("checked").ShouldBeTrue();
        checkbox.HasAttribute("disabled").ShouldBeTrue();
        cut.Find("label").ClassList.ShouldContain("vibe-checkbox-disabled");
    }

    // === Event Handling ===

    [Fact]
    public void Checkbox_WithNonBooleanValue_HandlesGracefully()
    {
        // Arrange
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, false)
            .Add(p => p.CheckedChanged, EventCallback.Factory.Create<bool>(this, _ => { })));

        // Act & Assert - Should not throw with string value
        var checkbox = cut.Find("input[type='checkbox']");
        checkbox.Change("not a boolean");
    }

    [Fact]
    public void Checkbox_MultipleRapidToggles_HandlesCorrectly()
    {
        // Arrange
        var toggleCount = 0;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.CheckedChanged, newValue => toggleCount++));

        var checkbox = cut.Find("input[type='checkbox']");

        // Act - Simulate multiple rapid toggles
        checkbox.Change(true);
        checkbox.Change(false);
        checkbox.Change(true);
        checkbox.Change(false);

        // Assert
        toggleCount.ShouldBe(4);
    }

    // === CSS Classes ===

    [Fact]
    public void Checkbox_HasBaseClass()
    {
        // Act
        var cut = Render<Checkbox>();

        // Assert
        cut.Find("label").ClassList.ShouldContain("vibe-checkbox");
    }

    [Fact]
    public void Checkbox_WhenNotDisabled_DoesNotHaveDisabledClass()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Disabled, false));

        // Assert
        cut.Find("label").ClassList.ShouldNotContain("vibe-checkbox-disabled");
    }

    // === Additional Attributes ===

    [Fact]
    public void Checkbox_WithAdditionalAttributes_MergesCorrectly()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .AddChildContent("Custom")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "my-checkbox" },
                { "aria-label", "Custom Checkbox" }
            }));

        // Assert
        var label = cut.Find("label");
        label.GetAttribute("data-testid")!.ShouldBe("my-checkbox");
        label.GetAttribute("aria-label")!.ShouldBe("Custom Checkbox");
    }

    [Fact]
    public void Checkbox_WithClassParameter_AppendsCustomClassToLabel()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Class, "terms-checkbox")
            .AddChildContent("Terms"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-checkbox");
        label.ClassList.ShouldContain("terms-checkbox");
    }

    [Fact]
    public void Checkbox_ForwardsFormSemantics_ToNativeInput()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Id, "terms")
            .Add(p => p.Name, "acceptedTerms")
            .Add(p => p.Value, "yes")
            .Add(p => p.Required, true)
            .AddChildContent("Accept terms"));

        // Assert
        var checkbox = cut.Find("input[type='checkbox']");
        checkbox.GetAttribute("id")!.ShouldBe("terms");
        checkbox.GetAttribute("name")!.ShouldBe("acceptedTerms");
        checkbox.GetAttribute("value")!.ShouldBe("yes");
        checkbox.HasAttribute("required").ShouldBeTrue();
    }

    [Fact]
    public void Checkbox_WhenDisabled_SetsAriaDisabledOnLabel()
    {
        // Act
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("Disabled"));

        // Assert
        cut.Find("label").GetAttribute("aria-disabled")!.ShouldBe("true");
    }

    [Fact]
    public void Checkbox_AppliesAccessibleNameAndInvalidStateToInput()
    {
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.AriaLabel, "Select all rows")
            .Add(p => p.AriaInvalid, true));

        var checkbox = cut.Find("input[type='checkbox']");
        checkbox.GetAttribute("aria-label")!.ShouldBe("Select all rows");
        checkbox.GetAttribute("aria-invalid")!.ShouldBe("true");
        cut.Find("label").ClassList.ShouldContain("vibe-checkbox-error");
    }

    [Fact]
    public void Checkbox_WithNonBooleanValue_DoesNotInvokeCallback()
    {
        // Arrange
        var invoked = false;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.CheckedChanged, _ => invoked = true));

        // Act
        cut.Find("input[type='checkbox']").Change("not a boolean");

        // Assert
        invoked.ShouldBeFalse();
    }
}
