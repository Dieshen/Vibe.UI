namespace Vibe.UI.Tests.Components.Inputs;

public class ToggleTests : TestBase
{
    [Fact]
    public void Toggle_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Toggle>(parameters => parameters
            .AddChildContent("Toggle"));

        // Assert
        var button = cut.Find("button");
        button.ShouldNotBeNull();
        button.ClassList.ShouldContain("vibe-toggle");
        button.TextContent.ShouldBe("Toggle");
    }

    [Fact]
    public void Toggle_Renders_AsPressed()
    {
        // Act
        var cut = Render<Toggle>(parameters => parameters
            .Add(p => p.Pressed, true)
            .AddChildContent("On"));

        // Assert
        var button = cut.Find("button");
        button.ClassList.ShouldContain("pressed");
    }

    [Fact]
    public void Toggle_Applies_Disabled_Attribute()
    {
        // Act
        var cut = Render<Toggle>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("Disabled"));

        // Assert
        var button = cut.Find("button");
        button.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Toggle_Applies_Variant_Class()
    {
        // Act
        var cut = Render<Toggle>(parameters => parameters
            .Add(p => p.Variant, "outline")
            .AddChildContent("Toggle"));

        // Assert
        cut.Find("button").ClassList.ShouldContain("toggle-outline");
    }

    [Fact]
    public void Toggle_Applies_Size_Class()
    {
        // Act
        var cut = Render<Toggle>(parameters => parameters
            .Add(p => p.Size, "sm")
            .AddChildContent("Small"));

        // Assert
        cut.Find("button").ClassList.ShouldContain("toggle-sm");
    }

    [Fact]
    public void Toggle_InvokesPressedChanged_WhenClicked()
    {
        // Arrange
        var pressedValue = false;
        var cut = Render<Toggle>(parameters => parameters
            .Add(p => p.PressedChanged, newValue => pressedValue = newValue)
            .AddChildContent("Toggle"));

        // Act
        cut.Find("button").Click();

        // Assert
        pressedValue.ShouldBeTrue();
    }

    [Fact]
    public void Toggle_DoesNotToggle_WhenDisabled()
    {
        // Arrange
        var pressedValue = false;
        var cut = Render<Toggle>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.PressedChanged, newValue => pressedValue = newValue)
            .AddChildContent("Disabled"));

        // Act
        cut.Find("button").Click();

        // Assert
        pressedValue.ShouldBeFalse();
    }

    [Fact]
    public void Toggle_ForwardsAdditionalAttributesAndAriaState()
    {
        // Act
        var cut = Render<Toggle>(parameters => parameters
            .Add(p => p.Pressed, true)
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "format-bold" }
            })
            .AddChildContent("Bold"));

        // Assert
        var button = cut.Find("button");
        button.GetAttribute("data-testid")!.ShouldBe("format-bold");
        button.GetAttribute("aria-pressed")!.ShouldBe("true");
        button.GetAttribute("aria-disabled")!.ShouldBe("false");
    }

    [Fact]
    public void Toggle_InvalidVariantAndSize_FallBackToDefaultClasses()
    {
        // Act
        var cut = Render<Toggle>(parameters => parameters
            .Add(p => p.Variant, "unknown")
            .Add(p => p.Size, "massive")
            .AddChildContent("Toggle"));

        // Assert
        var button = cut.Find("button");
        button.ClassList.ShouldContain("toggle-default");
        button.ClassList.ShouldNotContain("toggle-unknown");
        button.ClassList.ShouldNotContain("toggle-massive");
    }

    [Fact]
    public void Toggle_ClickUpdatesInternalPressedState_WhenNoCallbackIsRegistered()
    {
        // Act
        var cut = Render<Toggle>(parameters => parameters
            .AddChildContent("Toggle"));

        cut.Find("button").Click();

        // Assert
        var button = cut.Find("button");
        button.ClassList.ShouldContain("pressed");
        button.GetAttribute("aria-pressed")!.ShouldBe("true");
    }
}
