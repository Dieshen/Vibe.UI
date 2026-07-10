namespace Vibe.UI.Tests.Components.Advanced;

public class VirtualScrollTests : TestBase
{
    [Fact]
    public void VirtualScroll_RendersAccessibleEmptyStateByDefault()
    {
        var cut = Render<VirtualScroll<string>>();

        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.GetAttribute("role").ShouldBe("list");
        scroll.GetAttribute("aria-label").ShouldBe("Virtualized list");
        scroll.GetAttribute("aria-rowcount").ShouldBe("0");
        scroll.GetAttribute("style")!.ShouldContain("height: 400px");
        scroll.ClassList.ShouldContain("vibe-virtual-scroll-empty");

        var spacer = cut.Find(".virtual-scroll-spacer");
        spacer.GetAttribute("aria-hidden").ShouldBe("true");
        spacer.GetAttribute("style")!.ShouldContain("height: 0px");

        var empty = cut.Find(".virtual-scroll-empty");
        empty.GetAttribute("role").ShouldBe("status");
        empty.TextContent.ShouldBe("No items");
    }

    [Fact]
    public void VirtualScroll_RendersCustomEmptyContent()
    {
        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, new List<string>())
            .Add(p => p.EmptyContent, builder => builder.AddMarkupContent(0, "<strong>No matches</strong>")));

        cut.Find(".virtual-scroll-empty strong").TextContent.ShouldBe("No matches");
    }

    [Fact]
    public void VirtualScroll_RendersVisibleItemsWithListItemMetadata()
    {
        var items = Enumerable.Range(1, 10)
            .Select(index => $"Item {index}")
            .ToList();

        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Height, 100)
            .Add(p => p.ItemHeight, 25)
            .Add(p => p.BufferSize, 0));

        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.GetAttribute("aria-rowcount").ShouldBe("10");
        scroll.ClassList.ShouldNotContain("vibe-virtual-scroll-empty");

        var renderedItems = cut.FindAll(".virtual-scroll-item");
        renderedItems.Count.ShouldBe(4);
        renderedItems[0].GetAttribute("role").ShouldBe("listitem");
        renderedItems[0].GetAttribute("aria-setsize").ShouldBe("10");
        renderedItems[0].GetAttribute("aria-posinset").ShouldBe("1");
        renderedItems[0].TextContent.ShouldBe("Item 1");
        renderedItems[3].GetAttribute("aria-posinset").ShouldBe("4");
        renderedItems[3].TextContent.ShouldBe("Item 4");

        cut.Find(".virtual-scroll-spacer").GetAttribute("style")!.ShouldContain("height: 250px");
    }

    [Fact]
    public void VirtualScroll_UsesCustomItemTemplate()
    {
        var items = new List<string> { "Item 1" };

        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.ItemTemplate, item => $"<div class='custom-item'>{item}</div>"));

        cut.Find(".custom-item").TextContent.ShouldContain("Item 1");
    }

    [Fact]
    public void VirtualScroll_AppliesCustomClassAttributesAndAriaLabel()
    {
        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.CssClass, "legacy-scroll")
            .Add(p => p.Class, "base-scroll")
            .Add(p => p.AriaLabel, " Search results ")
            .AddUnmatched("data-testid", "virtual-scroll"));

        var scroll = cut.Find(".vibe-virtual-scroll");
        scroll.ClassList.ShouldContain("legacy-scroll");
        scroll.ClassList.ShouldContain("base-scroll");
        scroll.GetAttribute("aria-label").ShouldBe("Search results");
        scroll.GetAttribute("data-testid").ShouldBe("virtual-scroll");
    }

    [Fact]
    public void VirtualScroll_InvalidDimensionsAndBuffer_AreClamped()
    {
        var items = new List<string> { "Item 1", "Item 2", "Item 3" };

        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Height, 0)
            .Add(p => p.ItemHeight, 0)
            .Add(p => p.BufferSize, -10));

        cut.Find(".vibe-virtual-scroll").GetAttribute("style")!.ShouldContain("height: 1px");
        cut.Find(".virtual-scroll-spacer").GetAttribute("style")!.ShouldContain("height: 3px");
        cut.FindAll(".virtual-scroll-item").Count.ShouldBe(1);
    }

    [Fact]
    public async Task VirtualScroll_HandleScroll_UpdatesVisibleRangeAndSpacerOffset()
    {
        var items = Enumerable.Range(0, 10)
            .Select(index => $"Item {index}")
            .ToList();

        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Height, 40)
            .Add(p => p.ItemHeight, 20)
            .Add(p => p.BufferSize, 0));

        await cut.InvokeAsync(() => cut.Instance.HandleScroll(60));

        var renderedItems = cut.FindAll(".virtual-scroll-item");
        renderedItems.Select(item => item.TextContent).ShouldBe(["Item 3", "Item 4"]);
        renderedItems[0].GetAttribute("aria-posinset").ShouldBe("4");
        cut.Find(".virtual-scroll-spacer").GetAttribute("style")!.ShouldContain("padding-top: 60px");
    }

    [Fact]
    public async Task VirtualScroll_HandleScroll_ClampsNegativeScrollTop()
    {
        var items = Enumerable.Range(0, 5)
            .Select(index => $"Item {index}")
            .ToList();

        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Height, 20)
            .Add(p => p.ItemHeight, 20)
            .Add(p => p.BufferSize, 0));

        await cut.InvokeAsync(() => cut.Instance.HandleScroll(60));
        await cut.InvokeAsync(() => cut.Instance.HandleScroll(-100));

        cut.FindAll(".virtual-scroll-item").Single().TextContent.ShouldBe("Item 0");
        cut.Find(".virtual-scroll-spacer").GetAttribute("style")!.ShouldContain("padding-top: 0px");
    }

    [Fact]
    public async Task VirtualScroll_ScrollToIndex_UpdatesVisibleRange_WhenIndexIsValid()
    {
        var items = Enumerable.Range(0, 10)
            .Select(index => $"Item {index}")
            .ToList();

        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Height, 40)
            .Add(p => p.ItemHeight, 20)
            .Add(p => p.BufferSize, 0));

        await cut.InvokeAsync(() => cut.Instance.ScrollToIndex(7));

        cut.FindAll(".virtual-scroll-item")
            .Select(item => item.TextContent)
            .ShouldBe(["Item 7", "Item 8"]);
    }

    [Fact]
    public async Task VirtualScroll_ScrollToIndex_IgnoresInvalidIndex()
    {
        var items = Enumerable.Range(0, 10)
            .Select(index => $"Item {index}")
            .ToList();

        var cut = Render<VirtualScroll<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Height, 40)
            .Add(p => p.ItemHeight, 20)
            .Add(p => p.BufferSize, 0));

        await cut.InvokeAsync(() => cut.Instance.ScrollToIndex(7));
        await cut.InvokeAsync(() => cut.Instance.ScrollToIndex(99));

        cut.FindAll(".virtual-scroll-item")
            .Select(item => item.TextContent)
            .ShouldBe(["Item 7", "Item 8"]);
    }

    [Fact]
    public void VirtualScroll_HandlesNullItemsAndBlankKeySelector()
    {
        var items = new List<string?> { null, "Item 1" };

        var cut = Render<VirtualScroll<string?>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.KeySelector, _ => " ")
            .Add(p => p.Height, 100)
            .Add(p => p.ItemHeight, 20));

        var renderedItems = cut.FindAll(".virtual-scroll-item");
        renderedItems.Count.ShouldBe(2);
        renderedItems[0].TextContent.ShouldBe(string.Empty);
        renderedItems[0].GetAttribute("aria-posinset").ShouldBe("1");
        renderedItems[1].TextContent.ShouldBe("Item 1");
    }
}
