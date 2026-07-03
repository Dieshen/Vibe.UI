namespace Vibe.UI.Tests.Components.Navigation;

public class BreadcrumbItemTests : TestBase
{
    [Fact]
    public void BreadcrumbItem_RendersLinkContentAndSeparator()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .AddChildContent("Home"));

        var item = cut.Find("li.vibe-breadcrumb-item");
        item.ShouldNotBeNull();
        cut.Find("a.breadcrumb-link").TextContent.ShouldBe("Home");
        cut.Find(".breadcrumb-separator").TextContent.Trim().ShouldBe("/");
    }

    [Fact]
    public void BreadcrumbItem_RendersHref()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .AddChildContent("Docs"));

        cut.Find("a.breadcrumb-link").GetAttribute("href").ShouldBe("/docs");
    }

    [Fact]
    public void BreadcrumbItem_AppliesLastClassAndOmitsSeparator_WhenLast()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.IsLast, true)
            .AddChildContent("Current"));

        cut.Find(".vibe-breadcrumb-item").ClassList.ShouldContain("breadcrumb-last");
        cut.FindAll("a.breadcrumb-link").ShouldBeEmpty();
        var current = cut.Find("span.breadcrumb-link");
        current.TextContent.ShouldBe("Current");
        current.GetAttribute("aria-current").ShouldBe("page");
        cut.FindAll(".breadcrumb-separator").ShouldBeEmpty();
    }

    [Fact]
    public void BreadcrumbItem_RendersNavigableLink_WhenNotLast()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .AddChildContent("Docs"));

        var link = cut.Find("a.breadcrumb-link");
        link.GetAttribute("href").ShouldBe("/docs");
        link.HasAttribute("aria-current").ShouldBeFalse();
        cut.FindAll("span.breadcrumb-link").ShouldBeEmpty();
    }

    [Fact]
    public void BreadcrumbItem_RendersCustomSeparator()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.Separator, ">")
            .AddChildContent("Docs"));

        cut.Find(".breadcrumb-separator").TextContent.Trim().ShouldBe(">");
    }

    [Fact]
    public void BreadcrumbItem_RendersSeparatorContentBeforeSeparatorString()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.Separator, ">")
            .Add(p => p.SeparatorContent, builder => builder.AddMarkupContent(0, "<span data-testid='slash'>/</span>"))
            .AddChildContent("Docs"));

        cut.Find(".breadcrumb-separator [data-testid='slash']").ShouldNotBeNull();
        cut.Find(".breadcrumb-separator").TextContent.Trim().ShouldBe("/");
    }

    [Fact]
    public void BreadcrumbItem_OmitsSeparator_WhenSeparatorIsEmpty()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.Separator, string.Empty)
            .AddChildContent("Docs"));

        cut.FindAll(".breadcrumb-separator").ShouldBeEmpty();
    }

    [Fact]
    public void BreadcrumbItem_InvokesOnClick()
    {
        var clicked = false;
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.OnClick, () => clicked = true)
            .AddChildContent("Docs"));

        cut.Find("a.breadcrumb-link").Click();

        clicked.ShouldBeTrue();
    }

    [Fact]
    public void BreadcrumbItem_PreservesAdditionalAttributes()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "breadcrumb-item",
                ["aria-current"] = "page"
            })
            .AddChildContent("Current"));

        var item = cut.Find(".vibe-breadcrumb-item");
        item.GetAttribute("data-testid").ShouldBe("breadcrumb-item");
        item.GetAttribute("aria-current").ShouldBe("page");
    }

    [Fact]
    public void BreadcrumbItem_AppliesCustomClass()
    {
        var cut = Render<BreadcrumbItem>(parameters => parameters
            .Add(p => p.Class, "breadcrumb-node")
            .AddChildContent("Docs"));

        cut.Find(".vibe-breadcrumb-item").ClassList.ShouldContain("breadcrumb-node");
    }
}
