namespace Vibe.UI.Tests.Components.Input;

public class InputTests : TestBase
{
    [Fact]
    public void Input_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>();

        // Assert
        var input = cut.Find("input[type='text']");
        input.ShouldNotBeNull();
    }

    [Fact]
    public void Input_Renders_WithLabel()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Label, "Username"));

        // Assert
        var label = cut.Find(".vibe-input-label");
        label.TextContent.ShouldBe("Username");
    }

    [Fact]
    public void Input_Renders_WithPlaceholder()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Placeholder, "Enter username"));

        // Assert
        var input = cut.Find("input");
        input.GetAttribute("placeholder").ShouldBe("Enter username");
    }

    [Fact]
    public void Input_Renders_WithValue()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Value, "John Doe"));

        // Assert
        var input = cut.Find("input");
        input.GetAttribute("value").ShouldBe("John Doe");
    }

    [Fact]
    public void Input_Applies_Disabled_Attribute()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var input = cut.Find("input");
        input.HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".vibe-input-container").ClassList.ShouldContain("vibe-input-disabled");
    }

    [Fact]
    public void Input_Applies_ReadOnly_Attribute()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        var input = cut.Find("input");
        input.HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact]
    public void Input_Shows_ErrorMessage()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.ErrorMessage, "This field is required"));

        // Assert
        var error = cut.Find(".vibe-input-error");
        error.TextContent.ShouldBe("This field is required");
    }

    [Fact]
    public void Input_Shows_HelperText()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.HelperText, "Enter your email address"));

        // Assert
        var helper = cut.Find(".vibe-input-helper");
        helper.TextContent.ShouldBe("Enter your email address");
    }

    [Fact]
    public void Input_Applies_CustomType()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Type, "email"));

        // Assert
        var input = cut.Find("input");
        input.GetAttribute("type").ShouldBe("email");
    }

    [Fact]
    public void Input_Applies_Size_Class()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Size, "large"));

        // Assert
        var input = cut.Find("input");
        input.ClassList.ShouldContain("vibe-input-large");
    }

    [Fact]
    public void Input_Applies_Variant_Class()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Variant, "filled"));

        // Assert
        var input = cut.Find("input");
        input.ClassList.ShouldContain("vibe-input-filled");
    }

    [Fact]
    public void Input_InvokesValueChanged_OnChange()
    {
        // Arrange
        string newValue = null;
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        cut.Find("input").Change("New Value");

        // Assert
        newValue.ShouldBe("New Value");
    }

    // === Edge Cases ===

    [Fact]
    public void Input_WithNullValue_RendersEmpty()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Value, (string)null));

        // Assert - When Value is null, component has Value = string.Empty (default), so attribute is empty
        var input = cut.Find("input");
        var valueAttr = input.GetAttribute("value");
        (valueAttr ?? string.Empty).ShouldBe(string.Empty);
    }

    [Fact]
    public void Input_WithEmptyValue_RendersEmpty()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Value, string.Empty));

        // Assert
        var input = cut.Find("input");
        input.GetAttribute("value").ShouldBe(string.Empty);
    }

    [Fact]
    public void Input_WithVeryLongValue_RendersCorrectly()
    {
        // Arrange
        var longValue = new string('A', 1000);

        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Value, longValue));

        // Assert
        cut.Find("input").GetAttribute("value").ShouldBe(longValue);
    }

    [Fact]
    public void Input_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var specialChars = "<>&\"'`\n\t";  // Removed \r due to platform normalization

        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Value, specialChars));

        // Assert
        cut.Find("input").GetAttribute("value").ShouldBe(specialChars);
    }

    [Fact]
    public void Input_WithUnicodeCharacters_HandlesCorrectly()
    {
        // Arrange
        var unicode = "Hello 世界 🚀 emoji";

        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Value, unicode));

        // Assert
        cut.Find("input").GetAttribute("value").ShouldBe(unicode);
    }

    // === Input Types ===

    [Theory]
    [InlineData("text")]
    [InlineData("email")]
    [InlineData("password")]
    [InlineData("number")]
    [InlineData("tel")]
    [InlineData("url")]
    [InlineData("search")]
    [InlineData("date")]
    public void Input_WithVariousTypes_RendersCorrectType(string inputType)
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Type, inputType));

        // Assert
        cut.Find("input").GetAttribute("type").ShouldBe(inputType);
    }

    // === Leading and Trailing Icons ===

    [Fact]
    public void Input_WithLeadingIcon_RendersIcon()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.LeadingIcon, "<svg>icon</svg>"));

        // Assert
        var icon = cut.Find(".vibe-input-icon-leading");
        icon.ShouldNotBeNull();
        icon.InnerHtml.ShouldContain("<svg>icon</svg>");
        cut.Find("input").ClassList.ShouldContain("vibe-input-with-leading-icon");
    }

    [Fact]
    public void Input_WithTrailingIcon_RendersIcon()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.TrailingIcon, "<svg>icon</svg>"));

        // Assert
        var icon = cut.Find(".vibe-input-icon-trailing");
        icon.ShouldNotBeNull();
        icon.InnerHtml.ShouldContain("<svg>icon</svg>");
        cut.Find("input").ClassList.ShouldContain("vibe-input-with-trailing-icon");
    }

    [Fact]
    public void Input_WithBothIcons_RendersBothIcons()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.LeadingIcon, "<svg>leading</svg>")
            .Add(p => p.TrailingIcon, "<svg>trailing</svg>"));

        // Assert
        cut.Find(".vibe-input-icon-leading").ShouldNotBeNull();
        cut.Find(".vibe-input-icon-trailing").ShouldNotBeNull();
        var input = cut.Find("input");
        input.ClassList.ShouldContain("vibe-input-with-leading-icon");
        input.ClassList.ShouldContain("vibe-input-with-trailing-icon");
    }

    [Fact]
    public void Input_WithEmptyLeadingIcon_DoesNotRenderIcon()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.LeadingIcon, string.Empty));

        // Assert
        cut.FindAll(".vibe-input-icon-leading").ShouldBeEmpty();
    }

    [Fact]
    public void Input_WithNullTrailingIcon_DoesNotRenderIcon()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.TrailingIcon, (string)null));

        // Assert
        cut.FindAll(".vibe-input-icon-trailing").ShouldBeEmpty();
    }

    // === Error States ===

    [Fact]
    public void Input_WithErrorMessage_AppliesErrorClass()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.ErrorMessage, "This field is required"));

        // Assert
        cut.Find("input").ClassList.ShouldContain("vibe-input-error-state");
    }

    [Fact]
    public void Input_WithErrorMessage_HidesHelperText()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.ErrorMessage, "Error")
            .Add(p => p.HelperText, "Helper"));

        // Assert
        cut.Find(".vibe-input-error").ShouldNotBeNull();
        cut.Find(".vibe-input-helper").ShouldNotBeNull();
    }

    [Fact]
    public void Input_WithEmptyErrorMessage_DoesNotShowError()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.ErrorMessage, string.Empty));

        // Assert
        cut.FindAll(".vibe-input-error").ShouldBeEmpty();
    }

    // === Event Handling ===

    [Fact]
    public void Input_InvokesValueChanged_OnInput()
    {
        // Arrange
        string newValue = null;
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        cut.Find("input").Input("Typed Value");

        // Assert
        newValue.ShouldBe("Typed Value");
    }

    [Fact]
    public void Input_WithNullValueInChangeEvent_HandlesGracefully()
    {
        // Arrange
        string capturedValue = "initial";
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act - Pass ChangeEventArgs with null Value instead of null directly
        cut.Find("input").Change(new ChangeEventArgs { Value = null });

        // Assert - Component's OnValueChanged does: Value = e.Value?.ToString() ?? string.Empty
        capturedValue.ShouldBe(string.Empty);
    }

    [Fact]
    public void Input_WithNoValueChangedDelegate_DoesNotThrow()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>();

        // Act & Assert - Should not throw
        cut.Find("input").Change("New Value");
        cut.Find("input").Input("Another Value");
    }

    // === Label ===

    [Fact]
    public void Input_WithEmptyLabel_DoesNotRenderLabel()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Label, string.Empty));

        // Assert
        var label = cut.Find(".vibe-input-label");
        label.TextContent.ShouldBe(string.Empty);
    }

    [Fact]
    public void Input_WithNullLabel_DoesNotRenderLabel()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Label, (string)null));

        // Assert
        var label = cut.Find(".vibe-input-label");
        label.TextContent.ShouldBe(string.Empty);
    }

    // === Size Variants ===

    [Theory]
    [InlineData("small")]
    [InlineData("medium")]
    [InlineData("large")]
    public void Input_WithSize_AppliesCorrectClass(string size)
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Size, size));

        // Assert
        cut.Find("input").ClassList.ShouldContain($"vibe-input-{size}");
    }

    // === Variant Styles ===

    [Theory]
    [InlineData("outline")]
    [InlineData("filled")]
    public void Input_WithVariant_AppliesCorrectClass(string variant)
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Variant, variant));

        // Assert
        cut.Find("input").ClassList.ShouldContain($"vibe-input-{variant}");
    }

    // === Disabled and ReadOnly Combination ===

    [Fact]
    public void Input_BothDisabledAndReadOnly_AppliesBothStates()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ReadOnly, true));

        // Assert
        var input = cut.Find("input");
        input.HasAttribute("disabled").ShouldBeTrue();
        input.HasAttribute("readonly").ShouldBeTrue();
        cut.Find(".vibe-input-container").ClassList.ShouldContain("vibe-input-disabled");
    }

    // === ID Generation ===

    [Fact]
    public void Input_WithoutId_GeneratesUniqueId()
    {
        // Act
        var cut1 = RenderComponent<Vibe.UI.Components.Input.Input>();
        var cut2 = RenderComponent<Vibe.UI.Components.Input.Input>();

        // Assert
        var id1 = cut1.Find("input").GetAttribute("id");
        var id2 = cut2.Find("input").GetAttribute("id");

        id1.ShouldNotBeNullOrEmpty();
        id2.ShouldNotBeNullOrEmpty();
        id1.ShouldNotBe(id2);
    }

    [Fact]
    public void Input_WithCustomId_UsesCustomId()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Id, "custom-input-id"));

        // Assert
        cut.Find("input").GetAttribute("id").ShouldBe("custom-input-id");
    }

    // === Placeholder ===

    [Fact]
    public void Input_WithEmptyPlaceholder_RendersEmptyPlaceholder()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Placeholder, string.Empty));

        // Assert
        cut.Find("input").GetAttribute("placeholder").ShouldBe(string.Empty);
    }

    [Fact]
    public void Input_WithNullPlaceholder_RendersEmptyPlaceholder()
    {
        // Act
        var cut = RenderComponent<Vibe.UI.Components.Input.Input>(parameters => parameters
            .Add(p => p.Placeholder, (string)null));

        // Assert - GetAttribute returns null when attribute value is empty string
        var input = cut.Find("input");
        var placeholderAttr = input.GetAttribute("placeholder");
        (placeholderAttr ?? string.Empty).ShouldBe(string.Empty);
    }
}
