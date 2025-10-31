namespace Vibe.UI.Tests.Components.Feedback;

public class EmptyStateTests : TestContext
{
    public EmptyStateTests() { this.AddVibeUIServices(); }
    [Fact] public void EmptyState_Renders() { var cut = RenderComponent<EmptyState>(); cut.Markup.Should().NotBeEmpty(); }
}
