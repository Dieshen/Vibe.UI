namespace Vibe.UI.Tests.Components.Layout;

public class DividerTests : TestBase
{
    [Fact]
    public void Divider_RendersHorizontalRuleByDefault()
    {
        var cut = Render<Divider>();

        var divider = cut.Find("hr.vibe-divider");
        divider.ClassList.ShouldContain("vibe-divider-horizontal");
    }

    [Fact]
    public void Divider_AppliesVerticalAndDottedClasses()
    {
        var cut = Render<Divider>(parameters => parameters
            .Add(p => p.Orientation, Divider.DividerOrientation.Vertical)
            .Add(p => p.LineStyle, Divider.DividerLineStyle.Dotted));

        var divider = cut.Find(".vibe-divider");
        divider.ClassList.ShouldContain("vibe-divider-vertical");
        divider.ClassList.ShouldContain("vibe-divider-dotted");
    }

    [Fact]
    public void Divider_AppliesThicknessStyle_WhenGreaterThanOne()
    {
        var cut = Render<Divider>(parameters => parameters
            .Add(p => p.Thickness, 3));

        cut.Find(".vibe-divider").GetAttribute("style").ShouldBe("border-width: 3px");
    }

    [Fact]
    public void Divider_RendersLabelInsteadOfHr_WhenLabelProvided()
    {
        var cut = Render<Divider>(parameters => parameters
            .Add(p => p.Label, "Details")
            .Add(p => p.LabelPosition, Divider.DividerLabelPosition.Left));

        cut.FindAll("hr").ShouldBeEmpty();
        var divider = cut.Find(".vibe-divider-label");
        divider.TextContent.ShouldBe("Details");
        divider.ClassList.ShouldContain("vibe-divider-label-left");
    }

    [Fact]
    public void Divider_PreservesCustomClassAndAttributes()
    {
        var cut = Render<Divider>(parameters => parameters
            .Add(p => p.Class, "section-rule")
            .AddUnmatched("aria-hidden", "true"));

        var divider = cut.Find(".vibe-divider");
        divider.ClassList.ShouldContain("section-rule");
        divider.GetAttribute("aria-hidden").ShouldBe("true");
    }
}
