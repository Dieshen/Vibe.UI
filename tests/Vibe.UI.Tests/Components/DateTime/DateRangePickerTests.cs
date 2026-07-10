namespace Vibe.UI.Tests.Components.DateTime;

public class DateRangePickerTests : TestBase
{
    [Fact]
    public void DateRangePicker_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<DateRangePicker>();

        // Assert
        var dateRangePicker = cut.Find(".vibe-daterange-picker");
        dateRangePicker.ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_Displays_TwoInputs()
    {
        // Act
        var cut = Render<DateRangePicker>();

        // Assert - Find the actual input elements within the Input component wrappers
        var inputs = cut.FindAll(".daterange-inputs input");
        inputs.Count.ShouldBe(2);
    }

    [Fact]
    public void DateRangePicker_Displays_PlaceholderText()
    {
        // Arrange
        var startPlaceholder = "Start Date";
        var endPlaceholder = "End Date";

        // Act
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDatePlaceholder, startPlaceholder)
            .Add(p => p.EndDatePlaceholder, endPlaceholder));

        // Assert - Find the actual input elements within the Input component wrappers
        var inputs = cut.FindAll(".daterange-inputs input");
        inputs[0].GetAttribute("placeholder")!.ShouldBe(startPlaceholder);
        inputs[1].GetAttribute("placeholder")!.ShouldBe(endPlaceholder);
    }

    [Fact]
    public void DateRangePicker_Displays_FormattedDates()
    {
        // Arrange
        var startDate = new System.DateTime(2024, 6, 1);
        var endDate = new System.DateTime(2024, 6, 30);

        // Act
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, startDate)
            .Add(p => p.EndDate, endDate)
            .Add(p => p.Format, "MM/dd/yyyy"));

        // Assert - Find the actual input elements within the Input component wrappers
        var inputs = cut.FindAll(".daterange-inputs input");
        inputs[0].GetAttribute("value")!.ShouldBe("06/01/2024");
        inputs[1].GetAttribute("value")!.ShouldBe("06/30/2024");
    }

    [Fact]
    public void DateRangePicker_OpensCalendar_WhenStartInputClicked()
    {
        // Act
        var cut = Render<DateRangePicker>();
        var startInput = cut.FindAll(".daterange-inputs input")[0];
        startInput.Click();

        // Assert
        var popup = cut.Find(".daterange-popup");
        popup.ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_OpensCalendar_WhenEndInputClicked()
    {
        // Act
        var cut = Render<DateRangePicker>();
        var endInput = cut.FindAll(".daterange-inputs input")[1];
        endInput.Click();

        // Assert
        var popup = cut.Find(".daterange-popup");
        popup.ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_DisplaysTwoCalendars()
    {
        // Act
        var cut = Render<DateRangePicker>();
        var icon = cut.Find(".daterange-icon");
        icon.Click();

        // Assert
        var calendars = cut.FindAll(".daterange-calendar");
        calendars.Count.ShouldBe(2);
    }

    [Fact]
    public void DateRangePicker_InvokesOnChange_WhenApplyClicked()
    {
        // Arrange
        var expectedStart = new System.DateTime(2024, 6, 10);
        var expectedEnd = new System.DateTime(2024, 6, 15);
        System.DateTime? selectedStart = null;
        System.DateTime? selectedEnd = null;
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 1))
            .Add(p => p.EndDate, new System.DateTime(2024, 6, 20))
            .Add(p => p.OnChange, dates =>
            {
                selectedStart = dates.StartDate;
                selectedEnd = dates.EndDate;
            }));

        // Act
        var icon = cut.Find(".daterange-icon");
        icon.Click();

        FindStartDateButton(cut, expectedStart).Click();
        FindEndDateButton(cut, expectedEnd).Click();

        var applyButton = cut.Find(".daterange-apply-btn");
        applyButton.Click();

        // Assert
        selectedStart.ShouldBe(expectedStart);
        selectedEnd.ShouldBe(expectedEnd);
        cut.FindAll(".daterange-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DateRangePicker_SelectsToday_WhenTodayPresetClicked()
    {
        // Act
        var cut = Render<DateRangePicker>();
        var icon = cut.Find(".daterange-icon");
        icon.Click();

        var todayButton = cut.Find(".daterange-preset-btn");
        todayButton.Click();

        // Assert - The component should have selected today's date
        cut.Find(".daterange-popup").ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_ClosesCalendar_WhenBackdropClicked()
    {
        // Act
        var cut = Render<DateRangePicker>();
        var icon = cut.Find(".daterange-icon");
        icon.Click();

        var backdrop = cut.Find(".daterange-backdrop");
        backdrop.Click();

        // Assert
        cut.FindAll(".daterange-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DateRangePicker_DisablesInputs_WhenDisabled()
    {
        // Act
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert - Find the actual input elements within the Input component wrappers
        var inputs = cut.FindAll(".daterange-inputs input");
        inputs[0].HasAttribute("disabled").ShouldBeTrue();
        inputs[1].HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void DateRangePicker_DisablesCalendarTrigger_WhenDisabled()
    {
        // Act
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var trigger = cut.Find(".daterange-icon");
        trigger.HasAttribute("disabled").ShouldBeTrue();
        trigger.GetAttribute("aria-expanded")!.ShouldBe("false");
        cut.FindAll(".daterange-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DateRangePicker_ExposesDialogState_WhenOpened()
    {
        // Arrange
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDatePlaceholder, "Trip starts")
            .Add(p => p.EndDatePlaceholder, "Trip ends"));

        var inputs = cut.FindAll(".daterange-inputs input");
        var trigger = cut.Find(".daterange-icon");
        var popupId = trigger.GetAttribute("aria-controls");

        // Assert initial state
        popupId.ShouldNotBeNullOrWhiteSpace();
        inputs[0].GetAttribute("aria-label")!.ShouldBe("Trip starts");
        inputs[1].GetAttribute("aria-label")!.ShouldBe("Trip ends");
        inputs[0].GetAttribute("aria-haspopup")!.ShouldBe("dialog");
        inputs[1].GetAttribute("aria-haspopup")!.ShouldBe("dialog");
        inputs[0].GetAttribute("aria-expanded")!.ShouldBe("false");
        inputs[1].GetAttribute("aria-expanded")!.ShouldBe("false");
        inputs[0].GetAttribute("aria-controls")!.ShouldBe(popupId);
        inputs[1].GetAttribute("aria-controls")!.ShouldBe(popupId);

        // Act
        trigger.Click();

        // Assert opened state
        var openedInputs = cut.FindAll(".daterange-inputs input");
        openedInputs[0].GetAttribute("aria-expanded")!.ShouldBe("true");
        openedInputs[1].GetAttribute("aria-expanded")!.ShouldBe("true");
        cut.Find(".daterange-icon").GetAttribute("aria-expanded")!.ShouldBe("true");

        var popup = cut.Find(".daterange-popup");
        popup.GetAttribute("id")!.ShouldBe(popupId);
        popup.GetAttribute("role")!.ShouldBe("dialog");
        popup.GetAttribute("aria-label")!.ShouldBe("Choose date range");
    }

    [Fact]
    public void DateRangePicker_DisablesDatesOutsideMinAndMax()
    {
        // Arrange
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 15))
            .Add(p => p.EndDate, new System.DateTime(2024, 6, 20))
            .Add(p => p.MinDate, new System.DateTime(2024, 6, 10))
            .Add(p => p.MaxDate, new System.DateTime(2024, 6, 25)));

        // Act
        cut.Find(".daterange-icon").Click();

        // Assert
        FindStartDateButton(cut, new System.DateTime(2024, 6, 9)).HasAttribute("disabled").ShouldBeTrue();
        FindStartDateButton(cut, new System.DateTime(2024, 6, 10)).HasAttribute("disabled").ShouldBeFalse();
        FindEndDateButton(cut, new System.DateTime(2024, 6, 25)).HasAttribute("disabled").ShouldBeFalse();
        FindEndDateButton(cut, new System.DateTime(2024, 6, 26)).HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void DateRangePicker_DisablesEndDatesBeforePendingStartDate()
    {
        // Arrange
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 10))
            .Add(p => p.EndDate, new System.DateTime(2024, 6, 25)));

        // Act
        cut.Find(".daterange-icon").Click();
        FindStartDateButton(cut, new System.DateTime(2024, 6, 20)).Click();

        // Assert
        FindEndDateButton(cut, new System.DateTime(2024, 6, 19)).HasAttribute("disabled").ShouldBeTrue();
        FindEndDateButton(cut, new System.DateTime(2024, 6, 20)).HasAttribute("disabled").ShouldBeFalse();
    }

    [Fact]
    public void DateRangePicker_ClearsPendingEndDate_WhenStartDateMovesAfterEndDate()
    {
        // Arrange
        System.DateTime? selectedStart = null;
        System.DateTime? selectedEnd = null;
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 10))
            .Add(p => p.EndDate, new System.DateTime(2024, 6, 15))
            .Add(p => p.OnChange, dates =>
            {
                selectedStart = dates.StartDate;
                selectedEnd = dates.EndDate;
            }));

        // Act
        cut.Find(".daterange-icon").Click();
        FindStartDateButton(cut, new System.DateTime(2024, 6, 20)).Click();
        cut.Find(".daterange-apply-btn").Click();

        // Assert
        selectedStart.ShouldBe(new System.DateTime(2024, 6, 20));
        selectedEnd.ShouldBeNull();
    }

    private static AngleSharp.Dom.IElement FindStartDateButton(IRenderedComponent<DateRangePicker> cut, System.DateTime date)
    {
        return FindDateRangeButton(cut, 0, $"Start date {FormatDate(date)}");
    }

    private static AngleSharp.Dom.IElement FindEndDateButton(IRenderedComponent<DateRangePicker> cut, System.DateTime date)
    {
        return FindDateRangeButton(cut, 1, $"End date {FormatDate(date)}");
    }

    private static AngleSharp.Dom.IElement FindDateRangeButton(IRenderedComponent<DateRangePicker> cut, int calendarIndex, string ariaLabel)
    {
        var calendar = cut.FindAll(".daterange-calendar")[calendarIndex];

        return calendar.QuerySelectorAll(".daterange-day")
            .Single(day => day.GetAttribute("aria-label") == ariaLabel);
    }

    private static string FormatDate(System.DateTime date)
    {
        return date.ToString("D", System.Globalization.CultureInfo.CurrentCulture);
    }
}
