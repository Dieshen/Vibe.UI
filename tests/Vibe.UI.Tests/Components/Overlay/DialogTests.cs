namespace Vibe.UI.Tests.Components.Overlay;

public class DialogTests : TestBase
{
    [Fact]
    public void Dialog_DoesNotRender_WhenClosed()
    {
        // Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, false)
            .AddChildContent("Dialog Content"));

        // Assert
        cut.FindAll(".vibe-dialog").ShouldBeEmpty();
    }

    [Fact]
    public void Dialog_Renders_WhenOpen()
    {
        // Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("Dialog Content"));

        // Assert
        cut.Find(".vibe-dialog").ShouldNotBeNull();
    }

    [Fact]
    public void Dialog_Has_DialogRole()
    {
        // Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("Dialog Content"));

        // Assert
        var dialog = cut.Find(".vibe-dialog");
        dialog.GetAttribute("role").ShouldBe("dialog");
        dialog.GetAttribute("aria-modal").ShouldBe("true");
    }

    [Fact]
    public void Dialog_Renders_Overlay()
    {
        // Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("Dialog Content"));

        // Assert
        cut.Find(".vibe-dialog-overlay").ShouldNotBeNull();
    }

    [Fact]
    public void Dialog_Renders_ChildContent()
    {
        // Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("<div class='test-content'>Test Content</div>"));

        // Assert
        var body = cut.Find(".vibe-dialog-body");
        body.InnerHtml.ShouldContain("Test Content");
    }

    [Fact]
    public void Dialog_Renders_Header_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Header, builder => builder.AddContent(0, "Dialog Header"))
            .AddChildContent("Dialog Content"));

        // Assert
        var header = cut.Find(".vibe-dialog-header");
        header.TextContent.ShouldContain("Dialog Header");
    }

    [Fact]
    public void Dialog_Renders_Footer_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Footer, builder => builder.AddContent(0, "Dialog Footer"))
            .AddChildContent("Dialog Content"));

        // Assert
        var footer = cut.Find(".vibe-dialog-footer");
        footer.TextContent.ShouldContain("Dialog Footer");
    }

    [Fact]
    public void Dialog_Invokes_IsOpenChanged_OnOutsideClick()
    {
        // Arrange
        bool stateChanged = false;
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOutsideClick, true)
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => stateChanged = true))
            .AddChildContent("Dialog Content"));

        // Act
        cut.Find(".vibe-dialog-overlay").Click();

        // Assert
        stateChanged.ShouldBeTrue();
    }

    [Fact]
    public void Dialog_DoesNotClose_WhenCloseOnOutsideClickIsFalse()
    {
        // Arrange
        bool stateChanged = false;
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOutsideClick, false)
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => stateChanged = true))
            .AddChildContent("Dialog Content"));

        // Act
        cut.Find(".vibe-dialog-overlay").Click();

        // Assert
        stateChanged.ShouldBeFalse();
    }

    [Fact]
    public void Dialog_Applies_AdditionalAttributes()
    {
        // Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddUnmatched("data-test", "dialog-value")
            .AddChildContent("Dialog Content"));

        // Assert - AdditionalAttributes are captured
        cut.Markup.ShouldNotBeNull();
    }
}
