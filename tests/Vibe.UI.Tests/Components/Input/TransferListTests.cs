namespace Vibe.UI.Tests.Components.Input;

public class TransferListTests : TestBase
{
    [Fact]
    public void TransferList_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<TransferList<string>>();

        // Assert
        cut.Find(".vibe-transfer").ShouldNotBeNull();
    }

    [Fact]
    public void TransferList_Renders_TwoPanels()
    {
        // Act
        var cut = RenderComponent<TransferList<string>>();

        // Assert
        var panels = cut.FindAll(".transfer-panel");
        panels.Count.ShouldBe(2);
    }

    [Fact]
    public void TransferList_Renders_Controls()
    {
        // Act
        var cut = RenderComponent<TransferList<string>>();

        // Assert
        cut.Find(".transfer-controls").ShouldNotBeNull();
        var buttons = cut.FindAll(".transfer-btn");
        buttons.Count.ShouldBe(4); // Move all right, move right, move left, move all left
    }

    [Fact]
    public void TransferList_Has_DefaultSourceTitle()
    {
        // Act
        var cut = RenderComponent<TransferList<string>>();

        // Assert
        var panels = cut.FindAll(".panel-title");
        panels[0].TextContent.ShouldBe("Available");
    }

    [Fact]
    public void TransferList_Has_DefaultTargetTitle()
    {
        // Act
        var cut = RenderComponent<TransferList<string>>();

        // Assert
        var panels = cut.FindAll(".panel-title");
        panels[1].TextContent.ShouldBe("Selected");
    }

    [Fact]
    public void TransferList_Accepts_CustomTitles()
    {
        // Act
        var cut = RenderComponent<TransferList<string>>(parameters => parameters
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
        var cut = RenderComponent<TransferList<string>>(parameters => parameters
            .Add(p => p.ShowSearch, true));

        // Assert
        var searchInputs = cut.FindAll(".search-input");
        searchInputs.Count.ShouldBe(2);
    }

    [Fact]
    public void TransferList_Hides_Search_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<TransferList<string>>(parameters => parameters
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
        var cut = RenderComponent<TransferList<string>>(parameters => parameters
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
        var cut = RenderComponent<TransferList<string>>(parameters => parameters
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
        var cut = RenderComponent<TransferList<string>>(parameters => parameters
            .Add(p => p.SourceItems, sourceItems));

        // Assert
        var items = cut.FindAll(".transfer-item");
        items.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void TransferList_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<TransferList<string>>(parameters => parameters
            .Add(p => p.CssClass, "custom-transfer"));

        // Assert
        cut.Find(".vibe-transfer").ClassList.ShouldContain("custom-transfer");
    }
}
