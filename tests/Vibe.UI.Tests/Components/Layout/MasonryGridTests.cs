namespace Vibe.UI.Tests.Components.Layout;

public class MasonryGridTests : TestBase
{
    [Fact]
    public void MasonryGrid_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<MasonryGrid>();

        // Assert
        var grid = cut.Find(".vibe-masonry");
        grid.ShouldNotBeNull();
    }

    [Fact]
    public void MasonryGrid_Applies_DefaultColumns()
    {
        // Act
        var cut = RenderComponent<MasonryGrid>();

        // Assert
        var columns = cut.FindAll(".masonry-column");
        columns.Count.ShouldBe(3);
    }

    [Fact]
    public void MasonryGrid_Applies_CustomColumns()
    {
        // Arrange
        var columnCount = 5;

        // Act
        var cut = RenderComponent<MasonryGrid>(parameters => parameters
            .Add(p => p.Columns, columnCount));

        // Assert
        var columns = cut.FindAll(".masonry-column");
        columns.Count.ShouldBe(columnCount);
    }

    [Fact]
    public void MasonryGrid_Applies_GapStyle()
    {
        // Arrange
        var gap = 20;

        // Act
        var cut = RenderComponent<MasonryGrid>(parameters => parameters
            .Add(p => p.Gap, gap));

        // Assert
        var grid = cut.Find(".vibe-masonry");
        grid.GetAttribute("style").ShouldContain($"gap: {gap}px");
    }

    [Fact]
    public void MasonryGrid_Renders_Items()
    {
        // Arrange
        var items = new List<MasonryGrid.MasonryItem>
        {
            new() { Content = builder => builder.AddContent(0, "Item 1") },
            new() { Content = builder => builder.AddContent(0, "Item 2") },
            new() { Content = builder => builder.AddContent(0, "Item 3") }
        };

        // Act
        var cut = RenderComponent<MasonryGrid>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var renderedItems = cut.FindAll(".masonry-item");
        renderedItems.Count.ShouldBe(3);
    }

    [Fact]
    public void MasonryGrid_Shows_EmptyContent_WhenNoItems()
    {
        // Act
        var cut = RenderComponent<MasonryGrid>(parameters => parameters
            .Add(p => p.EmptyContent, builder => builder.AddContent(0, "No items")));

        // Assert
        var empty = cut.Find(".masonry-empty");
        empty.ShouldNotBeNull();
        empty.TextContent.ShouldContain("No items");
    }

    [Fact]
    public void MasonryGrid_DistributesItems_AcrossColumns()
    {
        // Arrange
        var items = new List<MasonryGrid.MasonryItem>
        {
            new() { Content = builder => builder.AddContent(0, "Item 1") },
            new() { Content = builder => builder.AddContent(0, "Item 2") },
            new() { Content = builder => builder.AddContent(0, "Item 3") },
            new() { Content = builder => builder.AddContent(0, "Item 4") }
        };

        // Act
        var cut = RenderComponent<MasonryGrid>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Columns, 2));

        // Assert
        var columns = cut.FindAll(".masonry-column");
        columns.Count.ShouldBe(2);
    }

    [Fact]
    public void MasonryGrid_UsesItemTemplate_WhenProvided()
    {
        // Arrange
        var items = new List<MasonryGrid.MasonryItem>
        {
            new() { Data = "Custom Data" }
        };

        // Act
        var cut = RenderComponent<MasonryGrid>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.ItemTemplate, item => builder => builder.AddContent(0, item.Data?.ToString())));

        // Assert
        var renderedItem = cut.Find(".masonry-item");
        renderedItem.TextContent.ShouldContain("Custom Data");
    }

    [Fact]
    public void MasonryGrid_HandlesEmptyItems()
    {
        // Act
        var cut = RenderComponent<MasonryGrid>(parameters => parameters
            .Add(p => p.Items, new List<MasonryGrid.MasonryItem>()));

        // Assert
        var items = cut.FindAll(".masonry-item");
        items.ShouldBeEmpty();
    }
}
