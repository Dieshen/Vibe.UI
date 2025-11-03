namespace Vibe.UI.Tests.Components.Input;

public class TextAreaTests : TestBase
{
    [Fact]
    public void TextArea_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<TextArea>();

        // Assert
        var textarea = cut.Find("textarea");
        textarea.ShouldNotBeNull();
        textarea.ClassList.ShouldContain("vibe-textarea");
        textarea.GetAttribute("rows").ShouldBe("4");
    }

    [Fact]
    public void TextArea_Renders_WithValue()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Value, "Test content"));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.TextContent.ShouldBe("Test content");
    }

    [Fact]
    public void TextArea_Renders_WithPlaceholder()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Placeholder, "Enter text..."));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("placeholder").ShouldBe("Enter text...");
    }

    [Fact]
    public void TextArea_Applies_Disabled_Attribute()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void TextArea_Applies_ReadOnly_Attribute()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact]
    public void TextArea_Applies_Custom_Rows()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Rows, 10));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("rows").ShouldBe("10");
    }

    [Fact]
    public void TextArea_InvokesValueChanged_OnInput()
    {
        // Arrange
        string newValue = null;
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        cut.Find("textarea").Input("New text");

        // Assert
        newValue.ShouldBe("New text");
    }

    [Fact]
    public void TextArea_InvokesOnInput_Callback()
    {
        // Arrange
        ChangeEventArgs capturedArgs = null;
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.OnInput, args => capturedArgs = args));

        // Act
        cut.Find("textarea").Input("Test");

        // Assert
        capturedArgs.ShouldNotBeNull();
        capturedArgs.Value.ShouldBe("Test");
    }

    [Fact]
    public void TextArea_InvokesOnChange_Callback()
    {
        // Arrange
        ChangeEventArgs capturedArgs = null;
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.OnChange, args => capturedArgs = args));

        // Act
        cut.Find("textarea").Change("Changed");

        // Assert
        capturedArgs.ShouldNotBeNull();
        capturedArgs.Value.ShouldBe("Changed");
    }

    // === Edge Cases ===

    [Fact]
    public void TextArea_WithNullValue_RendersEmpty()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Value, (string)null));

        // Assert
        cut.Find("textarea").TextContent.ShouldBe(string.Empty);
    }

    [Fact]
    public void TextArea_WithEmptyValue_RendersEmpty()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Value, string.Empty));

        // Assert
        cut.Find("textarea").TextContent.ShouldBe(string.Empty);
    }

    [Fact]
    public void TextArea_WithVeryLongText_RendersCorrectly()
    {
        // Arrange
        var longText = new string('A', 10000);

        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Value, longText));

        // Assert
        cut.Find("textarea").TextContent.ShouldBe(longText);
    }

    [Fact]
    public void TextArea_WithMultilineText_RendersCorrectly()
    {
        // Arrange
        var multilineText = "Line 1\nLine 2\nLine 3\n\nLine 5";

        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Value, multilineText));

        // Assert
        cut.Find("textarea").TextContent.ShouldBe(multilineText);
    }

    [Fact]
    public void TextArea_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange - Remove \r as it gets normalized to \n on Windows
        var specialChars = "<>&\"'`\n\t";

        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Value, specialChars));

        // Assert
        cut.Find("textarea").TextContent.ShouldBe(specialChars);
    }

    [Fact]
    public void TextArea_WithUnicodeCharacters_HandlesCorrectly()
    {
        // Arrange
        var unicode = "Hello 世界 🚀 emoji\n日本語テキスト";

        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Value, unicode));

        // Assert
        cut.Find("textarea").TextContent.ShouldBe(unicode);
    }

    // === Rows Configuration ===

    [Fact]
    public void TextArea_WithZeroRows_RendersZeroRows()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Rows, 0));

        // Assert
        cut.Find("textarea").GetAttribute("rows").ShouldBe("0");
    }

    [Fact]
    public void TextArea_WithLargeRowCount_RendersCorrectly()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Rows, 50));

        // Assert
        cut.Find("textarea").GetAttribute("rows").ShouldBe("50");
    }

    // === Placeholder ===

    [Fact]
    public void TextArea_WithNullPlaceholder_RendersEmpty()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Placeholder, (string)null));

        // Assert
        cut.Find("textarea").GetAttribute("placeholder").ShouldBeNull();
    }

    [Fact]
    public void TextArea_WithEmptyPlaceholder_RendersEmpty()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Placeholder, string.Empty));

        // Assert
        cut.Find("textarea").GetAttribute("placeholder").ShouldBe(string.Empty);
    }

    // === Event Handling ===

    [Fact]
    public void TextArea_WithNoCallbacks_DoesNotThrow()
    {
        // Act
        var cut = RenderComponent<TextArea>();

        // Act & Assert - Should not throw
        cut.Find("textarea").Input("Test");
        cut.Find("textarea").Change("Changed");
    }

    [Fact]
    public void TextArea_WithNullValueInEvent_HandlesGracefully()
    {
        // Arrange
        string capturedValue = "initial";
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act - Pass ChangeEventArgs with null Value
        cut.Find("textarea").Input(new ChangeEventArgs { Value = null });

        // Assert - Component checks: args.Value is string value - when null, doesn't invoke ValueChanged
        // So value remains "initial" - the callback isn't invoked when Value is not a string
        capturedValue.ShouldBe("initial");
    }

    [Fact]
    public void TextArea_MultipleRapidInputs_HandlesCorrectly()
    {
        // Arrange
        var inputCount = 0;
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.OnInput, args => inputCount++));

        var textarea = cut.Find("textarea");

        // Act - Simulate rapid typing
        textarea.Input("A");
        textarea.Input("AB");
        textarea.Input("ABC");

        // Assert
        inputCount.ShouldBe(3);
    }

    [Fact]
    public void TextArea_OnInput_UpdatesValueAndInvokesCallback()
    {
        // Arrange
        string capturedValue = null;
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.ValueChanged, value => capturedValue = value));

        // Act
        cut.Find("textarea").Input("New text");

        // Assert
        capturedValue.ShouldBe("New text");
    }

    // === Disabled and ReadOnly States ===

    [Fact]
    public void TextArea_BothDisabledAndReadOnly_AppliesBothAttributes()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ReadOnly, true));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.HasAttribute("disabled").ShouldBeTrue();
        textarea.HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact]
    public void TextArea_WhenDisabled_HasDisabledAttribute()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        cut.Find("textarea").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void TextArea_WhenReadOnly_HasReadOnlyAttribute()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        cut.Find("textarea").HasAttribute("readonly").ShouldBeTrue();
    }

    // === CSS Classes ===

    [Fact]
    public void TextArea_HasBaseClass()
    {
        // Act
        var cut = RenderComponent<TextArea>();

        // Assert
        cut.Find("textarea").ClassList.ShouldContain("vibe-textarea");
    }

    // === Additional Attributes ===

    [Fact]
    public void TextArea_WithAdditionalAttributes_MergesCorrectly()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "my-textarea" },
                { "aria-label", "Custom TextArea" },
                { "maxlength", "500" }
            }));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("data-testid").ShouldBe("my-textarea");
        textarea.GetAttribute("aria-label").ShouldBe("Custom TextArea");
        textarea.GetAttribute("maxlength").ShouldBe("500");
    }

    // === Complex Scenarios ===

    [Fact]
    public void TextArea_WithAllProperties_RendersCorrectly()
    {
        // Act
        var cut = RenderComponent<TextArea>(parameters => parameters
            .Add(p => p.Value, "Initial text")
            .Add(p => p.Placeholder, "Enter text...")
            .Add(p => p.Rows, 8)
            .Add(p => p.Disabled, false)
            .Add(p => p.ReadOnly, false));

        // Assert
        var textarea = cut.Find("textarea");
        textarea.TextContent.ShouldBe("Initial text");
        textarea.GetAttribute("placeholder").ShouldBe("Enter text...");
        textarea.GetAttribute("rows").ShouldBe("8");
        textarea.HasAttribute("disabled").ShouldBeFalse();
        textarea.HasAttribute("readonly").ShouldBeFalse();
    }
}
