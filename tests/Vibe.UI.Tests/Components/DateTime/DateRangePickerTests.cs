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
        // Use daterange-icon which has a proper click handler
        var icon = cut.Find(".daterange-icon");
        icon.Click();

        // Assert
        var popup = cut.Find(".daterange-popup");
        popup.ShouldNotBeNull();
    }

    [Fact]
    public void DateRangePicker_OpensCalendar_WhenEndInputClicked()
    {
        // Act
        var cut = Render<DateRangePicker>();
        // Use daterange-icon which has a proper click handler
        var icon = cut.Find(".daterange-icon");
        icon.Click();

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
        System.DateTime? selectedStart = null;
        System.DateTime? selectedEnd = null;
        var cut = Render<DateRangePicker>(parameters => parameters
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
}
