namespace Vibe.UI.Tests.Components.Theme;

public class ThemeProviderTests : TestBase
{
    [Fact]
    public void ThemeProvider_RendersStyleElement()
    {
        var cut = Render<ThemeProvider>();

        cut.Find("style").ShouldNotBeNull();
    }

    [Fact]
    public void ThemeProvider_RendersGeneratedThemeVariables()
    {
        var cut = Render<ThemeProvider>();

        var css = cut.Find("style").TextContent;
        css.ShouldContain(":root");
        css.ShouldContain(".dark");
        css.ShouldContain("--vibe-background:");
        css.ShouldContain("--vibe-primary:");
        css.ShouldContain("--vibe-radius:");
    }
}
