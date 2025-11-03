namespace Vibe.UI.Tests.Components.Overlay;

public class DrawerTests : TestBase
{
    [Fact]
    public void Drawer_DoesNotRender_WhenClosed()
    {
        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, false));

        // Assert
        cut.FindAll(".vibe-drawer").ShouldBeEmpty();
    }

    [Fact]
    public void Drawer_Renders_WhenOpen()
    {
        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var drawer = cut.Find(".vibe-drawer");
        drawer.ShouldNotBeNull();
    }

    [Fact]
    public void Drawer_Displays_ChildContent()
    {
        // Arrange
        var content = "Drawer Content";

        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var body = cut.Find(".drawer-body");
        body.TextContent.ShouldContain(content);
    }

    [Fact]
    public void Drawer_Applies_SideClass()
    {
        // Arrange
        var side = "left";

        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Side, side));

        // Assert
        var drawer = cut.Find(".vibe-drawer");
        drawer.ClassList.ShouldContain($"drawer-{side}");
    }

    [Fact]
    public void Drawer_Applies_DefaultSide()
    {
        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var drawer = cut.Find(".vibe-drawer");
        drawer.ClassList.ShouldContain("drawer-right");
    }

    [Fact]
    public void Drawer_Shows_CloseButton_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, true));

        // Assert
        var closeButton = cut.Find(".drawer-close");
        closeButton.ShouldNotBeNull();
    }

    [Fact]
    public void Drawer_Hides_CloseButton_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, false));

        // Assert
        cut.FindAll(".drawer-close").ShouldBeEmpty();
    }

    [Fact]
    public void Drawer_Renders_Overlay()
    {
        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var overlay = cut.Find(".drawer-overlay");
        overlay.ShouldNotBeNull();
    }

    [Fact]
    public void Drawer_Renders_Content()
    {
        // Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var content = cut.Find(".drawer-content");
        content.ShouldNotBeNull();
    }
}
