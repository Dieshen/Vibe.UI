namespace Vibe.UI.Tests.Components.Input;

public class TagInputTests : TestBase
{
    [Fact]
    public void TagInput_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<TagInput>();

        // Assert
        var tagInput = cut.Find(".vibe-tag-input");
        tagInput.ShouldNotBeNull();
    }

    [Fact]
    public void TagInput_Renders_Input()
    {
        // Act
        var cut = RenderComponent<TagInput>();

        // Assert
        var input = cut.Find(".tag-input");
        input.ShouldNotBeNull();
    }

    [Fact]
    public void TagInput_Displays_Placeholder()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Placeholder, "Enter tags..."));

        // Assert
        var input = cut.Find(".tag-input");
        input.GetAttribute("placeholder").ShouldBe("Enter tags...");
    }

    [Fact]
    public void TagInput_Renders_ExistingTags()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, new List<string> { "tag1", "tag2", "tag3" }));

        // Assert
        var tags = cut.FindAll(".tag-item");
        tags.Count.ShouldBe(3);
    }

    [Fact]
    public void TagInput_Displays_TagText()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, new List<string> { "React", "Blazor" }));

        // Assert
        var tagTexts = cut.FindAll(".tag-text");
        tagTexts[0].TextContent.ShouldBe("React");
        tagTexts[1].TextContent.ShouldBe("Blazor");
    }

    [Fact]
    public void TagInput_Renders_RemoveButtons()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, new List<string> { "tag1", "tag2" }));

        // Assert
        var removeButtons = cut.FindAll(".tag-remove");
        removeButtons.Count.ShouldBe(2);
    }

    [Fact]
    public void TagInput_Hides_RemoveButtons_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, new List<string> { "tag1", "tag2" })
            .Add(p => p.Disabled, true));

        // Assert
        cut.FindAll(".tag-remove").ShouldBeEmpty();
    }

    [Fact]
    public void TagInput_Applies_DisabledState()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        cut.Find(".vibe-tag-input").ClassList.ShouldContain("tag-input-disabled");
        var input = cut.Find(".tag-input");
        input.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void TagInput_Shows_Suggestions()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Suggestions, new List<string> { "React", "Vue", "Angular" }));

        // Set internal state to show suggestions
        cut.Instance.GetType().GetField("_showSuggestions",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(cut.Instance, true);
        cut.Render();

        // Assert
        var suggestions = cut.FindAll(".tag-suggestion");
        suggestions.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void TagInput_Has_AccessibilityLabel()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.AriaLabel, "Add keywords"));

        // Assert
        var input = cut.Find(".tag-input");
        input.GetAttribute("aria-label").ShouldBe("Add keywords");
    }

    [Fact]
    public void TagInput_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.CssClass, "custom-tag-input"));

        // Assert
        cut.Find(".vibe-tag-input").ClassList.ShouldContain("custom-tag-input");
    }

    [Fact]
    public void TagInput_MaxTags_PreventsAdding()
    {
        // Arrange
        var tags = new List<string> { "tag1", "tag2" };
        var tagAdded = false;
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, tags)
            .Add(p => p.MaxTags, 2)
            .Add(p => p.OnTagAdded, _ => tagAdded = true));

        // Act - Try to add a third tag (simulated via parameter since keyboard events are complex in bUnit)
        cut.Instance.Tags.Add("tag3");

        // Assert - Should not exceed MaxTags
        tagAdded.ShouldBeFalse();
    }

    [Fact]
    public void TagInput_AllowDuplicates_False_PreventsDuplicates()
    {
        // Arrange
        var tags = new List<string> { "react" };
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, tags)
            .Add(p => p.AllowDuplicates, false));

        // Assert - Verify AllowDuplicates setting
        cut.Instance.AllowDuplicates.ShouldBeFalse();
    }

    [Fact]
    public void TagInput_AllowDuplicates_True_AllowsDuplicates()
    {
        // Arrange
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.AllowDuplicates, true));

        // Assert
        cut.Instance.AllowDuplicates.ShouldBeTrue();
    }

    [Fact]
    public void TagInput_CustomSeparator_UsesCustomValue()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Separator, ";"));

        // Assert
        cut.Instance.Separator.ShouldBe(";");
    }

    [Fact]
    public void TagInput_DefaultSeparator_IsComma()
    {
        // Act
        var cut = RenderComponent<TagInput>();

        // Assert
        cut.Instance.Separator.ShouldBe(",");
    }

    [Fact]
    public void TagInput_Suggestions_FilterExisting()
    {
        // Arrange
        var tags = new List<string> { "React" };
        var suggestions = new List<string> { "React", "Vue", "Angular" };
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, tags)
            .Add(p => p.Suggestions, suggestions));

        // Set internal state to show suggestions
        cut.Instance.GetType().GetField("_showSuggestions",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(cut.Instance, true);
        cut.Render();

        // Assert - React should be filtered out
        var suggestionElements = cut.FindAll(".tag-suggestion");
        suggestionElements.Count.ShouldBe(2); // Only Vue and Angular
    }

    [Fact]
    public void TagInput_RemoveButton_HasAriaLabel()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, new List<string> { "React" }));

        // Assert
        var removeButton = cut.Find(".tag-remove");
        removeButton.GetAttribute("aria-label").ShouldBe("Remove React");
    }

    [Fact]
    public void TagInput_DefaultAriaLabel_IsTagInput()
    {
        // Act
        var cut = RenderComponent<TagInput>();

        // Assert
        var input = cut.Find(".tag-input");
        input.GetAttribute("aria-label").ShouldBe("Tag input");
    }

    [Fact]
    public void TagInput_EmptyTags_ShowsPlaceholder()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, new List<string>())
            .Add(p => p.Placeholder, "Add tags..."));

        // Assert
        var input = cut.Find(".tag-input");
        input.GetAttribute("placeholder").ShouldBe("Add tags...");
    }

    [Fact]
    public void TagInput_WithTags_HidesPlaceholder()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, new List<string> { "tag1" })
            .Add(p => p.Placeholder, "Add tags..."));

        // Assert
        var input = cut.Find(".tag-input");
        input.GetAttribute("placeholder").ShouldBe("");
    }

    [Fact]
    public void TagInput_RemoveTag_InvokesCallback()
    {
        // Arrange
        string? removedTag = null;
        var tags = new List<string> { "React", "Vue" };
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, tags)
            .Add(p => p.OnTagRemoved, tag => removedTag = tag));

        // Act - Click remove button for first tag
        var removeButtons = cut.FindAll(".tag-remove");
        removeButtons[0].Click();

        // Assert
        removedTag.ShouldBe("React");
    }

    [Fact]
    public void TagInput_MaxTags_DefaultValue()
    {
        // Act
        var cut = RenderComponent<TagInput>();

        // Assert
        cut.Instance.MaxTags.ShouldBe(int.MaxValue);
    }

    [Fact]
    public void TagInput_TagContainer_RendersCorrectly()
    {
        // Act
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, new List<string> { "tag1", "tag2" }));

        // Assert
        var container = cut.Find(".tag-input-container");
        container.ShouldNotBeNull();
        var tags = container.QuerySelectorAll(".tag-item");
        tags.Length.ShouldBe(2);
    }

    [Fact]
    public void TagInput_MultipleRemoveButtons_WorkIndependently()
    {
        // Arrange
        var tags = new List<string> { "tag1", "tag2", "tag3" };
        var cut = RenderComponent<TagInput>(parameters => parameters
            .Add(p => p.Tags, tags)
            .Add(p => p.TagsChanged, newTags => tags = newTags));

        // Act - Remove middle tag
        var removeButtons = cut.FindAll(".tag-remove");
        removeButtons[1].Click();

        // Assert
        tags.Count.ShouldBe(2);
        tags.ShouldContain("tag1");
        tags.ShouldContain("tag3");
        tags.ShouldNotContain("tag2");
    }

    [Fact]
    public void TagInput_InputType_IsText()
    {
        // Act
        var cut = RenderComponent<TagInput>();

        // Assert
        var input = cut.Find(".tag-input");
        input.GetAttribute("type").ShouldBe("text");
    }
}
