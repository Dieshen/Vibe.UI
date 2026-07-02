namespace Vibe.UI.Tests.Components.Navigation;

public class MenuTests : TestBase
{
    [Fact]
    public void Menu_RendersDefaultTriggerAndBaseClass()
    {
        var cut = RenderComponent<Menu>();

        cut.Find(".vibe-menu").ShouldNotBeNull();
        var button = cut.Find(".vibe-menu-trigger button");
        button.GetAttribute("type").ShouldBe("button");
        button.TextContent.Trim().ShouldBe("Menu");
    }

    [Fact]
    public void Menu_RendersCustomTrigger()
    {
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.Trigger, builder => builder.AddMarkupContent(0, "<button class='custom-trigger'>Actions</button>")));

        cut.Find(".custom-trigger").TextContent.ShouldBe("Actions");
    }

    [Fact]
    public void Menu_DoesNotRenderContent_WhenClosed()
    {
        var cut = RenderComponent<Menu>(parameters => parameters
            .AddChildContent("Menu content"));

        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
        cut.FindAll(".menu-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_RendersContentAndBackdrop_WhenOpen()
    {
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent("<button>Delete</button>"));

        cut.Find(".vibe-menu-content").TextContent.ShouldContain("Delete");
        cut.Find(".menu-backdrop").ShouldNotBeNull();
    }

    [Fact]
    public void Menu_ToggleInvokesIsOpenChanged()
    {
        bool? changedValue = null;
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.IsOpenChanged, value => changedValue = value));

        cut.Find(".vibe-menu-trigger").Click();

        changedValue.ShouldBe(true);
        cut.Find(".vibe-menu-content").ShouldNotBeNull();
    }

    [Fact]
    public void Menu_BackdropClosesAndInvokesIsOpenChanged()
    {
        bool? changedValue = null;
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.IsOpenChanged, value => changedValue = value)
            .AddChildContent("Menu content"));

        cut.Find(".menu-backdrop").Click();

        changedValue.ShouldBe(false);
        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_OmitsBackdrop_WhenCloseOnClickOutsideIsFalse()
    {
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnClickOutside, false)
            .AddChildContent("Menu content"));

        cut.Find(".vibe-menu-content").ShouldNotBeNull();
        cut.FindAll(".menu-backdrop").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(Menu.MenuPlacement.BottomStart, "top: 100%; left: 0; margin-top: 0.25rem;")]
    [InlineData(Menu.MenuPlacement.BottomEnd, "top: 100%; right: 0; margin-top: 0.25rem;")]
    [InlineData(Menu.MenuPlacement.TopStart, "bottom: 100%; left: 0; margin-bottom: 0.25rem;")]
    [InlineData(Menu.MenuPlacement.TopEnd, "bottom: 100%; right: 0; margin-bottom: 0.25rem;")]
    public void Menu_AppliesPlacementStyle(Menu.MenuPlacement placement, string expectedStyle)
    {
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Placement, placement)
            .AddChildContent("Menu content"));

        cut.Find(".vibe-menu-content").GetAttribute("style").ShouldBe(expectedStyle);
    }

    [Fact]
    public void Menu_PreservesAdditionalAttributes()
    {
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "menu",
                ["aria-label"] = "Actions"
            }));

        var menu = cut.Find(".vibe-menu");
        menu.GetAttribute("data-testid").ShouldBe("menu");
        menu.GetAttribute("aria-label").ShouldBe("Actions");
    }

    [Fact]
    public void Menu_AppliesCustomClass()
    {
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.Class, "dense-menu"));

        cut.Find(".vibe-menu").ClassList.ShouldContain("dense-menu");
    }
}
