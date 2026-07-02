namespace Vibe.UI.Tests.Components.Overlay;

public class DialogCloseTests : TestBase
{
    [Fact]
    public void DialogClose_RendersDefaultAccessibleButton()
    {
        var cut = RenderComponent<DialogClose>();

        var button = cut.Find("button.vibe-dialog-close");
        button.GetAttribute("type").ShouldBe("button");
        button.GetAttribute("aria-label").ShouldBe("Close");
        cut.Find(".vibe-dialog-close-icon").GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void DialogClose_RendersCustomContentAndAttributes()
    {
        var cut = RenderComponent<DialogClose>(parameters => parameters
            .Add(p => p.Class, "close-button")
            .AddUnmatched("data-close", "dialog")
            .AddChildContent("Dismiss"));

        var button = cut.Find(".vibe-dialog-close");
        button.TextContent.ShouldBe("Dismiss");
        button.ClassList.ShouldContain("close-button");
        button.GetAttribute("data-close").ShouldBe("dialog");
    }

    [Fact]
    public void DialogClose_InvokesClickCallback()
    {
        var clicked = false;
        var cut = RenderComponent<DialogClose>(parameters => parameters
            .Add(p => p.OnClick, EventCallback.Factory.Create(this, () => clicked = true)));

        cut.Find(".vibe-dialog-close").Click();

        clicked.ShouldBeTrue();
    }

    [Fact]
    public void DialogClose_ClosesParentDialog()
    {
        bool? changed = null;
        var cut = RenderComponent<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => changed = value))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogClose>(0);
                builder.CloseComponent();
            }));

        cut.Find(".vibe-dialog-close").Click();

        changed.ShouldBe(false);
        cut.FindAll(".vibe-dialog").ShouldBeEmpty();
    }
}
