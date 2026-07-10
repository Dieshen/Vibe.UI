namespace Vibe.UI.Tests.Components.Layout;

public class StackTests : TestBase
{
    [Fact]
    public void Stack_RendersVerticalByDefault()
    {
        var cut = Render<Stack>(parameters => parameters
            .AddChildContent("<span>Item</span>"));

        var stack = cut.Find(".vibe-stack");
        stack.ClassList.ShouldContain("vibe-stack-vertical");
        stack.InnerHtml.ShouldContain("Item");
    }

    [Fact]
    public void Stack_AppliesHorizontalAndWrapClasses()
    {
        var cut = Render<Stack>(parameters => parameters
            .Add(p => p.Direction, Stack.StackDirection.Horizontal)
            .Add(p => p.Wrap, true));

        var stack = cut.Find(".vibe-stack");
        stack.ClassList.ShouldContain("vibe-stack-horizontal");
        stack.ClassList.ShouldContain("vibe-stack-wrap");
    }

    [Fact]
    public void Stack_AppliesSpacingAlignAndJustifyStyles()
    {
        var cut = Render<Stack>(parameters => parameters
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
        var cut = Render<Stack>();

        var style = cut.Find(".vibe-stack").GetAttribute("style")!;
        style.ShouldContain("align-items: stretch");
        style.ShouldContain("justify-content: flex-start");
    }

    [Fact]
    public void Stack_PreservesCustomClassAndAttributes()
    {
        var cut = Render<Stack>(parameters => parameters
            .Add(p => p.Class, "toolbar-stack")
            .AddUnmatched("data-stack", "toolbar"));

        var stack = cut.Find(".vibe-stack");
        stack.ClassList.ShouldContain("toolbar-stack");
        stack.GetAttribute("data-stack").ShouldBe("toolbar");
    }

    [Fact]
    public void Stack_MergesAdditionalClassStyleAndRootAttributes()
    {
        var cut = Render<Stack>(parameters => parameters
            .Add(p => p.Class, "toolbar-stack")
            .Add(p => p.Spacing, "20px")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-stack",
                ["style"] = "min-width: 0;",
                ["data-testid"] = "stack"
            }));

        var stack = cut.Find(".vibe-stack");
        stack.ClassList.ShouldContain("toolbar-stack");
        stack.ClassList.ShouldContain("attribute-stack");
        stack.GetAttribute("data-testid").ShouldBe("stack");

        var style = stack.GetAttribute("style")!;
        style.ShouldContain("min-width: 0");
        style.ShouldContain("gap: 20px");
        style.ShouldContain("align-items: stretch");
    }

    [Fact]
    public void Stack_WithInvalidEnumsAndBlankSpacing_FallsBackToDefaults()
    {
        var cut = Render<Stack>(parameters => parameters
            .Add(p => p.Direction, (Stack.StackDirection)999)
            .Add(p => p.Align, (Stack.StackAlign)999)
            .Add(p => p.Justify, (Stack.StackJustify)999)
            .Add(p => p.Spacing, "   "));

        var stack = cut.Find(".vibe-stack");
        stack.ClassList.ShouldContain("vibe-stack-vertical");

        var style = stack.GetAttribute("style")!;
        style.ShouldContain("gap: 1rem");
        style.ShouldContain("align-items: stretch");
        style.ShouldContain("justify-content: flex-start");
    }

    [Fact]
    public void Stack_WithNullChildContent_RendersEmptyRoot()
    {
        var cut = Render<Stack>();

        cut.Find(".vibe-stack").InnerHtml.Trim().ShouldBeEmpty();
    }
}
