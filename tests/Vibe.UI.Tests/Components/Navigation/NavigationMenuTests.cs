namespace Vibe.UI.Tests.Components.Navigation;

public class NavigationMenuTests : TestBase
{
    [Fact]
    public void NavigationMenu_RendersBaseStructure()
    {
        var cut = RenderComponent<NavigationMenu>();

        cut.Find("nav.vibe-navigation-menu").ShouldNotBeNull();
        cut.Find(".navigation-menu-list").ShouldNotBeNull();
    }

    [Fact]
    public void NavigationMenu_RendersChildContent()
    {
        var cut = RenderComponent<NavigationMenu>(parameters => parameters
            .AddChildContent("<button>Products</button>"));

        cut.Find(".navigation-menu-list button").TextContent.ShouldBe("Products");
    }

    [Fact]
    public void NavigationMenu_RendersViewportContent()
    {
        var cut = RenderComponent<NavigationMenu>(parameters => parameters
            .Add(p => p.ViewportContent, builder => builder.AddMarkupContent(0, "<section>Viewport</section>")));

        cut.Find(".navigation-menu-viewport").TextContent.ShouldContain("Viewport");
    }

    [Fact]
    public void NavigationMenu_DoesNotRenderViewport_WhenContentIsNull()
    {
        var cut = RenderComponent<NavigationMenu>();

        cut.FindAll(".navigation-menu-viewport").ShouldBeEmpty();
    }

    [Fact]
    public async Task NavigationMenu_RendersViewportIndicator_ForRegisteredActiveItem()
    {
        var cut = RenderComponent<NavigationMenu>();

        await cut.InvokeAsync(() =>
        {
            cut.Instance.RegisterItem("products", 12, 64);
            cut.Instance.ActivateItem("products");
        });

        var indicator = cut.Find(".navigation-menu-viewport-indicator");
        indicator.GetAttribute("style").ShouldBe("left: 12px; width: 64px");
    }

    [Fact]
    public async Task NavigationMenu_DoesNotRenderViewportIndicator_ForUnregisteredActiveItem()
    {
        var cut = RenderComponent<NavigationMenu>();

        await cut.InvokeAsync(() => cut.Instance.ActivateItem("missing"));

        cut.FindAll(".navigation-menu-viewport-indicator").ShouldBeEmpty();
    }

    [Fact]
    public async Task NavigationMenu_DeactivateItemHidesViewportIndicator()
    {
        var cut = RenderComponent<NavigationMenu>();

        await cut.InvokeAsync(() =>
        {
            cut.Instance.RegisterItem("products", 12, 64);
            cut.Instance.ActivateItem("products");
            cut.Instance.DeactivateItem();
        });

        cut.FindAll(".navigation-menu-viewport-indicator").ShouldBeEmpty();
    }

    [Fact]
    public void NavigationMenu_AppliesCustomClass()
    {
        var cut = RenderComponent<NavigationMenu>(parameters => parameters
            .Add(p => p.Class, "main-navigation"));

        cut.Find(".vibe-navigation-menu").ClassList.ShouldContain("main-navigation");
    }
}
