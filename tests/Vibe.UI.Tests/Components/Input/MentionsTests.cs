namespace Vibe.UI.Tests.Components.Input;

public class MentionsTests : TestContext
{
    public MentionsTests() { this.AddVibeUIServices(); }
    [Fact] public void Mentions_Renders() { var cut = RenderComponent<Mentions>(); cut.Markup.Should().NotBeEmpty(); }
}
