namespace Vibe.UI.Tests.Components.Navigation;

public class MenuSeparatorTests : TestBase
{
    [Fact]
    public void MenuSeparator_RendersBaseClass()
    {
        var cut = Render<MenuSeparator>();

        cut.Find(".vibe-menu-separator").ShouldNotBeNull();
    }

    [Fact]
    public void MenuSeparator_AppliesCustomClass()
    {
        var cut = Render<MenuSeparator>(parameters => parameters
            .Add(p => p.Class, "menu-divider"));

        cut.Find(".vibe-menu-separator").ClassList.ShouldContain("menu-divider");
    }

    [Fact]
    public void MenuSeparator_PreservesAdditionalAttributes()
    {
        var cut = Render<MenuSeparator>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "menu-separator",
                ["role"] = "separator"
            }));

        var separator = cut.Find(".vibe-menu-separator");
        separator.GetAttribute("data-testid").ShouldBe("menu-separator");
        separator.GetAttribute("role").ShouldBe("separator");
    }
}
