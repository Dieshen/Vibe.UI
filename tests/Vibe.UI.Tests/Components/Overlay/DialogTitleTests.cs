namespace Vibe.UI.Tests.Components.Overlay;

public class DialogTitleTests : TestBase
{
    [Fact]
    public void DialogTitle_RendersHeadingWithProvidedId()
    {
        var cut = Render<DialogTitle>(parameters => parameters
            .Add(p => p.Id, "dialog-title")
            .AddChildContent("Title"));

        var title = cut.Find("h2.vibe-dialog-title");
        title.GetAttribute("id").ShouldBe("dialog-title");
        title.TextContent.ShouldBe("Title");
    }

    [Fact]
    public void DialogTitle_GeneratesIdWhenMissing()
    {
        var cut = Render<DialogTitle>();

        cut.Instance.Id.ShouldBeNull();
        cut.Find(".vibe-dialog-title").GetAttribute("id")!.ShouldStartWith("vibe-dialog-title-");
    }

    [Fact]
    public void DialogTitle_RegistersIdWithParentDialog()
    {
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTitle>(0);
                builder.AddAttribute(1, "Id", "registered-title");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(title => title.AddContent(0, "Title")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-dialog").GetAttribute("aria-labelledby").ShouldBe("registered-title");
    }

    [Fact]
    public void DialogTitle_PreservesCustomClassAndAttributes()
    {
        var cut = Render<DialogTitle>(parameters => parameters
            .Add(p => p.Class, "dialog-heading")
            .AddUnmatched("data-title", "dialog"));

        var title = cut.Find(".vibe-dialog-title");
        title.ClassList.ShouldContain("dialog-heading");
        title.GetAttribute("data-title").ShouldBe("dialog");
    }
}
