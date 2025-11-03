namespace Vibe.UI.Tests.Components.Input;

public class RadioGroupItemTests : TestBase
{
    [Fact]
    public void RadioGroupItem_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<RadioGroupItem>(parameters => parameters
            .Add(p => p.Value, "option1")
            .AddChildContent("Option 1"));

        // Assert
        var item = cut.Find(".vibe-radio-group-item");
        item.ShouldNotBeNull();
    }

    [Fact]
    public void RadioGroupItem_Renders_RadioInput()
    {
        // Act
        var cut = RenderComponent<RadioGroupItem>(parameters => parameters
            .Add(p => p.Value, "option1")
            .AddChildContent("Option 1"));

        // Assert
        var input = cut.Find(".vibe-radio-group-item-input");
        input.GetAttribute("type").ShouldBe("radio");
        input.GetAttribute("value").ShouldBe("option1");
    }

    [Fact]
    public void RadioGroupItem_Renders_Label()
    {
        // Act
        var cut = RenderComponent<RadioGroupItem>(parameters => parameters
            .Add(p => p.Value, "option1")
            .AddChildContent("Option 1"));

        // Assert
        var label = cut.Find(".vibe-radio-group-item-label");
        label.TextContent.ShouldContain("Option 1");
    }

    [Fact]
    public void RadioGroupItem_Applies_DisabledState()
    {
        // Act
        var cut = RenderComponent<RadioGroupItem>(parameters => parameters
            .Add(p => p.Value, "option1")
            .Add(p => p.Disabled, true)
            .AddChildContent("Option 1"));

        // Assert
        var input = cut.Find(".vibe-radio-group-item-input");
        input.HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".vibe-radio-group-item").ClassList.ShouldContain("vibe-radio-group-item-disabled");
    }

    [Fact]
    public void RadioGroupItem_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<RadioGroupItem>(parameters => parameters
            .Add(p => p.Value, "option1")
            .Add(p => p.CssClass, "custom-radio")
            .AddChildContent("Option 1"));

        // Assert
        cut.Find(".vibe-radio-group-item").ClassList.ShouldContain("custom-radio");
    }

    [Fact]
    public void RadioGroupItem_Renders_Control()
    {
        // Act
        var cut = RenderComponent<RadioGroupItem>(parameters => parameters
            .Add(p => p.Value, "option1")
            .AddChildContent("Option 1"));

        // Assert
        var control = cut.Find(".vibe-radio-group-item-control");
        control.ShouldNotBeNull();
    }

    [Fact]
    public void RadioGroupItem_WithParent_InheritsGroupName()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Name, "parent-group")
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1")));

        // Assert
        var input = cut.Find("input[type='radio']");
        input.GetAttribute("name").ShouldBe("parent-group");
    }

    [Fact]
    public void RadioGroupItem_WithParentValue_ShowsChecked()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, "option1")
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1")));

        // Assert
        var input = cut.Find("input[type='radio']");
        input.HasAttribute("checked").ShouldBeTrue();
        cut.Find(".vibe-radio-group-item").ClassList.ShouldContain("vibe-radio-group-item-checked");
    }

    [Fact]
    public void RadioGroupItem_WithDifferentParentValue_NotChecked()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, "option2")
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1")));

        // Assert
        var input = cut.Find("input[type='radio']");
        input.HasAttribute("checked").ShouldBeFalse();
        cut.Find(".vibe-radio-group-item").ClassList.ShouldNotContain("vibe-radio-group-item-checked");
    }

    [Fact]
    public void RadioGroupItem_ParentDisabled_ItemBecomesDisabled()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1")));

        // Assert
        var input = cut.Find("input[type='radio']");
        input.HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".vibe-radio-group-item").ClassList.ShouldContain("vibe-radio-group-item-disabled");
    }

    [Fact]
    public void RadioGroupItem_BothDisabled_RemainsDisabled()
    {
        // Act
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .Add(i => i.Disabled, true)
                .AddChildContent("Option 1")));

        // Assert
        var input = cut.Find("input[type='radio']");
        input.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void RadioGroupItem_Click_UpdatesParentValue()
    {
        // Arrange
        string? selectedValue = null;
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.ValueChanged, value => selectedValue = value)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1")));

        // Act
        cut.Find("input[type='radio']").Change(true);

        // Assert
        selectedValue.ShouldBe("option1");
    }

    [Fact]
    public void RadioGroupItem_DisabledItem_NoChange()
    {
        // Arrange
        string? selectedValue = null;
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.ValueChanged, value => selectedValue = value)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .Add(i => i.Disabled, true)
                .AddChildContent("Option 1")));

        // Act
        cut.Find("input[type='radio']").Change(true);

        // Assert
        selectedValue.ShouldBeNull();
    }

    [Fact]
    public void RadioGroupItem_WithoutParent_HasEmptyName()
    {
        // Act
        var cut = RenderComponent<RadioGroupItem>(parameters => parameters
            .Add(p => p.Value, "option1")
            .AddChildContent("Option 1"));

        // Assert
        var input = cut.Find("input[type='radio']");
        input.GetAttribute("name").ShouldBe(string.Empty);
    }

    [Fact]
    public void RadioGroupItem_WithId_AppliesId()
    {
        // Act
        var cut = RenderComponent<RadioGroupItem>(parameters => parameters
            .Add(p => p.Value, "option1")
            .Add(p => p.Id, "custom-id")
            .AddChildContent("Option 1"));

        // Assert
        cut.Instance.Id.ShouldBe("custom-id");
    }

    [Fact]
    public void RadioGroupItem_ToggleSelection_UpdatesCheckedClass()
    {
        // Arrange
        string? selectedValue = "option1";
        var cut = RenderComponent<RadioGroup>(parameters => parameters
            .Add(p => p.Value, selectedValue)
            .Add(p => p.ValueChanged, value => selectedValue = value)
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option1")
                .AddChildContent("Option 1"))
            .AddChildContent<RadioGroupItem>(item => item
                .Add(i => i.Value, "option2")
                .AddChildContent("Option 2")));

        // Act - Click second item
        var inputs = cut.FindAll("input[type='radio']");
        inputs[1].Change(true);

        // Re-render with updated value
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Value, selectedValue));

        // Assert
        var items = cut.FindAll(".vibe-radio-group-item");
        items[0].ClassList.ShouldNotContain("vibe-radio-group-item-checked");
        items[1].ClassList.ShouldContain("vibe-radio-group-item-checked");
    }
}
