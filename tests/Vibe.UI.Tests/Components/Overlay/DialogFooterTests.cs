namespace Vibe.UI.Tests.Components.Overlay;

public class DialogFooterTests : TestBase
{
    [Fact]
    public void DialogFooter_RendersContent()
    {
        var cut = Render<DialogFooter>(parameters => parameters
            .AddChildContent("<button>Save</button>"));

        var footer = cut.Find(".vibe-dialog-footer");
        footer.InnerHtml.ShouldContain("Save");
    }

    [Fact]
    public void DialogFooter_PreservesCustomClassAndAttributes()
    {
        var cut = Render<DialogFooter>(parameters => parameters
            .Add(p => p.Class, "dialog-actions")
            .AddUnmatched("data-footer", "dialog"));

        var footer = cut.Find(".vibe-dialog-footer");
        footer.ClassList.ShouldContain("dialog-actions");
        footer.GetAttribute("data-footer").ShouldBe("dialog");
    }

    [Fact]
    public void DialogFooter_RendersStableElement_WhenContentIsNull()
    {
        var cut = Render<DialogFooter>();

        var footer = cut.Find(".vibe-dialog-footer");
        footer.TagName.ShouldBe("DIV");
        footer.TextContent.ShouldBeEmpty();
    }
}
