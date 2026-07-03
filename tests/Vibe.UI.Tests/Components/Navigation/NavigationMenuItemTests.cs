namespace Vibe.UI.Tests.Components.Navigation;

public class NavigationMenuItemTests : TestBase
{
    [Fact]
    public void NavigationMenuItem_RendersTriggerAndBaseClass()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Id, "products")
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products")));

        var item = cut.Find(".vibe-navigation-menu-item");
        item.GetAttribute("id").ShouldBe("products");
        cut.Find(".navigation-menu-item-trigger").TextContent.ShouldBe("Products");
    }

    [Fact]
    public void NavigationMenuItem_GeneratesId_WhenIdIsEmpty()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Id, string.Empty)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products")));

        cut.Find(".vibe-navigation-menu-item").GetAttribute("id").ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void NavigationMenuItem_AppliesDisabledAttribute()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products")));

        cut.Find(".navigation-menu-item-trigger").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void NavigationMenuItem_ClickInvokesCallback_WhenEnabled()
    {
        var clicked = false;
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.OnClick, _ => clicked = true));

        cut.Find(".navigation-menu-item-trigger").Click();

        clicked.ShouldBeTrue();
    }

    [Fact]
    public void NavigationMenuItem_ClickDoesNotInvokeCallback_WhenDisabled()
    {
        var clicked = false;
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.OnClick, _ => clicked = true));

        cut.Find(".navigation-menu-item-trigger").Click();

        clicked.ShouldBeFalse();
    }

    [Fact]
    public void NavigationMenuItem_ShowsContentOnMouseEnter()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<section>Product menu</section>")));

        cut.Find(".vibe-navigation-menu-item").TriggerEvent(
            "onmouseenter",
            new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        cut.Find(".navigation-menu-item-content").TextContent.ShouldContain("Product menu");
    }

    [Fact]
    public void NavigationMenuItem_HidesContentOnMouseLeave()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<section>Product menu</section>")));

        cut.Find(".vibe-navigation-menu-item").TriggerEvent(
            "onmouseenter",
            new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
        cut.Find(".vibe-navigation-menu-item").TriggerEvent(
            "onmouseleave",
            new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }

    [Fact]
    public void NavigationMenuItem_DoesNotShowContentOnMouseEnter_WhenDisabled()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<section>Product menu</section>")));

        cut.Find(".vibe-navigation-menu-item").TriggerEvent(
            "onmouseenter",
            new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }

    [Fact]
    public void NavigationMenuItem_AppliesCustomClass()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Class, "nav-product-item"));

        cut.Find(".vibe-navigation-menu-item").ClassList.ShouldContain("nav-product-item");
    }
}
