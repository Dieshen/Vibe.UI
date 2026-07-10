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
    public void Breadcrumb_RendersEmptyList_WhenChildContentIsNull()
    {
        var cut = Render<Breadcrumb>();

        var list = cut.Find("ol.breadcrumb-list");
        list.TextContent.Trim().ShouldBeEmpty();
        cut.FindAll("li").ShouldBeEmpty();
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
    public void Breadcrumb_PreservesAdditionalAttributesAndCustomAriaLabel()
    {
        var cut = Render<Breadcrumb>(parameters => parameters
            .Add(p => p.AriaLabel, " Product trail ")
            .AddUnmatched("data-testid", "breadcrumb"));

        var nav = cut.Find(".vibe-breadcrumb");
        nav.GetAttribute("aria-label").ShouldBe("Product trail");
        nav.GetAttribute("data-testid").ShouldBe("breadcrumb");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Breadcrumb_FallsBackToDefaultAriaLabel_WhenLabelIsMissing(string? ariaLabel)
    {
        var cut = Render<Breadcrumb>(parameters => parameters
            .Add(p => p.AriaLabel, ariaLabel));

        cut.Find(".vibe-breadcrumb").GetAttribute("aria-label").ShouldBe("Breadcrumb");
    }

    [Fact]
    public void Breadcrumb_ExposesSeparatorParameter()
    {
        var cut = Render<Breadcrumb>(parameters => parameters
            .Add(p => p.Separator, ">"));

        cut.Instance.Separator.ShouldBe(">");
    }
}
