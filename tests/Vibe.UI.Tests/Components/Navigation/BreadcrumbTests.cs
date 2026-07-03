namespace Vibe.UI.Tests.Components.Navigation;

public class BreadcrumbTests : TestBase
{
    [Fact]
    public void Breadcrumb_RendersNavAndList()
    {
        var cut = Render<Breadcrumb>();

        var nav = cut.Find("nav.vibe-breadcrumb");
        nav.GetAttribute("aria-label").ShouldBe("Breadcrumb");
        cut.Find("ol.breadcrumb-list").ShouldNotBeNull();
    }

    [Fact]
    public void Breadcrumb_RendersChildContent()
    {
        var cut = Render<Breadcrumb>(parameters => parameters
            .AddChildContent("<li>Home</li><li>Docs</li>"));

        var items = cut.FindAll("li");
        items.Count.ShouldBe(2);
        items[0].TextContent.ShouldBe("Home");
        items[1].TextContent.ShouldBe("Docs");
    }

    [Fact]
    public void Breadcrumb_AppliesCustomClass()
    {
        var cut = Render<Breadcrumb>(parameters => parameters
            .Add(p => p.Class, "compact-breadcrumb"));

        cut.Find(".vibe-breadcrumb").ClassList.ShouldContain("compact-breadcrumb");
    }

    [Fact]
    public void Breadcrumb_ExposesSeparatorParameter()
    {
        var cut = Render<Breadcrumb>(parameters => parameters
            .Add(p => p.Separator, ">"));

        cut.Instance.Separator.ShouldBe(">");
    }
}
