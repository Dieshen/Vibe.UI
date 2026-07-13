namespace Vibe.UI.Tests.Components.Inputs;

public class TransferListTests : TestBase
{
    [Fact]
    public void TransferList_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<TransferList<string>>();

        // Assert
        cut.Find(".vibe-transfer").ShouldNotBeNull();
    }

    [Fact]
    public void TransferList_Renders_TwoPanels()
    {
        // Act
        var cut = Render<TransferList<string>>();

        // Assert
        var panels = cut.FindAll(".transfer-panel");
        panels.Count.ShouldBe(2);
    }

    [Fact]
    public void TransferList_Renders_Controls()
    {
        // Act
        var cut = Render<TransferList<string>>();

        // Assert
        cut.Find(".transfer-controls").ShouldNotBeNull();
        var buttons = cut.FindAll(".transfer-btn");
        buttons.Count.ShouldBe(4); // Move all right, move right, move left, move all left
    }

    [Fact]
    public void TransferList_Has_DefaultSourceTitle()
    {
        // Act
        var cut = Render<TransferList<string>>();

        // Assert
        var panels = cut.FindAll(".panel-title");
        panels[0].TextContent.ShouldBe("Available");
    }

    [Fact]
    public void TransferList_Has_DefaultTargetTitle()
    {
        // Act
        var cut = Render<TransferList<string>>();

        // Assert
        var panels = cut.FindAll(".panel-title");
        panels[1].TextContent.ShouldBe("Selected");
    }

    [Fact]
    public void TransferList_Accepts_CustomTitles()
    {
        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceTitle, "Source Items")
            .Add(p => p.TargetTitle, "Target Items"));

        // Assert
        var panels = cut.FindAll(".panel-title");
        panels[0].TextContent.ShouldBe("Source Items");
        panels[1].TextContent.ShouldBe("Target Items");
    }

    [Fact]
    public void TransferList_Shows_Search_ByDefault()
    {
        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.ShowSearch, true));

        // Assert
        var searchInputs = cut.FindAll(".search-input");
        searchInputs.Count.ShouldBe(2);
    }

    [Fact]
    public void TransferList_Hides_Search_WhenDisabled()
    {
        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.ShowSearch, false));

        // Assert
        cut.FindAll(".search-input").ShouldBeEmpty();
    }

    [Fact]
    public void TransferList_Shows_Checkboxes_ByDefault()
    {
        // Arrange
        var sourceItems = new List<string> { "Item 1", "Item 2" };

        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.ShowCheckboxes, true));

        // Assert
        cut.FindAll(".item-checkbox").ShouldNotBeEmpty();
    }

    [Fact]
    public void TransferList_Displays_ItemCounts()
    {
        // Arrange
        var sourceItems = new List<string> { "Item 1", "Item 2", "Item 3" };

        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems));

        // Assert
        var counts = cut.FindAll(".panel-count");
        counts[0].TextContent.ShouldBe("3");
    }

    [Fact]
    public void TransferList_Renders_Items()
    {
        // Arrange
        var sourceItems = new List<string> { "Item 1", "Item 2" };

        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems));

        // Assert
        var items = cut.FindAll(".transfer-item");
        items.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void TransferList_Applies_CustomCssClass()
    {
        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.CssClass, "custom-transfer"));

        // Assert
        cut.Find(".vibe-transfer").ClassList.ShouldContain("custom-transfer");
    }

    [Fact]
    public void TransferList_Exposes_ListboxSemantics_AndAccessibleControls()
    {
        // Arrange
        var sourceItems = new List<string> { "Alpha", "Beta" };
        var targetItems = new List<string> { "Gamma" };

        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.TargetItems, targetItems)
            .Add(p => p.SourceTitle, "Available people")
            .Add(p => p.TargetTitle, "Assigned people")
            .Add(p => p.AriaLabel, "Assign people"));

        // Assert
        var root = cut.Find(".vibe-transfer");
        root.GetAttribute("role").ShouldBe("group");
        root.GetAttribute("aria-label").ShouldBe("Assign people");

        var searchInputs = cut.FindAll(".search-input");
        searchInputs[0].GetAttribute("aria-label").ShouldBe("Available people search");
        searchInputs[1].GetAttribute("aria-label").ShouldBe("Assigned people search");

        var lists = cut.FindAll("[role='listbox']");
        lists.Count.ShouldBe(2);
        lists[0].GetAttribute("aria-multiselectable").ShouldBe("true");
        lists[0].GetAttribute("aria-labelledby").ShouldBe(cut.FindAll(".panel-title")[0].GetAttribute("id"));
        lists[1].GetAttribute("aria-labelledby").ShouldBe(cut.FindAll(".panel-title")[1].GetAttribute("id"));

        var options = cut.FindAll("[role='option']");
        options.Count.ShouldBe(3);
        options[0].GetAttribute("tabindex").ShouldBe("0");
        options[1].GetAttribute("tabindex").ShouldBe("-1");
        options[2].GetAttribute("tabindex").ShouldBe("0");
        options[0].GetAttribute("aria-selected").ShouldBe("false");
        options[0].GetAttribute("aria-disabled").ShouldBe("false");
        options[0].QuerySelector(".item-checkbox")!.GetAttribute("aria-hidden").ShouldBe("true");

        var buttons = cut.FindAll(".transfer-btn");
        buttons[0].GetAttribute("aria-label").ShouldBe("Move all Available people items to Assigned people");
        buttons[1].GetAttribute("aria-label").ShouldBe("Move selected Available people items to Assigned people");
        buttons[2].GetAttribute("aria-label").ShouldBe("Move selected Assigned people items to Available people");
        buttons[3].GetAttribute("aria-label").ShouldBe("Move all Assigned people items to Available people");
        buttons[1].HasAttribute("disabled").ShouldBeTrue();
        buttons[2].HasAttribute("disabled").ShouldBeTrue();
        buttons.Select(button => button.QuerySelectorAll("svg.vibe-icon").Length)
            .ShouldBe(new[] { 2, 1, 1, 2 });
        buttons.ShouldAllBe(button => string.IsNullOrWhiteSpace(button.TextContent));
    }

    [Fact]
    public void TransferList_UsesRovingFocus_WithArrowHomeAndEndKeys()
    {
        // Arrange
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, new List<string> { "Alpha", "Beta", "Gamma" })
            .Add(p => p.TargetItems, new List<string> { "Delta", "Epsilon" }));

        var sourceOptions = GetPanelOptions(cut, 0);
        var targetOptions = GetPanelOptions(cut, 1);
        sourceOptions.Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "0", "-1", "-1" });
        targetOptions.Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "0", "-1" });

        // Act - arrow to the next source option.
        sourceOptions[0].KeyDown("ArrowDown");

        // Assert
        sourceOptions = GetPanelOptions(cut, 0);
        sourceOptions.Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "-1", "0", "-1" });

        // Act - jump to the end, then back home.
        sourceOptions[1].KeyDown("End");
        sourceOptions = GetPanelOptions(cut, 0);
        sourceOptions.Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "-1", "-1", "0" });

        sourceOptions[2].KeyDown("Home");
        sourceOptions = GetPanelOptions(cut, 0);
        sourceOptions.Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "0", "-1", "-1" });

        // Act - clamp at the first option without disturbing the target list.
        sourceOptions[0].KeyDown("ArrowUp");

        // Assert
        GetPanelOptions(cut, 0).Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "0", "-1", "-1" });
        GetPanelOptions(cut, 1).Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "0", "-1" });
    }

    [Fact]
    public void TransferList_DisabledState_IsInertAndRemovedFromTabOrder()
    {
        // Arrange
        var sourceItems = new List<string> { "Alpha", "Beta" };
        var targetItems = new List<string> { "Gamma" };
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.TargetItems, targetItems)
            .Add(p => p.Disabled, true));

        // Assert
        var root = cut.Find(".vibe-transfer");
        root.ClassList.ShouldContain("is-disabled");
        root.GetAttribute("aria-disabled").ShouldBe("true");
        cut.FindAll(".search-input").ShouldAllBe(input => input.HasAttribute("disabled"));
        cut.FindAll("[role='listbox']").ShouldAllBe(list => list.GetAttribute("aria-disabled") == "true");
        cut.FindAll("[role='option']").ShouldAllBe(option =>
            option.GetAttribute("tabindex") == "-1" && option.GetAttribute("aria-disabled") == "true");
        cut.FindAll(".transfer-btn").ShouldAllBe(button => button.HasAttribute("disabled"));

        // Act
        cut.FindAll("[role='option']")[0].Click();

        // Assert
        cut.FindAll(".transfer-item.selected").ShouldBeEmpty();
        sourceItems.ShouldBe(new List<string> { "Alpha", "Beta" });
        targetItems.ShouldBe(new List<string> { "Gamma" });
    }

    [Fact]
    public void TransferList_SelectsItems_WithMouseAndKeyboard()
    {
        // Arrange
        var sourceItems = new List<string> { "Alpha", "Beta" };
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems));

        // Act
        cut.FindAll(".transfer-item")[0].Click();
        cut.FindAll(".transfer-item")[1].KeyDown("Enter");

        // Assert
        var selectedItems = cut.FindAll(".transfer-item.selected");
        selectedItems.Count.ShouldBe(2);
        selectedItems[0].GetAttribute("aria-selected").ShouldBe("true");
        selectedItems[0].QuerySelector(".item-checkbox")!.HasAttribute("checked").ShouldBeTrue();
        cut.FindAll(".transfer-btn")[1].HasAttribute("disabled").ShouldBeFalse();

        // Act
        cut.FindAll(".transfer-item")[1].KeyDown(" ");

        // Assert
        var remainingSelected = cut.FindAll(".transfer-item.selected");
        remainingSelected.Count.ShouldBe(1);
        remainingSelected[0].TextContent.ShouldContain("Alpha");
    }

    [Fact]
    public void TransferList_MovesSelectedItems_ToTarget_AndInvokesCallbacks()
    {
        // Arrange
        var sourceItems = new List<string> { "Alpha", "Beta", "Gamma" };
        var targetItems = new List<string> { "Delta" };
        List<string>? changedSource = null;
        List<string>? changedTarget = null;

        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.TargetItems, targetItems)
            .Add(p => p.SourceItemsChanged, items => changedSource = items.ToList())
            .Add(p => p.TargetItemsChanged, items => changedTarget = items.ToList()));

        // Act
        cut.FindAll(".transfer-item")[0].Click();
        cut.FindAll(".transfer-item")[2].Click();
        cut.FindAll(".transfer-btn")[1].Click();

        // Assert
        sourceItems.ShouldBe(new List<string> { "Beta" });
        targetItems.ShouldBe(new List<string> { "Delta", "Alpha", "Gamma" });
        changedSource.ShouldBe(new List<string> { "Beta" });
        changedTarget.ShouldBe(new List<string> { "Delta", "Alpha", "Gamma" });
        cut.FindAll(".transfer-btn")[1].HasAttribute("disabled").ShouldBeTrue();
        cut.FindAll(".panel-count")[0].TextContent.ShouldBe("1");
        cut.FindAll(".panel-count")[1].TextContent.ShouldBe("3");
        GetPanelOptions(cut, 1).Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "-1", "0", "-1" });
        cut.Find(".transfer-status").TextContent.ShouldBe("Moved 2 items to Selected.");
    }

    [Fact]
    public void TransferList_MovesSelectedItems_BackToSource_AndInvokesCallbacks()
    {
        // Arrange
        var sourceItems = new List<string> { "Alpha" };
        var targetItems = new List<string> { "Beta", "Gamma" };
        List<string>? changedSource = null;
        List<string>? changedTarget = null;

        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.TargetItems, targetItems)
            .Add(p => p.SourceItemsChanged, items => changedSource = items.ToList())
            .Add(p => p.TargetItemsChanged, items => changedTarget = items.ToList()));

        // Act
        var targetOptions = cut.FindAll(".transfer-panel")[1].QuerySelectorAll(".transfer-item");
        targetOptions[1].Click();
        cut.FindAll(".transfer-btn")[2].Click();

        // Assert
        sourceItems.ShouldBe(new List<string> { "Alpha", "Gamma" });
        targetItems.ShouldBe(new List<string> { "Beta" });
        changedSource.ShouldBe(new List<string> { "Alpha", "Gamma" });
        changedTarget.ShouldBe(new List<string> { "Beta" });
        cut.FindAll(".transfer-btn")[2].HasAttribute("disabled").ShouldBeTrue();
        GetPanelOptions(cut, 0).Select(option => option.GetAttribute("tabindex"))
            .ShouldBe(new[] { "-1", "0" });
        cut.Find(".transfer-status").TextContent.ShouldBe("Moved 1 item to Available.");
    }

    [Fact]
    public void TransferList_MovesAllItems_BothDirections()
    {
        // Arrange
        var sourceItems = new List<string> { "Alpha", "Beta" };
        var targetItems = new List<string> { "Gamma" };

        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.TargetItems, targetItems));

        // Act
        cut.FindAll(".transfer-btn")[0].Click();

        // Assert
        sourceItems.ShouldBeEmpty();
        targetItems.ShouldBe(new List<string> { "Gamma", "Alpha", "Beta" });
        cut.FindAll(".transfer-btn")[0].HasAttribute("disabled").ShouldBeTrue();
        cut.FindAll(".transfer-btn")[3].HasAttribute("disabled").ShouldBeFalse();

        // Act
        cut.FindAll(".transfer-btn")[3].Click();

        // Assert
        sourceItems.ShouldBe(new List<string> { "Gamma", "Alpha", "Beta" });
        targetItems.ShouldBeEmpty();
        cut.FindAll(".transfer-btn")[0].HasAttribute("disabled").ShouldBeFalse();
        cut.FindAll(".transfer-btn")[3].HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void TransferList_FiltersSourceAndTargetItems_WithTextSelector()
    {
        // Arrange
        var sourceItems = new List<TransferListTestItem>
        {
            new("1", "Alpha user"),
            new("2", "Beta user")
        };
        var targetItems = new List<TransferListTestItem>
        {
            new("3", "Gamma user")
        };

        var cut = Render<TransferList<TransferListTestItem>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.TargetItems, targetItems)
            .Add(p => p.TextSelector, item => item.Name)
            .Add(p => p.KeySelector, item => item.Id));

        // Act
        cut.FindAll(".search-input")[0].Input("Alpha");
        cut.FindAll(".search-input")[1].Input("Gamma");

        // Assert
        var panels = cut.FindAll(".transfer-panel");
        panels[0].QuerySelectorAll(".transfer-item").Length.ShouldBe(1);
        panels[0].TextContent.ShouldContain("Alpha user");
        panels[0].TextContent.ShouldNotContain("Beta user");
        panels[1].QuerySelectorAll(".transfer-item").Length.ShouldBe(1);
        panels[1].TextContent.ShouldContain("Gamma user");

        // Act
        cut.FindAll(".search-input")[0].Input("Missing");

        // Assert
        cut.FindAll(".transfer-panel")[0].QuerySelector(".transfer-empty")!.TextContent
            .ShouldBe("No available items found");
    }

    [Fact]
    public void TransferList_SearchResetsRovingTabStopToFirstVisibleOption()
    {
        // Arrange
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, new List<string> { "Alpha", "Beta", "Gamma" }));

        var sourceOptions = GetPanelOptions(cut, 0);
        sourceOptions[0].KeyDown("End");
        GetPanelOptions(cut, 0)[2].GetAttribute("tabindex").ShouldBe("0");

        // Act
        cut.FindAll(".search-input")[0].Input("Beta");

        // Assert
        sourceOptions = GetPanelOptions(cut, 0);
        sourceOptions.Count.ShouldBe(1);
        sourceOptions[0].TextContent.ShouldContain("Beta");
        sourceOptions[0].GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact]
    public void TransferList_RendersCustomItemTemplate()
    {
        // Arrange
        RenderFragment<string> template = item => builder =>
        {
            builder.OpenElement(0, "strong");
            builder.AddAttribute(1, "class", "custom-item");
            builder.AddContent(2, item.ToUpperInvariant());
            builder.CloseElement();
        };

        // Act
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, new List<string> { "Alpha" })
            .Add(p => p.ItemTemplate, template));

        // Assert
        var customItem = cut.Find(".custom-item");
        customItem.TextContent.ShouldBe("ALPHA");
    }

    [Fact]
    public void TransferList_PrunesSelections_WhenItemsChange()
    {
        // Arrange
        var sourceItems = new List<string> { "Alpha", "Beta" };
        var targetItems = new List<string>();
        var cut = Render<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.TargetItems, targetItems));

        cut.FindAll(".transfer-item")[0].Click();

        // Act
        sourceItems = new List<string> { "Beta" };
        cut.Render(parameters => parameters
            .Add(p => p.SourceItems, sourceItems)
            .Add(p => p.TargetItems, targetItems));

        // Assert
        cut.FindAll(".transfer-item.selected").ShouldBeEmpty();
        cut.FindAll(".transfer-btn")[1].HasAttribute("disabled").ShouldBeTrue();
    }

    private static IReadOnlyList<AngleSharp.Dom.IElement> GetPanelOptions(
        IRenderedComponent<TransferList<string>> cut,
        int panelIndex)
    {
        return cut.FindAll(".transfer-panel")[panelIndex]
            .QuerySelectorAll("[role='option']")
            .ToList();
    }

    private sealed record TransferListTestItem(string Id, string Name);
}
