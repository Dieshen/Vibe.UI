namespace Vibe.UI.Tests.Components.Feedback;

public class NotificationCenterTests : TestContext
{
    public NotificationCenterTests() { this.AddVibeUIServices(); }
    [Fact] public void NotificationCenter_Renders() { var cut = RenderComponent<NotificationCenter>(); cut.Markup.Should().NotBeEmpty(); }
}
