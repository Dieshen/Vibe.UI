namespace Vibe.UI.Tests.Components.Theme;

public class ThemeToggleTests : TestBase
{
    [Fact]
    public void ThemeToggle_RendersButtonWithDefaultAttributes()
    {
        var cut = RenderComponent<ThemeToggle>();

        var button = cut.Find("button.vibe-theme-toggle");
        button.GetAttribute("type").ShouldBe("button");
        button.GetAttribute("aria-label").ShouldBe("Toggle theme");
        button.GetAttribute("title").ShouldBe("Switch to dark mode");
        cut.Find(".vibe-theme-toggle-icon").ShouldNotBeNull();
    }

    [Fact]
    public void ThemeToggle_OmitsTooltip_WhenShowTooltipIsFalse()
    {
        var cut = RenderComponent<ThemeToggle>(parameters => parameters
            .Add(p => p.ShowTooltip, false));

        cut.Find(".vibe-theme-toggle").HasAttribute("title").ShouldBeFalse();
    }

    [Fact]
    public void ThemeToggle_RendersLabel_WhenShowLabelIsTrue()
    {
        var cut = RenderComponent<ThemeToggle>(parameters => parameters
            .Add(p => p.ShowLabel, true));

        cut.Find(".vibe-theme-toggle-label").TextContent.ShouldBe("Dark");
    }

    [Fact]
    public void ThemeToggle_TogglesLabelAndTooltipOnClick()
    {
        var cut = RenderComponent<ThemeToggle>(parameters => parameters
            .Add(p => p.ShowLabel, true)
            .Add(p => p.DarkModeTooltip, "Use dark")
            .Add(p => p.LightModeTooltip, "Use light"));

        cut.Find(".vibe-theme-toggle").Click();

        cut.WaitForAssertion(() =>
        {
            cut.Find(".vibe-theme-toggle-label").TextContent.ShouldBe("Light");
            cut.Find(".vibe-theme-toggle").GetAttribute("title").ShouldBe("Use light");
        });
    }

    [Fact]
    public void ThemeToggle_RendersCustomDarkIconInLightMode()
    {
        var cut = RenderComponent<ThemeToggle>(parameters => parameters
            .Add(p => p.DarkIcon, builder => builder.AddMarkupContent(0, "<span data-testid='moon'>moon</span>")));

        cut.Find("[data-testid='moon']").TextContent.ShouldBe("moon");
    }

    [Fact]
    public void ThemeToggle_RendersCustomLightIconAfterSwitchingToDarkMode()
    {
        var cut = RenderComponent<ThemeToggle>(parameters => parameters
            .Add(p => p.LightIcon, builder => builder.AddMarkupContent(0, "<span data-testid='sun'>sun</span>")));

        cut.Find(".vibe-theme-toggle").Click();

        cut.WaitForAssertion(() => cut.Find("[data-testid='sun']").TextContent.ShouldBe("sun"));
    }

    [Fact]
    public void ThemeToggle_PreservesAdditionalAttributes()
    {
        var cut = RenderComponent<ThemeToggle>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "theme-toggle",
                ["aria-pressed"] = "false"
            }));

        var button = cut.Find(".vibe-theme-toggle");
        button.GetAttribute("data-testid").ShouldBe("theme-toggle");
        button.GetAttribute("aria-pressed").ShouldBe("false");
    }

    [Fact]
    public void ThemeToggle_AppliesCustomClass()
    {
        var cut = RenderComponent<ThemeToggle>(parameters => parameters
            .Add(p => p.Class, "toolbar-theme-toggle"));

        cut.Find(".vibe-theme-toggle").ClassList.ShouldContain("toolbar-theme-toggle");
    }
}
