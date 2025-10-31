namespace Vibe.UI.Tests.Components.Layout;

public class MasonryGridTests : TestContext
{
    public MasonryGridTests() { this.AddVibeUIServices(); }
    [Fact] public void MasonryGrid_Renders() { var cut = RenderComponent<MasonryGrid>(); cut.Markup.Should().NotBeEmpty(); }
}
