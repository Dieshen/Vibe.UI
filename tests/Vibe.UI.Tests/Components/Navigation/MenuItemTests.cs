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
        item.TextContent.ShouldContain("Save");
    }

    [Fact]
    public void MenuItem_RendersIcon()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Icon, "!")
            .AddChildContent("Delete"));

        cut.Find(".menu-item-icon").TextContent.ShouldBe("!");
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
        item.ClassList.ShouldContain("vibe-menu-item-disabled");
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
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "menu-item",
                ["aria-label"] = "Save file"
            })
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
}
