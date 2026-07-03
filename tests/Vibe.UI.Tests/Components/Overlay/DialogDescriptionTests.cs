namespace Vibe.UI.Tests.Components.Overlay;

public class DialogDescriptionTests : TestBase
{
    [Fact]
    public void DialogDescription_RendersParagraphWithProvidedId()
    {
        var cut = Render<DialogDescription>(parameters => parameters
            .Add(p => p.Id, "dialog-description")
            .AddChildContent("Description"));

        var description = cut.Find("p.vibe-dialog-description");
        description.GetAttribute("id").ShouldBe("dialog-description");
        description.TextContent.ShouldBe("Description");
    }

    [Fact]
    public void DialogDescription_GeneratesIdWhenMissing()
    {
        var cut = Render<DialogDescription>();

        cut.Instance.Id.ShouldNotBeNullOrWhiteSpace();
        cut.Find(".vibe-dialog-description").GetAttribute("id").ShouldBe(cut.Instance.Id);
    }

    [Fact]
    public void DialogDescription_RegistersIdWithParentDialog()
    {
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogDescription>(0);
                builder.AddAttribute(1, "Id", "registered-description");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(description => description.AddContent(0, "Description")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-dialog").GetAttribute("aria-describedby").ShouldBe("registered-description");
    }

    [Fact]
    public void DialogDescription_PreservesCustomClassAndAttributes()
    {
        var cut = Render<DialogDescription>(parameters => parameters
            .Add(p => p.Class, "dialog-copy")
            .AddUnmatched("data-description", "dialog"));

        var description = cut.Find(".vibe-dialog-description");
        description.ClassList.ShouldContain("dialog-copy");
        description.GetAttribute("data-description").ShouldBe("dialog");
    }
}
