namespace Vibe.UI.Tests.Components.Inputs;

public class SelectTests : TestBase
{
    [Fact]
    public void Select_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .AddChildContent("<option value='1'>Option 1</option>"));

        // Assert
        var select = cut.Find("select");
        select.ShouldNotBeNull();
    }

    [Fact]
    public void Select_Renders_WithLabel()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Label, "Choose Option")
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        var label = cut.Find(".vibe-select-label");
        label.TextContent.ShouldBe("Choose Option");
    }

    [Fact]
    public void Select_Renders_WithPlaceholder()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Placeholder, "Select an option")
            .AddChildContent("<option value='1'>Option 1</option>"));

        // Assert
        var placeholder = cut.Find("option[disabled][selected]");
        placeholder.TextContent.ShouldBe("Select an option");
    }

    [Fact]
    public void Select_Applies_Disabled_Attribute()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        var select = cut.Find("select");
        select.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Select_Shows_HelperText()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.HelperText, "Please select one option")
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        var helper = cut.Find(".vibe-select-helper-text");
        helper.TextContent.ShouldBe("Please select one option");
    }

    [Fact]
    public void Select_Renders_Options()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .AddChildContent(@"
                <option value='1'>Option 1</option>
                <option value='2'>Option 2</option>
                <option value='3'>Option 3</option>
            "));

        // Assert
        var options = cut.FindAll("option");
        options.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void Select_InvokesOnChange_WhenSelectionChanges()
    {
        // Arrange
        ChangeEventArgs? capturedArgs = null;
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.OnChange, args => capturedArgs = args)
            .AddChildContent(@"
                <option value='1'>Option 1</option>
                <option value='2'>Option 2</option>
            "));

        // Act
        cut.Find("select").Change("2");

        // Assert
        capturedArgs.ShouldNotBeNull();
        capturedArgs.Value.ShouldBe("2");
    }

    [Fact]
    public void Select_HasCorrectId()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Id, "my-select")
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        var select = cut.Find("select");
        select.GetAttribute("id")!.ShouldBe("my-select");
    }

    // === Edge Cases ===

    [Fact]
    public void Select_WithNullChildContent_RendersEmpty()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment?)null));

        // Assert
        var select = cut.Find("select");
        select.Children.Length.ShouldBe(0);
    }

    [Fact]
    public void Select_WithEmptyOptions_RendersEmpty()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .AddChildContent(string.Empty));

        // Assert
        cut.Find("select").InnerHtml.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Select_WithManyOptions_RendersAllOptions()
    {
        // Arrange
        var optionsHtml = string.Join("\n", Enumerable.Range(1, 100).Select(i => $"<option value='{i}'>Option {i}</option>"));

        // Act
        var cut = Render<Select>(parameters => parameters
            .AddChildContent(optionsHtml));

        // Assert
        var options = cut.FindAll("option");
        options.Count.ShouldBeGreaterThanOrEqualTo(100);
    }

    // === Label ===

    [Fact]
    public void Select_WithEmptyLabel_DoesNotRenderLabel()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Label, string.Empty)
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        cut.FindAll(".vibe-select-label").ShouldBeEmpty();
    }

    [Fact]
    public void Select_WithNullLabel_DoesNotRenderLabel()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Label, null)
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        cut.FindAll(".vibe-select-label").ShouldBeEmpty();
    }

    // === Placeholder ===

    [Fact]
    public void Select_WithEmptyPlaceholder_DoesNotRenderPlaceholder()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Placeholder, string.Empty)
            .AddChildContent("<option value='1'>Option 1</option>"));

        // Assert
        cut.FindAll("option[disabled][selected]").ShouldBeEmpty();
    }

    [Fact]
    public void Select_WithNullPlaceholder_DoesNotRenderPlaceholder()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Placeholder, null)
            .AddChildContent("<option value='1'>Option 1</option>"));

        // Assert
        cut.FindAll("option[disabled][selected]").ShouldBeEmpty();
    }

    [Fact]
    public void Select_Placeholder_IsDisabledAndSelected()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Placeholder, "Choose one")
            .AddChildContent("<option value='1'>Option 1</option>"));

        // Assert
        var placeholder = cut.Find("option[disabled][selected]");
        placeholder.ShouldNotBeNull();
        placeholder.GetAttribute("value")!.ShouldBe(string.Empty);
    }

    // === Helper Text ===

    [Fact]
    public void Select_WithEmptyHelperText_DoesNotRenderHelperText()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.HelperText, string.Empty)
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        cut.FindAll(".vibe-select-helper-text").ShouldBeEmpty();
    }

    [Fact]
    public void Select_WithNullHelperText_DoesNotRenderHelperText()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.HelperText, null)
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        cut.FindAll(".vibe-select-helper-text").ShouldBeEmpty();
    }

    // === ID Generation ===

    [Fact]
    public void Select_WithoutId_GeneratesUniqueId()
    {
        // Act
        var cut1 = Render<Select>(parameters => parameters
            .AddChildContent("<option>Option 1</option>"));
        var cut2 = Render<Select>(parameters => parameters
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        var id1 = cut1.Find("select").GetAttribute("id");
        var id2 = cut2.Find("select").GetAttribute("id");
        id1.ShouldNotBeNullOrEmpty();
        id2.ShouldNotBeNullOrEmpty();
        id1.ShouldNotBe(id2);
    }

    // === Event Handling ===

    [Fact]
    public void Select_WithNoOnChangeDelegate_DoesNotThrow()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .AddChildContent("<option value='1'>Option 1</option>"));

        // Act & Assert - Should not throw
        cut.Find("select").Change("1");
    }

    [Fact]
    public void Select_MultipleChanges_HandlesCorrectly()
    {
        // Arrange
        var changeCount = 0;
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.OnChange, args => changeCount++)
            .AddChildContent(@"
                <option value='1'>Option 1</option>
                <option value='2'>Option 2</option>
                <option value='3'>Option 3</option>
            "));

        var select = cut.Find("select");

        // Act
        select.Change("1");
        select.Change("2");
        select.Change("3");

        // Assert
        changeCount.ShouldBe(3);
    }

    [Fact]
    public void Select_ChangingToSameValue_StillTriggersCallback()
    {
        // Arrange
        var changeCount = 0;
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.OnChange, args => changeCount++)
            .AddChildContent("<option value='1'>Option 1</option>"));

        var select = cut.Find("select");

        // Act
        select.Change("1");
        select.Change("1");

        // Assert
        changeCount.ShouldBe(2);
    }

    // === Disabled State ===

    [Fact]
    public void Select_WhenDisabled_HasDisabledAttribute()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        cut.Find("select").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Select_WhenDisabled_DoesNotInvokeCallbacksFromSyntheticChange()
    {
        // Arrange
        string? selectedValue = "unchanged";
        ChangeEventArgs? capturedArgs = null;
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ValueChanged, value => selectedValue = value)
            .Add(p => p.OnChange, args => capturedArgs = args)
            .AddChildContent(@"
                <option value='1'>Option 1</option>
                <option value='2'>Option 2</option>
            "));

        // Act
        cut.Find("select").Change("2");

        // Assert
        selectedValue.ShouldBe("unchanged");
        capturedArgs.ShouldBeNull();
    }

    // === CSS Classes ===

    [Fact]
    public void Select_HasBaseClass()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        cut.Find(".vibe-select").ShouldNotBeNull();
    }

    // === Additional Attributes ===

    [Fact]
    public void Select_WithAdditionalAttributes_MergesCorrectly()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .AddChildContent("<option>Option 1</option>")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "my-select" },
                { "aria-label", "Custom Select" }
            }));

        // Assert
        var select = cut.Find("select");
        select.GetAttribute("data-testid")!.ShouldBe("my-select");
        select.GetAttribute("aria-label")!.ShouldBe("Custom Select");
    }

    [Fact]
    public void Select_WithClassParameter_AppendsCustomClassToContainer()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Class, "country-select")
            .AddChildContent("<option>Option 1</option>"));

        // Assert
        var container = cut.Find(".vibe-select");
        container.ClassList.ShouldContain("country-select");
    }

    [Fact]
    public void Select_WithValue_RendersValueAndDoesNotSelectPlaceholder()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Placeholder, "Choose one")
            .Add(p => p.Value, "2")
            .AddChildContent(@"
                <option value='1'>Option 1</option>
                <option value='2'>Option 2</option>
            "));

        // Assert
        var select = cut.Find("select");
        select.GetAttribute("value")!.ShouldBe("2");
        cut.Find("option[value='']").HasAttribute("selected").ShouldBeFalse();
    }

    [Fact]
    public void Select_InvokesValueChangedAndOnChange_WhenSelectionChanges()
    {
        // Arrange
        string? selectedValue = null;
        ChangeEventArgs? capturedArgs = null;
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.ValueChanged, value => selectedValue = value)
            .Add(p => p.OnChange, args => capturedArgs = args)
            .AddChildContent(@"
                <option value='1'>Option 1</option>
                <option value='2'>Option 2</option>
            "));

        // Act
        cut.Find("select").Change("2");

        // Assert
        selectedValue.ShouldBe("2");
        capturedArgs.ShouldNotBeNull();
        capturedArgs.Value.ShouldBe("2");
    }

    [Fact]
    public void Select_WithHelperText_ComputesAriaDescribedBy()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Id, "country")
            .Add(p => p.HelperText, "Choose your country")
            .AddChildContent("<option value='us'>United States</option>"));

        // Assert
        cut.Find("select").GetAttribute("aria-describedby")!.ShouldBe("country-helper");
        cut.Find("#country-helper").TextContent.ShouldBe("Choose your country");
    }

    [Fact]
    public void Select_WithAdditionalAriaDescribedBy_MergesHelperDescription()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Id, "state")
            .Add(p => p.HelperText, "Choose your state")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "aria-describedby", "external-description" }
            })
            .AddChildContent("<option value='ny'>New York</option>"));

        // Assert
        cut.Find("select").GetAttribute("aria-describedby")!.Split(' ')
            .ShouldBe(new[] { "external-description", "state-helper" });
    }

    // === Complex Scenarios ===

    [Fact]
    public void Select_WithLabelPlaceholderAndHelperText_RendersAllElements()
    {
        // Act
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Label, "Choose Option")
            .Add(p => p.Placeholder, "Select...")
            .Add(p => p.HelperText, "Pick one option")
            .AddChildContent("<option value='1'>Option 1</option>"));

        // Assert
        cut.Find(".vibe-select-label").ShouldNotBeNull();
        cut.Find("option[disabled][selected]").ShouldNotBeNull();
        cut.Find(".vibe-select-helper-text").ShouldNotBeNull();
    }
}
