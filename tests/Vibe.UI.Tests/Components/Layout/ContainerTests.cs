namespace Vibe.UI.Tests.Components.Layout;

public class ContainerTests : TestBase
{
    [Fact]
    public void Container_RendersDefaultSizeAndContent()
    {
        var cut = RenderComponent<Container>(parameters => parameters
            .AddChildContent("<span class='inside'>Content</span>"));

        var container = cut.Find(".vibe-container");
        container.ClassList.ShouldContain("vibe-container-lg");
        container.InnerHtml.ShouldContain("inside");
    }

    [Fact]
    public void Container_AppliesConfiguredSize()
    {
        var cut = RenderComponent<Container>(parameters => parameters
            .Add(p => p.MaxWidth, Container.ContainerSize.XXLarge));

        cut.Find(".vibe-container").ClassList.ShouldContain("vibe-container-2xl");
    }

    [Fact]
    public void Container_FluidOverridesMaxWidth()
    {
        var cut = RenderComponent<Container>(parameters => parameters
            .Add(p => p.Fluid, true)
            .Add(p => p.MaxWidth, Container.ContainerSize.Small));

        var container = cut.Find(".vibe-container");
        container.ClassList.ShouldContain("vibe-container-full");
        container.ClassList.ShouldNotContain("vibe-container-sm");
    }

    [Fact]
    public void Container_AppliesPaddingAndUncenteredStyles()
    {
        var cut = RenderComponent<Container>(parameters => parameters
            .Add(p => p.Padding, "2rem")
            .Add(p => p.Centered, false));

        var style = cut.Find(".vibe-container").GetAttribute("style")!;
        style.ShouldContain("padding: 2rem");
        style.ShouldContain("margin-left: 0");
        style.ShouldContain("margin-right: 0");
    }

    [Fact]
    public void Container_PreservesCustomClassAndAttributes()
    {
        var cut = RenderComponent<Container>(parameters => parameters
            .Add(p => p.Class, "dashboard-shell")
            .AddUnmatched("data-testid", "container"));

        var container = cut.Find(".vibe-container");
        container.ClassList.ShouldContain("dashboard-shell");
        container.GetAttribute("data-testid").ShouldBe("container");
    }
}
