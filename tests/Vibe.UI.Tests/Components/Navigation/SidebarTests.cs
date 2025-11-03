namespace Vibe.UI.Tests.Components.Navigation;

public class SidebarTests : TestBase
{
    [Fact]
    public void Sidebar_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .AddChildContent("Sidebar Content"));

        // Assert
        cut.Find(".vibe-sidebar").ShouldNotBeNull();
    }

    [Fact]
    public void Sidebar_IsOpen_ByDefault()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .AddChildContent("Sidebar Content"));

        // Assert
        cut.Instance.IsOpen.ShouldBeTrue();
        cut.Find(".vibe-sidebar").ClassList.ShouldContain("sidebar-open");
    }

    [Fact]
    public void Sidebar_Has_AsideElement()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .AddChildContent("Sidebar Content"));

        // Assert
        cut.Markup.ShouldContain("<aside");
    }

    [Fact]
    public void Sidebar_Renders_ChildContent()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .AddChildContent("<div class='test-content'>Test Content</div>"));

        // Assert
        var content = cut.Find(".sidebar-content");
        content.InnerHtml.ShouldContain("Test Content");
    }

    [Fact]
    public void Sidebar_Renders_Title_InHeader()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Title, "My Sidebar")
            .AddChildContent("Content"));

        // Assert
        var title = cut.Find(".sidebar-title");
        title.TextContent.ShouldBe("My Sidebar");
    }

    [Fact]
    public void Sidebar_Renders_CustomHeader()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Header, builder => builder.AddContent(0, "Custom Header"))
            .AddChildContent("Content"));

        // Assert
        var header = cut.Find(".sidebar-header");
        header.TextContent.ShouldContain("Custom Header");
    }

    [Fact]
    public void Sidebar_Renders_Footer_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Footer, builder => builder.AddContent(0, "Footer Content"))
            .AddChildContent("Content"));

        // Assert
        var footer = cut.Find(".sidebar-footer");
        footer.TextContent.ShouldContain("Footer Content");
    }

    [Fact]
    public void Sidebar_Shows_ToggleButton_WhenCollapsible()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Collapsible, true)
            .AddChildContent("Content"));

        // Assert
        cut.Find(".sidebar-toggle").ShouldNotBeNull();
    }

    [Fact]
    public void Sidebar_Toggles_State()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Collapsible, true)
            .AddChildContent("Content"));

        var initialState = cut.Instance.IsOpen;
        cut.Instance.Toggle();

        // Assert
        cut.Instance.IsOpen.ShouldBe(!initialState);
    }

    [Fact]
    public void Sidebar_Applies_PositionClass()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Position, Sidebar.SidebarPosition.Right)
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-sidebar").ClassList.ShouldContain("sidebar-right");
    }

    [Fact]
    public void Sidebar_Applies_ResizableClass_WhenResizable()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.Resizable, true)
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-sidebar").ClassList.ShouldContain("sidebar-resizable");
    }

    [Fact]
    public void Sidebar_Has_DataState_Attribute()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("Content"));

        // Assert
        var sidebar = cut.Find(".vibe-sidebar");
        sidebar.GetAttribute("data-state").ShouldBe("open");
    }

    [Fact]
    public void Sidebar_Invokes_IsOpenChanged()
    {
        // Arrange
        bool stateChanged = false;
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => stateChanged = true))
            .AddChildContent("Content"));

        // Act
        cut.Instance.Toggle();

        // Assert
        stateChanged.ShouldBeTrue();
    }

    [Fact]
    public void Sidebar_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<Sidebar>(parameters => parameters
            .Add(p => p.CssClass, "custom-sidebar")
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-sidebar").ClassList.ShouldContain("custom-sidebar");
    }
}
