namespace Vibe.UI.Tests.Components.Feedback;

public class SonnerTests : TestContext
{
    public SonnerTests() { this.AddVibeUIServices(); }
    [Fact] public void Sonner_Renders() { var cut = RenderComponent<Sonner>(); cut.Markup.Should().NotBeEmpty(); }
}
