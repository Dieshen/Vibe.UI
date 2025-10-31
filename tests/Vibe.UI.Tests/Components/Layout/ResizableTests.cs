namespace Vibe.UI.Tests.Components.Layout;

public class ResizableTests : TestContext
{
    public ResizableTests() { this.AddVibeUIServices(); }
    [Fact] public void Resizable_Renders() { var cut = RenderComponent<Resizable>(); cut.Markup.Should().NotBeEmpty(); }
}
