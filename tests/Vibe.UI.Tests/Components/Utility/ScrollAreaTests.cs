namespace Vibe.UI.Tests.Components.Utility;

public class ScrollAreaTests : TestContext
{
    public ScrollAreaTests() { this.AddVibeUIServices(); }
    [Fact] public void ScrollArea_Renders() { var cut = RenderComponent<ScrollArea>(); cut.Markup.Should().NotBeEmpty(); }
}
