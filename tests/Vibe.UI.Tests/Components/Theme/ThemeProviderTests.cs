namespace Vibe.UI.Tests.Components.Theme;

public class ThemeProviderTests : TestBase
{
    [Fact]
    public void ThemeProvider_RendersStyleElement()
    {
        var cut = RenderComponent<ThemeProvider>();

        cut.Find("style").ShouldNotBeNull();
    }

    [Fact]
    public void ThemeProvider_RendersGeneratedThemeVariables()
    {
        var cut = RenderComponent<ThemeProvider>();

        var css = cut.Find("style").TextContent;
        css.ShouldContain(":root");
        css.ShouldContain(".dark");
        css.ShouldContain("--vibe-background:");
        css.ShouldContain("--vibe-primary:");
        css.ShouldContain("--vibe-radius:");
    }
}
