namespace Vibe.UI.Tests.Components.Input;

public class ToggleGroupItemTests : TestContext
{
    public ToggleGroupItemTests() { this.AddVibeUIServices(); }
    [Fact] public void ToggleGroupItem_Renders() { var cut = RenderComponent<ToggleGroupItem>(); cut.Markup.Should().NotBeEmpty(); }
}
