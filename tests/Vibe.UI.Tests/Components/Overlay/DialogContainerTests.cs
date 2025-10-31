namespace Vibe.UI.Tests.Components.Overlay;

public class DialogContainerTests : TestContext
{
    public DialogContainerTests() { this.AddVibeUIServices(); }
    [Fact] public void DialogContainer_Renders() { var cut = RenderComponent<DialogContainer>(); cut.Markup.Should().NotBeEmpty(); }
}
