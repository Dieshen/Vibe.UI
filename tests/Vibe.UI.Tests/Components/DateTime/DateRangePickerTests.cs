namespace Vibe.UI.Tests.Components.DateTime;

public class DateRangePickerTests : TestContext
{
    public DateRangePickerTests() { this.AddVibeUIServices(); }
    [Fact] public void DateRangePicker_Renders() { var cut = RenderComponent<DateRangePicker>(); cut.Markup.Should().NotBeEmpty(); }
}
