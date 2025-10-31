namespace Vibe.UI.Tests.Components.Theme;

public class ThemeSelectorTests : TestContext
{
    public ThemeSelectorTests() { this.AddVibeUIServices(); }
    [Fact] public void ThemeSelector_Renders() { var cut = RenderComponent<ThemeSelector>(); cut.Markup.Should().NotBeEmpty(); }
}
