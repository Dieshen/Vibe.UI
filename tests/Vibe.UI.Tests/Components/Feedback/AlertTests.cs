namespace Vibe.UI.Tests.Components.Feedback;

public class AlertTests : TestBase
{
    [Fact]
    public void Alert_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
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
        var cut = RenderComponent<Alert>(parameters => parameters
            .Add(p => p.Variant, Alert.AlertVariant.Success)
            .AddChildContent("Success"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-success");
    }

    [Fact]
    public void Alert_Renders_Dismissible_WithCloseButton()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .AddChildContent("Dismissible alert"));

        // Assert
        cut.FindAll("button").ShouldNotBeEmpty();
    }

    [Fact]
    public void Alert_HasCloseButton_WhenDismissible()
    {
        // Arrange
        var cut = RenderComponent<Alert>(parameters => parameters
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
        var cut = RenderComponent<Alert>(parameters => parameters
            .Add(p => p.Variant, Alert.AlertVariant.Default)
            .AddChildContent("Default alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-default");
    }

    [Fact]
    public void Alert_SuccessVariant_HasSuccessClass()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .Add(p => p.Variant, Alert.AlertVariant.Success)
            .AddChildContent("Success alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-success");
    }

    [Fact]
    public void Alert_InfoVariant_HasInfoClass()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .Add(p => p.Variant, Alert.AlertVariant.Info)
            .AddChildContent("Info alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-info");
    }

    [Fact]
    public void Alert_WarningVariant_HasWarningClass()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .Add(p => p.Variant, Alert.AlertVariant.Warning)
            .AddChildContent("Warning alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-warning");
    }

    [Fact]
    public void Alert_DestructiveVariant_HasDestructiveClass()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .Add(p => p.Variant, Alert.AlertVariant.Destructive)
            .AddChildContent("Destructive alert"));

        // Assert
        cut.Find(".vibe-alert").ClassList.ShouldContain("vibe-alert-destructive");
    }

    // ===== Title Tests =====

    [Fact]
    public void Alert_WithTitle_RendersTitleElement()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
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
        var cut = RenderComponent<Alert>(parameters => parameters
            .AddChildContent("Alert content"));

        // Assert
        cut.FindAll(".vibe-alert-title").ShouldBeEmpty();
    }

    [Fact]
    public void Alert_WithTitleAndContent_RendersBothSections()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
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
        var cut = RenderComponent<Alert>(parameters => parameters
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
        var cut = RenderComponent<Alert>(parameters => parameters
            .AddChildContent("Alert without icon"));

        // Assert
        cut.FindAll(".vibe-alert-icon").ShouldBeEmpty();
    }

    // ===== Dismissible Behavior Tests =====

    [Fact]
    public void Alert_NonDismissible_NoCloseButton()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
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
        var cut = RenderComponent<Alert>(parameters => parameters
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
        var cut = RenderComponent<Alert>(parameters => parameters
            .Add(p => p.Dismissible, true)
            .AddChildContent("Alert"));

        // Assert
        var closeButton = cut.Find("button");
        closeButton.GetAttribute("aria-label").ShouldBe("Close");
    }

    // ===== Accessibility Tests =====

    [Fact]
    public void Alert_HasCorrectAriaRole()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .AddChildContent("Accessible alert"));

        // Assert
        var alert = cut.Find(".vibe-alert");
        alert.GetAttribute("role").ShouldBe("alert");
    }

    // ===== Edge Case Tests =====

    [Fact]
    public void Alert_WithEmptyContent_Renders()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .AddChildContent(""));

        // Assert
        var alert = cut.Find(".vibe-alert");
        alert.ShouldNotBeNull();
    }

    [Fact]
    public void Alert_WithLongContent_HandlesOverflow()
    {
        // Arrange
        var longContent = new string('a', 500);

        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .AddChildContent(longContent));

        // Assert
        var description = cut.Find(".vibe-alert-description");
        description.TextContent.ShouldContain(longContent);
    }

    [Fact]
    public void Alert_ContentSection_RendersInDescriptionDiv()
    {
        // Act
        var cut = RenderComponent<Alert>(parameters => parameters
            .AddChildContent("Test content"));

        // Assert
        var content = cut.Find(".vibe-alert-content");
        var description = cut.Find(".vibe-alert-description");

        content.ShouldNotBeNull();
        description.ShouldNotBeNull();
        description.TextContent.ShouldBe("Test content");
    }
}
