namespace Vibe.UI.Tests.Components.DateTime;

public class CalendarTests : TestBase
{
    [Fact]
    public void Calendar_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Calendar>();

        // Assert
        var calendar = cut.Find(".vibe-calendar");
        calendar.ShouldNotBeNull();
    }

    [Fact]
    public void Calendar_Displays_CurrentMonth()
    {
        // Arrange
        var currentDate = System.DateTime.Today;

        // Act
        var cut = RenderComponent<Calendar>();

        // Assert
        var monthYear = cut.Find(".calendar-month-year");
        monthYear.TextContent.ShouldBe(currentDate.ToString("MMMM yyyy"));
    }

    [Fact]
    public void Calendar_Displays_DayNames()
    {
        // Act
        var cut = RenderComponent<Calendar>();

        // Assert
        var dayNames = cut.FindAll(".calendar-weekday");
        dayNames.Count.ShouldBe(7);
        dayNames[0].TextContent.ShouldBe("Su");
        dayNames[6].TextContent.ShouldBe("Sa");
    }

    [Fact]
    public void Calendar_Renders_DaysInCurrentMonth()
    {
        // Arrange
        var today = System.DateTime.Today;
        var daysInMonth = System.DateTime.DaysInMonth(today.Year, today.Month);

        // Act
        var cut = RenderComponent<Calendar>();

        // Assert
        var days = cut.FindAll(".calendar-day:not(.empty)");
        days.Count.ShouldBeGreaterThanOrEqualTo(daysInMonth);
    }

    [Fact]
    public void Calendar_HighlightsSelectedDate()
    {
        // Arrange
        var selectedDate = new System.DateTime(2024, 6, 15);

        // Act
        var cut = RenderComponent<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate));

        // Assert
        var selectedDay = cut.Find(".calendar-day.selected");
        selectedDay.ShouldNotBeNull();
        selectedDay.TextContent.Trim().ShouldBe("15");
    }

    [Fact]
    public void Calendar_HighlightsTodayDate()
    {
        // Arrange
        var today = System.DateTime.Today;

        // Act
        var cut = RenderComponent<Calendar>();

        // Assert
        var todayElement = cut.Find(".calendar-day.today");
        todayElement.ShouldNotBeNull();
    }

    [Fact]
    public void Calendar_InvokesDateSelected_WhenDayClicked()
    {
        // Arrange
        System.DateTime? selectedDate = null;
        var cut = RenderComponent<Calendar>(parameters => parameters
            .Add(p => p.DateSelected, date => selectedDate = date));

        // Act
        var days = cut.FindAll(".calendar-day:not(.empty):not(.disabled)");
        days.First().Click();

        // Assert
        selectedDate.ShouldNotBeNull();
    }

    [Fact]
    public void Calendar_NavigatesToPreviousMonth()
    {
        // Arrange
        var initialDate = new System.DateTime(2024, 6, 15);
        var cut = RenderComponent<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, initialDate));

        // Act
        var prevButton = cut.FindAll(".calendar-nav-button")[0];
        prevButton.Click();

        // Assert
        var monthYear = cut.Find(".calendar-month-year");
        monthYear.TextContent.ShouldBe("May 2024");
    }

    [Fact]
    public void Calendar_NavigatesToNextMonth()
    {
        // Arrange
        var initialDate = new System.DateTime(2024, 6, 15);
        var cut = RenderComponent<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, initialDate));

        // Act
        var nextButton = cut.FindAll(".calendar-nav-button")[1];
        nextButton.Click();

        // Assert
        var monthYear = cut.Find(".calendar-month-year");
        monthYear.TextContent.ShouldBe("July 2024");
    }

    [Fact]
    public void Calendar_DisablesDatesOutsideMinMaxRange()
    {
        // Arrange
        var minDate = new System.DateTime(2024, 6, 10);
        var maxDate = new System.DateTime(2024, 6, 20);

        // Act
        var cut = RenderComponent<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, new System.DateTime(2024, 6, 15))
            .Add(p => p.MinDate, minDate)
            .Add(p => p.MaxDate, maxDate));

        // Assert
        var disabledDays = cut.FindAll(".calendar-day.disabled");
        disabledDays.ShouldNotBeEmpty();
    }
}
