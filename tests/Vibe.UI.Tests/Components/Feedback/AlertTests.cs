namespace Vibe.UI.Tests.Components.Feedback;

public class AlertTests : TestBase
{
    [Fact]
    public void Alert_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .AddChildContent("Test alert"));

        // Assert
        var alert = cut.Find(".vibe-alert");
        alert.ShouldNotBeNull();
        alert.TextContent.ShouldContain("Test alert");
    }

    [Fact]
    public void Alert_Applies_Variant_Class()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Variant, AlertVariant.Success)
            .AddChildContent("Success"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-success");
    }

    [Fact]
    public void Alert_Renders_Dismissible_WithCloseButton()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .AddChildContent("Dismissible alert"));

        // Assert
        cut.FindAll("button").ShouldNotBeEmpty();
    }

    [Fact]
    public void Alert_HasCloseButton_WhenDismissible()
    {
        // Arrange
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .AddChildContent("Alert"));

        // Assert
        var closeButton = cut.FindAll("button");
        closeButton.ShouldNotBeEmpty();
    }

    // ===== Variant Tests =====

    [Fact]
    public void Alert_DefaultVariant_HasDefaultClass()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Variant, AlertVariant.Default)
            .AddChildContent("Default alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-default");
    }

    [Fact]
    public void Alert_SuccessVariant_HasSuccessClass()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Variant, AlertVariant.Success)
            .AddChildContent("Success alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-success");
    }

    [Fact]
    public void Alert_InfoVariant_HasInfoClass()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Variant, AlertVariant.Info)
            .AddChildContent("Info alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-info");
    }

    [Fact]
    public void Alert_WarningVariant_HasWarningClass()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Variant, AlertVariant.Warning)
            .AddChildContent("Warning alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-warning");
    }

    [Fact]
    public void Alert_DestructiveVariant_HasDestructiveClass()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Variant, AlertVariant.Destructive)
            .AddChildContent("Destructive alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-destructive");
    }

    // ===== Title Tests =====

    [Fact]
    public void Alert_WithTitle_RendersTitleElement()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Title, "Important Notice")
            .AddChildContent("Alert content"));

        // Assert
        var title = cut.Find(".vibe-alert-title");
        title.ShouldNotBeNull();
        title.TextContent.ShouldBe("Important Notice");
    }

    [Fact]
    public void Alert_WithoutTitle_NoTitleElement()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .AddChildContent("Alert content"));

        // Assert
        cut.FindAll(".vibe-alert-title").ShouldBeEmpty();
    }

    [Fact]
    public void Alert_WithTitleAndContent_RendersBothSections()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Title, "Alert Title")
            .AddChildContent("Alert Description"));

        // Assert
        var title = cut.Find(".vibe-alert-title");
        var description = cut.Find(".vibe-alert-description");

        title.TextContent.ShouldBe("Alert Title");
        description.TextContent.ShouldContain("Alert Description");
    }

    // ===== Icon Tests =====

    [Fact]
    public void Alert_WithIcon_RendersIconContent()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Icon, builder => builder.AddMarkupContent(0, "<span class='test-icon'>!</span>"))
            .AddChildContent("Alert with icon"));

        // Assert
        var iconContainer = cut.Find(".vibe-alert-icon");
        iconContainer.ShouldNotBeNull();
        iconContainer.InnerHtml.ShouldContain("test-icon");
    }

    [Fact]
    public void Alert_WithoutIcon_NoIconElement()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .AddChildContent("Alert without icon"));

        // Assert
        cut.FindAll(".vibe-alert-icon").ShouldBeEmpty();
    }

    // ===== Dismissible Behavior Tests =====

    [Fact]
    public void Alert_NonDismissible_NoCloseButton()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Dismissible, false)
            .AddChildContent("Non-dismissible alert"));

        // Assert
        cut.FindAll("button").ShouldBeEmpty();
    }

    [Fact]
    public void Alert_CloseButton_InvokesOnDismiss()
    {
        // Arrange
        var dismissCalled = false;
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .Add(p => p.OnDismiss, EventCallback.Factory.Create(this, () => dismissCalled = true))
            .AddChildContent("Dismissible alert"));

        // Act
        var closeButton = cut.Find("button");
        closeButton.Click();

        // Assert
        dismissCalled.ShouldBeTrue();
    }

    [Fact]
    public void Alert_CloseButtonHasAriaLabel()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .AddChildContent("Alert"));

        // Assert
        var closeButton = cut.Find("button");
        closeButton.GetAttribute("aria-label")!.ShouldBe("Close");
    }

    [Fact]
    public void Alert_CloseButton_UsesCustomAriaLabel()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .Add(p => p.CloseButtonAriaLabel, "Dismiss notification")
            .AddChildContent("Alert"));

        // Assert
        cut.Find("button").GetAttribute("aria-label")!.ShouldBe("Dismiss notification");
    }

    [Fact]
    public void Alert_DisabledDismiss_DoesNotInvokeOnDismiss()
    {
        // Arrange
        var dismissCalled = false;
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.OnDismiss, EventCallback.Factory.Create(this, () => dismissCalled = true))
            .AddChildContent("Disabled dismiss"));

        // Act
        var closeButton = cut.Find("button");
        closeButton.Click();

        // Assert
        dismissCalled.ShouldBeFalse();
        closeButton.HasAttribute("disabled").ShouldBeTrue();
        closeButton.GetAttribute("aria-disabled")!.ShouldBe("true");
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-disabled");
    }

    // ===== Accessibility Tests =====

    [Fact]
    public void Alert_HasCorrectAriaRole()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .AddChildContent("Accessible alert"));

        // Assert
        var alert = cut.Find(".vibe-alert");
        alert.GetAttribute("role")!.ShouldBe("alert");
    }

    [Fact]
    public void Alert_HasLiveRegionAttributes()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .AddChildContent("Accessible alert"));

        // Assert
        var alert = cut.Find(".vibe-alert");
        alert.GetAttribute("aria-live")!.ShouldBe("assertive");
        alert.GetAttribute("aria-atomic")!.ShouldBe("true");
        alert.GetAttribute("aria-disabled")!.ShouldBe("false");
    }

    [Fact]
    public void Alert_CustomAccessibilityAttributes_AreApplied()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Role, "status")
            .Add(p => p.AriaLive, "polite")
            .Add(p => p.AriaAtomic, false)
            .Add(p => p.AriaLabel, "Sync status")
            .AddChildContent("Saved"));

        // Assert
        var alert = cut.Find(".vibe-alert");
        alert.GetAttribute("role")!.ShouldBe("status");
        alert.GetAttribute("aria-live")!.ShouldBe("polite");
        alert.GetAttribute("aria-atomic")!.ShouldBe("false");
        alert.GetAttribute("aria-label")!.ShouldBe("Sync status");
    }

    [Fact]
    public void Alert_Icon_IsHiddenFromAssistiveTechnologyByDefault()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Icon, builder => builder.AddMarkupContent(0, "<span>!</span>"))
            .AddChildContent("Alert with icon"));

        // Assert
        cut.Find(".vibe-alert-icon").GetAttribute("aria-hidden")!.ShouldBe("true");
    }

    [Fact]
    public void Alert_IconAriaHiddenFalse_ExposesIcon()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.IconAriaHidden, false)
            .Add(p => p.Icon, builder => builder.AddMarkupContent(0, "<span>!</span>"))
            .AddChildContent("Alert with icon"));

        // Assert
        cut.Find(".vibe-alert-icon").GetAttribute("aria-hidden")!.ShouldBe("false");
    }

    [Fact]
    public void Alert_AppliesCustomClassAndAdditionalAttributes()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Class, "custom-alert")
            .AddUnmatched("data-testid", "alert-root")
            .AddChildContent("Custom alert"));

        // Assert
        var alert = cut.Find(".vibe-alert");
        alert.ClassList.ShouldContain("custom-alert");
        alert.GetAttribute("data-testid")!.ShouldBe("alert-root");
    }

    // ===== Edge Case Tests =====

    [Fact]
    public void Alert_WithEmptyContent_Renders()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .AddChildContent(""));

        // Assert
        var alert = cut.Find(".vibe-alert");
        alert.ShouldNotBeNull();
    }

    [Fact]
    public void Alert_WithWhitespaceTitle_DoesNotRenderTitleElement()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .Add(p => p.Title, "   ")
            .AddChildContent("Alert content"));

        // Assert
        cut.FindAll(".vibe-alert-title").ShouldBeEmpty();
    }

    [Fact]
    public void Alert_WithLongContent_HandlesOverflow()
    {
        // Arrange
        var longContent = new string('a', 500);

        // Act
        var cut = Render<Alert>(parameters => parameters
            .AddChildContent(longContent));

        // Assert
        var description = cut.Find(".vibe-alert-description");
        description.TextContent.ShouldContain(longContent);
    }

    [Fact]
    public void Alert_ContentSection_RendersInDescriptionDiv()
    {
        // Act
        var cut = Render<Alert>(parameters => parameters
            .AddChildContent("Test content"));

        // Assert
        var content = cut.Find(".vibe-alert-content");
        var description = cut.Find(".vibe-alert-description");

        content.ShouldNotBeNull();
        description.ShouldNotBeNull();
        description.TextContent.ShouldBe("Test content");
    }
}
