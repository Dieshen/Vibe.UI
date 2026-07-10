namespace Vibe.UI.Tests.Components.Inputs;

public class RichTextEditorTests : TestBase
{
    private const string ModulePath = "./_content/Vibe.UI/js/vibe-richtext.js";

    [Fact]
    public void RichTextEditor_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<RichTextEditor>();

        // Assert
        var root = cut.Find(".vibe-richtext");
        root.ShouldNotBeNull();
        root.ClassList.ShouldNotContain("richtext-disabled");
        root.ClassList.ShouldNotContain("richtext-readonly");
    }

    [Fact]
    public void RichTextEditor_Renders_Toolbar_ByDefault_WithAccessibleSemantics()
    {
        // Act
        var cut = Render<RichTextEditor>();

        // Assert
        var toolbar = cut.Find(".richtext-toolbar");
        toolbar.GetAttribute("role").ShouldBe("toolbar");
        toolbar.GetAttribute("aria-label").ShouldBe("Rich text formatting");

        var groups = cut.FindAll(".toolbar-group");
        groups.Count.ShouldBe(6);
        groups[0].GetAttribute("role").ShouldBe("group");
        groups[0].GetAttribute("aria-label").ShouldBe("Text formatting");

        var boldButton = cut.Find("button[aria-label='Bold']");
        boldButton.GetAttribute("type").ShouldBe("button");
        boldButton.GetAttribute("title").ShouldBe("Bold");
        boldButton.HasAttribute("disabled").ShouldBeFalse();
    }

    [Fact]
    public void RichTextEditor_Hides_Toolbar_WhenShowToolbarIsFalse()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.ShowToolbar, false));

        // Assert
        cut.FindAll(".richtext-toolbar").ShouldBeEmpty();
    }

    [Fact]
    public void RichTextEditor_Renders_EditorArea_WithAccessibleTextboxSemantics()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Placeholder, "Start typing..."));

        // Assert
        var editor = cut.Find(".richtext-editor");
        editor.GetAttribute("contenteditable").ShouldBe("true");
        editor.GetAttribute("role").ShouldBe("textbox");
        editor.GetAttribute("aria-multiline").ShouldBe("true");
        editor.GetAttribute("aria-label").ShouldBe("Rich text editor");
        editor.GetAttribute("data-placeholder").ShouldBe("Start typing...");
        editor.GetAttribute("aria-placeholder").ShouldBe("Start typing...");
        editor.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact]
    public void RichTextEditor_UsesFallbackAccessibleNames_WhenLabelsAreBlank()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.AriaLabel, " ")
            .Add(p => p.ToolbarAriaLabel, string.Empty));

        // Assert
        cut.Find(".richtext-editor").GetAttribute("aria-label").ShouldBe("Rich text editor");
        cut.Find(".richtext-toolbar").GetAttribute("aria-label").ShouldBe("Rich text formatting");
    }

    [Fact]
    public void RichTextEditor_Renders_ContentAsEncodedText_ByDefault()
    {
        // Arrange
        const string content = "<img src=x onerror=alert(1)><strong>Safe</strong>";

        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var editor = cut.Find(".richtext-editor");
        editor.TextContent.ShouldContain("<img src=x onerror=alert(1)>");
        editor.TextContent.ShouldContain("<strong>Safe</strong>");
        editor.InnerHtml.ShouldContain("&lt;img");
        editor.InnerHtml.ShouldNotContain("<img");
        editor.InnerHtml.ShouldNotContain("<strong>Safe</strong>");
    }

    [Fact]
    public void RichTextEditor_Renders_RawHtml_WhenTrustedRenderingIsEnabled()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Content, "<p>Hello <strong>world</strong></p>")
            .Add(p => p.RenderTrustedHtml, true));

        // Assert
        var editor = cut.Find(".richtext-editor");
        editor.InnerHtml.ShouldContain("<p>Hello <strong>world</strong></p>");
        editor.TextContent.ShouldBe("Hello world");
    }

    [Fact]
    public void RichTextEditor_InvokesContentAndValueChanged_OnInput_WhenEditable()
    {
        // Arrange
        string? contentChanged = null;
        string? valueChanged = null;
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.ContentChanged, value => contentChanged = value)
            .Add(p => p.ValueChanged, value => valueChanged = value));

        // Act
        cut.Find(".richtext-editor").Input("<p>Updated</p>");

        // Assert
        contentChanged.ShouldBe("<p>Updated</p>");
        valueChanged.ShouldBe("<p>Updated</p>");
        cut.Find(".richtext-editor").TextContent.ShouldBe("<p>Updated</p>");
    }

    [Fact]
    public void RichTextEditor_ReadsContentEditableHtml_WhenEventHasNoStringValue()
    {
        // Arrange
        string? contentChanged = null;
        var module = JSInterop.SetupModule(ModulePath);
        module.Setup<string>("getHtml", _ => true).SetResult("<p>Browser content</p>");
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Content, "Original")
            .Add(p => p.ContentChanged, value => contentChanged = value));

        // Act
        cut.Find(".richtext-editor").Input(new ChangeEventArgs { Value = null });

        // Assert
        contentChanged.ShouldBe("<p>Browser content</p>");
        cut.Find(".richtext-editor").TextContent.ShouldBe("<p>Browser content</p>");
        JSInterop.Invocations.ElementAt(0).Identifier.ShouldBe("import");
        JSInterop.Invocations.ElementAt(0).Arguments[0].ShouldBe(ModulePath);
        JSInterop.Invocations.ElementAt(1).Identifier.ShouldBe("getHtml");
    }

    [Fact]
    public void RichTextEditor_UsesValueAlias_WhenContentIsNotProvided()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Value, "Alias content"));

        // Assert
        cut.Find(".richtext-editor").TextContent.ShouldBe("Alias content");
    }

    [Fact]
    public void RichTextEditor_ContentTakesPrecedence_WhenContentAndValueAreProvided()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Content, "Content wins")
            .Add(p => p.Value, "Value loses"));

        // Assert
        cut.Find(".richtext-editor").TextContent.ShouldBe("Content wins");
    }

    [Fact]
    public void RichTextEditor_IsReadOnly_WhenReadOnlyIsTrue()
    {
        // Arrange
        string? changed = null;
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Content, "Read only")
            .Add(p => p.ReadOnly, true)
            .Add(p => p.ContentChanged, value => changed = value));

        // Act
        cut.Find(".richtext-editor").Input("Changed");

        // Assert
        var editor = cut.Find(".richtext-editor");
        editor.GetAttribute("contenteditable").ShouldBe("false");
        editor.GetAttribute("aria-readonly").ShouldBe("true");
        editor.GetAttribute("tabindex").ShouldBe("0");
        cut.Find(".vibe-richtext").ClassList.ShouldContain("richtext-readonly");
        cut.FindAll(".toolbar-button").ShouldAllBe(button => button.HasAttribute("disabled"));
        changed.ShouldBeNull();
        editor.TextContent.ShouldBe("Read only");
    }

    [Fact]
    public void RichTextEditor_IsDisabled_WhenDisabledIsTrue()
    {
        // Arrange
        string? changed = null;
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Content, "Disabled")
            .Add(p => p.Disabled, true)
            .Add(p => p.ContentChanged, value => changed = value));

        // Act
        cut.Find(".richtext-editor").Input("Changed");

        // Assert
        var editor = cut.Find(".richtext-editor");
        editor.GetAttribute("contenteditable").ShouldBe("false");
        editor.GetAttribute("aria-disabled").ShouldBe("true");
        editor.GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".vibe-richtext").ClassList.ShouldContain("richtext-disabled");
        cut.FindAll(".toolbar-button").ShouldAllBe(button => button.HasAttribute("disabled"));
        changed.ShouldBeNull();
        editor.TextContent.ShouldBe("Disabled");
    }

    [Fact]
    public void RichTextEditor_Hides_LinkAndImageButtons_WhenNotAllowed()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.AllowLinks, false)
            .Add(p => p.AllowImages, false));

        // Assert
        cut.FindAll("button[aria-label='Insert link']").ShouldBeEmpty();
        cut.FindAll("button[aria-label='Insert image']").ShouldBeEmpty();
    }

    [Fact]
    public void RichTextEditor_ExecutesToolbarCommand_ThroughJsInterop()
    {
        // Arrange
        var cut = Render<RichTextEditor>();

        // Act
        cut.Find("button[aria-label='Bold']").Click();

        // Assert
        JSInterop.Invocations.Count.ShouldBe(2);
        JSInterop.Invocations.ElementAt(0).Identifier.ShouldBe("import");
        JSInterop.Invocations.ElementAt(0).Arguments[0].ShouldBe(ModulePath);
        var invocation = JSInterop.Invocations.ElementAt(1);
        invocation.Identifier.ShouldBe("executeCommand");
        invocation.Arguments[1].ShouldBe("bold");
        invocation.Arguments[2].ShouldBeNull();
    }

    [Fact]
    public void RichTextEditor_DoesNotInsertLink_WhenPromptReturnsUnsafeUrl()
    {
        // Arrange
        var module = JSInterop.SetupModule(ModulePath);
        module.Setup<string?>("promptForUrl", "Enter link URL")
            .SetResult("javascript:alert(1)");
        var cut = Render<RichTextEditor>();

        // Act
        cut.Find("button[aria-label='Insert link']").Click();

        // Assert
        JSInterop.Invocations.Count.ShouldBe(2);
        JSInterop.Invocations.ElementAt(0).Identifier.ShouldBe("import");
        JSInterop.Invocations.ElementAt(1).Identifier.ShouldBe("promptForUrl");
        JSInterop.Invocations.ShouldNotContain(invocation => invocation.Identifier == "executeCommand");
    }

    [Fact]
    public void RichTextEditor_DoesNotExecuteToolbarCommand_WhenDisabled()
    {
        // Arrange
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Act
        cut.Find("button[aria-label='Bold']").Click();

        // Assert
        JSInterop.Invocations.ShouldBeEmpty();
    }

    [Fact]
    public void RichTextEditor_Shows_CharCount_ForEncodedText()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Content, "<p>Hello</p>")
            .Add(p => p.ShowCharCount, true));

        // Assert
        cut.Find(".char-count").TextContent.ShouldBe("12 characters");
        cut.Find(".char-count").GetAttribute("aria-live").ShouldBe("polite");
    }

    [Fact]
    public void RichTextEditor_Shows_CharCount_ForTrustedHtmlVisibleText()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.Content, "<p>Hello <strong>world</strong></p>")
            .Add(p => p.RenderTrustedHtml, true)
            .Add(p => p.ShowCharCount, true));

        // Assert
        cut.Find(".char-count").TextContent.ShouldBe("11 characters");
    }

    [Fact]
    public void RichTextEditor_InvokesFocusAndBlurCallbacks()
    {
        // Arrange
        var focused = false;
        var blurred = false;
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.OnFocus, _ => focused = true)
            .Add(p => p.OnBlur, _ => blurred = true));

        // Act
        var editor = cut.Find(".richtext-editor");
        editor.Focus();
        editor.Blur();

        // Assert
        focused.ShouldBeTrue();
        blurred.ShouldBeTrue();
    }

    [Fact]
    public void RichTextEditor_Applies_CustomClasses_AndAdditionalAttributes()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.CssClass, "legacy-custom-editor")
            .Add(p => p.Class, "custom-editor")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "richtext" }
            }));

        // Assert
        var root = cut.Find(".vibe-richtext");
        root.ClassList.ShouldContain("legacy-custom-editor");
        root.ClassList.ShouldContain("custom-editor");
        root.GetAttribute("data-testid").ShouldBe("richtext");
    }

    [Fact]
    public void RichTextEditor_Applies_MinAndMaxHeightCssVariables()
    {
        // Act
        var cut = Render<RichTextEditor>(parameters => parameters
            .Add(p => p.MinHeight, 120)
            .Add(p => p.MaxHeight, 240));

        // Assert
        var style = cut.Find(".richtext-editor").GetAttribute("style") ?? string.Empty;
        style.ShouldContain("--richtext-min-height: 120px");
        style.ShouldContain("--richtext-max-height: 240px");
    }
}
