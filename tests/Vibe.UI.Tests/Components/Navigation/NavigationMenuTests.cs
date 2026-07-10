namespace Vibe.UI.Tests.Components.Navigation;

public class NavigationMenuTests : TestBase
{
    [Fact]
    public void NavigationMenu_RendersBaseStructure()
    {
        var cut = Render<NavigationMenu>();

        var nav = cut.Find("nav.vibe-navigation-menu");
        nav.GetAttribute("aria-label").ShouldBe("Navigation menu");

        cut.Find(".navigation-menu-list").GetAttribute("role").ShouldBe("list");
    }

    [Fact]
    public void NavigationMenu_RendersChildContent()
    {
        var cut = Render<NavigationMenu>(parameters => parameters
            .AddChildContent("<button>Products</button>"));

        cut.Find(".navigation-menu-list button").TextContent.ShouldBe("Products");
    }

    [Fact]
    public void NavigationMenu_RendersViewportContent()
    {
        var cut = Render<NavigationMenu>(parameters => parameters
            .Add(p => p.ViewportAriaLabel, " Product navigation viewport ")
            .Add(p => p.ViewportContent, builder => builder.AddMarkupContent(0, "<section>Viewport</section>")));

        var viewport = cut.Find(".navigation-menu-viewport");
        viewport.GetAttribute("role").ShouldBe("region");
        viewport.GetAttribute("aria-label").ShouldBe("Product navigation viewport");
        viewport.TextContent.ShouldContain("Viewport");
    }

    [Fact]
    public void NavigationMenu_DoesNotRenderViewport_WhenContentIsNull()
    {
        var cut = Render<NavigationMenu>();

        cut.FindAll(".navigation-menu-viewport").ShouldBeEmpty();
    }

    [Fact]
    public async Task NavigationMenu_RendersViewportIndicator_ForRegisteredActiveItem()
    {
        var cut = Render<NavigationMenu>();

        await cut.InvokeAsync(() =>
        {
            cut.Instance.RegisterItem("products", 12, 64);
            cut.Instance.ActivateItem("products");
        });

        var indicator = cut.Find(".navigation-menu-viewport-indicator");
        indicator.GetAttribute("style").ShouldBe("left: 12px; width: 64px");
    }

    [Fact]
    public async Task NavigationMenu_TrimsIdsAndClampsNegativeIndicatorPosition()
    {
        var cut = Render<NavigationMenu>();

        await cut.InvokeAsync(() =>
        {
            cut.Instance.RegisterItem(" products ", -12, 64.5);
            cut.Instance.ActivateItem(" products ");
        });

        cut.Find(".navigation-menu-viewport-indicator")
            .GetAttribute("style")
            .ShouldBe("left: 0px; width: 64.5px");
    }

    [Fact]
    public async Task NavigationMenu_DoesNotRenderViewportIndicator_ForUnregisteredActiveItem()
    {
        var cut = Render<NavigationMenu>();

        await cut.InvokeAsync(() => cut.Instance.ActivateItem("missing"));

        cut.FindAll(".navigation-menu-viewport-indicator").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task NavigationMenu_IgnoresBlankItemIds(string id)
    {
        var cut = Render<NavigationMenu>();

        await cut.InvokeAsync(() =>
        {
            cut.Instance.RegisterItem(id, 12, 64);
            cut.Instance.ActivateItem(id);
        });

        cut.FindAll(".navigation-menu-viewport-indicator").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public async Task NavigationMenu_RemovesViewportIndicator_ForInvalidRegisteredWidth(double width)
    {
        var cut = Render<NavigationMenu>();

        await cut.InvokeAsync(() =>
        {
            cut.Instance.RegisterItem("products", 12, 64);
            cut.Instance.ActivateItem("products");
            cut.Instance.RegisterItem("products", 12, width);
        });

        cut.FindAll(".navigation-menu-viewport-indicator").ShouldBeEmpty();
    }

    [Fact]
    public async Task NavigationMenu_DeactivateItemHidesViewportIndicator()
    {
        var cut = Render<NavigationMenu>();

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
        var cut = Render<NavigationMenu>(parameters => parameters
            .Add(p => p.Class, "main-navigation"));

        cut.Find(".vibe-navigation-menu").ClassList.ShouldContain("main-navigation");
    }

    [Fact]
    public void NavigationMenu_PreservesAdditionalAttributesAndCustomAriaLabel()
    {
        var cut = Render<NavigationMenu>(parameters => parameters
            .Add(p => p.AriaLabel, " Product navigation ")
            .AddUnmatched("data-testid", "product-nav"));

        var nav = cut.Find(".vibe-navigation-menu");
        nav.GetAttribute("aria-label").ShouldBe("Product navigation");
        nav.GetAttribute("data-testid").ShouldBe("product-nav");
    }

    [Fact]
    public void NavigationMenu_RendersEmptyList_WhenChildContentIsNull()
    {
        var cut = Render<NavigationMenu>();

        cut.Find(".navigation-menu-list").TextContent.Trim().ShouldBeEmpty();
        cut.FindAll(".navigation-menu-viewport").ShouldBeEmpty();
        cut.FindAll(".navigation-menu-viewport-indicator").ShouldBeEmpty();
    }
}
