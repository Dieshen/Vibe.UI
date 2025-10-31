namespace Vibe.UI.Tests.Components.DateTime;

public class CalendarTests : TestContext
{
    public CalendarTests() { this.AddVibeUIServices(); }
    [Fact] public void Calendar_Renders() { var cut = RenderComponent<Calendar>(); cut.Markup.Should().NotBeEmpty(); }
}
