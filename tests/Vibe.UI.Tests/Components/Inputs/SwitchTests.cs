namespace Vibe.UI.Tests.Components.Inputs;

public class SwitchTests : TestBase
{
    [Fact]
    public void Switch_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Switch>();

        // Assert
        var switchInput = cut.Find("input[type='checkbox']");
        switchInput.ShouldNotBeNull();
        cut.Find(".vibe-switch").ShouldNotBeNull();
    }

    [Fact]
    public void Switch_Renders_AsChecked()
    {
        // Act
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Checked, true));

        // Assert
        var switchInput = cut.Find("input[type='checkbox']");
        switchInput.HasAttribute("checked").ShouldBeTrue();
    }

    [Fact]
    public void Switch_Applies_Disabled_Attribute()
    {
        // Act
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var switchInput = cut.Find("input[type='checkbox']");
        switchInput.HasAttribute("disabled").ShouldBeTrue();
        cut.Find("label").ClassList.ShouldContain("disabled");
    }

    [Fact]
    public void Switch_InvokesCheckedChanged_WhenToggled()
    {
        // Arrange
        var checkedValue = false;
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.CheckedChanged, newValue => checkedValue = newValue));

        // Act
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        checkedValue.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithNoCheckedChangedDelegate_UpdatesRenderedCheckedState()
    {
        // Arrange
        var cut = Render<Switch>();

        // Act
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        cut.Find("input[type='checkbox']").HasAttribute("checked").ShouldBeTrue();
        cut.Find("input[type='checkbox']").GetAttribute("aria-checked")!.ShouldBe("true");
    }

    [Fact]
    public void Switch_DoesNotToggle_WhenDisabled()
    {
        // Arrange
        var checkedValue = false;
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.CheckedChanged, newValue => checkedValue = newValue));

        // Act
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        checkedValue.ShouldBeFalse();
    }

    [Fact]
    public void Switch_RendersSwitchRoleAndAriaChecked()
    {
        // Act
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Checked, true));

        // Assert
        var switchInput = cut.Find("input[type='checkbox']");
        switchInput.GetAttribute("role")!.ShouldBe("switch");
        switchInput.GetAttribute("aria-checked")!.ShouldBe("true");
    }

    [Fact]
    public void Switch_UsesLabelAsAccessibleName()
    {
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Label, "Enable notifications"));

        cut.Find("input[role='switch']").GetAttribute("aria-label")!.ShouldBe("Enable notifications");
    }

    [Fact]
    public void Switch_AriaLabelTakesPrecedenceOverLabel()
    {
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Label, "Visible context")
            .Add(p => p.AriaLabel, "Share across devices"));

        cut.Find("input[role='switch']").GetAttribute("aria-label")!.ShouldBe("Share across devices");
    }

    [Theory]
    [InlineData("sm", "vibe-switch-sm")]
    [InlineData("Small", "vibe-switch-sm")]
    [InlineData("lg", "vibe-switch-lg")]
    [InlineData("Large", "vibe-switch-lg")]
    public void Switch_AppliesNormalizedSizeClass(string size, string expectedClass)
    {
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Size, size));

        cut.Find("label").ClassList.ShouldContain(expectedClass);
    }

    [Fact]
    public void Switch_InvalidSizeFallsBackToDefault()
    {
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Size, "oversized injected"));

        var classes = cut.Find("label").ClassList;
        classes.ShouldNotContain("vibe-switch-sm");
        classes.ShouldNotContain("vibe-switch-lg");
        classes.ShouldNotContain("injected");
    }

    [Fact]
    public void Switch_ForwardsFormSemantics_ToNativeInput()
    {
        // Act
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Id, "marketing")
            .Add(p => p.Name, "marketingOptIn")
            .Add(p => p.Value, "yes")
            .Add(p => p.Required, true));

        // Assert
        var switchInput = cut.Find("input[type='checkbox']");
        switchInput.GetAttribute("id")!.ShouldBe("marketing");
        switchInput.GetAttribute("name")!.ShouldBe("marketingOptIn");
        switchInput.GetAttribute("value")!.ShouldBe("yes");
        switchInput.HasAttribute("required").ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithClassParameter_AppendsCustomClassToLabel()
    {
        // Act
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.Class, "settings-switch"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-switch");
        label.ClassList.ShouldContain("settings-switch");
    }

    [Fact]
    public void Switch_WithAdditionalAttributes_ForwardsToNativeInput()
    {
        // Act
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "marketing-switch" },
                { "aria-label", "Marketing opt in" }
            }));

        // Assert
        var switchInput = cut.Find("input[type='checkbox']");
        switchInput.GetAttribute("data-testid")!.ShouldBe("marketing-switch");
        switchInput.GetAttribute("aria-label")!.ShouldBe("Marketing opt in");
    }

    [Fact]
    public void Switch_WithNonBooleanChangeValue_DoesNotInvokeCallback()
    {
        // Arrange
        var invoked = false;
        var cut = Render<Switch>(parameters => parameters
            .Add(p => p.CheckedChanged, _ => invoked = true));

        // Act
        cut.Find("input[type='checkbox']").Change("not a boolean");

        // Assert
        invoked.ShouldBeFalse();
    }
}
