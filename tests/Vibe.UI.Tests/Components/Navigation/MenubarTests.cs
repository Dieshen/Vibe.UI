namespace Vibe.UI.Tests.Components.Navigation;

public class MenubarTests : TestBase
{
    [Fact]
    public void Menubar_RendersBaseClassAndRole()
    {
        var cut = Render<Menubar>();

        var menubar = cut.Find(".vibe-menubar");
        menubar.GetAttribute("role").ShouldBe("menubar");
    }

    [Fact]
    public void Menubar_RendersMenuTriggers()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Menus, CreateMenus()));

        var triggers = cut.FindAll(".menubar-trigger");
        triggers.Count.ShouldBe(2);
        triggers[0].TextContent.ShouldBe("File");
        triggers[0].GetAttribute("role").ShouldBe("menuitem");
        triggers[0].GetAttribute("aria-haspopup").ShouldBe("true");
        triggers[0].GetAttribute("aria-expanded").ShouldBe("false");
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
        cut.Find(".menubar-content").TextContent.ShouldContain("New file");
        cut.Find(".menubar-backdrop").ShouldNotBeNull();
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
    public void Menubar_AppliesCustomClass()
    {
        var cut = Render<Menubar>(parameters => parameters
            .Add(p => p.Class, "app-menubar"));

        cut.Find(".vibe-menubar").ClassList.ShouldContain("app-menubar");
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
