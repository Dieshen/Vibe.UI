namespace Vibe.UI.Tests.Components.Layout;

public class AspectRatioTests : TestContext
{
    public AspectRatioTests() { this.AddVibeUIServices(); }
    [Fact] public void AspectRatio_Renders() { var cut = RenderComponent<AspectRatio>(); cut.Markup.Should().NotBeEmpty(); }
}
