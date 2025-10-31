namespace Vibe.UI.Tests.Components.Overlay;

public class DialogTests : TestContext
{
    public DialogTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Dialog_DoesNotRender_WhenClosed()
    {
        // Arrange & Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, false));

        // Assert
        cut.FindAll(".vibe-dialog").Should().BeEmpty();
    }

    [Fact]
    public void Dialog_Renders_WhenOpen()
    {
        // Arrange & Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("Dialog content"));

        // Assert
        cut.Find(".vibe-dialog").Should().NotBeNull();
        cut.Find(".vibe-dialog-overlay").Should().NotBeNull();
    }

    [Fact]
    public void Dialog_AppliesAriaAttributes()
    {
        // Arrange & Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var dialog = cut.Find("[role='dialog']");
        dialog.Should().NotBeNull();
        dialog.GetAttribute("aria-modal").Should().Be("true");
    }

    [Fact]
    public void Dialog_RendersContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("My dialog content"));

        // Assert
        cut.Find(".vibe-dialog-body").TextContent.Should().Contain("My dialog content");
    }

    [Fact]
    public void Dialog_RendersHeader_WhenProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Header, builder => builder.AddContent(0, "Dialog Title")));

        // Assert
        cut.Find(".vibe-dialog-header").Should().NotBeNull();
        cut.Markup.Should().Contain("Dialog Title");
    }

    [Fact]
    public void Dialog_RendersFooter_WhenProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Footer, builder => builder.AddContent(0, "<button>OK</button>")));

        // Assert
        cut.Find(".vibe-dialog-footer").Should().NotBeNull();
        cut.Markup.Should().Contain("<button>OK</button>");
    }

    [Fact]
    public void Dialog_SupportsCloseOnOutsideClick()
    {
        // Arrange & Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOutsideClick, true));

        // Assert
        cut.Instance.CloseOnOutsideClick.Should().BeTrue();
    }

    [Fact]
    public void Dialog_SupportsCloseOnEscape()
    {
        // Arrange & Act
        var cut = RenderComponent<Dialog>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnEscape, true));

        // Assert
        cut.Instance.CloseOnEscape.Should().BeTrue();
    }
}
