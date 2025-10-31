namespace Vibe.UI.Tests.Components.Input;

public class RadioGroupItemTests : TestContext
{
    public RadioGroupItemTests() { this.AddVibeUIServices(); }
    [Fact] public void RadioGroupItem_Renders() { var cut = RenderComponent<RadioGroupItem>(); cut.Markup.Should().NotBeEmpty(); }
}
