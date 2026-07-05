namespace Vibe.UI.Tests.Components.Layout;

public class DividerTests : TestBase
{
    [Fact]
    public void Divider_RendersHorizontalRuleByDefault()
    {
        var cut = Render<Divider>();

        var divider = cut.Find("hr.vibe-divider");
        divider.ClassList.ShouldContain("vibe-divider-horizontal");
        divider.GetAttribute("role").ShouldBe("separator");
        divider.GetAttribute("aria-orientation").ShouldBe("horizontal");
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
        divider.GetAttribute("aria-orientation").ShouldBe("vertical");
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

    [Fact]
    public void Divider_MergesAdditionalClassStyleAndRootAttributes()
    {
        var cut = Render<Divider>(parameters => parameters
            .Add(p => p.Class, "section-rule")
            .Add(p => p.Thickness, 3)
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-rule",
                ["style"] = "margin-block: 2rem;",
                ["data-testid"] = "section-divider"
            }));

        var divider = cut.Find(".vibe-divider");
        divider.ClassList.ShouldContain("section-rule");
        divider.ClassList.ShouldContain("attribute-rule");
        divider.GetAttribute("data-testid").ShouldBe("section-divider");

        var style = divider.GetAttribute("style")!;
        style.ShouldContain("margin-block: 2rem");
        style.ShouldContain("border-width: 3px");
    }

    [Fact]
    public void Divider_WithInvalidEnumValues_FallsBackToStableClasses()
    {
        var cut = Render<Divider>(parameters => parameters
            .Add(p => p.Orientation, (Divider.DividerOrientation)999)
            .Add(p => p.LineStyle, (Divider.DividerLineStyle)999)
            .Add(p => p.Label, "  Section  ")
            .Add(p => p.LabelPosition, (Divider.DividerLabelPosition)999));

        var divider = cut.Find(".vibe-divider-label");
        divider.TextContent.ShouldBe("Section");
        divider.ClassList.ShouldContain("vibe-divider-horizontal");
        divider.ClassList.ShouldNotContain("vibe-divider-dashed");
        divider.ClassList.ShouldNotContain("vibe-divider-dotted");
        divider.ClassList.ShouldNotContain("vibe-divider-label-left");
        divider.ClassList.ShouldNotContain("vibe-divider-label-right");
        divider.GetAttribute("aria-orientation").ShouldBe("horizontal");
    }

    [Fact]
    public void Divider_WithWhitespaceLabel_RendersRule()
    {
        var cut = Render<Divider>(parameters => parameters
            .Add(p => p.Label, "   "));

        cut.Find("hr.vibe-divider").ShouldNotBeNull();
        cut.FindAll(".vibe-divider-label").ShouldBeEmpty();
    }
}
