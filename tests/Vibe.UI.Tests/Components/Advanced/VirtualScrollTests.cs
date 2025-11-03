namespace Vibe.UI.Tests.Components.Advanced;

public class VirtualScrollTests : TestBase
{
    [Fact]
    public void VirtualScroll_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<VirtualScroll<string>>();

        // Assert
        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.ShouldNotBeNull();
    }

    [Fact]
    public void VirtualScroll_Renders_Items()
    {
        // Arrange
        var items = new List<string> { "Item 1", "Item 2", "Item 3" };

        // Act
        var cut = RenderComponent<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var itemElements = cut.FindAll(".virtual-scroll-item");
        itemElements.ShouldNotBeEmpty();
    }

    [Fact]
    public void VirtualScroll_Displays_EmptyContent_WhenNoItems()
    {
        // Arrange
        var emptyMarkup = "<div>No items</div>";

        // Act
        var cut = RenderComponent<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, new List<string>())
            .Add(p => p.EmptyContent, emptyMarkup));

        // Assert
        var empty = cut.Find(".virtual-scroll-empty");
        empty.ShouldNotBeNull();
        empty.InnerHtml.ShouldContain("No items");
    }

    [Fact]
    public void VirtualScroll_Uses_CustomItemTemplate()
    {
        // Arrange
        var items = new List<string> { "Item 1" };

        // Act
        var cut = RenderComponent<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.ItemTemplate, item => $"<div class='custom-item'>{item}</div>"));

        // Assert
        var customItem = cut.Find(".custom-item");
        customItem.ShouldNotBeNull();
        customItem.TextContent.ShouldContain("Item 1");
    }

    [Fact]
    public void VirtualScroll_Applies_CustomHeight()
    {
        // Act
        var cut = RenderComponent<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Height, 600));

        // Assert
        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.GetAttribute("style").ShouldContain("height: 600px");
    }

    [Fact]
    public void VirtualScroll_Uses_CustomItemHeight()
    {
        // Arrange
        var items = new List<string> { "Item 1", "Item 2", "Item 3" };

        // Act
        var cut = RenderComponent<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.ItemHeight, 100));

        // Assert
        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.ShouldNotBeNull();
    }

    [Fact]
    public void VirtualScroll_Uses_CustomBufferSize()
    {
        // Arrange
        var items = new List<string> { "Item 1", "Item 2", "Item 3" };

        // Act
        var cut = RenderComponent<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.BufferSize, 10));

        // Assert
        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.ShouldNotBeNull();
    }

    [Fact]
    public void VirtualScroll_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.CssClass, "custom-scroll"));

        // Assert
        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.ClassList.ShouldContain("custom-scroll");
    }

    [Fact]
    public void VirtualScroll_Handles_ComplexTypes()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "Item 1" },
            new() { Id = 2, Name = "Item 2" }
        };

        // Act
        var cut = RenderComponent<VirtualScroll<TestItem>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.KeySelector, item => item.Id.ToString()));

        // Assert
        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.ShouldNotBeNull();
    }

    private class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
