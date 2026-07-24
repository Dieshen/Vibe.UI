namespace Vibe.UI.Tests.Components.Navigation;

public class MenuItemTests : TestBase
{
    [Fact]
    public void MenuItem_RendersButtonContentAndBaseClass()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .AddChildContent("Save"));

        var item = cut.Find("button.vibe-menu-item");
        item.GetAttribute("type").ShouldBe("button");
        item.GetAttribute("role").ShouldBe("menuitem");
        item.GetAttribute("tabindex").ShouldBe("0");
        item.GetAttribute("aria-disabled").ShouldBe("false");
        item.TextContent.ShouldContain("Save");
    }

    [Fact]
    public void MenuItem_RendersIcon()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Icon, "!")
            .AddChildContent("Delete"));

        var icon = cut.Find(".menu-item-icon");
        icon.TextContent.ShouldBe("!");
        icon.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void MenuItem_RendersLucideIconByName()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.IconName, "settings")
            .AddChildContent("Settings"));

        var icon = cut.Find(".menu-item-icon");
        icon.GetAttribute("aria-hidden").ShouldBe("true");
        icon.QuerySelector("svg").ShouldNotBeNull();
        icon.TextContent.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_OmitsIcon_WhenIconIsEmpty()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Icon, string.Empty)
            .AddChildContent("Save"));

        cut.FindAll(".menu-item-icon").ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_AppliesDangerClass()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Danger, true)
            .AddChildContent("Delete"));

        cut.Find(".vibe-menu-item").ClassList.ShouldContain("vibe-menu-item-danger");
    }

    [Fact]
    public void MenuItem_AppliesDisabledState()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("Save"));

        var item = cut.Find(".vibe-menu-item");
        item.HasAttribute("disabled").ShouldBeTrue();
        item.GetAttribute("tabindex").ShouldBe("-1");
        item.GetAttribute("aria-disabled").ShouldBe("true");
        item.ClassList.ShouldContain("vibe-menu-item-disabled");
    }

    [Fact]
    public void MenuItem_AppliesDangerAndDisabledStatesTogether()
    {
        var clicked = false;
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Danger, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.OnClick, _ => clicked = true)
            .AddChildContent("Delete"));

        var item = cut.Find(".vibe-menu-item");
        item.ClassList.ShouldContain("vibe-menu-item-danger");
        item.ClassList.ShouldContain("vibe-menu-item-disabled");
        item.GetAttribute("aria-disabled").ShouldBe("true");

        item.Click();

        clicked.ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_ClickInvokesCallback_WhenEnabled()
    {
        var clicked = false;
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.OnClick, _ => clicked = true)
            .AddChildContent("Save"));

        cut.Find(".vibe-menu-item").Click();

        clicked.ShouldBeTrue();
    }

    [Fact]
    public void MenuItem_ClickDoesNotInvokeCallback_WhenDisabled()
    {
        var clicked = false;
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnClick, _ => clicked = true)
            .AddChildContent("Save"));

        cut.Find(".vibe-menu-item").Click();

        clicked.ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_PreservesAdditionalAttributes()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .AddUnmatched("data-testid", "menu-item")
            .AddUnmatched("aria-label", "Save file")
            .AddChildContent("Save"));

        var item = cut.Find(".vibe-menu-item");
        item.GetAttribute("data-testid").ShouldBe("menu-item");
        item.GetAttribute("aria-label").ShouldBe("Save file");
    }

    [Fact]
    public void MenuItem_AppliesCustomClass()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Class, "primary-action")
            .AddChildContent("Save"));

        cut.Find(".vibe-menu-item").ClassList.ShouldContain("primary-action");
    }

    [Fact]
    public void MenuItem_RendersEmptyButton_WhenChildContentAndIconAreNull()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Icon, null));

        var item = cut.Find(".vibe-menu-item");
        item.TextContent.Trim().ShouldBeEmpty();
        cut.FindAll(".menu-item-icon").ShouldBeEmpty();
    }
}
