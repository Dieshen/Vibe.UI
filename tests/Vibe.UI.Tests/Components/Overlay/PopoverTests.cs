namespace Vibe.UI.Tests.Components.Overlay;

public class PopoverTests : TestBase
{
    [Fact]
    public void Popover_RendersTriggerAndBaseClass()
    {
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details")));

        var root = cut.Find(".vibe-popover");
        root.ShouldNotBeNull();

        var trigger = cut.Find(".popover-trigger");
        trigger.TextContent.ShouldBe("Details");
    }

    [Fact]
    public void Popover_DoesNotRenderContentInitially()
    {
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        cut.FindAll(".popover-content").ShouldBeEmpty();
        cut.FindAll(".popover-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void Popover_TogglesContent_WhenTriggerIsClicked()
    {
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<p>Popover content</p>")));

        cut.Find(".popover-trigger").Click();

        cut.Find(".popover-content").TextContent.ShouldContain("Popover content");
        cut.Find(".popover-backdrop").ShouldNotBeNull();

        cut.Find(".popover-trigger").Click();

        cut.FindAll(".popover-content").ShouldBeEmpty();
        cut.FindAll(".popover-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void Popover_BackdropClosesContent_WhenCloseOnClickOutsideIsEnabled()
    {
        var cut = RenderComponent<Popover>(parameters => parameters
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
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Details"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content"))
            .Add(p => p.CloseOnClickOutside, false));

        cut.Find(".popover-trigger").Click();
        cut.Find(".popover-backdrop").Click();

        cut.Find(".popover-content").ShouldNotBeNull();
    }

    [Fact]
    public void Popover_AppliesPositionAndAlignmentClasses()
    {
        var cut = RenderComponent<Popover>(parameters => parameters
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
    public void Popover_AppliesCustomClass()
    {
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Class, "wide-popover"));

        cut.Find(".vibe-popover").ClassList.ShouldContain("wide-popover");
    }
}
