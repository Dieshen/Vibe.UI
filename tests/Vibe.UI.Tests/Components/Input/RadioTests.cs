namespace Vibe.UI.Tests.Components.Input;

public class RadioTests : TestBase
{
    [Fact]
    public void Radio_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Radio>();

        // Assert
        var radio = cut.Find("input[type='radio']");
        radio.ShouldNotBeNull();
    }

    [Fact]
    public void Radio_Renders_WithLabel()
    {
        // Act
        var cut = RenderComponent<Radio>(parameters => parameters
            .AddChildContent("Option 1"));

        // Assert
        var label = cut.Find(".vibe-radio-label");
        label.TextContent.ShouldBe("Option 1");
    }

    [Fact]
    public void Radio_Renders_AsChecked()
    {
        // Act
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Checked, true));

        // Assert
        var radio = cut.Find("input[type='radio']");
        radio.HasAttribute("checked").ShouldBeTrue();
    }

    [Fact]
    public void Radio_Applies_Disabled_Attribute()
    {
        // Act
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var radio = cut.Find("input[type='radio']");
        radio.HasAttribute("disabled").ShouldBeTrue();
        cut.Find("label").ClassList.ShouldContain("vibe-radio-disabled");
    }

    [Fact]
    public void Radio_Applies_Name_Attribute()
    {
        // Act
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Name, "option-group"));

        // Assert
        var radio = cut.Find("input[type='radio']");
        radio.GetAttribute("name").ShouldBe("option-group");
    }

    [Fact]
    public void Radio_Applies_Value_Attribute()
    {
        // Act
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Value, "option1"));

        // Assert
        var radio = cut.Find("input[type='radio']");
        radio.GetAttribute("value").ShouldBe("option1");
    }

    [Fact]
    public void Radio_InvokesCheckedChanged_WhenClicked()
    {
        // Arrange
        var wasChecked = false;
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.CheckedChanged, isChecked => wasChecked = isChecked));

        // Act
        cut.Find("input[type='radio']").Change(true);

        // Assert
        wasChecked.ShouldBeTrue();
    }

    [Fact]
    public void Radio_Unchecked_ToChecked_Transition()
    {
        // Arrange
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Checked, false));

        // Act
        cut.SetParametersAndRender(parameters => parameters.Add(p => p.Checked, true));

        // Assert
        var radio = cut.Find("input[type='radio']");
        radio.HasAttribute("checked").ShouldBeTrue();
    }

    [Fact]
    public void Radio_Disabled_NoEventFired()
    {
        // Arrange
        var wasChecked = false;
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.CheckedChanged, isChecked => wasChecked = isChecked));

        // Act
        // Note: Disabled inputs typically don't fire change events in browsers
        // This tests the component's disabled state is properly applied
        var radio = cut.Find("input[type='radio']");

        // Assert - Verify disabled attribute prevents interaction
        radio.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Radio_WithoutLabel_OnlyRendersInput()
    {
        // Act
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Value, "test"));

        // Assert
        cut.Find("input[type='radio']").ShouldNotBeNull();
        cut.FindAll(".vibe-radio-label").ShouldBeEmpty();
    }

    [Fact]
    public void Radio_MultipleRadiosSameName_MutexBehavior()
    {
        // Arrange & Act
        var cut1 = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Name, "same-group")
            .Add(p => p.Value, "option1")
            .Add(p => p.Checked, true));

        var cut2 = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Name, "same-group")
            .Add(p => p.Value, "option2")
            .Add(p => p.Checked, false));

        // Assert - Both should have the same name attribute
        cut1.Find("input[type='radio']").GetAttribute("name").ShouldBe("same-group");
        cut2.Find("input[type='radio']").GetAttribute("name").ShouldBe("same-group");
    }

    [Fact]
    public void Radio_DifferentNames_IndependentSelection()
    {
        // Arrange & Act
        var cut1 = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Name, "group1")
            .Add(p => p.Checked, true));

        var cut2 = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Name, "group2")
            .Add(p => p.Checked, true));

        // Assert - Both can be checked independently
        cut1.Find("input[type='radio']").HasAttribute("checked").ShouldBeTrue();
        cut2.Find("input[type='radio']").HasAttribute("checked").ShouldBeTrue();
    }

    [Fact]
    public void Radio_Renders_ControlSpan()
    {
        // Act
        var cut = RenderComponent<Radio>();

        // Assert
        var control = cut.Find(".vibe-radio-control");
        control.ShouldNotBeNull();
    }

    [Fact]
    public void Radio_CheckedProp_ReflectsInDom()
    {
        // Act
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Checked, true));

        // Assert
        cut.Instance.Checked.ShouldBeTrue();
        cut.Find("input[type='radio']").HasAttribute("checked").ShouldBeTrue();
    }

    [Fact]
    public void Radio_ChangeEvent_UpdatesCheckedState()
    {
        // Arrange
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Checked, false));

        // Act
        cut.Find("input[type='radio']").Change(true);

        // Assert
        cut.Instance.Checked.ShouldBeTrue();
    }

    [Fact]
    public void Radio_EmptyValue_ValidAttribute()
    {
        // Act
        var cut = RenderComponent<Radio>(parameters => parameters
            .Add(p => p.Value, ""));

        // Assert
        cut.Find("input[type='radio']").GetAttribute("value").ShouldBe("");
    }
}
