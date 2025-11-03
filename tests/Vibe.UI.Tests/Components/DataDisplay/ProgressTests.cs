namespace Vibe.UI.Tests.Components.DataDisplay;

public class ProgressTests : TestBase
{
    [Fact]
    public void Progress_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Progress>();

        // Assert
        var progress = cut.Find(".vibe-progress");
        progress.ShouldNotBeNull();
        progress.GetAttribute("role").ShouldBe("progressbar");
    }

    [Fact]
    public void Progress_Renders_WithValue()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 50));

        // Assert
        var progress = cut.Find(".vibe-progress");
        progress.GetAttribute("aria-valuenow").ShouldBe("50");
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 50%");
    }

    [Fact]
    public void Progress_ClampsValueTo100()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 150));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 100%");
    }

    [Fact]
    public void Progress_ClampsValueTo0()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, -50));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 0%");
    }

    [Fact]
    public void Progress_ShowsIndeterminateAnimation()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.IndeterminateAnimation, true));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldContain("animate");
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 100%");
    }

    [Fact]
    public void Progress_Applies_Variant_Class()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Variant, "success")
            .Add(p => p.Value, 75));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldContain("vibe-progress-success");
    }

    [Fact]
    public void Progress_HasAccessibilityAttributes()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 30));

        // Assert
        var progress = cut.Find(".vibe-progress");
        progress.GetAttribute("aria-valuemin").ShouldBe("0");
        progress.GetAttribute("aria-valuemax").ShouldBe("100");
        progress.GetAttribute("aria-valuenow").ShouldBe("30");
    }

    // === Boundary Value Tests ===

    [Fact]
    public void Progress_Value0_RendersEmpty()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 0));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 0%");
        cut.Find(".vibe-progress").GetAttribute("aria-valuenow").ShouldBe("0");
    }

    [Fact]
    public void Progress_Value100_RendersFull()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 100));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 100%");
        cut.Find(".vibe-progress").GetAttribute("aria-valuenow").ShouldBe("100");
    }

    [Fact]
    public void Progress_Value1_RendersMinimalProgress()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 1));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 1%");
        cut.Find(".vibe-progress").GetAttribute("aria-valuenow").ShouldBe("1");
    }

    [Fact]
    public void Progress_Value99_RendersNearComplete()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 99));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 99%");
        cut.Find(".vibe-progress").GetAttribute("aria-valuenow").ShouldBe("99");
    }

    [Fact]
    public void Progress_NegativeValue_ClampsToZero()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, -100));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 0%");
        cut.Find(".vibe-progress").GetAttribute("aria-valuenow").ShouldBe("-100");
    }

    [Fact]
    public void Progress_OverMaxValue_ClampsTo100()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 500));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 100%");
        cut.Find(".vibe-progress").GetAttribute("aria-valuenow").ShouldBe("500");
    }

    // === Variant Tests ===

    [Fact]
    public void Progress_DefaultVariant_HasDefaultClass()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 50));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldContain("vibe-progress-default");
    }

    [Fact]
    public void Progress_SuccessVariant_HasSuccessClass()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Variant, "success")
            .Add(p => p.Value, 80));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldContain("vibe-progress-success");
    }

    [Fact]
    public void Progress_WarningVariant_HasWarningClass()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Variant, "warning")
            .Add(p => p.Value, 50));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldContain("vibe-progress-warning");
    }

    [Fact]
    public void Progress_ErrorVariant_HasErrorClass()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Variant, "error")
            .Add(p => p.Value, 20));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldContain("vibe-progress-error");
    }

    [Fact]
    public void Progress_InfoVariant_HasInfoClass()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Variant, "info")
            .Add(p => p.Value, 60));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldContain("vibe-progress-info");
    }

    [Fact]
    public void Progress_PrimaryVariant_HasPrimaryClass()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Variant, "primary")
            .Add(p => p.Value, 75));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldContain("vibe-progress-primary");
    }

    // === Indeterminate Mode Tests ===

    [Fact]
    public void Progress_IndeterminateMode_FullWidth()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.IndeterminateAnimation, true));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 100%");
    }

    [Fact]
    public void Progress_IndeterminateMode_IgnoresValue()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.IndeterminateAnimation, true)
            .Add(p => p.Value, 30));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 100%");
    }

    [Fact]
    public void Progress_DeterminateMode_NoAnimateClass()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.IndeterminateAnimation, false)
            .Add(p => p.Value, 50));

        // Assert
        cut.Find(".vibe-progress").ClassList.ShouldNotContain("animate");
    }

    // === Accessibility Tests ===

    [Fact]
    public void Progress_AriaAttributes_SetCorrectly()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 45));

        // Assert
        var progress = cut.Find(".vibe-progress");
        progress.GetAttribute("role").ShouldBe("progressbar");
        progress.GetAttribute("aria-valuemin").ShouldBe("0");
        progress.GetAttribute("aria-valuemax").ShouldBe("100");
        progress.GetAttribute("aria-valuenow").ShouldBe("45");
    }

    [Fact]
    public void Progress_AriaAttributes_UpdateWithValue()
    {
        // Arrange
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 25));

        // Act
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Value, 75));

        // Assert
        var progress = cut.Find(".vibe-progress");
        progress.GetAttribute("aria-valuenow").ShouldBe("75");
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 75%");
    }

    [Fact]
    public void Progress_AriaAttributes_IndeterminateMode()
    {
        // Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.IndeterminateAnimation, true));

        // Assert
        var progress = cut.Find(".vibe-progress");
        progress.GetAttribute("role").ShouldBe("progressbar");
        progress.GetAttribute("aria-valuemin").ShouldBe("0");
        progress.GetAttribute("aria-valuemax").ShouldBe("100");
    }

    // === State Update Tests ===

    [Fact]
    public void Progress_ValueUpdate_UpdatesWidth()
    {
        // Arrange
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 10));

        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 10%");

        // Act
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Value, 90));

        // Assert
        indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").ShouldContain("width: 90%");
    }

    [Fact]
    public void Progress_VariantUpdate_UpdatesClass()
    {
        // Arrange
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Variant, "default")
            .Add(p => p.Value, 50));

        cut.Find(".vibe-progress").ClassList.ShouldContain("vibe-progress-default");

        // Act
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Variant, "success")
            .Add(p => p.Value, 50));

        // Assert
        var progress = cut.Find(".vibe-progress");
        progress.ClassList.ShouldContain("vibe-progress-success");
        progress.ClassList.ShouldNotContain("vibe-progress-default");
    }
}
