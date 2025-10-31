namespace Vibe.UI.Tests.Components.Utility;

public class CommandTests : TestContext
{
    public CommandTests() { this.AddVibeUIServices(); }
    [Fact] public void Command_Renders() { var cut = RenderComponent<Command>(); cut.Markup.Should().NotBeEmpty(); }
}
