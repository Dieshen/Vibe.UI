namespace Vibe.UI.Tests.Components.Theme;

public class ThemeToggleTests : TestBase
{
    private const string ModulePath = "./_content/Vibe.UI/js/vibe-theme.js";

    [Fact]
    public void ThemeToggle_RendersButtonWithDefaultAttributes()
    {
        var cut = Render<ThemeToggle>();

        var button = cut.Find("button.vibe-theme-toggle");
        button.GetAttribute("type").ShouldBe("button");
        button.GetAttribute("aria-label").ShouldBe("Toggle theme");
        button.GetAttribute("aria-pressed").ShouldBe("false");
        button.GetAttribute("title").ShouldBe("Switch to dark mode");
        button.HasAttribute("disabled").ShouldBeFalse();
        cut.Find(".vibe-theme-toggle-icon").ShouldNotBeNull();
    }

    [Fact]
    public void ThemeToggle_OmitsTooltip_WhenShowTooltipIsFalse()
    {
        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.ShowTooltip, false));

        cut.Find(".vibe-theme-toggle").HasAttribute("title").ShouldBeFalse();
    }

    [Fact]
    public void ThemeToggle_RendersLabel_WhenShowLabelIsTrue()
    {
        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.ShowLabel, true));

        cut.Find(".vibe-theme-toggle-label").TextContent.ShouldBe("Dark");
    }

    [Fact]
    public void ThemeToggle_TogglesLabelAndTooltipOnClick()
    {
        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.ShowLabel, true)
            .Add(p => p.DarkModeTooltip, "Use dark")
            .Add(p => p.LightModeTooltip, "Use light"));

        cut.Find(".vibe-theme-toggle").Click();

        cut.WaitForAssertion(() =>
        {
            cut.Find(".vibe-theme-toggle-label").TextContent.ShouldBe("Light");
            cut.Find(".vibe-theme-toggle").GetAttribute("title").ShouldBe("Use light");
            cut.Find(".vibe-theme-toggle").GetAttribute("aria-pressed").ShouldBe("true");
        });
    }

    [Fact]
    public void ThemeToggle_RendersDisabledState()
    {
        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.Disabled, true));

        var button = cut.Find(".vibe-theme-toggle");
        button.HasAttribute("disabled").ShouldBeTrue();
        button.ClassList.ShouldContain("vibe-theme-toggle-disabled");
        button.GetAttribute("aria-pressed").ShouldBe("false");
    }

    [Fact]
    public void ThemeToggle_DoesNotToggle_WhenDisabled()
    {
        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ShowLabel, true));

        cut.WaitForAssertion(() =>
            JSInterop.Invocations.Any(invocation => invocation.Identifier == "VibeTheme.setTheme").ShouldBeTrue());
        var invocationCount = JSInterop.Invocations.Count;

        cut.Find(".vibe-theme-toggle").Click();

        cut.Find(".vibe-theme-toggle-label").TextContent.ShouldBe("Dark");
        cut.Find(".vibe-theme-toggle").GetAttribute("aria-pressed").ShouldBe("false");
        JSInterop.Invocations.Count.ShouldBe(invocationCount);
    }

    [Fact]
    public void ThemeToggle_RendersCustomDarkIconInLightMode()
    {
        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.DarkIcon, builder => builder.AddMarkupContent(0, "<span data-testid='moon'>moon</span>")));

        cut.Find("[data-testid='moon']").TextContent.ShouldBe("moon");
    }

    [Fact]
    public void ThemeToggle_RendersCustomLightIconAfterSwitchingToDarkMode()
    {
        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.LightIcon, builder => builder.AddMarkupContent(0, "<span data-testid='sun'>sun</span>")));

        cut.Find(".vibe-theme-toggle").Click();

        cut.WaitForAssertion(() => cut.Find("[data-testid='sun']").TextContent.ShouldBe("sun"));
    }

    [Fact]
    public void ThemeToggle_PreservesAdditionalAttributes()
    {
        var cut = Render<ThemeToggle>(parameters => parameters
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
        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.Class, "toolbar-theme-toggle"));

        cut.Find(".vibe-theme-toggle").ClassList.ShouldContain("toolbar-theme-toggle");
    }

    [Fact]
    public void ThemeToggle_UsesConfiguredStorageKey_ForThemeInterop()
    {
        JSInterop.SetupModule(ModulePath);

        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.StorageKey, "custom-theme-key"));

        cut.Find(".vibe-theme-toggle").Click();

        cut.WaitForAssertion(() =>
        {
            var setThemeInvocations = JSInterop.Invocations
                .Where(invocation => invocation.Identifier == "VibeTheme.setTheme")
                .ToList();

            setThemeInvocations.ShouldNotBeEmpty();
            setThemeInvocations
                .Select(invocation => Convert.ToString(invocation.Arguments[0]))
                .ShouldAllBe(storageKey => storageKey == "custom-theme-key");
            setThemeInvocations.Last().Arguments[1].ShouldBe(true);
        });
    }

    [Fact]
    public void ThemeToggle_UsesDefaultStorageKey_WhenStorageKeyIsBlank()
    {
        JSInterop.SetupModule(ModulePath);

        var cut = Render<ThemeToggle>(parameters => parameters
            .Add(p => p.StorageKey, " "));

        cut.Find(".vibe-theme-toggle").Click();

        cut.WaitForAssertion(() =>
        {
            var setThemeInvocations = JSInterop.Invocations
                .Where(invocation => invocation.Identifier == "VibeTheme.setTheme")
                .ToList();

            setThemeInvocations.ShouldNotBeEmpty();
            setThemeInvocations
                .Select(invocation => Convert.ToString(invocation.Arguments[0]))
                .ShouldAllBe(storageKey => storageKey == "vibe-theme");
        });
    }
}
