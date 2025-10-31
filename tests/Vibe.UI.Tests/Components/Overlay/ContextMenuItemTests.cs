namespace Vibe.UI.Tests.Components.Overlay;

public class ContextMenuItemTests : TestContext
{
    public ContextMenuItemTests() { this.AddVibeUIServices(); }
    [Fact] public void ContextMenuItem_Renders() { var cut = RenderComponent<ContextMenuItem>(); cut.Markup.Should().NotBeEmpty(); }
}
