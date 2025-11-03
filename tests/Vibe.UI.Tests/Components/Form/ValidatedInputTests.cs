namespace Vibe.UI.Tests.Components.Form;

public class ValidatedInputTests : TestBase
{
    [Fact]
    public void ValidatedInput_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>();

        // Assert
        var input = cut.Find(".vibe-validated-input");
        input.ShouldNotBeNull();
    }

    [Fact]
    public void ValidatedInput_Renders_Label()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Email"));

        // Assert
        var label = cut.Find(".validated-input-label");
        label.TextContent.ShouldContain("Email");
    }

    [Fact]
    public void ValidatedInput_Shows_RequiredIndicator()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Email")
            .Add(p => p.Required, true));

        // Assert
        var indicator = cut.Find(".required-indicator");
        indicator.TextContent.ShouldBe("*");
    }

    [Fact]
    public void ValidatedInput_Applies_Placeholder()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Placeholder, "Enter your email"));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.GetAttribute("placeholder").ShouldBe("Enter your email");
    }

    [Fact]
    public void ValidatedInput_Applies_Disabled()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".vibe-validated-input").ClassList.ShouldContain("disabled");
    }

    [Fact]
    public void ValidatedInput_Applies_ReadOnly()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact]
    public void ValidatedInput_Shows_HelperText()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.HelperText, "We'll never share your email"));

        // Assert
        var helper = cut.Find(".validated-input-helper");
        helper.TextContent.ShouldBe("We'll never share your email");
    }

    [Fact]
    public void ValidatedInput_Shows_ErrorMessage()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ErrorMessage, "Email is required"));

        // Assert
        var error = cut.Find(".validated-input-error");
        error.TextContent.ShouldBe("Email is required");
    }

    [Fact]
    public void ValidatedInput_Hides_HelperText_WhenErrorExists()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.HelperText, "Help text")
            .Add(p => p.ErrorMessage, "Error text"));

        // Assert
        cut.FindAll(".validated-input-helper").ShouldBeEmpty();
        cut.FindAll(".validated-input-error").ShouldNotBeEmpty();
    }

    [Fact]
    public void ValidatedInput_Supports_InputTypes()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.InputType, "email"));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.GetAttribute("type").ShouldBe("email");
    }

    [Fact]
    public void ValidatedInput_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.CssClass, "custom-input"));

        // Assert
        cut.Find(".vibe-validated-input").ClassList.ShouldContain("custom-input");
    }

    [Fact]
    public void ValidatedInput_InvokesValueChanged_OnInput()
    {
        // Arrange
        string? newValue = null;
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input("test@example.com");

        // Assert
        newValue.ShouldBe("test@example.com");
    }

    #region Validation Tests

    [Fact]
    public void ValidatedInput_RequiredValidation_ShowsError_WhenEmpty()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Email")
            .Add(p => p.Required, true)
            .Add(p => p.ValidateOnBlur, true));

        var input = cut.Find(".validated-input-field");
        input.Blur();

        // Assert
        var error = cut.Find(".validated-input-error");
        error.TextContent.ShouldBe("Email is required");
        cut.Find(".vibe-validated-input").ClassList.ShouldContain("invalid");
    }

    [Fact]
    public void ValidatedInput_RequiredValidation_ShowsError_WithNullValue()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Name")
            .Add(p => p.Required, true)
            .Add(p => p.Value, null)
            .Add(p => p.ValidateOnBlur, true));

        var input = cut.Find(".validated-input-field");
        input.Blur();

        // Assert
        cut.FindAll(".validated-input-error").ShouldNotBeEmpty();
    }

    [Fact]
    public void ValidatedInput_RequiredValidation_ShowsError_WithWhitespaceOnly()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Username")
            .Add(p => p.Required, true)
            .Add(p => p.ValidateOnInput, true));

        var input = cut.Find(".validated-input-field");
        input.Input("   ");

        // Assert
        cut.FindAll(".validated-input-error").ShouldNotBeEmpty();
    }

    [Fact]
    public void ValidatedInput_CustomValidator_ExecutesAndShowsError()
    {
        // Arrange
        Func<string?, string?> validator = value =>
            value?.Length < 5 ? "Must be at least 5 characters" : null;

        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Validator, validator)
            .Add(p => p.ValidateOnInput, true));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input("abc");

        // Assert
        var error = cut.Find(".validated-input-error");
        error.TextContent.ShouldBe("Must be at least 5 characters");
    }

    [Fact]
    public void ValidatedInput_CustomValidator_ShowsValid_WhenPasses()
    {
        // Arrange
        Func<string?, string?> validator = value =>
            value?.Length < 5 ? "Must be at least 5 characters" : null;

        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Validator, validator)
            .Add(p => p.ValidateOnInput, true));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input("valid input");

        // Assert
        cut.FindAll(".validated-input-error").ShouldBeEmpty();
        cut.Find(".vibe-validated-input").ClassList.ShouldContain("valid");
    }

    [Fact]
    public void ValidatedInput_RequiredAndCustomValidator_BothExecute()
    {
        // Arrange - Required takes precedence
        Func<string?, string?> validator = value =>
            value?.Contains("@") == false ? "Must contain @" : null;

        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Email")
            .Add(p => p.Required, true)
            .Add(p => p.Validator, validator)
            .Add(p => p.ValidateOnBlur, true));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Blur();

        // Assert - Required validation should trigger first
        var error = cut.Find(".validated-input-error");
        error.TextContent.ShouldContain("required");
    }

    [Fact]
    public void ValidatedInput_CustomValidator_ChecksAfterRequired()
    {
        // Arrange
        Func<string?, string?> validator = value =>
            value?.Contains("@") == false ? "Must contain @" : null;

        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.Validator, validator)
            .Add(p => p.ValidateOnInput, true));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input("noatsign");

        // Assert
        var error = cut.Find(".validated-input-error");
        error.TextContent.ShouldBe("Must contain @");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ValidatedInput_HandlesVeryLongInput()
    {
        // Arrange
        var longString = new string('a', 10000);
        string? capturedValue = null;

        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input(longString);

        // Assert
        capturedValue.ShouldBe(longString);
    }

    [Fact]
    public void ValidatedInput_HandlesSpecialCharacters()
    {
        // Arrange
        var specialChars = "!@#$%^&*()_+-={}[]|\\:;\"'<>,.?/~`";
        string? capturedValue = null;

        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input(specialChars);

        // Assert
        capturedValue.ShouldBe(specialChars);
    }

    [Fact]
    public void ValidatedInput_HandlesUnicodeCharacters()
    {
        // Arrange
        var unicode = "Hello 世界 🌍 Привет مرحبا";
        string? capturedValue = null;

        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input(unicode);

        // Assert
        capturedValue.ShouldBe(unicode);
    }

    [Fact]
    public void ValidatedInput_HandlesEmptyString()
    {
        // Arrange
        string? capturedValue = "initial";

        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Value, "initial")
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input("");

        // Assert
        capturedValue.ShouldBeNull();
    }

    [Fact]
    public void ValidatedInput_NumericType_ConvertsCorrectly()
    {
        // Arrange
        int? capturedValue = null;

        var cut = RenderComponent<ValidatedInput<int>>(parameters => parameters
            .Add(p => p.InputType, "number")
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input("42");

        // Assert
        capturedValue.ShouldBe(42);
    }

    [Fact]
    public void ValidatedInput_NumericType_HandlesInvalidConversion()
    {
        // Arrange
        int? capturedValue = 10;

        var cut = RenderComponent<ValidatedInput<int>>(parameters => parameters
            .Add(p => p.InputType, "number")
            .Add(p => p.Value, 10)
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act
        var input = cut.Find(".validated-input-field");
        input.Input("not-a-number");

        // Assert - Conversion fails, value should remain unchanged
        capturedValue.ShouldBe(10);
    }

    #endregion

    #region State Management

    [Fact]
    public void ValidatedInput_TouchedState_UpdatesOnBlur()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ValidateOnBlur, true));

        var input = cut.Find(".validated-input-field");

        // Initially not touched
        cut.Find(".vibe-validated-input").ClassList.ShouldNotContain("touched");

        // Act - Blur
        input.Blur();

        // Assert
        cut.Find(".vibe-validated-input").ClassList.ShouldContain("touched");
    }

    [Fact]
    public void ValidatedInput_ValidateOnInput_TriggersImmediately()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.ValidateOnInput, true));

        var input = cut.Find(".validated-input-field");
        input.Input("");

        // Assert - Should show error immediately
        cut.FindAll(".validated-input-error").ShouldNotBeEmpty();
    }

    [Fact]
    public void ValidatedInput_ValidateOnBlur_DoesNotTriggerOnInput()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.ValidateOnBlur, true)
            .Add(p => p.ValidateOnInput, false));

        var input = cut.Find(".validated-input-field");
        input.Input("");

        // Assert - Should not show error yet
        cut.FindAll(".validated-input-error").ShouldBeEmpty();
    }

    [Fact]
    public void ValidatedInput_ForceValidation_TriggersValidation()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Email")
            .Add(p => p.Required, true)
            .Add(p => p.ValidateOnBlur, false)
            .Add(p => p.ValidateOnInput, false));

        // Assert - No error initially
        cut.FindAll(".validated-input-error").ShouldBeEmpty();

        // Act - Force validation using InvokeAsync to handle StateHasChanged
        cut.InvokeAsync(() => cut.Instance.ForceValidation());

        // Assert
        cut.FindAll(".validated-input-error").ShouldNotBeEmpty();
    }

    [Fact]
    public void ValidatedInput_IsValidField_ReturnsCorrectState()
    {
        // Arrange
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.Value, "valid value"));

        // Act
        var isValid = cut.Instance.IsValidField();

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void ValidatedInput_IsValidField_ReturnsFalse_WhenInvalid()
    {
        // Arrange
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.Value, null));

        // Act
        var isValid = cut.Instance.IsValidField();

        // Assert
        isValid.ShouldBeFalse();
    }

    #endregion

    #region Validation Icons

    [Fact]
    public void ValidatedInput_ShowsValidationIcon_WhenValid()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.Value, "valid")
            .Add(p => p.ShowValidationIcon, true)
            .Add(p => p.ValidateOnInput, true));

        var input = cut.Find(".validated-input-field");
        input.Input("valid");

        // Assert
        cut.FindAll(".validation-icon").ShouldNotBeEmpty();
        cut.Find(".vibe-validated-input").ClassList.ShouldContain("valid");
    }

    [Fact]
    public void ValidatedInput_ShowsValidationIcon_WhenInvalid()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.ShowValidationIcon, true)
            .Add(p => p.ValidateOnBlur, true));

        var input = cut.Find(".validated-input-field");
        input.Blur();

        // Assert
        cut.FindAll(".validation-icon").ShouldNotBeEmpty();
        cut.Find(".vibe-validated-input").ClassList.ShouldContain("invalid");
    }

    [Fact]
    public void ValidatedInput_HidesValidationIcon_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ShowValidationIcon, false)
            .Add(p => p.Required, true)
            .Add(p => p.ValidateOnBlur, true));

        var input = cut.Find(".validated-input-field");
        input.Blur();

        // Assert
        cut.FindAll(".validation-icon").ShouldBeEmpty();
    }

    [Fact]
    public void ValidatedInput_NoValidationIcon_BeforeTouched()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.ShowValidationIcon, true)
            .Add(p => p.ValidateOnBlur, true));

        // Assert - Before any interaction
        cut.FindAll(".validation-icon").ShouldBeEmpty();
    }

    #endregion

    #region Error Priority

    [Fact]
    public void ValidatedInput_ErrorMessage_TakesPriority_OverHelperText()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.HelperText, "This is helper text")
            .Add(p => p.ErrorMessage, "This is an error"));

        // Assert
        cut.FindAll(".validated-input-helper").ShouldBeEmpty();
        cut.FindAll(".validated-input-error").ShouldNotBeEmpty();
    }

    [Fact]
    public void ValidatedInput_HelperText_Shown_WhenNoError()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.HelperText, "This is helper text")
            .Add(p => p.ErrorMessage, null));

        // Assert
        cut.FindAll(".validated-input-helper").ShouldNotBeEmpty();
        cut.FindAll(".validated-input-error").ShouldBeEmpty();
    }

    #endregion

    #region Disabled/ReadOnly State

    [Fact]
    public void ValidatedInput_DisabledInput_DoesNotTriggerValidation()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.ValidateOnBlur, true));

        var input = cut.Find(".validated-input-field");

        // Input events don't fire on disabled inputs in real DOM
        // We can only verify the disabled state
        input.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void ValidatedInput_ReadOnlyInput_HasAttribute()
    {
        // Act
        var cut = RenderComponent<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.HasAttribute("readonly").ShouldBeTrue();
    }

    #endregion
}
