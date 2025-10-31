namespace Vibe.UI.Tests.Components.Utility;

public class QRCodeTests : TestContext
{
    public QRCodeTests() { this.AddVibeUIServices(); }
    [Fact] public void QRCode_Renders() { var cut = RenderComponent<QRCode>(parameters => parameters.Add(p => p.Value, "test")); cut.Markup.Should().NotBeEmpty(); }
}
