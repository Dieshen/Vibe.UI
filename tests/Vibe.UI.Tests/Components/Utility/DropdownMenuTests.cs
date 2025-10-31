namespace Vibe.UI.Tests.Components.Utility;

public class DropdownMenuTests : TestContext
{
    public DropdownMenuTests() { this.AddVibeUIServices(); }
    [Fact] public void DropdownMenu_Renders() { var cut = RenderComponent<DropdownMenu>(); cut.Markup.Should().NotBeEmpty(); }
}
