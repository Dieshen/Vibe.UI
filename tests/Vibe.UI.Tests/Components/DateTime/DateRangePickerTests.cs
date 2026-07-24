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
    public void DateRangePicker_EachGridContainsItsWeekdayHeaderRow()
    {
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 10))
            .Add(p => p.EndDate, new System.DateTime(2024, 7, 10)));

        cut.Find(".daterange-icon").Click();

        var grids = cut.FindAll(".daterange-grid[role='grid']");
        var headerRows = cut.FindAll(".daterange-day-names[role='row']");
        grids.Count.ShouldBe(2);
        headerRows.Count.ShouldBe(2);

        for (var index = 0; index < grids.Count; index++)
        {
            headerRows[index].ParentElement.ShouldBe(grids[index]);
            headerRows[index].Children.Length.ShouldBe(7);
            headerRows[index].Children.ShouldAllBe(header => header.GetAttribute("role") == "columnheader");
        }
    }

    [Fact]
    public void DateRangePicker_PairedMonthNavigationRespectsEitherCalendarYearBoundary()
    {
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 10))
            .Add(p => p.EndDate, new System.DateTime(2024, 7, 10))
            .Add(p => p.MinYear, 2024)
            .Add(p => p.MaxYear, 2025));

        cut.Find(".daterange-icon").Click();

        cut.FindAll(".daterange-month-select")[1].Change("0");
        var previousButton = cut.Find(".daterange-nav-btn[aria-label='Previous month']");
        previousButton.HasAttribute("disabled").ShouldBeTrue();
        var startLabelBeforePrevious = cut.FindAll(".daterange-grid")[0].GetAttribute("aria-label");
        var endLabelBeforePrevious = cut.FindAll(".daterange-grid")[1].GetAttribute("aria-label");

        previousButton.Click();

        cut.FindAll(".daterange-grid")[0].GetAttribute("aria-label").ShouldBe(startLabelBeforePrevious);
        cut.FindAll(".daterange-grid")[1].GetAttribute("aria-label").ShouldBe(endLabelBeforePrevious);

        cut.FindAll(".daterange-year-select")[0].Change("2025");
        cut.FindAll(".daterange-month-select")[0].Change("11");
        var nextButton = cut.Find(".daterange-nav-btn[aria-label='Next month']");
        nextButton.HasAttribute("disabled").ShouldBeTrue();
        var startLabelBeforeNext = cut.FindAll(".daterange-grid")[0].GetAttribute("aria-label");
        var endLabelBeforeNext = cut.FindAll(".daterange-grid")[1].GetAttribute("aria-label");

        nextButton.Click();

        cut.FindAll(".daterange-grid")[0].GetAttribute("aria-label").ShouldBe(startLabelBeforeNext);
        cut.FindAll(".daterange-grid")[1].GetAttribute("aria-label").ShouldBe(endLabelBeforeNext);
    }

    [Fact]
    public void DateRangePicker_MarksOnlySelectedDatesAsInRange()
    {
        // Arrange
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 10))
            .Add(p => p.EndDate, new System.DateTime(2024, 6, 15)));

        // Act
        cut.Find(".daterange-icon").Click();

        // Assert
        FindStartDateButton(cut, new System.DateTime(2024, 6, 9)).ClassList.Contains("in-range").ShouldBeFalse();
        FindStartDateButton(cut, new System.DateTime(2024, 6, 10)).ClassList.Contains("range-start").ShouldBeTrue();
        FindStartDateButton(cut, new System.DateTime(2024, 6, 12)).ClassList.Contains("in-range").ShouldBeTrue();
        FindStartDateButton(cut, new System.DateTime(2024, 6, 16)).ClassList.Contains("in-range").ShouldBeFalse();
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

    [Fact]
    public void DateRangePicker_ApplyInvokesBindingAndCompatibilityCallbacksOnce()
    {
        var expectedStart = new System.DateTime(2024, 6, 10);
        var expectedEnd = new System.DateTime(2024, 6, 15);
        System.DateTime? boundStart = null;
        System.DateTime? boundEnd = null;
        (System.DateTime? StartDate, System.DateTime? EndDate) compatibilityRange = default;
        var startCallbackCount = 0;
        var endCallbackCount = 0;
        var compatibilityCallbackCount = 0;
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 1))
            .Add(p => p.EndDate, new System.DateTime(2024, 6, 20))
            .Add(p => p.StartDateChanged, date =>
            {
                startCallbackCount++;
                boundStart = date;
            })
            .Add(p => p.EndDateChanged, date =>
            {
                endCallbackCount++;
                boundEnd = date;
            })
            .Add(p => p.OnChange, range =>
            {
                compatibilityCallbackCount++;
                compatibilityRange = range;
            }));

        cut.Find(".daterange-icon").Click();
        FindStartDateButton(cut, expectedStart).Click();
        FindEndDateButton(cut, expectedEnd).Click();

        startCallbackCount.ShouldBe(0);
        endCallbackCount.ShouldBe(0);
        compatibilityCallbackCount.ShouldBe(0);

        cut.Find(".daterange-apply-btn").Click();

        startCallbackCount.ShouldBe(1);
        endCallbackCount.ShouldBe(1);
        compatibilityCallbackCount.ShouldBe(1);
        boundStart.ShouldBe(expectedStart);
        boundEnd.ShouldBe(expectedEnd);
        compatibilityRange.StartDate.ShouldBe(expectedStart);
        compatibilityRange.EndDate.ShouldBe(expectedEnd);
        cut.FindAll(".daterange-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DateRangePicker_ForwardsAdditionalAttributesToRoot()
    {
        var cut = Render<DateRangePicker>(parameters => parameters
            .AddUnmatched("data-testid", "trip-range")
            .AddUnmatched("aria-label", "Trip dates"));

        var root = cut.Find(".vibe-daterange-picker");
        root.GetAttribute("data-testid")!.ShouldBe("trip-range");
        root.GetAttribute("aria-label")!.ShouldBe("Trip dates");
    }

    [Fact]
    public void DateRangePicker_InputKeyboardOpen_FocusesBoundStartDate()
    {
        JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-dom.js");
        var startDate = new System.DateTime(2024, 6, 15);
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, startDate)
            .Add(p => p.EndDate, startDate.AddDays(5)));

        cut.FindAll(".daterange-inputs input")[0].KeyDown("Enter");

        var focusedDay = FindStartDateButton(cut, startDate);
        focusedDay.GetAttribute("tabindex")!.ShouldBe("0");
        cut.FindAll(".daterange-day").Count(day => day.GetAttribute("tabindex") == "0").ShouldBe(1);
        JSInterop.Invocations.Last().Identifier.ShouldBe("focusElement");
        JSInterop.Invocations.Last().Arguments[0].ShouldBe(focusedDay.Id);
    }

    [Fact]
    public void DateRangePicker_SelectingStart_MovesRovingFocusToSameDayInEndCalendar()
    {
        JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-dom.js");
        var newStartDate = new System.DateTime(2024, 6, 10);
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 1)));

        cut.Find(".daterange-icon").Click();
        FindStartDateButton(cut, newStartDate).Click();

        var focusedEndDay = FindEndDateButton(cut, newStartDate);
        focusedEndDay.GetAttribute("tabindex")!.ShouldBe("0");
        focusedEndDay.HasAttribute("disabled").ShouldBeFalse();
        JSInterop.Invocations.Last().Identifier.ShouldBe("focusElement");
        JSInterop.Invocations.Last().Arguments[0].ShouldBe(focusedEndDay.Id);
    }

    [Fact]
    public void DateRangePicker_RightArrow_MovesFocusAcrossMonthBoundary()
    {
        JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-dom.js");
        var startDate = new System.DateTime(2024, 6, 30);
        var nextDate = startDate.AddDays(1);
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, startDate)
            .Add(p => p.EndDate, new System.DateTime(2024, 7, 5)));

        cut.Find(".daterange-icon").Click();
        FindStartDateButton(cut, startDate).KeyDown("ArrowRight");

        var focusedDay = FindStartDateButton(cut, nextDate);
        focusedDay.GetAttribute("tabindex")!.ShouldBe("0");
        JSInterop.Invocations.Last().Identifier.ShouldBe("focusElement");
        JSInterop.Invocations.Last().Arguments[0].ShouldBe(focusedDay.Id);
    }

    [Fact]
    public void DateRangePicker_EscapeClosesPopup_AndRestoresOriginatingEndInputFocus()
    {
        JSInterop.SetupModule("./_content/Vibe.UI/js/vibe-dom.js");
        var cut = Render<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, new System.DateTime(2024, 6, 10))
            .Add(p => p.EndDate, new System.DateTime(2024, 6, 15)));
        var endInput = cut.FindAll(".daterange-inputs input")[1];

        endInput.Click();
        cut.Find(".daterange-popup").KeyDown("Escape");

        cut.FindAll(".daterange-popup").ShouldBeEmpty();
        JSInterop.Invocations.Last().Identifier.ShouldBe("focusElement");
        JSInterop.Invocations.Last().Arguments[0].ShouldBe(endInput.Id);
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
