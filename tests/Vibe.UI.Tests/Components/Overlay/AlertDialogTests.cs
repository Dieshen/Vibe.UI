namespace Vibe.UI.Tests.Components.Overlay;

public class AlertDialogTests : TestContext
{
    public AlertDialogTests() { this.AddVibeUIServices(); }
    [Fact] public void AlertDialog_Renders() { var cut = RenderComponent<AlertDialog>(); cut.Markup.Should().NotBeEmpty(); }
}
