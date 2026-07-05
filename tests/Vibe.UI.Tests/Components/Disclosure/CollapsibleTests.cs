namespace Vibe.UI.Tests.Components.Disclosure;

public class CollapsibleTests : TestBase
{
    [Fact]
    public void Collapsible_Renders_WithDefaultProps()
    {
        var cut = RenderBasicCollapsible();

        cut.Find(".vibe-collapsible").ShouldNotBeNull();
        var trigger = cut.Find(".collapsible-trigger-wrapper");
        var content = cut.Find(".collapsible-content");
        trigger.GetAttribute("role").ShouldBe("button");
        trigger.GetAttribute("tabindex").ShouldBe("0");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-controls").ShouldBe(content.GetAttribute("id"));
        trigger.GetAttribute("aria-label").ShouldBeNull();
        content.GetAttribute("role").ShouldBe("region");
        content.GetAttribute("aria-labelledby").ShouldBe(trigger.GetAttribute("id"));
        content.GetAttribute("aria-hidden").ShouldBe("true");
        content.HasAttribute("hidden").ShouldBeTrue();
    }

    [Fact]
    public void Collapsible_IsClosed_ByDefault()
    {
        var cut = RenderBasicCollapsible();

        cut.Instance.IsOpen.ShouldBeFalse();
        cut.Find(".collapsible-content").ClassList.ShouldNotContain("expanded");
    }

    [Fact]
    public void Collapsible_Renders_TriggerContent()
    {
        var cut = RenderBasicCollapsible("Toggle Me");

        cut.Markup.ShouldContain("Toggle Me");
    }

    [Fact]
    public void Collapsible_Renders_ChildContent()
    {
        var cut = Render<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, Trigger("Toggle"))
            .AddChildContent("<span class='test-content'>Test Content</span>"));

        cut.Find(".test-content").TextContent.ShouldBe("Test Content");
    }

    [Fact]
    public void Collapsible_RendersWithoutTriggerContent()
    {
        var cut = Render<Collapsible>(parameters => parameters
            .AddChildContent("Collapsible Content"));

        var trigger = cut.Find(".collapsible-trigger-wrapper");
        trigger.TextContent.Trim().ShouldBeEmpty();
        trigger.GetAttribute("aria-label").ShouldBe("Toggle collapsible");
    }

    [Fact]
    public void Collapsible_RendersWithoutChildContent()
    {
        var cut = Render<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, Trigger("Toggle")));

        cut.Find(".collapsible-content").TextContent.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Collapsible_Content_Expanded_WhenOpen()
    {
        var cut = RenderBasicCollapsible(isOpen: true);

        var content = cut.Find(".collapsible-content");
        content.ClassList.ShouldContain("expanded");
        content.HasAttribute("hidden").ShouldBeFalse();
        content.GetAttribute("aria-hidden").ShouldBeNull();
        cut.Find(".collapsible-trigger-wrapper").GetAttribute("aria-expanded").ShouldBe("true");
    }

    [Fact]
    public void Collapsible_ClickTogglesStateAndInvokesCallback()
    {
        var states = new List<bool>();
        var cut = Render<Collapsible>(parameters => parameters
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, states.Add))
            .Add(p => p.TriggerContent, Trigger("Toggle"))
            .AddChildContent("Collapsible Content"));

        var trigger = cut.Find(".collapsible-trigger-wrapper");
        trigger.Click();
        trigger.Click();

        states.Count.ShouldBe(2);
        states[0].ShouldBeTrue();
        states[1].ShouldBeFalse();
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        cut.Find(".collapsible-content").ClassList.ShouldNotContain("expanded");
    }

    [Fact]
    public async Task Collapsible_ToggleAsync_RerendersState()
    {
        var cut = RenderBasicCollapsible();

        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());

        cut.Find(".collapsible-trigger-wrapper").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find(".collapsible-content").ClassList.ShouldContain("expanded");
    }

    [Theory]
    [InlineData("Enter")]
    [InlineData(" ")]
    public void Collapsible_Toggles_WithKeyboardActivationKeys(string key)
    {
        var cut = RenderBasicCollapsible();

        cut.Find(".collapsible-trigger-wrapper").KeyDown(key);

        cut.Find(".collapsible-trigger-wrapper").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find(".collapsible-content").ClassList.ShouldContain("expanded");
    }

    [Fact]
    public void Collapsible_IgnoresNonActivationKeyboardInput()
    {
        var cut = RenderBasicCollapsible();

        cut.Find(".collapsible-trigger-wrapper").KeyDown("Escape");

        cut.Find(".collapsible-trigger-wrapper").GetAttribute("aria-expanded").ShouldBe("false");
        cut.Find(".collapsible-content").ClassList.ShouldNotContain("expanded");
    }

    [Fact]
    public void Collapsible_Passes_IsOpenState_ToTrigger()
    {
        var cut = Render<Collapsible>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.TriggerContent, isOpen => builder =>
                builder.AddContent(0, isOpen ? "Close" : "Open"))
            .AddChildContent("Collapsible Content"));

        cut.Markup.ShouldContain("Close");
    }

    [Fact]
    public void Collapsible_RespondsToExternalIsOpenParameterChanges()
    {
        var cut = RenderBasicCollapsible();

        cut.Render(parameters => parameters.Add(p => p.IsOpen, true));
        cut.Find(".collapsible-trigger-wrapper").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find(".collapsible-content").ClassList.ShouldContain("expanded");

        cut.Render(parameters => parameters.Add(p => p.IsOpen, false));
        cut.Find(".collapsible-trigger-wrapper").GetAttribute("aria-expanded").ShouldBe("false");
        cut.Find(".collapsible-content").ClassList.ShouldNotContain("expanded");
    }

    [Fact]
    public void Collapsible_PreservesCustomClassAndAdditionalAttributes()
    {
        var cut = Render<Collapsible>(parameters => parameters
            .Add(p => p.Class, "settings-collapsible")
            .Add(p => p.TriggerContent, Trigger("Toggle"))
            .AddUnmatched("data-test", "collapsible-value")
            .AddChildContent("Collapsible Content"));

        var root = cut.Find(".vibe-collapsible");
        root.ClassList.ShouldContain("settings-collapsible");
        root.GetAttribute("data-test").ShouldBe("collapsible-value");
    }

    private IRenderedComponent<Collapsible> RenderBasicCollapsible(string triggerText = "Toggle", bool isOpen = false)
    {
        return Render<Collapsible>(parameters => parameters
            .Add(p => p.IsOpen, isOpen)
            .Add(p => p.TriggerContent, Trigger(triggerText))
            .AddChildContent("Collapsible Content"));
    }

    private static RenderFragment<bool> Trigger(string text)
    {
        return _ => builder => builder.AddContent(0, text);
    }
}
