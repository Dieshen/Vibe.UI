namespace Vibe.UI.Tests.Components.Feedback;

public class ToastContainerTests : TestContext
{
    public ToastContainerTests() { this.AddVibeUIServices(); }
    [Fact] public void ToastContainer_Renders() { var cut = RenderComponent<ToastContainer>(); cut.Markup.Should().NotBeEmpty(); }
}
