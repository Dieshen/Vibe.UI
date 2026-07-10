namespace Vibe.UI.Tests.Components.Inputs;

public class MultiSelectTests : TestBase
{
    [Fact]
    public void MultiSelect_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2", "Option 3" }));

        // Assert
        var multiselect = cut.Find(".multiselect-container");
        multiselect.ShouldNotBeNull();
    }

    [Fact]
    public void MultiSelect_Renders_Label()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Label, "Select items")
            .Add(p => p.Items, new List<string> { "Option 1" }));

        // Assert
        var label = cut.Find(".multiselect-label");
        label.TextContent.ShouldBe("Select items");
    }

    [Fact]
    public void MultiSelect_Shows_Placeholder_WhenNoSelection()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Placeholder, "Choose options...")
            .Add(p => p.Items, new List<string> { "Option 1" }));

        // Assert
        var placeholder = cut.Find(".multiselect-placeholder");
        placeholder.TextContent.ShouldBe("Choose options...");
    }

    [Fact]
    public void MultiSelect_Renders_SearchInput()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1" }));

        // Assert
        var input = cut.Find(".multiselect-input");
        input.ShouldNotBeNull();
    }

    [Fact]
    public void MultiSelect_Applies_DisabledState()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1" })
            .Add(p => p.Disabled, true));

        // Assert
        var input = cut.Find(".multiselect-input");
        input.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void MultiSelect_Renders_Chevron()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1" }));

        // Assert
        var chevron = cut.Find(".multiselect-chevron");
        chevron.ShouldNotBeNull();
    }

    [Fact]
    public void MultiSelect_Opens_Dropdown_OnClick()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" }));

        var container = cut.Find(".multiselect-container");
        container.Click();

        // Assert
        cut.FindAll(".multiselect-dropdown").ShouldNotBeEmpty();
    }

    [Fact]
    public void MultiSelect_Renders_Options_InDropdown()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2", "Option 3" }));

        var container = cut.Find(".multiselect-container");
        container.Click();

        // Assert
        var options = cut.FindAll(".multiselect-option");
        options.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void MultiSelect_Shows_Checkboxes_ForOptions()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1" }));

        var container = cut.Find(".multiselect-container");
        container.Click();

        // Assert
        cut.FindAll(".multiselect-checkbox").ShouldNotBeEmpty();
    }

    [Fact]
    public void MultiSelect_Renders_Indicators()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1" })
            .Add(p => p.AllowClear, true));

        // Assert - Check indicators container exists
        var indicators = cut.Find(".multiselect-indicators");
        indicators.ShouldNotBeNull();
    }

    [Fact]
    public void MultiSelect_EmptySelection_ShowsPlaceholder()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItems, new List<string>())
            .Add(p => p.Placeholder, "Select items..."));

        // Assert
        var placeholder = cut.Find(".multiselect-placeholder");
        placeholder.TextContent.ShouldBe("Select items...");
    }

    [Fact]
    public void MultiSelect_SingleSelection_ShowsTag()
    {
        // Act
        var selectedItems = new List<string> { "Option 1" };
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItems, selectedItems));

        // Assert
        var tags = cut.FindAll(".multiselect-tag");
        tags.Count.ShouldBe(1);
        tags[0].TextContent.ShouldContain("Option 1");
    }

    [Fact]
    public void MultiSelect_MultipleSelections_ShowsMultipleTags()
    {
        // Act
        var selectedItems = new List<string> { "Option 1", "Option 2", "Option 3" };
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2", "Option 3" })
            .Add(p => p.SelectedItems, selectedItems));

        // Assert
        var tags = cut.FindAll(".multiselect-tag");
        tags.Count.ShouldBe(3);
    }

    [Fact]
    public void MultiSelect_ToggleItem_AddsToSelection()
    {
        // Arrange
        var selectedItems = new List<string>();
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItems, selectedItems)
            .Add(p => p.SelectedItemsChanged, items => selectedItems = items));

        // Act - Open dropdown and click first option
        cut.Find(".multiselect-container").Click();
        var options = cut.FindAll(".multiselect-option");
        options[0].Click();

        // Assert
        selectedItems.Count.ShouldBe(1);
        selectedItems[0].ShouldBe("Option 1");
    }

    [Fact]
    public void MultiSelect_ToggleItem_RemovesFromSelection()
    {
        // Arrange
        var selectedItems = new List<string> { "Option 1" };
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItems, selectedItems)
            .Add(p => p.SelectedItemsChanged, items => selectedItems = items));

        // Act - Open dropdown and click selected option to deselect
        cut.Find(".multiselect-container").Click();
        var options = cut.FindAll(".multiselect-option");
        options[0].Click();

        // Assert
        selectedItems.ShouldBeEmpty();
    }

    [Fact]
    public void MultiSelect_MaxSelectedItems_PreventsFurtherSelection()
    {
        // Arrange
        var selectedItems = new List<string> { "Option 1", "Option 2" };
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2", "Option 3" })
            .Add(p => p.SelectedItems, selectedItems)
            .Add(p => p.MaxSelectedItems, 2)
            .Add(p => p.SelectedItemsChanged, items => selectedItems = items));

        // Act - Try to select a third item
        cut.Find(".multiselect-container").Click();
        var options = cut.FindAll(".multiselect-option");
        options[2].Click(); // Try to select Option 3

        // Assert - Should still have only 2 items
        selectedItems.Count.ShouldBe(2);
    }

    [Fact]
    public void MultiSelect_RemoveTag_UpdatesSelection()
    {
        // Arrange
        var selectedItems = new List<string> { "Option 1", "Option 2" };
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItems, selectedItems)
            .Add(p => p.SelectedItemsChanged, items => selectedItems = items));

        // Act - Click remove button on first tag
        var removeButtons = cut.FindAll(".multiselect-tag-remove");
        removeButtons[0].Click();

        // Assert
        selectedItems.Count.ShouldBe(1);
        selectedItems[0].ShouldBe("Option 2");
    }

    [Fact]
    public void MultiSelect_HelperText_Displays()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1" })
            .Add(p => p.HelperText, "Select one or more options"));

        // Assert
        var helperText = cut.Find(".multiselect-helper-text");
        helperText.TextContent.ShouldBe("Select one or more options");
    }

    [Fact]
    public void MultiSelect_NoOptionsAvailable_ShowsMessage()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string>()));

        // Act - Open dropdown
        cut.Find(".multiselect-container").Click();

        // Assert
        var noOptions = cut.Find(".multiselect-no-options");
        noOptions.TextContent.ShouldBe("No options available");
    }

    [Fact]
    public void MultiSelect_SelectedOption_HasSelectedClass()
    {
        // Arrange
        var selectedItems = new List<string> { "Option 1" };
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItems, selectedItems));

        // Act - Open dropdown
        cut.Find(".multiselect-container").Click();

        // Assert
        var options = cut.FindAll(".multiselect-option");
        options[0].ClassList.ShouldContain("selected");
        options[1].ClassList.ShouldNotContain("selected");
    }

    [Fact]
    public void MultiSelect_SelectedOption_ShowsCheckmark()
    {
        // Arrange
        var selectedItems = new List<string> { "Option 1" };
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItems, selectedItems));

        // Act - Open dropdown
        cut.Find(".multiselect-container").Click();

        // Assert
        var checkboxes = cut.FindAll(".multiselect-checkbox");
        checkboxes[0].InnerHtml.ShouldContain("svg"); // First option has checkmark
    }

    [Fact]
    public void MultiSelect_LargeList_RendersAllItems()
    {
        // Arrange - Create 100 items
        var items = Enumerable.Range(1, 100).Select(i => $"Option {i}").ToList();

        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, items));

        cut.Find(".multiselect-container").Click();

        // Assert
        var options = cut.FindAll(".multiselect-option");
        options.Count.ShouldBe(100);
    }

    [Fact]
    public void MultiSelect_Disabled_NoDropdownInteraction()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.Disabled, true));

        cut.Find(".multiselect-container").Click();

        // Assert - Dropdown should not open
        cut.FindAll(".multiselect-dropdown").ShouldBeEmpty();
    }

    [Fact]
    public void MultiSelect_GeneratesUniqueId()
    {
        // Act
        var cut1 = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1" }));
        var cut2 = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1" }));

        // Assert - Each instance should have unique ID
        cut1.Instance.Id.ShouldNotBe(cut2.Instance.Id);
    }

    [Fact]
    public void MultiSelect_ForwardsClassAndAdditionalAttributes_ToRoot()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Class, "people-picker")
            .Add(p => p.Items, new List<string> { "Option 1" })
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "multi" }
            }));

        // Assert
        var root = cut.Find(".vibe-multiselect");
        root.ClassList.ShouldContain("people-picker");
        root.GetAttribute("data-testid")!.ShouldBe("multi");
    }

    [Fact]
    public void MultiSelect_ExposesComboboxAndListboxSemantics()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Id, "skills")
            .Add(p => p.Label, "Skills")
            .Add(p => p.HelperText, "Choose one or more skills")
            .Add(p => p.Items, new List<string> { "C#", "F#" })
            .Add(p => p.SelectedItems, new List<string> { "C#" }));

        var input = cut.Find("input[role='combobox']");
        input.GetAttribute("id")!.ShouldBe("skills");
        input.GetAttribute("aria-expanded")!.ShouldBe("false");
        input.GetAttribute("aria-describedby").ShouldNotBeNullOrEmpty();
        cut.Find("label").GetAttribute("for")!.ShouldBe("skills");
        cut.Find(".multiselect-helper-text").GetAttribute("id")!.ShouldBe(input.GetAttribute("aria-describedby"));

        // Act
        cut.Find(".multiselect-container").Click();

        // Assert
        input = cut.Find("input[role='combobox']");
        input.GetAttribute("aria-expanded")!.ShouldBe("true");
        cut.Find("[role='listbox']").GetAttribute("aria-multiselectable")!.ShouldBe("true");

        var options = cut.FindAll("[role='option']");
        options.Count.ShouldBe(2);
        options[0].GetAttribute("aria-selected")!.ShouldBe("true");
        options[1].GetAttribute("aria-selected")!.ShouldBe("false");
    }

    [Fact]
    public void MultiSelect_ToggleItem_DoesNotMutateCallerOwnedSelectionList()
    {
        // Arrange
        var originalSelection = new List<string> { "Option 1" };
        List<string>? callbackSelection = null;
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItems, originalSelection)
            .Add(p => p.SelectedItemsChanged, items => callbackSelection = items));

        // Act
        cut.Find(".multiselect-container").Click();
        cut.FindAll(".multiselect-option")[1].Click();

        // Assert
        originalSelection.ShouldBe(new List<string> { "Option 1" });
        callbackSelection.ShouldNotBeNull();
        callbackSelection.ShouldBe(new List<string> { "Option 1", "Option 2" });
        callbackSelection.ShouldNotBeSameAs(originalSelection);
    }

    [Fact]
    public void MultiSelect_KeyboardNavigation_HighlightsAndSelectsOption()
    {
        // Arrange
        List<string>? selectedItems = null;
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, new List<string> { "Option 1", "Option 2" })
            .Add(p => p.SelectedItemsChanged, items => selectedItems = items));

        var input = cut.Find(".multiselect-input");

        // Act
        input.KeyDown("ArrowDown");
        input.KeyDown("ArrowDown");

        // Assert
        cut.Find(".multiselect-option.highlighted").TextContent.ShouldContain("Option 1");
        cut.Find(".multiselect-input").GetAttribute("aria-activedescendant").ShouldNotBeNullOrEmpty();

        // Act
        input.KeyDown("Enter");

        // Assert
        selectedItems.ShouldBe(new List<string> { "Option 1" });
    }

    [Fact]
    public void MultiSelect_WithNullCollections_RendersEmptyDropdown()
    {
        // Act
        var cut = Render<MultiSelect<string>>(parameters => parameters
            .Add(p => p.Items, null!)
            .Add(p => p.SelectedItems, null!));

        cut.Find(".multiselect-container").Click();

        // Assert
        cut.Find(".multiselect-no-options").TextContent.ShouldBe("No options available");
    }
}
