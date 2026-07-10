namespace Vibe.UI.Tests.Components.Navigation;

public class MenubarTests : TestBase
{
    [Fact]
    public void Menubar_RendersBaseClassAndRole()
    {
        var cut = Render<Menubar>();

        var menubar = cut.Find(".vibe-menubar");
        menubar.GetAttribute("role").ShouldBe("menubar");
        menubar.GetAttribute("aria-label").ShouldBe("Menubar");
    }

    [Fact]
    public void Menubar_RendersMenuTriggers()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, CreateMenus()));

        var triggers = cut.FindAll(".menubar-trigger");
        triggers.Count.ShouldBe(2);
        triggers[0].TextContent.ShouldBe("File");
        triggers[0].GetAttribute("type").ShouldBe("button");
        triggers[0].GetAttribute("role").ShouldBe("menuitem");
        triggers[0].GetAttribute("tabindex").ShouldBe("0");
        triggers[0].GetAttribute("aria-haspopup").ShouldBe("menu");
        triggers[0].GetAttribute("aria-expanded").ShouldBe("false");
        triggers[0].GetAttribute("aria-controls").ShouldNotBeNullOrWhiteSpace();
        cut.Find(".menubar-menu").GetAttribute("role").ShouldBe("none");
    }

    [Fact]
    public void Menubar_OpensMenuContent_WhenTriggerIsClicked()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, CreateMenus()));

        cut.FindAll(".menubar-trigger")[0].Click();

        var trigger = cut.FindAll(".menubar-trigger")[0];
        trigger.ClassList.ShouldContain("active");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");

        var content = cut.Find(".menubar-content");
        content.TextContent.ShouldContain("New file");
        content.GetAttribute("role").ShouldBe("menu");
        content.GetAttribute("tabindex").ShouldBe("-1");
        content.GetAttribute("id").ShouldBe(trigger.GetAttribute("aria-controls"));
        content.GetAttribute("aria-labelledby").ShouldBe(trigger.GetAttribute("id"));

        cut.Find(".menubar-backdrop").GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void Menubar_ClickingActiveTriggerClosesMenu()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, CreateMenus()));

        cut.FindAll(".menubar-trigger")[0].Click();
        cut.FindAll(".menubar-trigger")[0].Click();

        cut.FindAll(".menubar-content").ShouldBeEmpty();
        cut.FindAll(".menubar-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void Menubar_SwitchesActiveMenu()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, CreateMenus()));

        cut.FindAll(".menubar-trigger")[0].Click();
        cut.FindAll(".menubar-trigger")[1].Click();

        cut.Find(".menubar-content").TextContent.ShouldContain("Undo");
        var triggers = cut.FindAll(".menubar-trigger");
        triggers[0].ClassList.ShouldNotContain("active");
        triggers[1].ClassList.ShouldContain("active");
    }

    [Fact]
    public void Menubar_BackdropClosesActiveMenu()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, CreateMenus()));

        cut.FindAll(".menubar-trigger")[0].Click();
        cut.Find(".menubar-backdrop").Click();

        cut.FindAll(".menubar-content").ShouldBeEmpty();
    }

    [Fact]
    public void Menubar_KeyboardOpensSwitchesAndClosesMenus()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, CreateMenus()));

        cut.FindAll(".menubar-trigger")[0].KeyDown("ArrowDown");

        cut.Find(".menubar-content").TextContent.ShouldContain("New file");
        cut.FindAll(".menubar-trigger")[0].GetAttribute("aria-expanded").ShouldBe("true");

        cut.FindAll(".menubar-trigger")[0].KeyDown("ArrowRight");

        cut.Find(".menubar-content").TextContent.ShouldContain("Undo");
        cut.FindAll(".menubar-trigger")[0].GetAttribute("aria-expanded").ShouldBe("false");
        cut.FindAll(".menubar-trigger")[1].GetAttribute("aria-expanded").ShouldBe("true");

        cut.Find(".menubar-content").KeyDown("Escape");

        cut.FindAll(".menubar-content").ShouldBeEmpty();
        cut.FindAll(".menubar-trigger")[1].GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void Menubar_ArrowLeftWrapsToPreviousMenu()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, CreateMenus()));

        cut.FindAll(".menubar-trigger")[0].KeyDown("ArrowLeft");

        cut.Find(".menubar-content").TextContent.ShouldContain("Undo");
        cut.FindAll(".menubar-trigger")[1].GetAttribute("aria-expanded").ShouldBe("true");
    }

    [Fact]
    public void Menubar_AppliesCustomClass()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Class, "app-menubar"));

        cut.Find(".vibe-menubar").ClassList.ShouldContain("app-menubar");
    }

    [Fact]
    public void Menubar_PreservesAdditionalAttributesAndCustomAriaLabel()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.AriaLabel, " Application commands ")
            .AddUnmatched("data-testid", "app-menubar"));

        var menubar = cut.Find(".vibe-menubar");
        menubar.GetAttribute("aria-label").ShouldBe("Application commands");
        menubar.GetAttribute("data-testid").ShouldBe("app-menubar");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Menubar_FallsBackToDefaultAriaLabel_WhenLabelIsMissing(string? ariaLabel)
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.AriaLabel, ariaLabel));

        cut.Find(".vibe-menubar").GetAttribute("aria-label").ShouldBe("Menubar");
    }

    [Fact]
    public void Menubar_RendersNoTriggers_WhenMenusIsNull()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, null!));

        cut.FindAll(".menubar-trigger").ShouldBeEmpty();
        cut.FindAll(".menubar-content").ShouldBeEmpty();
    }

    [Fact]
    public void Menubar_RendersFallbackLabelAndEmptyMenu_WhenMenuLabelAndContentAreNull()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, new List<Menubar.MenubarItem>
            {
                new()
                {
                    Id = "   ",
                    Label = null!,
                    Content = null
                }
            }));

        var trigger = cut.Find(".menubar-trigger");
        trigger.TextContent.Trim().ShouldBe("Menu 1");
        var controls = trigger.GetAttribute("aria-controls");
        controls.ShouldNotBeNull();
        controls!.ShouldContain("item-1-0");

        trigger.Click();

        var content = cut.Find(".menubar-content");
        content.TextContent.Trim().ShouldBeEmpty();
        content.GetAttribute("role").ShouldBe("menu");
    }

    private static List<Menubar.MenubarItem> CreateMenus() =>
    [
        new()
        {
            Id = "file",
            Label = "File",
            Content = builder => builder.AddContent(0, "New file")
        },
        new()
        {
            Id = "edit",
            Label = "Edit",
            Content = builder => builder.AddContent(0, "Undo")
        }
    ];
}
