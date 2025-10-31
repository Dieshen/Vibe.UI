namespace Vibe.UI.Tests.Components.Navigation;

public class NavigationMenuItemTests : TestContext
{
    public NavigationMenuItemTests() { this.AddVibeUIServices(); }
    [Fact] public void NavigationMenuItem_Renders() { var cut = RenderComponent<NavigationMenuItem>(); cut.Markup.Should().NotBeEmpty(); }
}
