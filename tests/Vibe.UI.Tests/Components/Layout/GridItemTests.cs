namespace Vibe.UI.Tests.Components.Layout;

public class GridItemTests : TestBase
{
    [Fact]
    public void GridItem_RendersContent()
    {
        var cut = Render<GridItem>(parameters => parameters
            .AddChildContent("<strong>Widget</strong>"));

        var item = cut.Find(".vibe-grid-item");
        item.InnerHtml.ShouldContain("Widget");
    }

    [Fact]
    public void GridItem_DefaultSpanHasEmptyStyle()
    {
        var cut = Render<GridItem>();

        cut.Find(".vibe-grid-item").GetAttribute("style").ShouldBeNull();
    }

    [Fact]
    public void GridItem_AppliesColumnAndRowSpanStyles()
    {
        var cut = Render<GridItem>(parameters => parameters
            .Add(p => p.ColSpan, 4)
            .Add(p => p.RowSpan, 2));

        var style = cut.Find(".vibe-grid-item").GetAttribute("style")!;
        style.ShouldContain("grid-column: span 4");
        style.ShouldContain("grid-row: span 2");
    }

    [Fact]
    public void GridItem_PreservesResponsiveSpanParameters()
    {
        var cut = Render<GridItem>(parameters => parameters
            .Add(p => p.ColSpanMd, 6)
            .Add(p => p.ColSpanLg, 8));

        cut.Instance.ColSpanMd.ShouldBe(6);
        cut.Instance.ColSpanLg.ShouldBe(8);
    }

    [Fact]
    public void GridItem_PreservesCustomClassAndAttributes()
    {
        var cut = Render<GridItem>(parameters => parameters
            .Add(p => p.Class, "featured")
            .AddUnmatched("data-cell", "summary"));

        var item = cut.Find(".vibe-grid-item");
        item.ClassList.ShouldContain("featured");
        item.GetAttribute("data-cell").ShouldBe("summary");
    }

    [Fact]
    public void GridItem_MergesAdditionalClassStyleAndRootAttributes()
    {
        var cut = Render<GridItem>(parameters => parameters
            .Add(p => p.Class, "featured")
            .Add(p => p.ColSpan, 4)
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-cell",
                ["style"] = "align-self: stretch;",
                ["data-cell"] = "summary"
            }));

        var item = cut.Find(".vibe-grid-item");
        item.ClassList.ShouldContain("featured");
        item.ClassList.ShouldContain("attribute-cell");
        item.GetAttribute("data-cell").ShouldBe("summary");

        var style = item.GetAttribute("style")!;
        style.ShouldContain("align-self: stretch");
        style.ShouldContain("grid-column: span 4");
    }

    [Fact]
    public void GridItem_WithResponsiveSpans_RendersStableCssVariables()
    {
        var cut = Render<GridItem>(parameters => parameters
            .Add(p => p.ColSpanMd, 6)
            .Add(p => p.ColSpanLg, 8));

        var item = cut.Find(".vibe-grid-item");
        item.ClassList.ShouldContain("vibe-grid-item-responsive");

        var style = item.GetAttribute("style")!;
        style.ShouldContain("--vibe-grid-item-col-span-md: 6");
        style.ShouldContain("--vibe-grid-item-col-span-lg: 8");
    }

    [Fact]
    public void GridItem_WithInvalidSpans_DoesNotEmitInvalidStyles()
    {
        var cut = Render<GridItem>(parameters => parameters
            .Add(p => p.ColSpan, -2)
            .Add(p => p.RowSpan, 0)
            .Add(p => p.ColSpanMd, -6)
            .Add(p => p.ColSpanLg, 0));

        var item = cut.Find(".vibe-grid-item");
        item.GetAttribute("style").ShouldBeNull();
        item.ClassList.ShouldNotContain("vibe-grid-item-responsive");
    }
}
