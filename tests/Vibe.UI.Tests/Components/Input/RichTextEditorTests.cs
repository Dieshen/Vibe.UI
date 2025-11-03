namespace Vibe.UI.Tests.Components.Input;

public class RichTextEditorTests : TestBase
{
    [Fact]
    public void RichTextEditor_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>();

        // Assert
        cut.Find(".vibe-richtext").ShouldNotBeNull();
    }

    [Fact]
    public void RichTextEditor_Renders_Toolbar_ByDefault()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>(parameters => parameters
            .Add(p => p.ShowToolbar, true));

        // Assert
        cut.Find(".richtext-toolbar").ShouldNotBeNull();
    }

    [Fact]
    public void RichTextEditor_Hides_Toolbar_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>(parameters => parameters
            .Add(p => p.ShowToolbar, false));

        // Assert
        cut.FindAll(".richtext-toolbar").ShouldBeEmpty();
    }

    [Fact]
    public void RichTextEditor_Renders_EditorArea()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>();

        // Assert
        var editor = cut.Find(".richtext-editor");
        editor.ShouldNotBeNull();
        editor.GetAttribute("contenteditable").ShouldBe("true");
    }

    [Fact]
    public void RichTextEditor_IsReadOnly_WhenReadOnlyIsTrue()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        var editor = cut.Find(".richtext-editor");
        editor.GetAttribute("contenteditable").ShouldBe("false");
        cut.Find(".vibe-richtext").ClassList.ShouldContain("richtext-readonly");
    }

    [Fact]
    public void RichTextEditor_Renders_ToolbarButtons()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>(parameters => parameters
            .Add(p => p.ShowToolbar, true));

        // Assert
        var buttons = cut.FindAll(".toolbar-button");
        buttons.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void RichTextEditor_Allows_Links_ByDefault()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>();

        // Assert
        cut.Instance.AllowLinks.ShouldBeTrue();
    }

    [Fact]
    public void RichTextEditor_Allows_Images_ByDefault()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>();

        // Assert
        cut.Instance.AllowImages.ShouldBeTrue();
    }

    [Fact]
    public void RichTextEditor_Shows_CharCount_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>(parameters => parameters
            .Add(p => p.ShowCharCount, true));

        // Assert
        cut.Find(".richtext-footer").ShouldNotBeNull();
        cut.Find(".char-count").ShouldNotBeNull();
    }

    [Fact]
    public void RichTextEditor_Hides_CharCount_ByDefault()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>(parameters => parameters
            .Add(p => p.ShowCharCount, false));

        // Assert
        cut.FindAll(".richtext-footer").ShouldBeEmpty();
    }

    [Fact]
    public void RichTextEditor_Has_AriaLabel()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>(parameters => parameters
            .Add(p => p.AriaLabel, "Document editor"));

        // Assert
        var editor = cut.Find(".richtext-editor");
        editor.GetAttribute("aria-label").ShouldBe("Document editor");
    }

    [Fact]
    public void RichTextEditor_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<RichTextEditor>(parameters => parameters
            .Add(p => p.CssClass, "custom-editor"));

        // Assert
        cut.Find(".vibe-richtext").ClassList.ShouldContain("custom-editor");
    }
}
