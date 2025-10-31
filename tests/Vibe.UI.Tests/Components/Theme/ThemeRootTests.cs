namespace Vibe.UI.Tests.Components.Theme;

public class ThemeRootTests : TestContext
{
    public ThemeRootTests() { this.AddVibeUIServices(); }
    [Fact] public void ThemeRoot_Renders() { var cut = RenderComponent<ThemeRoot>(); cut.Markup.Should().NotBeEmpty(); }
}
