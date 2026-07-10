namespace Vibe.UI.Tests.Components.Layout;

public class AspectRatioTests : TestBase
{
    [Fact]
    public void AspectRatio_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        container.ShouldNotBeNull();
    }

    [Fact]
    public void AspectRatio_Applies_Default16_9_Ratio()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        var style = container.GetAttribute("style");
        // 100 / (16/9) ≈ 56.25%
        style!.ShouldContain("padding-bottom: 56.");
    }

    [Fact]
    public void AspectRatio_Applies_CustomRatio()
    {
        // Act (4:3 ratio)
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 4.0 / 3.0)
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        var style = container.GetAttribute("style");
        // 100 / (4/3) = 75%
        style!.ShouldContain("padding-bottom: 75%");
    }

    [Fact]
    public void AspectRatio_Applies_SquareRatio()
    {
        // Act (1:1 ratio)
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 1.0)
            .AddChildContent("Square"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        var style = container.GetAttribute("style");
        style!.ShouldContain("padding-bottom: 100%");
    }

    [Fact]
    public void AspectRatio_Renders_ChildContent()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .AddChildContent("<div class='test-content'>Test</div>"));

        // Assert
        var content = cut.Find(".aspect-ratio-content");
        content.InnerHtml.ShouldContain("test-content");
    }

    [Fact]
    public void AspectRatio_WithZeroRatio_HandlesGracefully()
    {
        // Act - Edge case: ratio of 0 would cause division by zero
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 0.0)
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        container.ShouldNotBeNull();
        container.GetAttribute("style")!.ShouldContain("padding-bottom: 56.25%");
    }

    [Fact]
    public void AspectRatio_WithNegativeRatio_HandlesGracefully()
    {
        // Act - Edge case: negative ratio
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, -1.0)
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        container.ShouldNotBeNull();
        container.GetAttribute("style")!.ShouldContain("padding-bottom: 56.25%");
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void AspectRatio_WithNonFiniteRatio_UsesDefaultRatio(double ratio)
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, ratio)
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-aspect-ratio")
            .GetAttribute("style")!
            .ShouldContain("padding-bottom: 56.25%");
    }

    [Fact]
    public void AspectRatio_WithTinyPositiveRatio_ClampsToMinimumRatio()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 0.000001)
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-aspect-ratio")
            .GetAttribute("style")!
            .ShouldContain("padding-bottom: 10000%");
    }

    [Fact]
    public void AspectRatio_WithHugeRatio_ClampsToMaximumRatio()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 1000.0)
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-aspect-ratio")
            .GetAttribute("style")!
            .ShouldContain("padding-bottom: 1%");
    }

    [Fact]
    public void AspectRatio_WithVeryLargeRatio_HandlesCorrectly()
    {
        // Act - Ultra-wide ratio (21:9)
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 21.0 / 9.0)
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        var style = container.GetAttribute("style");
        // 100 / (21/9) ≈ 42.86%
        style!.ShouldContain("padding-bottom: 42.");
    }

    [Fact]
    public void AspectRatio_WithVerySmallRatio_HandlesCorrectly()
    {
        // Act - Portrait ratio (9:16)
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 9.0 / 16.0)
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        var style = container.GetAttribute("style");
        // 100 / (9/16) ≈ 177.78%
        style!.ShouldContain("padding-bottom: 177.");
    }

    [Fact]
    public void AspectRatio_WithCinematicRatio_AppliesCorrectly()
    {
        // Act - Cinematic 2.39:1 ratio
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 2.39)
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        var style = container.GetAttribute("style");
        // 100 / 2.39 ≈ 41.84%
        style!.ShouldContain("padding-bottom: 41.");
    }

    [Fact]
    public void AspectRatio_WithCustomClass_AppliesCorrectly()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Class, "custom-aspect-class")
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        container.ClassList.ShouldContain("custom-aspect-class");
    }

    [Fact]
    public void AspectRatio_WithAdditionalAttributes_MergesAttributesClassAndStyle()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Class, "parameter-class")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-class",
                ["style"] = "background: red;",
                ["data-testid"] = "media-frame"
            })
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        container.ClassList.ShouldContain("parameter-class");
        container.ClassList.ShouldContain("attribute-class");
        container.GetAttribute("data-testid")!.ShouldBe("media-frame");

        var style = container.GetAttribute("style")!;
        style.ShouldContain("background: red");
        style.ShouldContain("padding-bottom: 56.25%");
    }

    [Fact]
    public void AspectRatio_WithEmptyContent_RendersEmpty()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .AddChildContent(""));

        // Assert
        var content = cut.Find(".aspect-ratio-content");
        content.InnerHtml.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void AspectRatio_WithNullContent_RendersEmptyContentWrapper()
    {
        // Act
        var cut = Render<AspectRatio>();

        // Assert
        var content = cut.Find(".aspect-ratio-content");
        content.InnerHtml.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void AspectRatio_WithAriaLabel_AddsGroupSemantics()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.AriaLabel, "Media preview")
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        container.GetAttribute("role")!.ShouldBe("group");
        container.GetAttribute("aria-label")!.ShouldBe("Media preview");
    }

    [Fact]
    public void AspectRatio_WithCallerProvidedSemantics_PreservesCallerValues()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.AriaLabel, "Parameter label")
            .AddUnmatched("role", "figure")
            .AddUnmatched("aria-label", "Caller label")
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        container.GetAttribute("role")!.ShouldBe("figure");
        container.GetAttribute("aria-label")!.ShouldBe("Caller label");
    }

    [Fact]
    public void AspectRatio_WithImageContent_RendersCorrectly()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 16.0 / 9.0)
            .AddChildContent("<img src='test.jpg' alt='test' />"));

        // Assert
        var content = cut.Find(".aspect-ratio-content");
        content.InnerHtml.ShouldContain("img");
        content.InnerHtml.ShouldContain("test.jpg");
    }

    [Fact]
    public void AspectRatio_WithVideoContent_RendersCorrectly()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 16.0 / 9.0)
            .AddChildContent("<video src='test.mp4'></video>"));

        // Assert
        var content = cut.Find(".aspect-ratio-content");
        content.InnerHtml.ShouldContain("video");
        content.InnerHtml.ShouldContain("test.mp4");
    }

    [Fact]
    public void AspectRatio_Ratio4_3_CalculatesCorrectPadding()
    {
        // Act - Classic TV ratio 4:3
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 4.0 / 3.0)
            .AddChildContent("Content"));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        var style = container.GetAttribute("style");
        // 100 / (4/3) = 75%
        style!.ShouldContain("padding-bottom: 75%");
    }

    [Fact]
    public void AspectRatio_WithNestedContent_MaintainsRatio()
    {
        // Act
        var cut = Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 1.0)
            .AddChildContent(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "nested");
                builder.OpenElement(2, "p");
                builder.AddContent(3, "Nested paragraph");
                builder.CloseElement();
                builder.CloseElement();
            }));

        // Assert
        var container = cut.Find(".vibe-aspect-ratio");
        var style = container.GetAttribute("style");
        style!.ShouldContain("padding-bottom: 100%"); // Square ratio

        var content = cut.Find(".aspect-ratio-content");
        content.InnerHtml.ShouldContain("nested");
        content.InnerHtml.ShouldContain("Nested paragraph");
    }
}
