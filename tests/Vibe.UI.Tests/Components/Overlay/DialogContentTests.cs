namespace Vibe.UI.Tests.Components.Overlay;

public class DialogContentTests : TestBase
{
    [Fact]
    public void DialogContent_RendersDocumentRoleAndContent()
    {
        var cut = Render<DialogContent>(parameters => parameters
            .AddChildContent("<p>Dialog content</p>"));

        var content = cut.Find(".vibe-dialog-content");
        content.GetAttribute("role").ShouldBe("document");
        content.InnerHtml.ShouldContain("Dialog content");
    }

    [Fact]
    public void DialogContent_HidesUntilParentDialogIsOpen()
    {
        var closed = Render<DialogRoot>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Dialog content")));
                builder.CloseComponent();
            }));

        closed.FindAll(".vibe-dialog-content").ShouldBeEmpty();

        var open = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Dialog content")));
                builder.CloseComponent();
            }));

        open.Find(".vibe-dialog-content").TextContent.ShouldBe("Dialog content");
    }

    [Fact]
    public void DialogContent_PreservesCustomClassAndAttributes()
    {
        var cut = Render<DialogContent>(parameters => parameters
            .Add(p => p.Class, "content-pane")
            .AddUnmatched("data-content", "dialog"));

        var content = cut.Find(".vibe-dialog-content");
        content.ClassList.ShouldContain("content-pane");
        content.GetAttribute("data-content").ShouldBe("dialog");
    }
}
