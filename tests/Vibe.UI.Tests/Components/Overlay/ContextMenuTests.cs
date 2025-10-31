namespace Vibe.UI.Tests.Components.Overlay;

public class ContextMenuTests : TestContext
{
    public ContextMenuTests() { this.AddVibeUIServices(); }
    [Fact] public void ContextMenu_Renders() { var cut = RenderComponent<ContextMenu>(); cut.Markup.Should().NotBeEmpty(); }
}
