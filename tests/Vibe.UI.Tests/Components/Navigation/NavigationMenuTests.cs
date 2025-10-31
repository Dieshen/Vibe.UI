namespace Vibe.UI.Tests.Components.Navigation;

public class NavigationMenuTests : TestContext
{
    public NavigationMenuTests() { this.AddVibeUIServices(); }
    [Fact] public void NavigationMenu_Renders() { var cut = RenderComponent<NavigationMenu>(); cut.Markup.Should().NotBeEmpty(); }
}
