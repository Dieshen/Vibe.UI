namespace Vibe.UI.Tests.Components.DateTime;

public class DatePickerTests : TestContext
{
    public DatePickerTests() { this.AddVibeUIServices(); }
    [Fact] public void DatePicker_Renders() { var cut = RenderComponent<DatePicker>(); cut.Markup.Should().NotBeEmpty(); }
}
