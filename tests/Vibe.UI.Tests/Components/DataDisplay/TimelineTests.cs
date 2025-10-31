namespace Vibe.UI.Tests.Components.DataDisplay;

public class TimelineTests : TestContext
{
    public TimelineTests() { this.AddVibeUIServices(); }
    [Fact] public void Timeline_Renders() { var cut = RenderComponent<Timeline>(); cut.Markup.Should().NotBeEmpty(); }
}
