namespace Vibe.UI.Tests.Components.Theme;

public class ThemeToggleTests : TestContext
{
    public ThemeToggleTests() { this.AddVibeUIServices(); }
    [Fact] public void ThemeToggle_Renders() { var cut = RenderComponent<ThemeToggle>(); cut.Markup.Should().NotBeEmpty(); }
}
