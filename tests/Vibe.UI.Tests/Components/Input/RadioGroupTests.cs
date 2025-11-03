namespace Vibe.UI.Tests.Components.Input;

public class RadioGroupTests : TestBase
{
    [Fact]
    public void RadioGroup_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .AddChildContent("<div>Radio items</div>"));

        // Assert
        var group = cut.Find(".vibe-radio-group");
        group.ShouldNotBeNull();
        group.GetAttribute("role").ShouldBe("radiogroup");
    }

    [Fact]
    public void RadioGroup_Applies_VerticalOrientation_ByDefault()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-radio-group").ClassList.ShouldContain("vibe-radio-group-vertical");
    }

    [Fact]
    public void RadioGroup_Applies_HorizontalOrientation()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Orientation, RadioGroup.RadioGroupOrientation.Horizontal)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-radio-group").ClassList.ShouldContain("vibe-radio-group-horizontal");
    }

    [Fact]
    public void RadioGroup_Applies_DisabledClass()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-radio-group").ClassList.ShouldContain("vibe-radio-group-disabled");
    }

    [Fact]
    public void RadioGroup_Has_RequiredAttribute()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Required, true)
            .AddChildContent("<div>Items</div>"));

        // Assert - Check property value directly since Blazor boolean rendering varies
        cut.Instance.Required.ShouldBeTrue();
        cut.Find(".vibe-radio-group").HasAttribute("aria-required").ShouldBeTrue();
    }

    [Fact]
    public void RadioGroup_Has_AriaLabel()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.AriaLabel, "Select an option")
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-radio-group").GetAttribute("aria-label").ShouldBe("Select an option");
    }

    [Fact]
    public void RadioGroup_Generates_UniqueName()
    {
        // Act
        var cut1 = RenderComponent<RadioGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));
        var cut2 = RenderComponent<RadioGroup>(parameters => parameters
            .AddChildContent("<div>Items</div>"));

        // Assert - Each instance should have a unique name
        cut1.Instance.Name.ShouldNotBe(cut2.Instance.Name);
    }

    [Fact]
    public void RadioGroup_Accepts_CustomName()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Name, "custom-group")
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Instance.Name.ShouldBe("custom-group");
    }

    [Fact]
    public void RadioGroup_Renders_ChildContent()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .AddChildContent("<span class='test-child'>Test Content</span>"));

        // Assert
        var child = cut.Find(".test-child");
        child.TextContent.ShouldBe("Test Content");
    }

    [Fact]
    public void RadioGroup_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.CssClass, "custom-radio-group")
            .AddChildContent("<div>Items</div>"));

        // Assert
        cut.Find(".vibe-radio-group").ClassList.ShouldContain("custom-radio-group");
    }

    [Fact]
    public void RadioGroup_WithMultipleItems_OnlyOneSelected()
    {
        // Arrange & Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, "option2")
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option2")
                .AddChildContent("Option 2"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option3")
                .AddChildContent("Option 3")));

        // Assert
        var radioInputs = cut.FindAll("input[type='radio']");
        radioInputs.Count.ShouldBe(3);
        radioInputs[0].HasAttribute("checked").ShouldBeFalse();
        radioInputs[1].HasAttribute("checked").ShouldBeTrue();
        radioInputs[2].HasAttribute("checked").ShouldBeFalse();
    }

    [Fact]
    public void RadioGroup_ValueChange_UpdatesSelection()
    {
        // Arrange
        var selectedValue = "option1";
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, selectedValue)
            .Add(p => p.ValueChanged, value => selectedValue = value)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option2")
                .AddChildContent("Option 2")));

        // Act
        var radioInputs = cut.FindAll("input[type='radio']");
        radioInputs[1].Change(true);

        // Assert
        selectedValue.ShouldBe("option2");
    }

    [Fact]
    public void RadioGroup_DisabledGroup_PreventsSelection()
    {
        // Arrange
        var selectedValue = "option1";
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, selectedValue)
            .Add(p => p.ValueChanged, value => selectedValue = value)
            .Add(p => p.Disabled, true)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option2")
                .AddChildContent("Option 2")));

        // Act
        var radioInputs = cut.FindAll("input[type='radio']");
        radioInputs[1].Change(true);

        // Assert - Value should not change when disabled
        selectedValue.ShouldBe("option1");
    }

    [Fact]
    public void RadioGroup_InitialValue_SelectsCorrectItem()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, "option2")
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option2")
                .AddChildContent("Option 2"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option3")
                .AddChildContent("Option 3")));

        // Assert - Check via DOM
        var radioInputs = cut.FindAll("input[type='radio']");
        radioInputs[0].HasAttribute("checked").ShouldBeFalse();
        radioInputs[1].HasAttribute("checked").ShouldBeTrue();
        radioInputs[2].HasAttribute("checked").ShouldBeFalse();
    }

    [Fact]
    public void RadioGroup_NullValue_NoItemSelected()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, null)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1")));

        // Assert
        cut.Instance.Value.ShouldBeNull();
        var radioInput = cut.Find("input[type='radio']");
        radioInput.HasAttribute("checked").ShouldBeFalse();
    }

    [Fact]
    public void RadioGroup_InvalidValue_NoItemSelected()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, "nonexistent")
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option2")
                .AddChildContent("Option 2")));

        // Assert
        var radioInputs = cut.FindAll("input[type='radio']");
        radioInputs[0].HasAttribute("checked").ShouldBeFalse();
        radioInputs[1].HasAttribute("checked").ShouldBeFalse();
    }

    [Fact]
    public void RadioGroup_HorizontalOrientation_HasCorrectClass()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Orientation, RadioGroup.RadioGroupOrientation.Horizontal)
            .AddChildContent("<div>Items</div>"));

        // Assert
        var group = cut.Find(".vibe-radio-group");
        group.ClassList.ShouldContain("vibe-radio-group-horizontal");
        group.ClassList.ShouldNotContain("vibe-radio-group-vertical");
    }

    [Fact]
    public void RadioGroup_VerticalOrientation_HasCorrectClass()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Orientation, RadioGroup.RadioGroupOrientation.Vertical)
            .AddChildContent("<div>Items</div>"));

        // Assert
        var group = cut.Find(".vibe-radio-group");
        group.ClassList.ShouldContain("vibe-radio-group-vertical");
        group.ClassList.ShouldNotContain("vibe-radio-group-horizontal");
    }

    [Fact]
    public void RadioGroup_AllItemsShareSameName()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Name, "test-group")
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option2")
                .AddChildContent("Option 2"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option3")
                .AddChildContent("Option 3")));

        // Assert
        var radioInputs = cut.FindAll("input[type='radio']");
        radioInputs.Count.ShouldBe(3);
        radioInputs[0].GetAttribute("name").ShouldBe("test-group");
        radioInputs[1].GetAttribute("name").ShouldBe("test-group");
        radioInputs[2].GetAttribute("name").ShouldBe("test-group");
    }

    [Fact]
    public void RadioGroup_ValueChangedCallback_FiresOnSelection()
    {
        // Arrange
        string? callbackValue = null;
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.ValueChanged, value => callbackValue = value)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1")));

        // Act
        cut.Find("input[type='radio']").Change(true);

        // Assert
        callbackValue.ShouldBe("option1");
    }

    [Fact]
    public void RadioGroup_EmptyStringValue_ValidSelection()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, "")
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "")
                .AddChildContent("Empty Option")));

        // Assert
        cut.Instance.Value.ShouldBe("");
        var radioInput = cut.Find("input[type='radio']");
        radioInput.HasAttribute("checked").ShouldBeTrue();
    }
}
