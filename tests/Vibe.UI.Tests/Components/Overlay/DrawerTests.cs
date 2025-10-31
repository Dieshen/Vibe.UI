namespace Vibe.UI.Tests.Components.Overlay;

public class DrawerTests : TestContext
{
    public DrawerTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Drawer_DoesNotRender_WhenClosed()
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, false));

        // Assert
        cut.FindAll(".vibe-drawer").Should().BeEmpty();
    }

    [Fact]
    public void Drawer_Renders_WhenOpen()
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("Drawer content"));

        // Assert
        cut.Find(".vibe-drawer").Should().NotBeNull();
        cut.Find(".drawer-overlay").Should().NotBeNull();
        cut.Find(".drawer-content").Should().NotBeNull();
    }

    [Fact]
    public void Drawer_RendersContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("My drawer content"));

        // Assert
        cut.Find(".drawer-body").TextContent.Should().Contain("My drawer content");
    }

    [Fact]
    public void Drawer_ShowsCloseButton_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        cut.Find(".drawer-close").Should().NotBeNull();
    }

    [Fact]
    public void Drawer_HidesCloseButton_WhenDisabled()
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, false));

        // Assert
        cut.FindAll(".drawer-close").Should().BeEmpty();
    }

    [Fact]
    public void Drawer_AppliesRightSide_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        cut.Find(".vibe-drawer").ClassList.Should().Contain("drawer-right");
    }

    [Theory]
    [InlineData("left", "drawer-left")]
    [InlineData("right", "drawer-right")]
    [InlineData("top", "drawer-top")]
    [InlineData("bottom", "drawer-bottom")]
    public void Drawer_AppliesCorrectSideClass(string side, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Side, side));

        // Assert
        cut.Find(".vibe-drawer").ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void Drawer_SupportsCloseOnOverlayClick()
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOverlayClick, true));

        // Assert
        cut.Instance.CloseOnOverlayClick.Should().BeTrue();
    }

    [Fact]
    public void Drawer_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CssClass, "custom-drawer"));

        // Assert
        cut.Find(".vibe-drawer").ClassList.Should().Contain("custom-drawer");
    }
}
