namespace Vibe.UI.Tests.Components.Theme;

public class ThemePanelTests : TestContext
{
    public ThemePanelTests() { this.AddVibeUIServices(); }
    [Fact] public void ThemePanel_Renders() { var cut = RenderComponent<ThemePanel>(); cut.Markup.Should().NotBeEmpty(); }
}
