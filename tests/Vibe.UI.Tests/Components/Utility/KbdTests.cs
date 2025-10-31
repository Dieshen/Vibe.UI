namespace Vibe.UI.Tests.Components.Utility;

public class KbdTests : TestContext
{
    public KbdTests() { this.AddVibeUIServices(); }
    [Fact] public void Kbd_Renders() { var cut = RenderComponent<Kbd>(); cut.Markup.Should().NotBeEmpty(); }
}
