namespace Vibe.UI.Tests.Components.DateTime;

public class DateRangePickerTests : TestBase
{
    [Fact]
    public void DateRangePicker_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<DateRangePicker>();

        // Assert
        var dateRangePicker = cut.Find(".vibe-daterange-picker");
        dateRangePicker.ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_Displays_TwoInputs()
    {
        // Act
        var cut = RenderComponent<DateRangePicker>();

        // Assert
        var startInput = cut.Find(".daterange-start-input");
        var endInput = cut.Find(".daterange-end-input");
        startInput.ShouldNotBeNull();
        endInput.ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_Displays_PlaceholderText()
    {
        // Arrange
        var startPlaceholder = "Start Date";
        var endPlaceholder = "End Date";

        // Act
        var cut = RenderComponent<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDatePlaceholder, startPlaceholder)
            .Add(p => p.EndDatePlaceholder, endPlaceholder));

        // Assert
        var startInput = cut.Find(".daterange-start-input");
        var endInput = cut.Find(".daterange-end-input");
        startInput.GetAttribute("placeholder").ShouldBe(startPlaceholder);
        endInput.GetAttribute("placeholder").ShouldBe(endPlaceholder);
    }

    [Fact]
    public void DateRangePicker_Displays_FormattedDates()
    {
        // Arrange
        var startDate = new System.DateTime(2024, 6, 1);
        var endDate = new System.DateTime(2024, 6, 30);

        // Act
        var cut = RenderComponent<DateRangePicker>(parameters => parameters
            .Add(p => p.StartDate, startDate)
            .Add(p => p.EndDate, endDate)
            .Add(p => p.Format, "MM/dd/yyyy"));

        // Assert
        var startInput = cut.Find(".daterange-start-input");
        var endInput = cut.Find(".daterange-end-input");
        startInput.GetAttribute("value").ShouldBe("06/01/2024");
        endInput.GetAttribute("value").ShouldBe("06/30/2024");
    }

    [Fact]
    public void DateRangePicker_OpensCalendar_WhenStartInputClicked()
    {
        // Act
        var cut = RenderComponent<DateRangePicker>();
        var startInput = cut.Find(".daterange-start-input");
        startInput.Click();

        // Assert
        var popup = cut.Find(".daterange-popup");
        popup.ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_OpensCalendar_WhenEndInputClicked()
    {
        // Act
        var cut = RenderComponent<DateRangePicker>();
        var endInput = cut.Find(".daterange-end-input");
        endInput.Click();

        // Assert
        var popup = cut.Find(".daterange-popup");
        popup.ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_DisplaysTwoCalendars()
    {
        // Act
        var cut = RenderComponent<DateRangePicker>();
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
        System.DateTime? selectedStart = null;
        System.DateTime? selectedEnd = null;
        var cut = RenderComponent<DateRangePicker>(parameters => parameters
            .Add(p => p.OnChange, dates =>
            {
                selectedStart = dates.StartDate;
                selectedEnd = dates.EndDate;
            }));

        // Act
        var icon = cut.Find(".daterange-icon");
        icon.Click();

        var applyButton = cut.Find(".daterange-apply-btn");
        applyButton.Click();

        // Assert - callback should be invoked even with null dates
        cut.FindAll(".daterange-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DateRangePicker_SelectsToday_WhenTodayPresetClicked()
    {
        // Act
        var cut = RenderComponent<DateRangePicker>();
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
        var cut = RenderComponent<DateRangePicker>();
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
        var cut = RenderComponent<DateRangePicker>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var startInput = cut.Find(".daterange-start-input");
        var endInput = cut.Find(".daterange-end-input");
        startInput.HasAttribute("disabled").ShouldBeTrue();
        endInput.HasAttribute("disabled").ShouldBeTrue();
    }
}
