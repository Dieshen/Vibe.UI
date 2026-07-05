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
