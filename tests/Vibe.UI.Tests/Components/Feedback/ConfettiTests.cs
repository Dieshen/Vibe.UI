namespace Vibe.UI.Tests.Components.Feedback;

public class ConfettiTests : TestContext
{
    public ConfettiTests() { this.AddVibeUIServices(); }
    [Fact] public void Confetti_Renders() { var cut = RenderComponent<Confetti>(); cut.Markup.Should().NotBeEmpty(); }
}
