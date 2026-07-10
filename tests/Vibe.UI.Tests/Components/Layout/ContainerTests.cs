namespace Vibe.UI.Tests.Components.Layout;

public class ContainerTests : TestBase
{
    [Fact]
    public void Container_RendersDefaultSizeAndContent()
    {
        var cut = Render<Container>(parameters => parameters
            .AddChildContent("<span class='inside'>Content</span>"));

        var container = cut.Find(".vibe-container");
        container.ClassList.ShouldContain("vibe-container-lg");
        container.InnerHtml.ShouldContain("inside");
    }

    [Fact]
    public void Container_AppliesConfiguredSize()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.MaxWidth, Container.ContainerSize.XXLarge));

        cut.Find(".vibe-container").ClassList.ShouldContain("vibe-container-2xl");
    }

    [Fact]
    public void Container_WithInvalidSize_FallsBackToLarge()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.MaxWidth, (Container.ContainerSize)999));

        var container = cut.Find(".vibe-container");
        container.ClassList.ShouldContain("vibe-container-lg");
        container.ClassList.ShouldNotContain("vibe-container-999");
    }

    [Fact]
    public void Container_FluidOverridesMaxWidth()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.Fluid, true)
            .Add(p => p.MaxWidth, Container.ContainerSize.Small));

        var container = cut.Find(".vibe-container");
        container.ClassList.ShouldContain("vibe-container-full");
        container.ClassList.ShouldNotContain("vibe-container-sm");
    }

    [Fact]
    public void Container_AppliesPaddingAndUncenteredStyles()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.Padding, " 2rem ")
            .Add(p => p.Centered, false));

        var style = cut.Find(".vibe-container").GetAttribute("style")!;
        style.ShouldContain("padding: 2rem");
        style.ShouldContain("margin-left: 0");
        style.ShouldContain("margin-right: 0");
    }

    [Fact]
    public void Container_PreservesCustomClassAndAttributes()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.Class, "dashboard-shell")
            .AddUnmatched("data-testid", "container"));

        var container = cut.Find(".vibe-container");
        container.ClassList.ShouldContain("dashboard-shell");
        container.GetAttribute("data-testid").ShouldBe("container");
    }

    [Fact]
    public void Container_MergesAdditionalClassAndStyle()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.Class, "dashboard-shell")
            .Add(p => p.Padding, "2rem")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-shell",
                ["style"] = "background: red;"
            }));

        var container = cut.Find(".vibe-container");
        container.ClassList.ShouldContain("vibe-container-lg");
        container.ClassList.ShouldContain("dashboard-shell");
        container.ClassList.ShouldContain("attribute-shell");

        var style = container.GetAttribute("style")!;
        style.ShouldContain("background: red");
        style.ShouldContain("padding: 2rem");
    }

    [Fact]
    public void Container_WithEmptyPadding_DoesNotRenderStyle()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.Padding, "   "));

        cut.Find(".vibe-container").GetAttribute("style").ShouldBeNull();
    }

    [Fact]
    public void Container_WithNullContent_RendersEmpty()
    {
        var cut = Render<Container>();

        cut.Find(".vibe-container").InnerHtml.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Container_WithAriaLabel_AddsRegionSemantics()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.AriaLabel, "Dashboard content"));

        var container = cut.Find(".vibe-container");
        container.GetAttribute("role")!.ShouldBe("region");
        container.GetAttribute("aria-label")!.ShouldBe("Dashboard content");
    }

    [Fact]
    public void Container_WithCallerProvidedSemantics_PreservesCallerValues()
    {
        var cut = Render<Container>(parameters => parameters
            .Add(p => p.AriaLabel, "Parameter label")
            .Add(p => p.Role, "region")
            .AddUnmatched("role", "main")
            .AddUnmatched("aria-label", "Caller label"));

        var container = cut.Find(".vibe-container");
        container.GetAttribute("role")!.ShouldBe("main");
        container.GetAttribute("aria-label")!.ShouldBe("Caller label");
    }
}
