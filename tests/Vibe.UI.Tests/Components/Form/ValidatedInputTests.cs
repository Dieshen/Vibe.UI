using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Vibe.UI.Tests.Components.Form;

public class ValidatedInputTests : TestBase
{
    private sealed class FieldValidationModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
    }

    [Fact]
    public void ValidatedInput_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<ValidatedInput<string>>();

        // Assert
        var input = cut.Find(".vibe-validated-input");
        input.ShouldNotBeNull();
    }

    [Fact]
    public void ValidatedInput_Renders_Label()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Email"));

        // Assert
        var label = cut.Find(".validated-input-label");
        label.TextContent.ShouldContain("Email");
    }

    [Fact]
    public void ValidatedInput_Shows_RequiredIndicator()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Email")
            .Add(p => p.Required, true));

        // Assert
        var indicator = cut.Find(".required-indicator");
        indicator.TextContent.ShouldBe("*");
        indicator.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void ValidatedInput_Applies_Placeholder()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Placeholder, "Enter your email"));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.GetAttribute("placeholder")!.ShouldBe("Enter your email");
    }

    [Fact]
    public void ValidatedInput_Applies_Disabled()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact]
    public void ValidatedInput_UsesProvidedId_ForInputLabelAndHelper()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Id, "email")
            .Add(p => p.Label, "Email")
            .Add(p => p.HelperText, "Use your work email")
            .Add(p => p.Required, true));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.GetAttribute("id").ShouldBe("email");
        input.GetAttribute("aria-describedby").ShouldBe("email-helper");
        input.GetAttribute("aria-required").ShouldBe("true");

        cut.Find(".validated-input-label").GetAttribute("for").ShouldBe("email");
        cut.Find(".validated-input-helper").GetAttribute("id").ShouldBe("email-helper");
    }

    [Fact]
    public void ValidatedInput_GeneratedId_RemainsStableAcrossRerender()
    {
        // Arrange
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Label, "Email"));

        var originalId = cut.Find(".validated-input-field").GetAttribute("id");

        // Act
        cut.Render(parameters => parameters
            .Add(p => p.Label, "Work Email"));

        // Assert
        var updatedId = cut.Find(".validated-input-field").GetAttribute("id");
        updatedId.ShouldBe(originalId);
        cut.Find(".validated-input-label").GetAttribute("for").ShouldBe(originalId);
    }

    [Fact]
    public void ValidatedInput_Shows_HelperText()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.HelperText, "We'll never share your email"));

        // Assert
        var helper = cut.Find(".validated-input-helper");
        helper.TextContent.ShouldBe("We'll never share your email");
    }

    [Fact]
    public void ValidatedInput_Shows_ErrorMessage()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Id, "email")
            .Add(p => p.ErrorMessage, "Email is required"));

        // Assert
        var error = cut.Find(".validated-input-error");
        error.TextContent.ShouldBe("Email is required");
        error.GetAttribute("id").ShouldBe("email-error");
        error.GetAttribute("role").ShouldBe("alert");

        var input = cut.Find(".validated-input-field");
        input.GetAttribute("aria-invalid").ShouldBe("true");
        input.GetAttribute("aria-errormessage").ShouldBe("email-error");
        input.GetAttribute("aria-describedby").ShouldBe("email-error");
    }

    [Fact]
    public void ValidatedInput_Hides_HelperText_WhenErrorExists()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.InputType, "email"));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.GetAttribute("type")!.ShouldBe("email");
    }

    [Fact]
    public void ValidatedInput_Applies_CustomCssClass()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.CssClass, "custom-input"));

        // Assert
        cut.Find(".vibe-validated-input").ClassList.ShouldContain("custom-input");
    }

    [Fact]
    public void ValidatedInput_MergesClassParametersAndForwardsRootAttributes()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.CssClass, "css-class")
            .Add(p => p.Class, "class-parameter")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "class", "unmatched-class" },
                { "data-testid", "email-field" }
            }));

        // Assert
        var root = cut.Find(".vibe-validated-input");
        root.ClassList.ShouldContain("css-class");
        root.ClassList.ShouldContain("class-parameter");
        root.ClassList.ShouldContain("unmatched-class");
        root.GetAttribute("data-testid").ShouldBe("email-field");
    }

    [Fact]
    public void ValidatedInput_InvokesValueChanged_OnInput()
    {
        // Arrange
        string? newValue = null;
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

    [Fact]
    public void ValidatedInput_DoesNotClearExternalErrorMessage_WhenValidationRuns()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Value, "valid@example.com")
            .Add(p => p.Required, true)
            .Add(p => p.ErrorMessage, "Server-side error")
            .Add(p => p.ValidateOnBlur, false)
            .Add(p => p.ValidateOnInput, false));

        cut.InvokeAsync(() => cut.Instance.ForceValidation());

        // Assert
        cut.Find(".validated-input-error").TextContent.ShouldBe("Server-side error");
        cut.Instance.ErrorMessage.ShouldBe("Server-side error");
    }

    [Fact]
    public void ValidatedInput_RendersEditContextValidationMessage_ForField()
    {
        // Arrange
        var model = new FieldValidationModel();
        Expression<Func<string>> validationFor = () => model.Email;

        // Act
        var cut = Render(builder =>
        {
            builder.OpenComponent<Vibe.UI.Components.Form<FieldValidationModel>>(0);
            builder.AddAttribute(1, "Model", model);
            builder.AddAttribute(2, "ShowValidationSummary", false);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<ValidatedInput<string>>(0);
                childBuilder.AddAttribute(1, "Id", "email");
                childBuilder.AddAttribute(2, "Label", "Email");
                childBuilder.AddAttribute(3, "For", validationFor);
                childBuilder.AddAttribute(4, "Value", model.Email);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Email = value ?? string.Empty));
                childBuilder.CloseComponent();
                childBuilder.AddMarkupContent(6, "<button type=\"submit\">Submit</button>");
            }));
            builder.CloseComponent();
        });

        cut.Find("form").Submit();

        // Assert
        var input = cut.Find(".validated-input-field");
        input.GetAttribute("aria-invalid").ShouldBe("true");
        input.GetAttribute("aria-errormessage").ShouldBe("email-error");
        input.GetAttribute("aria-describedby").ShouldBe("email-error");

        var error = cut.Find(".validated-input-error");
        error.GetAttribute("id").ShouldBe("email-error");
        error.GetAttribute("role").ShouldBe("alert");
        error.TextContent.ShouldContain("Email is required");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ValidatedInput_HandlesVeryLongInput()
    {
        // Arrange
        var longString = new string('a', 10000);
        string? capturedValue = null;

        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

        var cut = Render<ValidatedInput<string>>(parameters => parameters
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

        var cut = Render<ValidatedInput<int>>(parameters => parameters
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

        var cut = Render<ValidatedInput<int>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var cut = Render<ValidatedInput<string>>(parameters => parameters
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
        var callbackCount = 0;
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.ValidateOnBlur, true)
            .Add(p => p.ValidateOnInput, true)
            .Add(p => p.ValueChanged, _ => callbackCount++));

        var input = cut.Find(".validated-input-field");
        input.Input("");
        input.Blur();

        // Assert
        input.HasAttribute("disabled").ShouldBeTrue();
        cut.FindAll(".validated-input-error").ShouldBeEmpty();
        cut.Find(".vibe-validated-input").ClassList.ShouldNotContain("touched");
        callbackCount.ShouldBe(0);
    }

    [Fact]
    public void ValidatedInput_ReadOnlyInput_HasAttribute()
    {
        // Act
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        var input = cut.Find(".validated-input-field");
        input.HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact]
    public void ValidatedInput_ReadOnlyInput_DoesNotInvokeValueChanged()
    {
        // Arrange
        string? capturedValue = "initial";
        var cut = Render<ValidatedInput<string>>(parameters => parameters
            .Add(p => p.Value, "initial")
            .Add(p => p.ReadOnly, true)
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act
        cut.Find(".validated-input-field").Input("changed");

        // Assert
        capturedValue.ShouldBe("initial");
    }

    #endregion
}
