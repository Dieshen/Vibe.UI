namespace Vibe.UI.Tests.Components.Form;

public class LabelTests : TestBase
{
    [Fact]
    public void Label_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("Username"));

        // Assert
        var label = cut.Find("label");
        label.ShouldNotBeNull();
        label.TextContent.ShouldBe("Username");
    }

    [Fact]
    public void Label_Applies_For_Attribute()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.For, "username-input")
            .AddChildContent("Username"));

        // Assert
        var label = cut.Find("label");
        label.GetAttribute("for").ShouldBe("username-input");
    }

    [Fact]
    public void Label_Applies_Required_Class()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.Required, true)
            .AddChildContent("Email"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-label-required");
    }

    [Fact]
    public void Label_Applies_Disabled_Class()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("Disabled Field"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-label-disabled");
    }

    [Fact]
    public void Label_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.CssClass, "custom-label")
            .AddChildContent("Custom"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("custom-label");
    }

    [Fact]
    public void Label_Renders_ChildContent()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("<strong>Bold Label</strong>"));

        // Assert
        var label = cut.Find("label");
        label.InnerHtml.ShouldContain("<strong>Bold Label</strong>");
    }

    #region State Combinations

    [Fact]
    public void Label_Applies_RequiredAndDisabled_Together()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.Disabled, true)
            .AddChildContent("Field"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-label-required");
        label.ClassList.ShouldContain("vibe-label-disabled");
    }

    [Fact]
    public void Label_Applies_AllStates_Together()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.Required, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.CssClass, "custom")
            .Add(p => p.For, "my-input")
            .AddChildContent("Complex Label"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-label");
        label.ClassList.ShouldContain("vibe-label-required");
        label.ClassList.ShouldContain("vibe-label-disabled");
        label.ClassList.ShouldContain("custom");
        label.GetAttribute("for").ShouldBe("my-input");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Label_HandlesEmptyFor_Attribute()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.For, "")
            .AddChildContent("Label"));

        // Assert
        var label = cut.Find("label");
        label.GetAttribute("for").ShouldBe("");
    }

    [Fact]
    public void Label_HandlesNullFor_Attribute()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.For, null)
            .AddChildContent("Label"));

        // Assert
        var label = cut.Find("label");
        label.HasAttribute("for").ShouldBeFalse();
    }

    [Fact]
    public void Label_HandlesVeryLongContent()
    {
        // Arrange
        var longText = new string('a', 1000);

        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent(longText));

        // Assert
        var label = cut.Find("label");
        label.TextContent.ShouldBe(longText);
    }

    [Fact]
    public void Label_HandlesSpecialCharacters()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("Label with special chars: !@#$%^&*()"));

        // Assert
        var label = cut.Find("label");
        label.TextContent.ShouldContain("!@#$%^&*()");
    }

    [Fact]
    public void Label_HandlesUnicodeCharacters()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("Unicode: 你好 🌍 مرحبا"));

        // Assert
        var label = cut.Find("label");
        label.TextContent.ShouldContain("你好");
        label.TextContent.ShouldContain("🌍");
    }

    [Fact]
    public void Label_HandlesNestedElements()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("<span class='icon'>*</span><span>Required Field</span>"));

        // Assert
        var label = cut.Find("label");
        cut.FindAll("span").Count.ShouldBe(2);
        label.TextContent.ShouldContain("Required Field");
    }

    #endregion

    #region CSS Class Management

    [Fact]
    public void Label_CombinesMultipleClasses_Correctly()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.CssClass, "class1 class2")
            .Add(p => p.Required, true)
            .AddChildContent("Label"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-label");
        label.ClassList.ShouldContain("vibe-label-required");
        label.ClassList.ShouldContain("class1");
        label.ClassList.ShouldContain("class2");
    }

    [Fact]
    public void Label_HandlesNullCssClass()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.CssClass, null)
            .AddChildContent("Label"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-label");
    }

    [Fact]
    public void Label_HandlesEmptyCssClass()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.CssClass, "")
            .AddChildContent("Label"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("vibe-label");
    }

    #endregion

    #region Default States

    [Fact]
    public void Label_Required_DefaultsToFalse()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("Label"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldNotContain("vibe-label-required");
    }

    [Fact]
    public void Label_Disabled_DefaultsToFalse()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("Label"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldNotContain("vibe-label-disabled");
    }

    #endregion

    #region Accessibility

    [Fact]
    public void Label_AlwaysRendersAsLabelElement()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("Accessible Label"));

        // Assert
        var label = cut.Find("label");
        label.ShouldNotBeNull();
        label.NodeName.ShouldBe("LABEL");
    }

    [Fact]
    public void Label_WithFor_CreatesAccessibleConnection()
    {
        // Act
        var cut = RenderComponent<Label>(parameters => parameters
            .Add(p => p.For, "input-123")
            .AddChildContent("Email Address"));

        // Assert
        var label = cut.Find("label");
        label.GetAttribute("for").ShouldBe("input-123");
    }

    #endregion
}
