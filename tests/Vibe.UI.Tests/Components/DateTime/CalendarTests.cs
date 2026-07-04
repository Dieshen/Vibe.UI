namespace Vibe.UI.Tests.Components.DateTime;

public class CalendarTests : TestBase
{
    [Fact]
    public void Calendar_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Calendar>();

        // Assert
        var calendar = cut.Find(".vibe-calendar");
        calendar.ShouldNotBeNull();
        calendar.GetAttribute("role").ShouldBe("group");
        calendar.GetAttribute("aria-label").ShouldBe("Calendar");
    }

    [Fact]
    public void Calendar_ForwardsAdditionalAttributes_ToRoot()
    {
        // Act
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.AriaLabel, "Booking calendar")
            .AddUnmatched("data-testid", "calendar-root"));

        // Assert
        var calendar = cut.Find(".vibe-calendar");
        calendar.GetAttribute("aria-label").ShouldBe("Booking calendar");
        calendar.GetAttribute("data-testid").ShouldBe("calendar-root");
    }

    [Fact]
    public void Calendar_Displays_CurrentMonth()
    {
        // Arrange
        var currentDate = System.DateTime.Today;

        // Act
        var cut = Render<Calendar>();

        // Assert
        var monthYear = cut.Find(".calendar-month-year");
        monthYear.TextContent.ShouldBe(currentDate.ToString("MMMM yyyy"));
    }

    [Fact]
    public void Calendar_Displays_DayNames()
    {
        // Act
        var cut = Render<Calendar>();

        // Assert
        var dayNames = cut.FindAll(".calendar-weekday");
        dayNames.Count.ShouldBe(7);
        dayNames[0].TextContent.ShouldBe("Su");
        dayNames[0].GetAttribute("role").ShouldBe("columnheader");
        dayNames[0].GetAttribute("aria-label").ShouldBe("Sunday");
        dayNames[6].TextContent.ShouldBe("Sa");
        dayNames[6].GetAttribute("aria-label").ShouldBe("Saturday");
    }

    [Fact]
    public void Calendar_Renders_DaysInCurrentMonth()
    {
        // Arrange
        var today = System.DateTime.Today;
        var daysInMonth = System.DateTime.DaysInMonth(today.Year, today.Month);

        // Act
        var cut = Render<Calendar>();

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
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate));

        // Assert
        var selectedDay = cut.Find(".calendar-day.selected");
        selectedDay.ShouldNotBeNull();
        selectedDay.TextContent.Trim().ShouldBe("15");
        selectedDay.GetAttribute("role").ShouldBe("gridcell");
        selectedDay.GetAttribute("aria-selected").ShouldBe("true");
        selectedDay.GetAttribute("aria-label")!.ShouldContain(
            selectedDate.ToString("D", System.Globalization.CultureInfo.CurrentCulture));
        selectedDay.GetAttribute("aria-label")!.ShouldContain("selected");
    }

    [Fact]
    public void Calendar_HighlightsTodayDate()
    {
        // Arrange
        var today = System.DateTime.Today;

        // Act
        var cut = Render<Calendar>();

        // Assert
        var todayElement = cut.Find(".calendar-day.today");
        todayElement.ShouldNotBeNull();
        todayElement.GetAttribute("aria-current").ShouldBe("date");
        todayElement.GetAttribute("aria-label")!.ShouldContain("today");
    }

    [Fact]
    public void Calendar_InvokesDateSelected_WhenDayClicked()
    {
        // Arrange
        System.DateTime? selectedDate = null;
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.DateSelected, date => selectedDate = date));

        // Act
        var days = cut.FindAll(".calendar-day:not(.empty):not(.disabled)");
        days.First().Click();

        // Assert
        selectedDate.ShouldNotBeNull();
    }

    [Fact]
    public void Calendar_InvokesCallbacks_WhenDayActivatedWithKeyboard()
    {
        // Arrange
        System.DateTime? dateSelected = null;
        System.DateTime? selectedDateChanged = null;
        var date = new System.DateTime(2024, 6, 15);
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, date)
            .Add(p => p.DateSelected, selected => dateSelected = selected)
            .Add(p => p.SelectedDateChanged, selected => selectedDateChanged = selected));

        // Act
        FindDateButton(cut, date).KeyDown("Enter");

        // Assert
        dateSelected.ShouldBe(date);
        selectedDateChanged.ShouldBe(date);
    }

    [Fact]
    public void Calendar_NavigatesToPreviousMonth()
    {
        // Arrange
        var initialDate = new System.DateTime(2024, 6, 15);
        var cut = Render<Calendar>(parameters => parameters
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
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, initialDate));

        // Act
        var nextButton = cut.FindAll(".calendar-nav-button")[1];
        nextButton.Click();

        // Assert
        var monthYear = cut.Find(".calendar-month-year");
        monthYear.TextContent.ShouldBe("July 2024");
    }

    [Fact]
    public void Calendar_LabelsNavigationButtons_WithTargetMonths()
    {
        // Arrange
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, new System.DateTime(2024, 6, 15)));

        // Act
        var buttons = cut.FindAll(".calendar-nav-button");

        // Assert
        buttons[0].GetAttribute("aria-label").ShouldBe("Previous month, May 2024");
        buttons[1].GetAttribute("aria-label").ShouldBe("Next month, July 2024");
        buttons[0].QuerySelector("svg")!.GetAttribute("aria-hidden").ShouldBe("true");
        buttons[1].QuerySelector("svg")!.GetAttribute("focusable").ShouldBe("false");
    }

    [Fact]
    public void Calendar_DisablesDatesOutsideMinMaxRange()
    {
        // Arrange
        var minDate = new System.DateTime(2024, 6, 10);
        var maxDate = new System.DateTime(2024, 6, 20);

        // Act
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, new System.DateTime(2024, 6, 15))
            .Add(p => p.MinDate, minDate)
            .Add(p => p.MaxDate, maxDate));

        // Assert
        var disabledDays = cut.FindAll(".calendar-day.disabled");
        disabledDays.ShouldNotBeEmpty();
        FindDateButton(cut, new System.DateTime(2024, 6, 9)).HasAttribute("disabled").ShouldBeTrue();
        FindDateButton(cut, new System.DateTime(2024, 6, 9)).GetAttribute("aria-disabled").ShouldBe("true");
        FindDateButton(cut, new System.DateTime(2024, 6, 10)).HasAttribute("disabled").ShouldBeFalse();
        FindDateButton(cut, new System.DateTime(2024, 6, 20)).HasAttribute("disabled").ShouldBeFalse();
        FindDateButton(cut, new System.DateTime(2024, 6, 21)).HasAttribute("disabled").ShouldBeTrue();
        FindDateButton(cut, new System.DateTime(2024, 6, 21)).GetAttribute("aria-label")!.ShouldContain("unavailable");
    }

    [Fact]
    public void Calendar_DisablesMonthNavigation_WhenAdjacentMonthOutsideRange()
    {
        // Act
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, new System.DateTime(2024, 6, 15))
            .Add(p => p.MinDate, new System.DateTime(2024, 6, 1))
            .Add(p => p.MaxDate, new System.DateTime(2024, 6, 30)));

        // Assert
        var buttons = cut.FindAll(".calendar-nav-button");
        buttons[0].HasAttribute("disabled").ShouldBeTrue();
        buttons[1].HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Calendar_UpdatesDisplayedMonth_WhenSelectedDateParameterChanges()
    {
        // Arrange
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, new System.DateTime(2024, 6, 15)));

        // Act
        cut.Render(parameters => parameters
            .Add(p => p.SelectedDate, new System.DateTime(2024, 9, 10)));

        // Assert
        cut.Find(".calendar-month-year").TextContent.ShouldBe("September 2024");
        FindDateButton(cut, new System.DateTime(2024, 9, 10)).GetAttribute("aria-selected").ShouldBe("true");
    }

    [Fact]
    public void Calendar_RendersGridSemantics_ForDateGrid()
    {
        // Act
        var cut = Render<Calendar>(parameters => parameters
            .Add(p => p.SelectedDate, new System.DateTime(2024, 6, 15)));

        // Assert
        var monthYearId = cut.Find(".calendar-month-year").GetAttribute("id");
        var grid = cut.Find(".calendar-days");
        grid.GetAttribute("role").ShouldBe("grid");
        grid.GetAttribute("aria-labelledby").ShouldBe(monthYearId);
        foreach (var empty in cut.FindAll(".calendar-day.empty"))
        {
            empty.GetAttribute("role").ShouldBe("presentation");
            empty.GetAttribute("aria-hidden").ShouldBe("true");
        }
    }

    private static AngleSharp.Dom.IElement FindDateButton(IRenderedComponent<Calendar> cut, System.DateTime date)
    {
        var label = date.ToString("D", System.Globalization.CultureInfo.CurrentCulture);

        return cut.FindAll(".calendar-day:not(.empty)")
            .Single(day => day.GetAttribute("aria-label")?.StartsWith(label, System.StringComparison.Ordinal) == true);
    }
}
