namespace Vibe.UI.Tests.Components.Overlay;

public class DialogRootTests : TestBase
{
    [Fact]
    public void DialogRoot_RendersTriggerContent_WhenClosed()
    {
        var cut = Render<DialogRoot>(parameters => parameters
            .AddChildContent("<span class='closed-content'>Open dialog</span>"));

        cut.FindAll(".vibe-dialog").ShouldBeEmpty();
        cut.Find(".closed-content").TextContent.ShouldBe("Open dialog");
    }

    [Fact]
    public void DialogRoot_HidesDialogContentUntilTriggerOpensComposedDialog()
    {
        bool? changed = null;
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => changed = value))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<DialogContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Dialog body")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-dialog-trigger").TextContent.ShouldBe("Open");
        cut.FindAll(".vibe-dialog-content").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("Dialog body");

        cut.Find(".vibe-dialog-trigger").Click();

        changed.ShouldBe(true);
        cut.FindAll(".vibe-dialog-trigger").ShouldBeEmpty();
        cut.Find(".vibe-dialog-content").TextContent.ShouldBe("Dialog body");
    }

    [Fact]
    public void DialogRoot_RendersDialog_WhenOpen()
    {
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("Dialog body"));

        var dialog = cut.Find(".vibe-dialog");
        dialog.GetAttribute("role").ShouldBe("dialog");
        dialog.GetAttribute("aria-modal").ShouldBe("true");
        cut.Find(".vibe-dialog-overlay").GetAttribute("tabindex").ShouldBe("-1");
        dialog.TextContent.ShouldContain("Dialog body");
    }

    [Fact]
    public void DialogRoot_UsesAriaLabelFallback_WhenNoTitleIsRegistered()
    {
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.AriaLabel, "Custom dialog")
            .AddChildContent("Dialog body"));

        var dialog = cut.Find(".vibe-dialog");
        dialog.GetAttribute("aria-label").ShouldBe("Custom dialog");
        dialog.HasAttribute("aria-labelledby").ShouldBeFalse();
    }

    [Fact]
    public void DialogRoot_WiresTitleAndDescriptionIds()
    {
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTitle>(0);
                builder.AddAttribute(1, "Id", "title-id");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(title => title.AddContent(0, "Title")));
                builder.CloseComponent();
                builder.OpenComponent<DialogDescription>(3);
                builder.AddAttribute(4, "Id", "description-id");
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(description => description.AddContent(0, "Description")));
                builder.CloseComponent();
            }));

        var dialog = cut.Find(".vibe-dialog");
        dialog.GetAttribute("aria-labelledby").ShouldBe("title-id");
        dialog.GetAttribute("aria-describedby").ShouldBe("description-id");
    }

    [Fact]
    public void DialogRoot_BackdropClickClosesWhenEnabled()
    {
        var changedTo = true;
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => changedTo = value))
            .AddChildContent("Dialog body"));

        cut.Find(".vibe-dialog-overlay").Click();

        changedTo.ShouldBeFalse();
        cut.FindAll(".vibe-dialog").ShouldBeEmpty();
    }

    [Fact]
    public void DialogRoot_BackdropClickDoesNotCloseWhenDisabled()
    {
        var changed = false;
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOutsideClick, false)
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, _ => changed = true))
            .AddChildContent("Dialog body"));

        cut.Find(".vibe-dialog-overlay").Click();

        changed.ShouldBeFalse();
        cut.Find(".vibe-dialog").ShouldNotBeNull();
    }

    [Fact]
    public async Task DialogRoot_HandleDialogEscape_ClosesWhenEnabled()
    {
        bool? changedTo = null;
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => changedTo = value))
            .AddChildContent("Dialog body"));

        await cut.InvokeAsync(cut.Instance.HandleDialogEscape);

        changedTo.ShouldBe(false);
        cut.FindAll(".vibe-dialog").ShouldBeEmpty();
    }

    [Fact]
    public void DialogRoot_SyncsInternalOpenState_WhenParameterChanges()
    {
        var closed = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Dialog body")));
                builder.CloseComponent();
            }));

        closed.FindAll(".vibe-dialog").ShouldBeEmpty();
        closed.FindAll(".vibe-dialog-content").ShouldBeEmpty();

        var open = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Dialog body")));
                builder.CloseComponent();
            }));

        open.Find(".vibe-dialog-content").TextContent.ShouldBe("Dialog body");

        var closedAgain = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpen, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Dialog body")));
                builder.CloseComponent();
            }));

        closedAgain.FindAll(".vibe-dialog").ShouldBeEmpty();
        closedAgain.FindAll(".vibe-dialog-content").ShouldBeEmpty();
    }
}
