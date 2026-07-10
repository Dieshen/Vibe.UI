namespace Vibe.UI.Tests.Components.Layout;

public class GridTests : TestBase
{
    [Fact]
    public void Grid_RendersColumnsGapAndContent()
    {
        var cut = Render<Grid>(parameters => parameters
            .Add(p => p.Columns, 3)
            .Add(p => p.Gap, "8px")
            .AddChildContent("<div class='cell'>Cell</div>"));

        var grid = cut.Find(".vibe-grid");
        var style = grid.GetAttribute("style")!;
        style.ShouldContain("grid-template-columns: repeat(3, 1fr)");
        style.ShouldContain("gap: 8px");
        grid.InnerHtml.ShouldContain("cell");
    }

    [Fact]
    public void Grid_AppliesAutoFitColumns()
    {
        var cut = Render<Grid>(parameters => parameters
            .Add(p => p.AutoFit, true)
            .Add(p => p.MinColumnWidth, "18rem"));

        cut.Find(".vibe-grid")
            .GetAttribute("style")!
            .ShouldContain("repeat(auto-fit, minmax(18rem, 1fr))");
    }

    [Fact]
    public void Grid_UsesRowAndColumnGapOverrides()
    {
        var cut = Render<Grid>(parameters => parameters
            .Add(p => p.Gap, "1rem")
            .Add(p => p.RowGap, "2rem")
            .Add(p => p.ColumnGap, "3rem"));

        var style = cut.Find(".vibe-grid").GetAttribute("style")!;
        style.ShouldContain("row-gap: 2rem");
        style.ShouldContain("column-gap: 3rem");
        style.ShouldNotContain("gap: 1rem");
    }

    [Fact]
    public void Grid_UsesBaseGapForMissingGapAxis()
    {
        var cut = Render<Grid>(parameters => parameters
            .Add(p => p.Gap, "12px")
            .Add(p => p.RowGap, "20px"));

        var style = cut.Find(".vibe-grid").GetAttribute("style")!;
        style.ShouldContain("row-gap: 20px");
        style.ShouldContain("column-gap: 12px");
    }

    [Fact]
    public void Grid_PreservesCustomClassAndAttributes()
    {
        var cut = Render<Grid>(parameters => parameters
            .Add(p => p.Class, "metrics-grid")
            .AddUnmatched("data-grid", "metrics"));

        var grid = cut.Find(".vibe-grid");
        grid.ClassList.ShouldContain("metrics-grid");
        grid.GetAttribute("data-grid").ShouldBe("metrics");
    }

    [Fact]
    public void Grid_MergesAdditionalClassStyleAndRootAttributes()
    {
        var cut = Render<Grid>(parameters => parameters
            .Add(p => p.Class, "metrics-grid")
            .Add(p => p.Columns, 4)
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-grid",
                ["style"] = "background: red;",
                ["data-testid"] = "metrics"
            }));

        var grid = cut.Find(".vibe-grid");
        grid.ClassList.ShouldContain("metrics-grid");
        grid.ClassList.ShouldContain("attribute-grid");
        grid.GetAttribute("data-testid").ShouldBe("metrics");

        var style = grid.GetAttribute("style")!;
        style.ShouldContain("background: red");
        style.ShouldContain("grid-template-columns: repeat(4, 1fr)");
        style.ShouldContain("gap: 1rem");
    }

    [Fact]
    public void Grid_WithInvalidColumnsAndBlankGap_FallsBackToDefaults()
    {
        var cut = Render<Grid>(parameters => parameters
            .Add(p => p.Columns, 0)
            .Add(p => p.Gap, "   "));

        var style = cut.Find(".vibe-grid").GetAttribute("style")!;
        style.ShouldContain("grid-template-columns: repeat(12, 1fr)");
        style.ShouldContain("gap: 1rem");
    }

    [Fact]
    public void Grid_WithAutoFitAndBlankMinWidth_FallsBackToStableMinWidth()
    {
        var cut = Render<Grid>(parameters => parameters
            .Add(p => p.AutoFit, true)
            .Add(p => p.MinColumnWidth, "   "));

        cut.Find(".vibe-grid")
            .GetAttribute("style")!
            .ShouldContain("repeat(auto-fit, minmax(250px, 1fr))");
    }

    [Fact]
    public void Grid_WithNullChildContent_RendersEmptyRoot()
    {
        var cut = Render<Grid>();

        cut.Find(".vibe-grid").InnerHtml.Trim().ShouldBeEmpty();
    }
}
