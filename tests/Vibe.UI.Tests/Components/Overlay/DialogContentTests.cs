namespace Vibe.UI.Tests.Components.Overlay;

public class DialogContentTests : TestBase
{
    [Fact]
    public void DialogContent_RendersDocumentRoleAndContent()
    {
        var cut = RenderComponent<DialogContent>(parameters => parameters
            .AddChildContent("<p>Dialog content</p>"));

        var content = cut.Find(".vibe-dialog-content");
        content.GetAttribute("role").ShouldBe("document");
        content.InnerHtml.ShouldContain("Dialog content");
    }

    [Fact]
    public void DialogContent_PreservesCustomClassAndAttributes()
    {
        var cut = RenderComponent<DialogContent>(parameters => parameters
            .Add(p => p.Class, "content-pane")
            .AddUnmatched("data-content", "dialog"));

        var content = cut.Find(".vibe-dialog-content");
        content.ClassList.ShouldContain("content-pane");
        content.GetAttribute("data-content").ShouldBe("dialog");
    }
}
