namespace Vibe.UI.Tests.Components.Overlay;

public class HoverCardTests : TestContext
{
    public HoverCardTests() { this.AddVibeUIServices(); }
    [Fact] public void HoverCard_Renders() { var cut = RenderComponent<HoverCard>(); cut.Markup.Should().NotBeEmpty(); }
}
