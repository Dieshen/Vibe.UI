namespace Vibe.UI.Tests.Components.Navigation;

public class SidebarTests : TestBase
{
    [Fact]
    public void Sidebar_Renders_WithAccessibleDefaultProps()
    {
        // Act
        var cut = Render<Sidebar>(parameters => parameters
            .AddChildContent("Sidebar Content"));

        // Assert
        var sidebar = cut.Find(".vibe-sidebar");
        sidebar.TagName.ShouldBe("ASIDE");
        sidebar.GetAttribute("role").ShouldBe("complementary");
        sidebar.GetAttribute("aria-label").ShouldBe("Sidebar");
        sidebar.GetAttribute("aria-disabled").ShouldBe("false");
        sidebar.GetAttribute("data-state").ShouldBe("open");
        sidebar.GetAttribute("style").ShouldBe("width: 280px;");
        sidebar.ClassList.ShouldContain("sidebar-open");

        var content = cut.Find(".sidebar-content");
        content.GetAttribute("role").ShouldBe("region");
        content.GetAttribute("aria-label").ShouldBe("Sidebar content");
        content.TextContent.ShouldContain("Sidebar Content");
    }

    [Fact]
    public void Sidebar_UsesTitleAsAccessibleName_WhenAriaLabelMissing()
    {
        // Act
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.Title, "Project navigation")
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-sidebar").GetAttribute("aria-label").ShouldBe("Project navigation");
        cut.Find(".sidebar-title").TextContent.ShouldBe("Project navigation");
    }

    [Fact]
    public void Sidebar_UsesExplicitAriaLabels()
    {
        // Act
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.AriaLabel, "Workspace sidebar")
            .Add(p => p.ContentAriaLabel, "Workspace links")
            .Add(p => p.ResizeHandleAriaLabel, "Resize workspace sidebar")
            .Add(p => p.Resizable, true)
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-sidebar").GetAttribute("aria-label").ShouldBe("Workspace sidebar");
        cut.Find(".sidebar-content").GetAttribute("aria-label").ShouldBe("Workspace links");
        cut.Find(".sidebar-resize-handle").GetAttribute("aria-label").ShouldBe("Resize workspace sidebar");
    }

    [Fact]
    public void Sidebar_Renders_CustomHeaderAndFooter()
    {
        // Act
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.Header, builder => builder.AddMarkupContent(0, "<strong>Custom Header</strong>"))
            .Add(p => p.Footer, builder => builder.AddMarkupContent(0, "<span>Footer Content</span>"))
            .AddChildContent("Content"));

        // Assert
        cut.Find(".sidebar-header").TextContent.ShouldContain("Custom Header");
        cut.Find(".sidebar-footer").TextContent.ShouldContain("Footer Content");
    }

    [Fact]
    public void Sidebar_HidesHeaderAndToggle_WhenShowHeaderFalse()
    {
        // Act
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.ShowHeader, false)
            .AddChildContent("Content"));

        // Assert
        cut.FindAll(".sidebar-header").ShouldBeEmpty();
        cut.FindAll(".sidebar-toggle").ShouldBeEmpty();
    }

    [Fact]
    public void Sidebar_RendersToggleButton_WhenCollapsible()
    {
        // Act
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.Collapsible, true)
            .AddChildContent("Content"));

        // Assert
        var toggle = cut.Find(".sidebar-toggle");
        toggle.GetAttribute("type").ShouldBe("button");
        toggle.GetAttribute("aria-label").ShouldBe("Collapse sidebar");
        toggle.GetAttribute("aria-expanded").ShouldBe("true");
        toggle.GetAttribute("aria-controls").ShouldBe(cut.Find(".vibe-sidebar").GetAttribute("id"));
        toggle.GetAttribute("aria-disabled").ShouldBe("false");
        toggle.QuerySelector(".sidebar-toggle-icon")!.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void Sidebar_ToggleButton_ChangesStateAndInvokesCallback()
    {
        // Arrange
        var changedValues = new List<bool>();
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.IsOpenChanged, changedValues.Add)
            .AddChildContent("Content"));

        // Act
        cut.Find(".sidebar-toggle").Click();

        // Assert
        changedValues.ShouldBe(new[] { false });
        cut.Instance.IsOpen.ShouldBeFalse();
        var sidebar = cut.Find(".vibe-sidebar");
        sidebar.ClassList.ShouldContain("sidebar-closed");
        sidebar.GetAttribute("data-state").ShouldBe("closed");
        sidebar.GetAttribute("style").ShouldBe("width: 60px;");
        cut.Find(".sidebar-toggle").GetAttribute("aria-label").ShouldBe("Expand sidebar");
        cut.Find(".sidebar-toggle").GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public async Task Sidebar_PublicOpenCloseMethods_OnlyInvokeOnStateChange()
    {
        // Arrange
        var changedValues = new List<bool>();
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.IsOpen, false)
            .Add(p => p.IsOpenChanged, changedValues.Add)
            .AddChildContent("Content"));

        // Act
        await cut.Instance.Close();
        await cut.Instance.Open();
        await cut.Instance.Open();

        // Assert
        changedValues.ShouldBe(new[] { true });
        cut.Instance.IsOpen.ShouldBeTrue();
        cut.Find(".vibe-sidebar").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact]
    public void Sidebar_EscapeKeyCloses_WhenCollapsibleAndOpen()
    {
        // Arrange
        var changedValues = new List<bool>();
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.IsOpenChanged, changedValues.Add)
            .AddChildContent("Content"));

        // Act
        cut.Find(".vibe-sidebar").KeyDown("Escape");

        // Assert
        changedValues.ShouldBe(new[] { false });
        cut.Find(".vibe-sidebar").GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact]
    public void Sidebar_DoesNotToggle_WhenNotCollapsible()
    {
        // Arrange
        var changed = false;
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.Collapsible, false)
            .Add(p => p.IsOpenChanged, _ => changed = true)
            .AddChildContent("Content"));

        // Act
        cut.Instance.Toggle();
        cut.Find(".vibe-sidebar").KeyDown("Escape");

        // Assert
        changed.ShouldBeFalse();
        cut.Instance.IsOpen.ShouldBeTrue();
        cut.FindAll(".sidebar-toggle").ShouldBeEmpty();
    }

    [Fact]
    public void Sidebar_DisabledState_DisablesToggleAndPublicMutators()
    {
        // Arrange
        var changed = false;
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.IsOpenChanged, _ => changed = true)
            .AddChildContent("Content"));

        // Act
        cut.Find(".sidebar-toggle").Click();
        cut.Instance.Close();
        cut.Find(".vibe-sidebar").KeyDown("Escape");

        // Assert
        changed.ShouldBeFalse();
        var sidebar = cut.Find(".vibe-sidebar");
        sidebar.ClassList.ShouldContain("sidebar-disabled");
        sidebar.GetAttribute("aria-disabled").ShouldBe("true");
        sidebar.GetAttribute("data-state").ShouldBe("open");

        var toggle = cut.Find(".sidebar-toggle");
        toggle.HasAttribute("disabled").ShouldBeTrue();
        toggle.GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact]
    public void Sidebar_AppliesPositionResizableAndCustomClasses()
    {
        // Act
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.Position, Sidebar.SidebarPosition.Right)
            .Add(p => p.Resizable, true)
            .Add(p => p.CssClass, "legacy-sidebar")
            .Add(p => p.Class, "base-sidebar")
            .AddUnmatched("data-testid", "sidebar")
            .AddChildContent("Content"));

        // Assert
        var sidebar = cut.Find(".vibe-sidebar");
        sidebar.ClassList.ShouldContain("sidebar-right");
        sidebar.ClassList.ShouldContain("sidebar-resizable");
        sidebar.ClassList.ShouldContain("legacy-sidebar");
        sidebar.ClassList.ShouldContain("base-sidebar");
        sidebar.GetAttribute("data-testid").ShouldBe("sidebar");

        var resize = cut.Find(".sidebar-resize-handle");
        resize.GetAttribute("role").ShouldBe("separator");
        resize.GetAttribute("aria-orientation").ShouldBe("vertical");
        resize.GetAttribute("aria-disabled").ShouldBe("false");
    }

    [Fact]
    public void Sidebar_ClampsOpenAndCollapsedWidths()
    {
        // Act
        var open = Render<Sidebar>(parameters => parameters
            .Add(p => p.Width, 999)
            .Add(p => p.MinWidth, 100)
            .Add(p => p.MaxWidth, 320)
            .AddChildContent("Content"));

        var closed = Render<Sidebar>(parameters => parameters
            .Add(p => p.IsOpen, false)
            .Add(p => p.Width, 320)
            .Add(p => p.CollapsedWidth, 500)
            .AddChildContent("Content"));

        var invalidBounds = Render<Sidebar>(parameters => parameters
            .Add(p => p.Width, 400)
            .Add(p => p.MinWidth, 500)
            .Add(p => p.MaxWidth, 200)
            .AddChildContent("Content"));

        // Assert
        open.Find(".vibe-sidebar").GetAttribute("style").ShouldBe("width: 320px;");
        closed.Find(".vibe-sidebar").GetAttribute("style").ShouldBe("width: 320px;");
        invalidBounds.Find(".vibe-sidebar").GetAttribute("style").ShouldBe("width: 500px;");
    }

    [Fact]
    public void Sidebar_ParameterRerender_UpdatesState()
    {
        // Arrange
        var cut = Render<Sidebar>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("Content"));

        // Act
        cut.Render(parameters => parameters
            .Add(p => p.IsOpen, false)
            .AddChildContent("Content"));

        // Assert
        cut.Find(".vibe-sidebar").ClassList.ShouldContain("sidebar-closed");
        cut.Find(".vibe-sidebar").GetAttribute("data-state").ShouldBe("closed");
        cut.Find(".sidebar-toggle").GetAttribute("aria-expanded").ShouldBe("false");
    }
}
