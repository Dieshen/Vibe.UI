namespace Vibe.UI.Tests.Components.Inputs;

public class ButtonTests : TestBase
{
    [Fact]
    public void Button_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Button>();

        // Assert
        cut.MarkupMatches(@"<button class:ignore type=""button""></button>");
    }

    [Fact]
    public void Button_Renders_WithChildContent()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .AddChildContent("Click Me"));

        // Assert
        var button = cut.Find("button");
        button.TextContent.ShouldBe("Click Me");
    }

    [Fact]
    public void Button_Applies_Disabled_Attribute()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        cut.Find("button").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Button_Renders_AsLink_WhenHrefProvided()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .AddChildContent("Link"));

        // Assert
        var link = cut.Find("a");
        link.GetAttribute("href")!.ShouldBe("https://example.com");
        link.TextContent.ShouldBe("Link");
    }

    [Fact]
    public void Button_InvokesOnClick_WhenClicked()
    {
        // Arrange
        var clicked = false;
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.OnClick, () => clicked = true)
            .AddChildContent("Click"));

        // Act
        cut.Find("button").Click();

        // Assert
        clicked.ShouldBeTrue();
    }

    [Fact]
    public void Button_DoesNotInvokeOnClick_WhenDisabled()
    {
        // Arrange
        var clicked = false;
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnClick, () => clicked = true)
            .AddChildContent("Click"));

        // Act
        cut.Find("button").Click();

        // Assert
        clicked.ShouldBeFalse();
    }

    // === Edge Cases ===

    [Fact]
    public void Button_WithNullChildContent_RendersEmpty()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment?)null));

        // Assert
        var button = cut.Find("button");
        button.InnerHtml.ShouldNotContain("vibe-button-content");
    }

    [Fact]
    public void Button_WithComplexChildContent_RendersCorrectly()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .AddChildContent("<strong>Bold</strong> <em>Italic</em> Text"));

        // Assert
        var content = cut.Find(".vibe-button-content");
        content.InnerHtml.ShouldContain("<strong>Bold</strong>");
        content.InnerHtml.ShouldContain("<em>Italic</em>");
    }

    [Fact]
    public void Button_WithVeryLongText_RendersCorrectly()
    {
        // Arrange
        var longText = new string('X', 500);

        // Act
        var cut = Render<Button>(parameters => parameters
            .AddChildContent(longText));

        // Assert
        cut.Find(".vibe-button-content").TextContent.ShouldBe(longText);
    }

    [Fact]
    public void Button_WithSpecialCharacters_EncodesCorrectly()
    {
        // Arrange
        var specialText = "<script>alert('xss')</script> & \"quotes\" 'apostrophes'";

        // Act
        var cut = Render<Button>(parameters => parameters
            .AddChildContent(specialText));

        // Assert - TextContent shows decoded text, InnerHtml shows encoded HTML
        var content = cut.Find(".vibe-button-content");
        // TextContent is the decoded text, so HTML tags are stripped and entities decoded
        content.TextContent.ShouldContain("alert('xss')"); // <script> tags removed by browser
        content.TextContent.ShouldContain("&");
        content.TextContent.ShouldContain("\"quotes\"");
    }

    // === Button Variants ===

    [Fact]
    public void Button_AllVariants_ApplyCorrectClasses()
    {
        // Test each variant
        var variants = new[]
        {
            ButtonVariant.Primary,
            ButtonVariant.Secondary,
            ButtonVariant.Destructive,
            ButtonVariant.Outline,
            ButtonVariant.Ghost,
            ButtonVariant.Link
        };

        foreach (var variant in variants)
        {
            var cut = Render<Button>(parameters => parameters
                .Add(p => p.Variant, variant));

            cut.Find("button").ClassList.ShouldContain($"vibe-button-{variant.ToString().ToLowerInvariant()}");
        }
    }

    [Fact]
    public void Button_AllSizes_ApplyCorrectClasses()
    {
        // Test each size
        var sizes = new[]
        {
            ComponentSize.Small,
            ComponentSize.Medium,
            ComponentSize.Large
        };

        foreach (var size in sizes)
        {
            var cut = Render<Button>(parameters => parameters
                .Add(p => p.Size, size));

            cut.Find("button").ClassList.ShouldContain($"vibe-button-{size.ToString().ToLowerInvariant()}");
        }
    }

    // === Button Types ===

    [Fact]
    public void Button_WithSubmitType_RendersCorrectType()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Type, "submit"));

        // Assert
        cut.Find("button").GetAttribute("type")!.ShouldBe("submit");
    }

    [Fact]
    public void Button_WithResetType_RendersCorrectType()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Type, "reset"));

        // Assert
        cut.Find("button").GetAttribute("type")!.ShouldBe("reset");
    }

    // === Loading State ===

    [Fact]
    public void Button_WithLoading_ShowsSpinner()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Loading, true)
            .AddChildContent("Loading..."));

        // Assert
        cut.Find(".vibe-button-spinner").ShouldNotBeNull();
        cut.Find("button").ClassList.ShouldContain("vibe-button-loading");
    }

    [Fact]
    public void Button_WithLoading_DoesNotInvokeOnClick()
    {
        // Arrange
        var clicked = false;
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Loading, true)
            .Add(p => p.OnClick, () => clicked = true)
            .AddChildContent("Loading..."));

        // Act
        cut.Find("button").Click();

        // Assert
        clicked.ShouldBeFalse();
    }

    // === Icon ===

    [Fact]
    public void Button_WithIcon_RendersIcon()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Icon, builder => builder.AddContent(0, "<svg></svg>"))
            .AddChildContent("With Icon"));

        // Assert
        cut.Find(".vibe-button-icon").ShouldNotBeNull();
    }

    [Fact]
    public void Button_WithIconOnly_AppliesIconOnlyClass()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Icon, builder => builder.AddContent(0, "<svg></svg>")));

        // Assert
        cut.Find("button").ClassList.ShouldContain("vibe-button-icon-only");
    }

    // === Full Width ===

    [Fact]
    public void Button_WithFullWidth_AppliesFullWidthClass()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.FullWidth, true)
            .AddChildContent("Full Width"));

        // Assert
        cut.Find("button").ClassList.ShouldContain("vibe-button-full-width");
    }

    // === Link Rendering ===

    [Fact]
    public void Button_AsLink_WithTarget_RendersTarget()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Target, "_blank")
            .AddChildContent("Link"));

        // Assert
        var link = cut.Find("a");
        link.GetAttribute("target")!.ShouldBe("_blank");
    }

    [Fact]
    public void Button_AsLink_WithTarget_HasSecurityRel()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Target, "_blank")
            .AddChildContent("Link"));

        // Assert
        cut.Find("a").GetAttribute("rel")!.ShouldBe("noopener noreferrer");
    }

    [Fact]
    public void Button_AsLink_WithCustomRel_UsesCustomRel()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Rel, "custom")
            .AddChildContent("Link"));

        // Assert
        cut.Find("a").GetAttribute("rel")!.ShouldBe("custom");
    }

    [Fact]
    public void Button_AsLink_WithDisabled_HasAriaDisabledAttribute()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Disabled, true)
            .AddChildContent("Disabled Link"));

        // Assert - anchors don't support disabled, so we use aria-disabled for accessibility
        var anchor = cut.Find("a");
        anchor.GetAttribute("aria-disabled")!.ShouldBe("true");
        anchor.GetAttribute("tabindex")!.ShouldBe("-1");
        // href should be null when disabled to prevent navigation
        anchor.GetAttribute("href")!.ShouldBeNull();
        anchor.ClassList.ShouldContain("vibe-button-disabled");
    }

    // === Event Handlers ===

    [Fact]
    public void Button_WithNoOnClickDelegate_DoesNotThrow()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .AddChildContent("No Handler"));

        // Act & Assert - Should not throw
        cut.Find("button").Click();
    }

    [Fact]
    public void Button_MultipleRapidClicks_HandlesCorrectly()
    {
        // Arrange
        var clickCount = 0;
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.OnClick, () => clickCount++)
            .AddChildContent("Click"));

        var button = cut.Find("button");

        // Act - Simulate multiple rapid clicks
        button.Click();
        button.Click();
        button.Click();

        // Assert
        clickCount.ShouldBe(3);
    }

    // === Additional Attributes ===

    [Fact]
    public void Button_WithAdditionalAttributes_MergesCorrectly()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .AddChildContent("Custom")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "my-button" },
                { "aria-label", "Custom Button" }
            }));

        // Assert
        var button = cut.Find("button");
        button.GetAttribute("data-testid")!.ShouldBe("my-button");
        button.GetAttribute("aria-label")!.ShouldBe("Custom Button");
    }

    [Fact]
    public void Button_WithClassParameter_AppendsCustomClass()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Class, "tracking-button")
            .AddChildContent("Tracked"));

        // Assert
        var button = cut.Find("button");
        button.ClassList.ShouldContain("vibe-button");
        button.ClassList.ShouldContain("tracking-button");
    }

    [Fact]
    public void Button_WithLoading_DisablesNativeButtonAndSetsBusyState()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Loading, true)
            .AddChildContent("Saving"));

        // Assert
        var button = cut.Find("button");
        button.HasAttribute("disabled").ShouldBeTrue();
        button.GetAttribute("aria-busy")!.ShouldBe("true");
    }

    [Fact]
    public void Button_AsLink_WithLoading_RemovesNavigationAndSetsDisabledSemantics()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Loading, true)
            .AddChildContent("Loading Link"));

        // Assert
        var link = cut.Find("a");
        link.GetAttribute("href").ShouldBeNull();
        link.GetAttribute("aria-disabled")!.ShouldBe("true");
        link.GetAttribute("aria-busy")!.ShouldBe("true");
        link.GetAttribute("tabindex")!.ShouldBe("-1");
        link.ClassList.ShouldContain("vibe-button-disabled");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Button_WithNullOrWhitespaceType_FallsBackToButton(string? type)
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Type, type));

        // Assert
        cut.Find("button").GetAttribute("type")!.ShouldBe("button");
    }

    [Fact]
    public void Button_WithInvalidVariantAndSize_FallsBackToDefaults()
    {
        // Act
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Variant, (ButtonVariant)999)
            .Add(p => p.Size, (ComponentSize)999));

        // Assert
        var classes = cut.Find("button").ClassList;
        classes.ShouldContain("vibe-button-primary");
        classes.ShouldContain("vibe-button-medium");
        classes.ShouldNotContain("vibe-button-999");
    }
}
