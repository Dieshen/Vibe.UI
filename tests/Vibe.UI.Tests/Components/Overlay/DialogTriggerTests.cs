namespace Vibe.UI.Tests.Components.Overlay;

public class DialogTriggerTests : TestBase
{
    [Fact]
    public void DialogTrigger_RendersContentAndAttributes()
    {
        var cut = Render<DialogTrigger>(parameters => parameters
            .Add(p => p.Class, "trigger-shell")
            .AddUnmatched("data-trigger", "dialog")
            .AddChildContent("Open"));

        var trigger = cut.Find(".vibe-dialog-trigger");
        trigger.TagName.ShouldBe("SPAN");
        trigger.TextContent.ShouldBe("Open");
        trigger.GetAttribute("role").ShouldBe("button");
        trigger.GetAttribute("tabindex").ShouldBe("0");
        trigger.GetAttribute("aria-haspopup").ShouldBe("dialog");
        trigger.GetAttribute("aria-disabled").ShouldBe("false");
        trigger.ClassList.ShouldContain("trigger-shell");
        trigger.GetAttribute("data-trigger").ShouldBe("dialog");
    }

    [Fact]
    public void DialogTrigger_AddsFallbackAriaLabel_WhenContentIsMissing()
    {
        var cut = Render<DialogTrigger>();

        cut.Find(".vibe-dialog-trigger").GetAttribute("aria-label").ShouldBe("Open dialog");
    }

    [Fact]
    public void DialogTrigger_InvokesClickCallback()
    {
        var clicked = false;
        var cut = Render<DialogTrigger>(parameters => parameters
            .Add(p => p.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
            .AddChildContent("Open"));

        cut.Find(".vibe-dialog-trigger").Click();

        clicked.ShouldBeTrue();
    }

    [Fact]
    public void DialogTrigger_DoesNotInvokeCallback_WhenDisabled()
    {
        var clicked = false;
        var cut = Render<DialogTrigger>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
            .AddChildContent("Open"));

        var trigger = cut.Find(".vibe-dialog-trigger");
        trigger.GetAttribute("aria-disabled").ShouldBe("true");
        trigger.GetAttribute("tabindex").ShouldBe("-1");
        trigger.ClassList.ShouldContain("vibe-dialog-trigger-disabled");

        trigger.Click();

        clicked.ShouldBeFalse();
    }

    [Fact]
    public void DialogTrigger_OpensParentDialog()
    {
        bool? changed = null;
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => changed = value))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-dialog-trigger").Click();

        changed.ShouldBe(true);
        cut.Find(".vibe-dialog").ShouldNotBeNull();
    }

    [Fact]
    public void DialogTrigger_DoesNotOpenParentDialog_WhenDisabled()
    {
        bool? changed = null;
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => changed = value))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "Disabled", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-dialog-trigger").Click();
        cut.Find(".vibe-dialog-trigger").KeyDown("Enter");

        changed.ShouldBeNull();
        cut.FindAll(".vibe-dialog").ShouldBeEmpty();
    }

    [Fact]
    public void DialogTrigger_OpensParentDialogWithKeyboard()
    {
        bool? changed = null;
        var cut = Render<DialogRoot>(parameters => parameters
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => changed = value))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-dialog-trigger").KeyDown("Enter");

        changed.ShouldBe(true);
        cut.Find(".vibe-dialog").ShouldNotBeNull();
    }
}
