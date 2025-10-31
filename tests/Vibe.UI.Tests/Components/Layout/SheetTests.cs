namespace Vibe.UI.Tests.Components.Layout;

public class SheetTests : TestContext
{
    public SheetTests() { this.AddVibeUIServices(); }
    [Fact] public void Sheet_Renders() { var cut = RenderComponent<Sheet>(); cut.Markup.Should().NotBeEmpty(); }
}
