namespace Vibe.UI.Tests.Components.Overlay;

public class DialogHeaderTests : TestBase
{
    [Fact]
    public void DialogHeader_RendersContent()
    {
        var cut = RenderComponent<DialogHeader>(parameters => parameters
            .AddChildContent("<h2>Heading</h2>"));

        var header = cut.Find(".vibe-dialog-header");
        header.InnerHtml.ShouldContain("Heading");
    }

    [Fact]
    public void DialogHeader_PreservesCustomClassAndAttributes()
    {
        var cut = RenderComponent<DialogHeader>(parameters => parameters
            .Add(p => p.Class, "dialog-top")
            .AddUnmatched("data-header", "dialog"));

        var header = cut.Find(".vibe-dialog-header");
        header.ClassList.ShouldContain("dialog-top");
        header.GetAttribute("data-header").ShouldBe("dialog");
    }
}
