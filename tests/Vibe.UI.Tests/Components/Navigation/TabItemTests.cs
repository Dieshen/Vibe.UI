namespace Vibe.UI.Tests.Components.Navigation;

public class TabItemTests : TestContext
{
    public TabItemTests() { this.AddVibeUIServices(); }
    [Fact] public void TabItem_Renders() { var cut = RenderComponent<TabItem>(); cut.Markup.Should().NotBeEmpty(); }
}
