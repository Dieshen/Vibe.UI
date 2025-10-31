namespace Vibe.UI.Tests.Components.Advanced;

public class VirtualScrollTests : TestContext
{
    public VirtualScrollTests() { this.AddVibeUIServices(); }
    [Fact] public void VirtualScroll_Renders() { var cut = RenderComponent<VirtualScroll>(); cut.Markup.Should().NotBeEmpty(); }
}
