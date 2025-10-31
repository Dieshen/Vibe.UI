namespace Vibe.UI.Tests.Components.Navigation;

public class SidebarTests : TestContext
{
    public SidebarTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Sidebar_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>();

        // Assert
        cut.Find(".vibe-sidebar").Should().NotBeNull();
        cut.Find("aside").Should().NotBeNull();
        cut.Find(".sidebar-content").Should().NotBeNull();
    }

    [Fact]
    public void Sidebar_IsOpen_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>();

        // Assert
        cut.Find(".vibe-sidebar").GetAttribute("data-state").Should().Be("open");
        cut.Find(".vibe-sidebar").ClassList.Should().Contain("sidebar-open");
    }

    [Fact]
    public void Sidebar_ShowsHeader_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Title, "Navigation"));

        // Assert
        cut.Find(".sidebar-header").Should().NotBeNull();
        cut.Find(".sidebar-title").TextContent.Should().Be("Navigation");
    }

    [Fact]
    public void Sidebar_ShowsToggleButton_WhenCollapsible()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Collapsible, true));

        // Assert
        cut.Find(".sidebar-toggle").Should().NotBeNull();
    }

    [Fact]
    public void Sidebar_TogglesOpenState_OnButtonClick()
    {
        // Arrange
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Collapsible, true));

        // Act
        cut.Find(".sidebar-toggle").Click();

        // Assert
        cut.Instance.IsOpen.Should().BeFalse();
        cut.Find(".vibe-sidebar").ClassList.Should().Contain("sidebar-closed");
    }

    [Fact]
    public void Sidebar_AppliesWidthStyle_WhenOpen()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Width, 300));

        // Assert
        var style = cut.Find("aside").GetAttribute("style");
        style.Should().Contain("width: 300px");
    }

    [Fact]
    public void Sidebar_AppliesCollapsedWidth_WhenClosed()
    {
        // Arrange
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.IsOpen, false)
            .Add(p => p.CollapsedWidth, 60));

        // Assert
        var style = cut.Find("aside").GetAttribute("style");
        style.Should().Contain("width: 60px");
    }

    [Fact]
    public void Sidebar_AppliesLeftPosition_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>();

        // Assert
        cut.Find(".vibe-sidebar").ClassList.Should().Contain("sidebar-left");
    }

    [Fact]
    public void Sidebar_AppliesRightPosition_WhenSet()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Position, SidebarPosition.Right));

        // Assert
        cut.Find(".vibe-sidebar").ClassList.Should().Contain("sidebar-right");
    }

    [Fact]
    public void Sidebar_ShowsResizeHandle_WhenResizable()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Resizable, true));

        // Assert
        cut.Find(".sidebar-resize-handle").Should().NotBeNull();
        cut.Find(".vibe-sidebar").ClassList.Should().Contain("sidebar-resizable");
    }

    [Fact]
    public void Sidebar_RendersCustomHeader()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Header, builder => builder.AddContent(0, "<div class='custom-header'>My Header</div>")));

        // Assert
        cut.Markup.Should().Contain("My Header");
    }

    [Fact]
    public void Sidebar_RendersFooter_WhenProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Footer, builder => builder.AddContent(0, "<div>Footer Content</div>")));

        // Assert
        cut.Find(".sidebar-footer").Should().NotBeNull();
        cut.Markup.Should().Contain("Footer Content");
    }

    [Fact]
    public void Sidebar_ClampsWidth_ToMinMax()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Width, 1000)); // Should be clamped to 600

        // Assert
        cut.Instance.Width.Should().Be(600);
    }

    [Fact]
    public void Sidebar_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.CssClass, "custom-sidebar"));

        // Assert
        cut.Find(".vibe-sidebar").ClassList.Should().Contain("custom-sidebar");
    }
}
