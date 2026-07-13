namespace Vibe.UI.Tests.Components.Overlay;

public class PopoverTests : TestBase
{
    [Fact]
    public void Popover_RendersTriggerAndBaseClass()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details")));

        var root = cut.Find(".vibe-popover");
        root.ShouldNotBeNull();

        var trigger = cut.Find(".popover-trigger");
        trigger.TextContent.ShouldBe("Details");
        trigger.TagName.ShouldBe("BUTTON");
        trigger.GetAttribute("type").ShouldBe("button");
        trigger.QuerySelector("button").ShouldBeNull();
        trigger.GetAttribute("aria-haspopup").ShouldBe("dialog");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-disabled").ShouldBe("false");
        trigger.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact]
    public void Popover_DoesNotRenderContentInitially()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        cut.FindAll(".popover-content").ShouldBeEmpty();
        cut.FindAll(".popover-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void Popover_TogglesContent_WhenTriggerIsClicked()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<p>Popover content</p>")));

        cut.Find(".popover-trigger").Click();

        cut.Find(".popover-content").TextContent.ShouldContain("Popover content");
        cut.Find(".popover-content").GetAttribute("role").ShouldBe("dialog");
        cut.Find(".popover-content").GetAttribute("aria-label").ShouldBe("Popover");
        cut.Find(".popover-content").GetAttribute("data-state").ShouldBe("open");
        cut.Find(".popover-trigger").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find(".popover-backdrop").ShouldNotBeNull();

        cut.Find(".popover-trigger").Click();

        cut.FindAll(".popover-content").ShouldBeEmpty();
        cut.FindAll(".popover-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void Popover_ClosesWithEscapeFromTrigger()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        var trigger = cut.Find(".popover-trigger");
        trigger.Click();
        cut.Find(".popover-content").ShouldNotBeNull();

        trigger = cut.Find(".popover-trigger");
        trigger.KeyDown("Escape");
        cut.FindAll(".popover-content").ShouldBeEmpty();
    }

    [Fact]
    public void Popover_ClosesWithEscapeFromInteractiveContent()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        cut.Find(".popover-trigger").Click();
        cut.Find(".popover-content").KeyDown("Escape");

        cut.FindAll(".popover-content").ShouldBeEmpty();
    }

    [Fact]
    public void Popover_BackdropClosesContent_WhenCloseOnClickOutsideIsEnabled()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content"))
            .Add(p => p.CloseOnClickOutside, true));

        cut.Find(".popover-trigger").Click();
        cut.Find(".popover-backdrop").Click();

        cut.FindAll(".popover-content").ShouldBeEmpty();
    }

    [Fact]
    public void Popover_BackdropDoesNotCloseContent_WhenCloseOnClickOutsideIsDisabled()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content"))
            .Add(p => p.CloseOnClickOutside, false));

        cut.Find(".popover-trigger").Click();
        cut.Find(".popover-backdrop").Click();

        cut.Find(".popover-content").ShouldNotBeNull();
    }

    [Fact]
    public void Popover_EscapeDoesNotClose_WhenDisabled()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnEscape, false)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        cut.Find(".popover-trigger").KeyDown("Escape");

        cut.Find(".popover-content").ShouldNotBeNull();
    }

    [Fact]
    public void Popover_AppliesPositionAndAlignmentClasses()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content"))
            .Add(p => p.Position, "top")
            .Add(p => p.Align, "start"));

        cut.Find(".popover-trigger").Click();

        var content = cut.Find(".popover-content");
        content.ClassList.ShouldContain("popover-top");
        content.ClassList.ShouldContain("popover-start");
    }

    [Fact]
    public void Popover_FallsBackToDefaultPositionAndAlignment_WhenValuesAreInvalid()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content"))
            .Add(p => p.Position, "sideways")
            .Add(p => p.Align, "wide"));

        cut.Find(".popover-trigger").Click();

        var content = cut.Find(".popover-content");
        content.ClassList.ShouldContain("popover-bottom");
        content.ClassList.ShouldContain("popover-center");
        content.ClassList.ShouldNotContain("popover-sideways");
        content.ClassList.ShouldNotContain("popover-wide");
    }

    [Fact]
    public void Popover_AppliesCustomClass()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.Class, "wide-popover"));

        cut.Find(".vibe-popover").ClassList.ShouldContain("wide-popover");
    }

    [Fact]
    public void Popover_PreservesAdditionalAttributes_OnRoot()
    {
        var cut = Render<Popover>(parameters => parameters
            .AddUnmatched("data-testid", "popover-root"));

        cut.Find(".vibe-popover").GetAttribute("data-testid").ShouldBe("popover-root");
    }

    [Fact]
    public void Popover_RendersControlledOpenState()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.AriaLabel, "More details")
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        var trigger = cut.Find(".popover-trigger");
        var content = cut.Find(".popover-content");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
        content.GetAttribute("aria-label").ShouldBe("More details");
        cut.Find(".vibe-popover").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact]
    public void Popover_AppliesDialogLabellingAttributes()
    {
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.AriaLabel, "Ignored label")
            .Add(p => p.AriaLabelledBy, "popover-title")
            .Add(p => p.AriaDescribedBy, "popover-description")
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        var content = cut.Find(".popover-content");
        content.GetAttribute("aria-labelledby").ShouldBe("popover-title");
        content.GetAttribute("aria-describedby").ShouldBe("popover-description");
        content.GetAttribute("aria-label").ShouldBeNull();
    }

    [Fact]
    public void Popover_InvokesIsOpenChanged_WithoutMutatingParameter()
    {
        var changedValues = new List<bool>();
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content"))
            .Add(p => p.IsOpenChanged, changedValues.Add));

        cut.Find(".popover-trigger").Click();

        changedValues.ShouldBe([true]);
        cut.Instance.IsOpen.ShouldBeFalse();
        cut.Find(".popover-content").ShouldNotBeNull();
    }

    [Fact]
    public void Popover_DisabledTriggerDoesNotOpen()
    {
        var changed = false;
        var cut = Render<Popover>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content"))
            .Add(p => p.IsOpenChanged, _ => changed = true));

        var trigger = cut.Find(".popover-trigger");
        trigger.GetAttribute("aria-disabled").ShouldBe("true");
        trigger.HasAttribute("disabled").ShouldBeTrue();

        trigger.Click();
        trigger.KeyDown("Enter");

        changed.ShouldBeFalse();
        cut.FindAll(".popover-content").ShouldBeEmpty();
    }
}
