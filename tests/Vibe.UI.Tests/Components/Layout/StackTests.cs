namespace Vibe.UI.Tests.Components.Layout;

public class StackTests : TestBase
{
    [Fact]
    public void Stack_RendersVerticalByDefault()
    {
        var cut = RenderComponent<Stack>(parameters => parameters
            .AddChildContent("<span>Item</span>"));

        var stack = cut.Find(".vibe-stack");
        stack.ClassList.ShouldContain("vibe-stack-vertical");
        stack.InnerHtml.ShouldContain("Item");
    }

    [Fact]
    public void Stack_AppliesHorizontalAndWrapClasses()
    {
        var cut = RenderComponent<Stack>(parameters => parameters
            .Add(p => p.Direction, Stack.StackDirection.Horizontal)
            .Add(p => p.Wrap, true));

        var stack = cut.Find(".vibe-stack");
        stack.ClassList.ShouldContain("vibe-stack-horizontal");
        stack.ClassList.ShouldContain("vibe-stack-wrap");
    }

    [Fact]
    public void Stack_AppliesSpacingAlignAndJustifyStyles()
    {
        var cut = RenderComponent<Stack>(parameters => parameters
            .Add(p => p.Spacing, "24px")
            .Add(p => p.Align, Stack.StackAlign.Center)
            .Add(p => p.Justify, Stack.StackJustify.SpaceBetween));

        var style = cut.Find(".vibe-stack").GetAttribute("style")!;
        style.ShouldContain("gap: 24px");
        style.ShouldContain("align-items: center");
        style.ShouldContain("justify-content: space-between");
    }

    [Fact]
    public void Stack_UsesStretchAndStartDefaults()
    {
        var cut = RenderComponent<Stack>();

        var style = cut.Find(".vibe-stack").GetAttribute("style")!;
        style.ShouldContain("align-items: stretch");
        style.ShouldContain("justify-content: flex-start");
    }

    [Fact]
    public void Stack_PreservesCustomClassAndAttributes()
    {
        var cut = RenderComponent<Stack>(parameters => parameters
            .Add(p => p.Class, "toolbar-stack")
            .AddUnmatched("data-stack", "toolbar"));

        var stack = cut.Find(".vibe-stack");
        stack.ClassList.ShouldContain("toolbar-stack");
        stack.GetAttribute("data-stack").ShouldBe("toolbar");
    }
}
