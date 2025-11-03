namespace Vibe.UI.Tests.Components.Input;

public class ColorPickerTests : TestBase
{
    [Fact]
    public void ColorPicker_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<ColorPicker>();

        // Assert
        var picker = cut.Find(".vibe-color-picker");
        picker.ShouldNotBeNull();
    }

    [Fact]
    public void ColorPicker_Displays_ColorSwatch()
    {
        // Act
        var cut = RenderComponent<ColorPicker>(parameters => parameters
            .Add(p => p.Value, "#FF0000"));

        // Assert
        var swatch = cut.Find(".vibe-color-picker-swatch");
        swatch.GetAttribute("style").ShouldContain("#FF0000");
    }

    [Fact]
    public void ColorPicker_Displays_ColorValue()
    {
        // Act
        var cut = RenderComponent<ColorPicker>(parameters => parameters
            .Add(p => p.Value, "#00FF00"));

        // Assert
        var value = cut.Find(".vibe-color-picker-value");
        value.TextContent.ShouldBe("#00FF00");
    }

    [Fact]
    public void ColorPicker_Shows_Popover_WhenClicked()
    {
        // Act
        var cut = RenderComponent<ColorPicker>();
        var preview = cut.Find(".vibe-color-picker-preview");
        preview.Click();

        // Assert
        cut.Find(".vibe-color-picker").ClassList.ShouldContain("vibe-color-picker-open");
        cut.FindAll(".vibe-color-picker-popover").ShouldNotBeEmpty();
    }

    [Fact]
    public void ColorPicker_Shows_AlphaSlider_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<ColorPicker>(parameters => parameters
            .Add(p => p.ShowAlpha, true));

        var preview = cut.Find(".vibe-color-picker-preview");
        preview.Click();

        // Assert
        cut.FindAll(".vibe-color-picker-alpha-slider").ShouldNotBeEmpty();
    }

    [Fact]
    public void ColorPicker_Hides_AlphaSlider_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<ColorPicker>(parameters => parameters
            .Add(p => p.ShowAlpha, false));

        var preview = cut.Find(".vibe-color-picker-preview");
        preview.Click();

        // Assert
        cut.FindAll(".vibe-color-picker-alpha-slider").ShouldBeEmpty();
    }

    [Fact]
    public void ColorPicker_Shows_RgbInputs_ByDefault()
    {
        // Act
        var cut = RenderComponent<ColorPicker>();
        var preview = cut.Find(".vibe-color-picker-preview");
        preview.Click();

        // Assert
        var inputs = cut.FindAll(".vibe-color-picker-input-group");
        inputs.Count.ShouldBeGreaterThanOrEqualTo(4); // HEX + R + G + B
    }

    [Fact]
    public void ColorPicker_Shows_Presets_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<ColorPicker>(parameters => parameters
            .Add(p => p.ShowPresets, true));

        var preview = cut.Find(".vibe-color-picker-preview");
        preview.Click();

        // Assert
        cut.FindAll(".vibe-color-picker-presets").ShouldNotBeEmpty();
        cut.FindAll(".vibe-color-picker-preset").ShouldNotBeEmpty();
    }

    [Fact]
    public void ColorPicker_Applies_DisabledState()
    {
        // Act
        var cut = RenderComponent<ColorPicker>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        cut.Find(".vibe-color-picker").ClassList.ShouldContain("vibe-color-picker-disabled");
    }

    [Fact]
    public void ColorPicker_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<ColorPicker>(parameters => parameters
            .Add(p => p.CssClass, "custom-picker"));

        // Assert
        cut.Find(".vibe-color-picker").ClassList.ShouldContain("custom-picker");
    }
}
