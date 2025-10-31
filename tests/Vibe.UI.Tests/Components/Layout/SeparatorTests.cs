namespace Vibe.UI.Tests.Components.Layout;

public class SeparatorTests : TestContext
{
    public SeparatorTests() { this.AddVibeUIServices(); }
    [Fact] public void Separator_Renders() { var cut = RenderComponent<Separator>(); cut.Find(".vibe-separator").Should().NotBeNull(); }
}
