namespace Vibe.UI.Tests.Components.Navigation;

public class LinkTests : TestBase
{
    [Fact]
    public void Link_RendersAnchor_WhenHrefProvided()
    {
        var cut = RenderComponent<Link>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .AddChildContent("Docs"));

        var link = cut.Find("a.vibe-link");
        link.GetAttribute("href").ShouldBe("/docs");
        link.TextContent.ShouldBe("Docs");
    }

    [Fact]
    public void Link_AddsSafeRel_ForExternalBlankTarget()
    {
        var cut = RenderComponent<Link>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Target, "_blank")
            .AddChildContent("External"));

        var link = cut.Find("a.vibe-link");
        link.GetAttribute("target").ShouldBe("_blank");
        link.GetAttribute("rel").ShouldBe("noopener noreferrer");
    }

    [Fact]
    public void Link_RendersButton_WhenHrefMissing()
    {
        var cut = RenderComponent<Link>(parameters => parameters
            .AddChildContent("Action"));

        var button = cut.Find("button.vibe-link-button");
        button.GetAttribute("type").ShouldBe("button");
        button.TextContent.ShouldBe("Action");
    }

    [Fact]
    public void Link_RendersDisabledSpanAndSuppressesClick()
    {
        var cut = RenderComponent<Link>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.Href, "/disabled")
            .AddChildContent("Disabled"));

        var link = cut.Find("span.vibe-link-disabled");
        link.GetAttribute("aria-disabled").ShouldBe("true");
        link.GetAttribute("href").ShouldBeNull();
        cut.FindAll("a").ShouldBeEmpty();
        cut.FindAll("button").ShouldBeEmpty();
    }

    [Fact]
    public void Link_InvokesClickCallback_WhenEnabled()
    {
        var clicked = false;
        var cut = RenderComponent<Link>(parameters => parameters
            .Add(p => p.OnClick, EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, _ => clicked = true))
            .AddChildContent("Click"));

        cut.Find("button").Click();

        clicked.ShouldBeTrue();
    }

    [Fact]
    public void Link_AppliesVariantUnderlineClassAndAttributes()
    {
        var cut = RenderComponent<Link>(parameters => parameters
            .Add(p => p.Href, "/primary")
            .Add(p => p.Variant, Link.LinkVariant.Primary)
            .Add(p => p.Underline, Link.LinkUnderline.Always)
            .Add(p => p.Class, "nav-link")
            .AddUnmatched("data-link", "primary")
            .AddChildContent("Primary"));

        var link = cut.Find("a.vibe-link");
        link.ClassList.ShouldContain("vibe-link-primary");
        link.ClassList.ShouldContain("vibe-link-underline-always");
        link.ClassList.ShouldContain("nav-link");
        link.GetAttribute("data-link").ShouldBe("primary");
    }
}
