namespace Vibe.UI.Tests.Components.Overlay;

public class DialogTriggerTests : TestBase
{
    [Fact]
    public void DialogTrigger_RendersContentAndAttributes()
    {
        var cut = RenderComponent<DialogTrigger>(parameters => parameters
            .Add(p => p.Class, "trigger-shell")
            .AddUnmatched("data-trigger", "dialog")
            .AddChildContent("Open"));

        var trigger = cut.Find(".vibe-dialog-trigger");
        trigger.TagName.ShouldBe("SPAN");
        trigger.TextContent.ShouldBe("Open");
        trigger.ClassList.ShouldContain("trigger-shell");
        trigger.GetAttribute("data-trigger").ShouldBe("dialog");
    }

    [Fact]
    public void DialogTrigger_InvokesClickCallback()
    {
        var clicked = false;
        var cut = RenderComponent<DialogTrigger>(parameters => parameters
            .Add(p => p.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
            .AddChildContent("Open"));

        cut.Find(".vibe-dialog-trigger").Click();

        clicked.ShouldBeTrue();
    }

    [Fact]
    public void DialogTrigger_OpensParentDialog()
    {
        bool? changed = null;
        var cut = RenderComponent<DialogRoot>(parameters => parameters
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
}
