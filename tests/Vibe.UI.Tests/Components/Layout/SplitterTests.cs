namespace Vibe.UI.Tests.Components.Layout;

public class SplitterTests : TestContext
{
    public SplitterTests() { this.AddVibeUIServices(); }
    [Fact] public void Splitter_Renders() { var cut = RenderComponent<Splitter>(); cut.Markup.Should().NotBeEmpty(); }
}
