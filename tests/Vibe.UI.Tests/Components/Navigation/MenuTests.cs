namespace Vibe.UI.Tests.Components.Navigation;

public class MenuTests : TestBase
{
    [Fact]
    public void Menu_RendersDefaultTriggerAndBaseClass()
    {
        var cut = Render<Menu>();

        var root = cut.Find(".vibe-menu");
        root.GetAttribute("data-state").ShouldBe("closed");

        var trigger = cut.Find(".vibe-menu-trigger");
        trigger.GetAttribute("role").ShouldBe("button");
        trigger.GetAttribute("tabindex").ShouldBe("0");
        trigger.GetAttribute("aria-haspopup").ShouldBe("menu");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-disabled").ShouldBe("false");
        trigger.GetAttribute("aria-controls").ShouldNotBeNullOrWhiteSpace();
        trigger.GetAttribute("data-state").ShouldBe("closed");

        var defaultTrigger = cut.Find(".vibe-menu-default-trigger");
        defaultTrigger.TextContent.Trim().ShouldBe("Menu");
        cut.FindAll(".vibe-menu-trigger button").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_RendersCustomTrigger()
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.Trigger, builder => builder.AddMarkupContent(0, "<button class='custom-trigger'>Actions</button>")));

        cut.Find(".custom-trigger").TextContent.ShouldBe("Actions");
    }

    [Fact]
    public void Menu_DoesNotRenderContent_WhenClosed()
    {
        var cut = Render<Menu>(parameters => parameters
            .AddChildContent("Menu content"));

        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
        cut.FindAll(".menu-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_RendersContentAndBackdrop_WhenOpen()
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.MenuAriaLabel, "Actions")
            .AddChildContent("<button>Delete</button>"));

        var root = cut.Find(".vibe-menu");
        root.ClassList.ShouldContain("vibe-menu-open");
        root.GetAttribute("data-state").ShouldBe("open");

        var trigger = cut.Find(".vibe-menu-trigger");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
        trigger.GetAttribute("data-state").ShouldBe("open");

        cut.Find(".vibe-menu-content").TextContent.ShouldContain("Delete");
        cut.Find(".vibe-menu-content").GetAttribute("role").ShouldBe("menu");
        cut.Find(".vibe-menu-content").GetAttribute("id").ShouldBe(trigger.GetAttribute("aria-controls"));
        cut.Find(".vibe-menu-content").GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".vibe-menu-content").GetAttribute("aria-label").ShouldBe("Actions");
        cut.Find(".vibe-menu-content").GetAttribute("aria-orientation").ShouldBe("vertical");
        cut.Find(".vibe-menu-content").GetAttribute("data-state").ShouldBe("open");
        cut.Find(".menu-backdrop").GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void Menu_ToggleInvokesIsOpenChangedOnTransitions()
    {
        var changedValues = new List<bool>();
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpenChanged, value => changedValues.Add(value))
            .AddChildContent("Menu content"));

        cut.Find(".vibe-menu-trigger").Click();

        changedValues.ShouldBe([true]);
        cut.Find(".vibe-menu-content").ShouldNotBeNull();

        cut.Find(".vibe-menu-trigger").Click();

        changedValues.ShouldBe([true, false]);
        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
        cut.Find(".vibe-menu").GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact]
    public void Menu_TogglesWithEnterAndSpacebarAndClosesWithEscape()
    {
        var changedValues = new List<bool>();
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpenChanged, value => changedValues.Add(value))
            .AddChildContent("Menu content"));

        cut.Find(".vibe-menu-trigger").KeyDown("Enter");
        changedValues.ShouldBe([true]);
        cut.Find(".vibe-menu-content").ShouldNotBeNull();

        cut.Find(".vibe-menu-content").KeyDown("Escape");
        changedValues.ShouldBe([true, false]);
        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();

        cut.Find(".vibe-menu-trigger").KeyDown("Spacebar");
        changedValues.ShouldBe([true, false, true]);
        cut.Find(".vibe-menu-content").ShouldNotBeNull();

        cut.Find(".vibe-menu-trigger").KeyDown(" ");
        changedValues.ShouldBe([true, false, true, false]);
        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_ArrowKeysOpenMenuWithoutRepeatingCallback_WhenAlreadyOpen()
    {
        var changedValues = new List<bool>();
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpenChanged, value => changedValues.Add(value))
            .AddChildContent("Menu content"));

        cut.Find(".vibe-menu-trigger").KeyDown("ArrowDown");

        changedValues.ShouldBe([true]);
        cut.Find(".vibe-menu-content").ShouldNotBeNull();

        cut.Find(".vibe-menu-trigger").KeyDown("ArrowDown");

        changedValues.ShouldBe([true]);

        cut.Find(".vibe-menu-content").KeyDown("Escape");
        cut.Find(".vibe-menu-trigger").KeyDown("ArrowUp");

        changedValues.ShouldBe([true, false, true]);
        cut.Find(".vibe-menu-content").ShouldNotBeNull();
    }

    [Fact]
    public void Menu_EscapeDoesNotInvokeCallback_WhenAlreadyClosed()
    {
        var closeCount = 0;
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpenChanged, value =>
            {
                if (!value)
                {
                    closeCount++;
                }
            }));

        cut.Find(".vibe-menu-trigger").KeyDown("Escape");

        closeCount.ShouldBe(0);
    }

    [Fact]
    public void Menu_BackdropClosesAndInvokesIsOpenChanged()
    {
        bool? changedValue = null;
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.IsOpenChanged, value => changedValue = value)
            .AddChildContent("Menu content"));

        cut.Find(".menu-backdrop").Click();

        changedValue.ShouldBe(false);
        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_CloseOnSelectClosesAfterMenuItemClick()
    {
        var itemClickCount = 0;
        var changedValues = new List<bool>();
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.IsOpenChanged, value => changedValues.Add(value))
            .AddChildContent<MenuItem>(item => item
                .Add(p => p.OnClick, _ => itemClickCount++)
                .AddChildContent("Save")));

        cut.Find(".vibe-menu-item").Click();

        itemClickCount.ShouldBe(1);
        changedValues.ShouldBe([false]);
        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_CloseOnSelectFalseKeepsMenuOpenAfterContentClick()
    {
        var changedValues = new List<bool>();
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnSelect, false)
            .Add(p => p.IsOpenChanged, value => changedValues.Add(value))
            .AddChildContent(builder => builder.AddMarkupContent(0, "<button class='inner-action'>Save</button>")));

        cut.Find(".inner-action").Click();

        changedValues.ShouldBeEmpty();
        cut.Find(".vibe-menu-content").ShouldNotBeNull();
        cut.Find(".vibe-menu-trigger").GetAttribute("aria-expanded").ShouldBe("true");
    }

    [Fact]
    public void Menu_OmitsBackdrop_WhenCloseOnClickOutsideIsFalse()
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnClickOutside, false)
            .AddChildContent("Menu content"));

        cut.Find(".vibe-menu-content").ShouldNotBeNull();
        cut.FindAll(".menu-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_DisabledStatePreventsPointerAndKeyboardOpen()
    {
        var changedValues = new List<bool>();
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.IsOpenChanged, value => changedValues.Add(value))
            .AddChildContent("Menu content"));

        var root = cut.Find(".vibe-menu");
        root.ClassList.ShouldContain("vibe-menu-disabled");
        root.GetAttribute("data-state").ShouldBe("closed");

        var trigger = cut.Find(".vibe-menu-trigger");
        trigger.ClassList.ShouldContain("vibe-menu-trigger-disabled");
        trigger.GetAttribute("tabindex").ShouldBe("-1");
        trigger.GetAttribute("aria-disabled").ShouldBe("true");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");

        trigger.Click();
        trigger.KeyDown("Enter");
        trigger.KeyDown("ArrowDown");

        changedValues.ShouldBeEmpty();
        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_DisabledStateClosesInitiallyOpenMenuWithoutCallback()
    {
        var changedValues = new List<bool>();
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.IsOpenChanged, value => changedValues.Add(value))
            .AddChildContent("Menu content"));

        cut.Find(".vibe-menu").GetAttribute("data-state").ShouldBe("closed");
        cut.Find(".vibe-menu-trigger").GetAttribute("aria-expanded").ShouldBe("false");
        cut.FindAll(".vibe-menu-content").ShouldBeEmpty();
        changedValues.ShouldBeEmpty();
    }

    [Fact]
    public void Menu_RendersEmptyMenuAndFallbackAriaLabel_WhenContentAndLabelAreNull()
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.MenuAriaLabel, (string?)null));

        var content = cut.Find(".vibe-menu-content");
        content.TextContent.ShouldBe(string.Empty);
        content.GetAttribute("aria-label").ShouldBe("Menu");
    }

    [Theory]
    [InlineData(Menu.MenuPlacement.BottomStart, "top: 100%; left: 0; margin-top: 0.25rem;")]
    [InlineData(Menu.MenuPlacement.BottomEnd, "top: 100%; right: 0; margin-top: 0.25rem;")]
    [InlineData(Menu.MenuPlacement.TopStart, "bottom: 100%; left: 0; margin-bottom: 0.25rem;")]
    [InlineData(Menu.MenuPlacement.TopEnd, "bottom: 100%; right: 0; margin-bottom: 0.25rem;")]
    public void Menu_AppliesPlacementStyle(Menu.MenuPlacement placement, string expectedStyle)
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Placement, placement)
            .AddChildContent("Menu content"));

        cut.Find(".vibe-menu-content").GetAttribute("style").ShouldBe(expectedStyle);
    }

    [Fact]
    public void Menu_PreservesAdditionalAttributes()
    {
        var cut = Render<Menu>(parameters => parameters
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
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.Class, "dense-menu"));

        cut.Find(".vibe-menu").ClassList.ShouldContain("dense-menu");
    }
}
