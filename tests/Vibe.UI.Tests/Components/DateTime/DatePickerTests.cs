namespace Vibe.UI.Tests.Components.DateTime;

public class DatePickerTests : TestBase
{
    [Fact]
    public void DatePicker_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<DatePicker>();

        // Assert
        var datePicker = cut.Find(".vibe-datepicker");
        datePicker.ShouldNotBeNull();
    }

    [Fact]
    public void DatePicker_Displays_PlaceholderText()
    {
        // Arrange
        var placeholder = "Select a date";

        // Act
        var cut = RenderComponent<DatePicker>(parameters => parameters
            .Add(p => p.Placeholder, placeholder));

        // Assert
        var input = cut.Find(".date-input");
        input.GetAttribute("placeholder").ShouldBe(placeholder);
    }

    [Fact]
    public void DatePicker_Displays_FormattedDate()
    {
        // Arrange
        var date = new System.DateTime(2024, 6, 15);

        // Act
        var cut = RenderComponent<DatePicker>(parameters => parameters
            .Add(p => p.Date, date)
            .Add(p => p.Format, "MM/dd/yyyy"));

        // Assert
        var input = cut.Find(".date-input");
        input.GetAttribute("value").ShouldBe("06/15/2024");
    }

    [Fact]
    public void DatePicker_OpensCalendar_WhenInputClicked()
    {
        // Act
        var cut = RenderComponent<DatePicker>();
        var input = cut.Find(".date-input");
        input.Click();

        // Assert
        var popup = cut.Find(".date-popup");
        popup.ShouldNotBeNull();
    }

    [Fact]
    public void DatePicker_OpensCalendar_WhenIconClicked()
    {
        // Act
        var cut = RenderComponent<DatePicker>();
        var icon = cut.Find(".date-icon");
        icon.Click();

        // Assert
        var popup = cut.Find(".date-popup");
        popup.ShouldNotBeNull();
    }

    [Fact]
    public void DatePicker_ClosesCalendar_WhenBackdropClicked()
    {
        // Act
        var cut = RenderComponent<DatePicker>();
        var input = cut.Find(".date-input");
        input.Click();

        var backdrop = cut.Find(".date-backdrop");
        backdrop.Click();

        // Assert
        cut.FindAll(".date-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DatePicker_InvokesOnChange_WhenDateSelected()
    {
        // Arrange
        System.DateTime? selectedDate = null;
        var cut = RenderComponent<DatePicker>(parameters => parameters
            .Add(p => p.OnChange, date => selectedDate = date));

        // Act
        var input = cut.Find(".date-input");
        input.Click();

        var days = cut.FindAll(".date-day:not(.outside-month):not([disabled])");
        days.First().Click();

        // Assert
        selectedDate.ShouldNotBeNull();
    }

    [Fact]
    public void DatePicker_SelectsToday_WhenTodayButtonClicked()
    {
        // Arrange
        System.DateTime? selectedDate = null;
        var cut = RenderComponent<DatePicker>(parameters => parameters
            .Add(p => p.OnChange, date => selectedDate = date));

        // Act
        var input = cut.Find(".date-input");
        input.Click();

        var todayButton = cut.Find(".date-today-btn");
        todayButton.Click();

        // Assert
        selectedDate.ShouldNotBeNull();
        selectedDate.Value.Date.ShouldBe(System.DateTime.Today);
    }

    [Fact]
    public void DatePicker_DisablesInput_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<DatePicker>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var input = cut.Find(".date-input");
        input.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void DatePicker_NavigatesBetweenMonths()
    {
        // Act
        var cut = RenderComponent<DatePicker>();
        var input = cut.Find(".date-input");
        input.Click();

        var nextButton = cut.FindAll(".date-nav-btn")[1];
        nextButton.Click();

        // Assert
        var popup = cut.Find(".date-popup");
        popup.ShouldNotBeNull();
    }
}
