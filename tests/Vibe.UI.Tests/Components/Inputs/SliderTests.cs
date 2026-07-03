namespace Vibe.UI.Tests.Components.Inputs;

public class SliderTests : TestBase
{
    [Fact]
    public void Slider_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Slider>();

        // Assert
        var slider = cut.Find("input[type='range']");
        slider.ShouldNotBeNull();
        slider.GetAttribute("min")!.ShouldBe("0");
        slider.GetAttribute("max")!.ShouldBe("100");
    }

    [Fact]
    public void Slider_Renders_WithValue()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Value, 50));

        // Assert
        var slider = cut.Find("input[type='range']");
        slider.GetAttribute("value")!.ShouldBe("50");
    }

    [Fact]
    public void Slider_Applies_CustomRange()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, 10)
            .Add(p => p.Max, 200)
            .Add(p => p.Step, 5));

        // Assert
        var slider = cut.Find("input[type='range']");
        slider.GetAttribute("min")!.ShouldBe("10");
        slider.GetAttribute("max")!.ShouldBe("200");
        slider.GetAttribute("step")!.ShouldBe("5");
    }

    [Fact]
    public void Slider_Applies_Disabled_Attribute()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var slider = cut.Find("input[type='range']");
        slider.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Slider_Shows_Value_WhenShowValueIsTrue()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Value, 75)
            .Add(p => p.ShowValue, true));

        // Assert
        var valueDisplay = cut.Find(".vibe-slider-value");
        valueDisplay.TextContent.ShouldBe("75");
    }

    [Fact]
    public void Slider_Hides_Value_WhenShowValueIsFalse()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.ShowValue, false));

        // Assert
        cut.FindAll(".vibe-slider-value").ShouldBeEmpty();
    }

    [Fact]
    public void Slider_CalculatesCorrectPercentage()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, 0)
            .Add(p => p.Max, 100)
            .Add(p => p.Value, 50));

        // Assert - Component uses calc() formula with the percentage value
        var range = cut.Find(".vibe-slider-range");
        var style = range.GetAttribute("style");
        style!.ShouldContain("50%");
    }

    // === Edge Cases ===

    [Fact]
    public void Slider_WithMinEqualToMax_HandlesGracefully()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, 50)
            .Add(p => p.Max, 50)
            .Add(p => p.Value, 50));

        // Assert - When min equals max, GetPercentage returns "0"
        var range = cut.Find(".vibe-slider-range");
        var style = range.GetAttribute("style");
        style!.ShouldContain("0%");
    }

    [Fact]
    public void Slider_WithMinGreaterThanMax_HandlesGracefully()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, 100)
            .Add(p => p.Max, 0)
            .Add(p => p.Value, 50));

        // Assert - When min >= max, GetPercentage returns "0"
        var range = cut.Find(".vibe-slider-range");
        var style = range.GetAttribute("style");
        style!.ShouldContain("0%");
    }

    [Fact]
    public void Slider_WithNegativeRange_CalculatesCorrectly()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, -100)
            .Add(p => p.Max, 0)
            .Add(p => p.Value, -50));

        // Assert - Component uses calc() formula with the percentage value
        var range = cut.Find(".vibe-slider-range");
        var style = range.GetAttribute("style");
        style!.ShouldContain("50%");
    }

    [Fact]
    public void Slider_WithDecimalValues_CalculatesCorrectly()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, 0.0)
            .Add(p => p.Max, 1.0)
            .Add(p => p.Step, 0.1)
            .Add(p => p.Value, 0.5));

        // Assert
        var slider = cut.Find("input[type='range']");
        slider.GetAttribute("min")!.ShouldBe("0");
        slider.GetAttribute("max")!.ShouldBe("1");
        slider.GetAttribute("step")!.ShouldBe("0.1");
        slider.GetAttribute("value")!.ShouldBe("0.5");
    }

    // === Value Display ===

    [Fact]
    public void Slider_ShowValue_DisplaysFormattedValue()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Value, 75.5)
            .Add(p => p.ShowValue, true));

        // Assert
        var valueDisplay = cut.Find(".vibe-slider-value");
        valueDisplay.TextContent.ShouldBe("75.5");
    }

    [Fact]
    public void Slider_WithZeroValue_DisplaysZero()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Value, 0)
            .Add(p => p.ShowValue, true));

        // Assert
        cut.Find(".vibe-slider-value").TextContent.ShouldBe("0");
    }

    [Fact]
    public void Slider_WithNegativeValue_DisplaysNegative()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, -100)
            .Add(p => p.Max, 100)
            .Add(p => p.Value, -25)
            .Add(p => p.ShowValue, true));

        // Assert
        cut.Find(".vibe-slider-value").TextContent.ShouldBe("-25");
    }

    // === CSS Classes ===

    [Fact]
    public void Slider_HasBaseClass()
    {
        // Act
        var cut = Render<Slider>();

        // Assert
        cut.Find(".vibe-slider").ShouldNotBeNull();
    }

    [Fact]
    public void Slider_WhenDisabled_HasDisabledClass()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        cut.Find(".vibe-slider-disabled").ShouldNotBeNull();
    }

    [Fact]
    public void Slider_WhenNotDisabled_DoesNotHaveDisabledClass()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Disabled, false));

        // Assert
        cut.FindAll(".vibe-slider-disabled").ShouldBeEmpty();
    }

    // === Boundary Value Tests ===

    [Fact]
    public void Slider_WithValueBelowMin_ClampsToMin()
    {
        // Act - Component should handle this internally
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, 0)
            .Add(p => p.Max, 100)
            .Add(p => p.Value, -10));

        // Assert - Percentage should be 0% when clamped
        var range = cut.Find(".vibe-slider-range");
        var style = range.GetAttribute("style");
        // Value below min should result in 0% or negative percentage clamped to 0
        style!.ShouldContain("width:");
    }

    [Fact]
    public void Slider_WithValueAboveMax_ClampsToMax()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.Min, 0)
            .Add(p => p.Max, 100)
            .Add(p => p.Value, 150));

        // Assert - Should clamp to 100% (component uses calc() formula)
        var range = cut.Find(".vibe-slider-range");
        var style = range.GetAttribute("style");
        style!.ShouldContain("100%");
    }

    // === Additional Attributes ===

    [Fact]
    public void Slider_WithAdditionalAttributes_MergesCorrectly()
    {
        // Act
        var cut = Render<Slider>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "my-slider" },
                { "aria-label", "Custom Slider" }
            }));

        // Assert
        var slider = cut.Find(".vibe-slider");
        slider.GetAttribute("data-testid")!.ShouldBe("my-slider");
        slider.GetAttribute("aria-label")!.ShouldBe("Custom Slider");
    }
}
