namespace Vibe.UI.Tests.Components.Input;

public class SwitchTests : TestBase
{
    [Fact]
    public void Switch_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Switch>();

        // Assert
        var switchInput = cut.Find("input[type='checkbox']");
        switchInput.ShouldNotBeNull();
        cut.Find(".vibe-switch").ShouldNotBeNull();
    }

    [Fact]
    public void Switch_Renders_AsChecked()
    {
        // Act
        var cut = RenderComponent<Switch>(parameters => parameters
            .Add(p => p.Checked, true));

        // Assert
        var switchInput = cut.Find("input[type='checkbox']");
        switchInput.HasAttribute("checked").ShouldBeTrue();
    }

    [Fact]
    public void Switch_Applies_Disabled_Attribute()
    {
        // Act
        var cut = RenderComponent<Switch>(parameters => parameters
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
        var cut = RenderComponent<Switch>(parameters => parameters
            .Add(p => p.CheckedChanged, newValue => checkedValue = newValue));

        // Act
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        checkedValue.ShouldBeTrue();
    }

    [Fact]
    public void Switch_DoesNotToggle_WhenDisabled()
    {
        // Arrange
        var checkedValue = false;
        var cut = RenderComponent<Switch>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.CheckedChanged, newValue => checkedValue = newValue));

        // Act
        cut.Find("input[type='checkbox']").Change(true);

        // Assert
        checkedValue.ShouldBeFalse();
    }
}
