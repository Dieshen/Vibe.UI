namespace Vibe.UI.Tests.Components.Disclosure;

public class CollapsibleTests : TestContext
{
    public CollapsibleTests() { this.AddVibeUIServices(); }
    [Fact] public void Collapsible_Renders() { var cut = RenderComponent<Collapsible>(); cut.Markup.Should().NotBeEmpty(); }
}
