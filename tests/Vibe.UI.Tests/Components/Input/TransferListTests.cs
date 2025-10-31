namespace Vibe.UI.Tests.Components.Input;

public class TransferListTests : TestContext
{
    public TransferListTests() { this.AddVibeUIServices(); }
    [Fact] public void TransferList_Renders() { var cut = RenderComponent<TransferList>(); cut.Markup.Should().NotBeEmpty(); }
}
