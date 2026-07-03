namespace Vibe.UI.Tests.Components.DateTime;

public class DatePickerTests : TestBase
{
    [Fact]
    public void DatePicker_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<DatePicker>();

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
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.Placeholder, placeholder));

        // Assert - Find the actual input element within the Input component
        var input = cut.Find(".date-input-wrapper input");
        input.GetAttribute("placeholder")!.ShouldBe(placeholder);
    }

    [Fact]
    public void DatePicker_Displays_FormattedDate()
    {
        // Arrange
        var date = new System.DateTime(2024, 6, 15);

        // Act
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.Date, date)
            .Add(p => p.Format, "MM/dd/yyyy"));

        // Assert - Find the actual input element within the Input component
        var input = cut.Find(".date-input-wrapper input");
        input.GetAttribute("value")!.ShouldBe("06/15/2024");
    }

    [Fact]
    public void DatePicker_OpensCalendar_WhenInputClicked()
    {
        // Act
        var cut = Render<DatePicker>();
        var input = cut.Find(".date-input-wrapper input");
        input.Click();

        // Assert
        var popup = cut.Find(".date-popup");
        popup.ShouldNotBeNull();
    }

    [Fact]
    public void DatePicker_OpensCalendar_WhenIconClicked()
    {
        // Act
        var cut = Render<DatePicker>();
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
        var cut = Render<DatePicker>();
        var icon = cut.Find(".date-icon");
        icon.Click();

        var backdrop = cut.Find(".date-backdrop");
        backdrop.Click();

        // Assert
        cut.FindAll(".date-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DatePicker_InvokesOnChange_WhenDateSelected()
    {
        // Arrange
        var expectedDate = new System.DateTime(2024, 6, 20);
        System.DateTime? selectedDate = null;
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.Date, new System.DateTime(2024, 6, 15))
            .Add(p => p.OnChange, date => selectedDate = date));

        // Act - Use date-icon to open calendar
        var icon = cut.Find(".date-icon");
        icon.Click();

        FindDateButton(cut, expectedDate).Click();

        // Assert
        selectedDate.ShouldBe(expectedDate);
        cut.FindAll(".date-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DatePicker_SelectsToday_WhenTodayButtonClicked()
    {
        // Arrange
        System.DateTime? selectedDate = null;
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.OnChange, date => selectedDate = date));

        // Act - Use date-icon to open calendar
        var icon = cut.Find(".date-icon");
        icon.Click();

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
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert - Find the actual input element within the Input component
        var input = cut.Find(".date-input-wrapper input");
        input.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void DatePicker_DisablesCalendarTrigger_WhenDisabled()
    {
        // Act
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var trigger = cut.Find(".date-icon");
        trigger.HasAttribute("disabled").ShouldBeTrue();
        trigger.GetAttribute("aria-expanded")!.ShouldBe("false");
        cut.FindAll(".date-popup").ShouldBeEmpty();
    }

    [Fact]
    public void DatePicker_ExposesDialogState_WhenOpened()
    {
        // Arrange
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.Placeholder, "Choose due date"));

        var input = cut.Find(".date-input-wrapper input");
        var trigger = cut.Find(".date-icon");
        var popupId = trigger.GetAttribute("aria-controls");

        // Assert initial state
        popupId.ShouldNotBeNullOrWhiteSpace();
        input.GetAttribute("aria-label")!.ShouldBe("Choose due date");
        input.GetAttribute("aria-haspopup")!.ShouldBe("dialog");
        input.GetAttribute("aria-expanded")!.ShouldBe("false");
        input.GetAttribute("aria-controls")!.ShouldBe(popupId);
        trigger.GetAttribute("aria-expanded")!.ShouldBe("false");

        // Act
        trigger.Click();

        // Assert opened state
        cut.Find(".date-input-wrapper input").GetAttribute("aria-expanded")!.ShouldBe("true");
        cut.Find(".date-icon").GetAttribute("aria-expanded")!.ShouldBe("true");
        var popup = cut.Find(".date-popup");
        popup.GetAttribute("id")!.ShouldBe(popupId);
        popup.GetAttribute("role")!.ShouldBe("dialog");
        popup.GetAttribute("aria-label")!.ShouldBe("Choose date");
    }

    [Fact]
    public void DatePicker_DisablesDatesOutsideMinAndMax()
    {
        // Arrange
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.Date, new System.DateTime(2024, 6, 15))
            .Add(p => p.MinDate, new System.DateTime(2024, 6, 10))
            .Add(p => p.MaxDate, new System.DateTime(2024, 6, 20)));

        // Act
        cut.Find(".date-icon").Click();

        // Assert
        FindDateButton(cut, new System.DateTime(2024, 6, 9)).HasAttribute("disabled").ShouldBeTrue();
        FindDateButton(cut, new System.DateTime(2024, 6, 10)).HasAttribute("disabled").ShouldBeFalse();
        FindDateButton(cut, new System.DateTime(2024, 6, 20)).HasAttribute("disabled").ShouldBeFalse();
        FindDateButton(cut, new System.DateTime(2024, 6, 21)).HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void DatePicker_DisablesTodayButton_WhenTodayOutsideRange()
    {
        // Arrange
        var cut = Render<DatePicker>(parameters => parameters
            .Add(p => p.MinDate, System.DateTime.Today.AddDays(1))
            .Add(p => p.MaxDate, System.DateTime.Today.AddDays(10)));

        // Act
        cut.Find(".date-icon").Click();

        // Assert
        cut.Find(".date-today-btn").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void DatePicker_NavigatesBetweenMonths()
    {
        // Act
        var cut = Render<DatePicker>();
        var icon = cut.Find(".date-icon");
        icon.Click();

        var nextButton = cut.FindAll(".date-nav-btn")[1];
        nextButton.Click();

        // Assert
        var popup = cut.Find(".date-popup");
        popup.ShouldNotBeNull();
    }

    private static AngleSharp.Dom.IElement FindDateButton(IRenderedComponent<DatePicker> cut, System.DateTime date)
    {
        var label = date.ToString("D", System.Globalization.CultureInfo.CurrentCulture);

        return cut.FindAll(".date-day")
            .Single(day => day.GetAttribute("aria-label") == label);
    }
}
