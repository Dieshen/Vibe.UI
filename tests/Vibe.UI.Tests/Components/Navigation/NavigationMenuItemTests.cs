using Microsoft.AspNetCore.Components.Web;

namespace Vibe.UI.Tests.Components.Navigation;

public class NavigationMenuItemTests : TestBase
{
    [Fact]
    public void NavigationMenuItem_RendersAccessibleClosedState()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Id, " products ")
            .Add(p => p.Class, "nav-product-item")
            .Add(p => p.TriggerAriaLabel, " Products menu ")
            .Add(p => p.ContentAriaLabel, " Product sections ")
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<section>Product menu</section>"))
            .AddUnmatched("data-testid", "nav-item"));

        var item = cut.Find(".vibe-navigation-menu-item");
        item.GetAttribute("id").ShouldBe("products");
        item.GetAttribute("role").ShouldBe("none");
        item.GetAttribute("data-state").ShouldBe("closed");
        item.GetAttribute("aria-disabled").ShouldBe("false");
        item.GetAttribute("data-testid").ShouldBe("nav-item");
        item.ClassList.ShouldContain("navigation-menu-item-has-content");
        item.ClassList.ShouldContain("nav-product-item");

        var trigger = cut.Find(".navigation-menu-item-trigger");
        trigger.GetAttribute("type").ShouldBe("button");
        trigger.TextContent.ShouldBe("Products");
        trigger.GetAttribute("aria-label").ShouldBe("Products menu");
        trigger.GetAttribute("aria-disabled").ShouldBe("false");
        trigger.GetAttribute("aria-haspopup").ShouldBe("true");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-controls").ShouldBe("products-content");
        JSInterop.Invocations.ShouldContain(invocation =>
            invocation.Identifier == "import" &&
            invocation.Arguments[0] != null &&
            invocation.Arguments[0]!.ToString() == "./_content/Vibe.UI/js/vibe-menu-keyboard.js");

        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NavigationMenuItem_GeneratesStableId_WhenIdIsMissing(string id)
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Id, id)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products")));

        var generatedId = cut.Find(".vibe-navigation-menu-item").GetAttribute("id");
        generatedId.ShouldNotBeNullOrWhiteSpace();
        generatedId!.ShouldStartWith("vibe-navigation-menu-item-");
    }

    [Fact]
    public void NavigationMenuItem_RendersFallbackTrigger_WhenTriggerContentIsNull()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.FallbackTriggerText, "Browse")
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        var fallback = cut.Find(".navigation-menu-item-trigger-fallback");
        fallback.TextContent.ShouldBe("Browse");
        cut.Find(".navigation-menu-item-trigger").GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void NavigationMenuItem_ClickInvokesCallbackAndTogglesContent_WhenEnabled()
    {
        var clicked = 0;
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Id, "products")
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.ContentAriaLabel, "Products panel")
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<section>Product menu</section>"))
            .Add(p => p.OnClick, _ => clicked++));

        cut.Find(".navigation-menu-item-trigger").Click();

        clicked.ShouldBe(1);
        var item = cut.Find(".vibe-navigation-menu-item");
        item.GetAttribute("data-state").ShouldBe("open");
        item.ClassList.ShouldContain("navigation-menu-item-open");

        var content = cut.Find(".navigation-menu-item-content");
        content.GetAttribute("id").ShouldBe("products-content");
        content.GetAttribute("role").ShouldBe("region");
        content.GetAttribute("aria-label").ShouldBe("Products panel");
        content.GetAttribute("data-state").ShouldBe("open");
        content.TextContent.ShouldContain("Product menu");
        cut.Find(".navigation-menu-item-trigger").GetAttribute("aria-expanded").ShouldBe("true");

        cut.Find(".navigation-menu-item-trigger").Click();

        clicked.ShouldBe(2);
        cut.Find(".vibe-navigation-menu-item").GetAttribute("data-state").ShouldBe("closed");
        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }

    [Fact]
    public void NavigationMenuItem_OpenOnClickFalseStillInvokesCallbackWithoutOpeningContent()
    {
        var clicked = false;
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.OpenOnClick, false)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Product menu"))
            .Add(p => p.OnClick, _ => clicked = true));

        cut.Find(".navigation-menu-item-trigger").Click();

        clicked.ShouldBeTrue();
        cut.Find(".navigation-menu-item-trigger").GetAttribute("aria-expanded").ShouldBe("false");
        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }

    [Fact]
    public void NavigationMenuItem_HoverOpensAndClosesContent()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<section>Product menu</section>")));

        var item = cut.Find(".vibe-navigation-menu-item");

        item.TriggerEvent("onmouseenter", new MouseEventArgs());
        cut.Find(".navigation-menu-item-content").TextContent.ShouldContain("Product menu");
        cut.Find(".navigation-menu-item-trigger").GetAttribute("aria-expanded").ShouldBe("true");

        item.TriggerEvent("onmouseleave", new MouseEventArgs());
        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
        cut.Find(".navigation-menu-item-trigger").GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void NavigationMenuItem_KeyboardOpensAndClosesDisclosure()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<section>Product menu</section>")));

        cut.Find(".navigation-menu-item-trigger").KeyDown("ArrowDown");

        cut.Find(".navigation-menu-item-content").ShouldNotBeNull();
        cut.Find(".navigation-menu-item-content").GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".navigation-menu-item-trigger").GetAttribute("aria-expanded").ShouldBe("true");
        JSInterop.Invocations.ShouldContain(invocation => invocation.Identifier == "focusFirstItem");

        cut.Find(".navigation-menu-item-content").KeyDown("Escape");

        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();

        cut.Find(".navigation-menu-item-trigger").KeyDown("ArrowUp");
        JSInterop.Invocations.ShouldContain(invocation => invocation.Identifier == "focusLastItem");
        cut.Find(".navigation-menu-item-trigger").KeyDown("Escape");

        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }

    [Fact]
    public void NavigationMenuItem_KeyboardActivationFocusesFirstContentControl()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<a href=\"/one\">One</a>")));

        var trigger = cut.Find(".navigation-menu-item-trigger");
        trigger.KeyDown("Enter");
        trigger.Click();

        cut.Find(".navigation-menu-item-content").ShouldNotBeNull();
        JSInterop.Invocations.ShouldContain(invocation => invocation.Identifier == "focusFirstItem");
    }

    [Fact]
    public void NavigationMenuItem_DelegatesContentNavigationAndTypeahead()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0,
                "<a href=\"/alpha\">Alpha</a><a href=\"/beta\">Beta</a>")));

        cut.Find(".navigation-menu-item-trigger").KeyDown("ArrowDown");
        var content = cut.Find(".navigation-menu-item-content");

        content.KeyDown("ArrowDown");
        content.KeyDown("ArrowUp");
        content.KeyDown("Home");
        content.KeyDown("End");
        content.KeyDown("b");

        JSInterop.Invocations.Count(invocation => invocation.Identifier == "moveFocus").ShouldBe(2);
        JSInterop.Invocations.Count(invocation => invocation.Identifier == "focusFirstItem").ShouldBeGreaterThanOrEqualTo(2);
        JSInterop.Invocations.ShouldContain(invocation => invocation.Identifier == "focusLastItem");
        JSInterop.Invocations.ShouldContain(invocation => invocation.Identifier == "focusByTypeahead");
    }

    [Fact]
    public void NavigationMenuItem_DoesNotRerouteKeysFromEditableContent()
    {
        var module = JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-menu-keyboard.js");
        module.Setup<bool>("isActiveElementEditable", _ => true).SetResult(true);

        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<input aria-label=\"Search products\" />")));

        cut.Find(".navigation-menu-item-trigger").KeyDown("ArrowDown");
        var moveCount = JSInterop.Invocations.Count(invocation => invocation.Identifier == "moveFocus");

        cut.Find(".navigation-menu-item-content").KeyDown("ArrowDown");
        cut.Find(".navigation-menu-item-content").KeyDown("a");

        JSInterop.Invocations.Count(invocation => invocation.Identifier == "moveFocus").ShouldBe(moveCount);
        JSInterop.Invocations.ShouldNotContain(invocation => invocation.Identifier == "focusByTypeahead");
    }

    [Fact]
    public void NavigationMenuItem_DisabledStateSuppressesMouseClickAndKeyboardInteraction()
    {
        var clicked = false;
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<section>Product menu</section>"))
            .Add(p => p.OnClick, _ => clicked = true));

        var item = cut.Find(".vibe-navigation-menu-item");
        item.GetAttribute("aria-disabled").ShouldBe("true");
        item.ClassList.ShouldContain("navigation-menu-item-disabled");

        var trigger = cut.Find(".navigation-menu-item-trigger");
        trigger.HasAttribute("disabled").ShouldBeTrue();
        trigger.GetAttribute("aria-disabled").ShouldBe("true");

        trigger.Click();
        trigger.KeyDown("ArrowDown");
        item.TriggerEvent("onmouseenter", new MouseEventArgs());

        clicked.ShouldBeFalse();
        cut.Find(".vibe-navigation-menu-item").GetAttribute("data-state").ShouldBe("closed");
        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }

    [Fact]
    public void NavigationMenuItem_WithoutContentOmitsDisclosureAttributesAndStillInvokesClick()
    {
        var clicked = false;
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.OnClick, _ => clicked = true));

        var trigger = cut.Find(".navigation-menu-item-trigger");
        trigger.GetAttribute("aria-haspopup").ShouldBeNull();
        trigger.GetAttribute("aria-expanded").ShouldBeNull();
        trigger.GetAttribute("aria-controls").ShouldBeNull();

        trigger.Click();
        trigger.KeyDown("ArrowDown");

        clicked.ShouldBeTrue();
        cut.Find(".vibe-navigation-menu-item").GetAttribute("data-state").ShouldBe("closed");
        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }

    [Fact]
    public void NavigationMenuItem_UseViewportWithoutParentMenu_DoesNotThrow()
    {
        var cut = Render<NavigationMenuItem>(parameters => parameters
            .Add(p => p.UseViewport, true)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Products"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Product menu")));

        var item = cut.Find(".vibe-navigation-menu-item");

        item.TriggerEvent("onmouseenter", new MouseEventArgs());
        cut.Find(".navigation-menu-item-content").TextContent.ShouldBe("Product menu");

        item.TriggerEvent("onmouseleave", new MouseEventArgs());
        cut.FindAll(".navigation-menu-item-content").ShouldBeEmpty();
    }
}
