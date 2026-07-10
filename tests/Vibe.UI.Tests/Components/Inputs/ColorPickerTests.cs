namespace Vibe.UI.Tests.Components.Inputs;

public class ColorPickerTests : TestBase
{
    [Fact]
    public void ColorPicker_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<ColorPicker>();

        // Assert
        var picker = cut.Find(".vibe-color-picker");
        picker.ShouldNotBeNull();
    }

    [Fact]
    public void ColorPicker_Displays_ColorSwatch()
    {
        // Act
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.Value, "#FF0000"));

        // Assert
        var swatch = cut.Find(".vibe-color-picker-swatch");
        swatch.GetAttribute("style")!.ShouldContain("#FF0000");
    }

    [Fact]
    public void ColorPicker_Displays_ColorValue()
    {
        // Act
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.Value, "#00FF00"));

        // Assert
        var value = cut.Find(".vibe-color-picker-value");
        value.TextContent.ShouldBe("#00FF00");
    }

    [Fact]
    public void ColorPicker_InvalidValue_FallsBackToDefaultColor()
    {
        // Act
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.Value, "#GGGGGG"));

        // Assert
        cut.Find(".vibe-color-picker-value").TextContent.ShouldBe("#000000");
        cut.Find(".vibe-color-picker-swatch").GetAttribute("style")!.ShouldContain("#000000");
    }

    [Fact]
    public void ColorPicker_Shows_Popover_WhenClicked()
    {
        // Act
        var cut = Render<ColorPicker>();
        var preview = cut.Find(".vibe-color-picker-preview");
        preview.Click();

        // Assert
        cut.Find(".vibe-color-picker").ClassList.ShouldContain("vibe-color-picker-open");
        cut.FindAll(".vibe-color-picker-popover").ShouldNotBeEmpty();
    }

    [Fact]
    public void ColorPicker_Trigger_ReflectsOpenState_ForClickAndKeyboard()
    {
        // Arrange
        var cut = Render<ColorPicker>();
        var trigger = cut.Find(".vibe-color-picker-preview");

        // Assert
        trigger.GetAttribute("role").ShouldBe("button");
        trigger.GetAttribute("tabindex").ShouldBe("0");
        trigger.GetAttribute("aria-haspopup").ShouldBe("dialog");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");

        // Act
        trigger.Click();

        // Assert
        trigger = cut.Find(".vibe-color-picker-preview");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
        var controls = trigger.GetAttribute("aria-controls");
        string.IsNullOrWhiteSpace(controls).ShouldBeFalse();
        cut.Find($"#{controls}").GetAttribute("role").ShouldBe("dialog");

        // Act
        trigger.KeyDown("Escape");

        // Assert
        trigger = cut.Find(".vibe-color-picker-preview");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");

        // Act
        trigger.KeyDown("Enter");

        // Assert
        trigger = cut.Find(".vibe-color-picker-preview");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
    }

    [Fact]
    public void ColorPicker_Shows_AlphaSlider_WhenEnabled()
    {
        // Act
        var cut = Render<ColorPicker>(parameters => parameters
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
        var cut = Render<ColorPicker>(parameters => parameters
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
        var cut = Render<ColorPicker>();
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
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.ShowPresets, true));

        var preview = cut.Find(".vibe-color-picker-preview");
        preview.Click();

        // Assert
        cut.FindAll(".vibe-color-picker-presets").ShouldNotBeEmpty();
        cut.FindAll(".vibe-color-picker-preset").ShouldNotBeEmpty();
    }

    [Fact]
    public void ColorPicker_PresetButtons_HaveAccessibleNames()
    {
        // Act
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.Presets, new List<string> { "#FF0000", "#00FF00" }));

        cut.Find(".vibe-color-picker-preview").Click();

        // Assert
        var presets = cut.Find(".vibe-color-picker-presets");
        presets.GetAttribute("role").ShouldBe("group");
        var presetsLabelId = presets.GetAttribute("aria-labelledby");
        string.IsNullOrWhiteSpace(presetsLabelId).ShouldBeFalse();
        cut.Find($"#{presetsLabelId}").TextContent.ShouldBe("Preset colors");

        var buttons = cut.FindAll(".vibe-color-picker-preset");
        buttons.Count.ShouldBe(2);
        buttons[0].GetAttribute("aria-label").ShouldBe("Select preset color #FF0000");
        buttons[1].GetAttribute("aria-label").ShouldBe("Select preset color #00FF00");
    }

    [Fact]
    public void ColorPicker_AssociatesLabelsWithInputs()
    {
        // Act
        var cut = Render<ColorPicker>();

        cut.Find(".vibe-color-picker-preview").Click();

        // Assert
        foreach (var input in cut.FindAll("input"))
        {
            var id = input.GetAttribute("id");
            string.IsNullOrWhiteSpace(id).ShouldBeFalse();
            cut.Find($"label[for='{id}']").ShouldNotBeNull();
        }
    }

    [Fact]
    public void ColorPicker_InvalidHexInput_PreservesPreviousValueAndSkipsCallback()
    {
        // Arrange
        string? changedValue = null;
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.Value, "#123456")
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find(".vibe-color-picker-preview").Click();

        // Act
        cut.Find("input[type='text']").Input("#GGGGGG");

        // Assert
        cut.Find(".vibe-color-picker-value").TextContent.ShouldBe("#123456");
        changedValue.ShouldBeNull();
    }

    [Fact]
    public void ColorPicker_InvokesValueChanged_ForValidHexInputAndPreset()
    {
        // Arrange
        var changedValues = new List<string>();
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.Presets, new List<string> { "#112233" })
            .Add(p => p.ValueChanged, value => changedValues.Add(value)));

        cut.Find(".vibe-color-picker-preview").Click();

        // Act
        cut.Find("input[type='text']").Input("#ABCDEF");
        cut.Find(".vibe-color-picker-preset").Click();

        // Assert
        changedValues.ShouldBe(new[] { "#ABCDEF", "#112233" });
    }

    [Fact]
    public void ColorPicker_Applies_DisabledState()
    {
        // Act
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        cut.Find(".vibe-color-picker").ClassList.ShouldContain("vibe-color-picker-disabled");
    }

    [Fact]
    public void ColorPicker_Applies_CustomCssClass()
    {
        // Act
        var cut = Render<ColorPicker>(parameters => parameters
            .Add(p => p.CssClass, "custom-picker"));

        // Assert
        cut.Find(".vibe-color-picker").ClassList.ShouldContain("custom-picker");
    }
}
