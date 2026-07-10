namespace Vibe.UI.Tests.Components.DataDisplay;

public class DataTableTests : TestBase
{
    [Fact]
    public void DataTable_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<DataTable<string>>();

        // Assert
        cut.Find(".vibe-datatable").ShouldNotBeNull();
    }

    [Fact]
    public void DataTable_Renders_Toolbar_ByDefault()
    {
        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.ShowToolbar, true));

        // Assert
        cut.Find(".datatable-toolbar").ShouldNotBeNull();
    }

    [Fact]
    public void DataTable_Hides_Toolbar_WhenDisabled()
    {
        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.ShowToolbar, false));

        // Assert
        cut.FindAll(".datatable-toolbar").ShouldBeEmpty();
    }

    [Fact]
    public void DataTable_Shows_Search_ByDefault()
    {
        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.ShowToolbar, true)
            .Add(p => p.ShowSearch, true));

        // Assert
        cut.Find(".datatable-search").ShouldNotBeNull();
    }

    [Fact]
    public void DataTable_Has_DefaultSearchPlaceholder()
    {
        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.ShowToolbar, true)
            .Add(p => p.ShowSearch, true));

        // Assert
        var searchInput = cut.Find(".search-input");
        searchInput.GetAttribute("placeholder")!.ShouldBe("Search...");
    }

    [Fact]
    public void DataTable_SearchInput_HasAccessibleLabel()
    {
        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.ShowToolbar, true)
            .Add(p => p.ShowSearch, true));

        // Assert
        var searchInput = cut.Find(".search-input");
        searchInput.GetAttribute("aria-label")!.ShouldBe("Search table");
    }

    [Fact]
    public void DataTable_SearchInput_UsesCustomAccessibleLabel()
    {
        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.ShowToolbar, true)
            .Add(p => p.ShowSearch, true)
            .Add(p => p.SearchLabel, "Filter customers"));

        // Assert
        var searchInput = cut.Find(".search-input");
        searchInput.GetAttribute("aria-label")!.ShouldBe("Filter customers");
    }

    [Fact]
    public void DataTable_Shows_Pagination_ByDefault()
    {
        // Arrange
        var items = Enumerable.Range(1, 20).Select(i => $"Item {i}").ToList();
        var columns = new List<DataTable<string>.DataTableColumn<string>>
        {
            new() { Title = "Name", PropertyName = "ToString" }
        };

        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Columns, columns)
            .Add(p => p.ShowPagination, true));

        // Assert
        cut.Find(".datatable-pagination").ShouldNotBeNull();
    }

    [Fact]
    public void DataTable_PaginationInfo_UsesZeroRange_WhenEmpty()
    {
        // Arrange
        var columns = new List<DataTable<string>.DataTableColumn<string>>
        {
            new() { Title = "Name", PropertyName = "ToString" }
        };

        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.Items, new List<string>())
            .Add(p => p.Columns, columns)
            .Add(p => p.ShowPagination, true));

        // Assert
        cut.Find(".pagination-info").TextContent.Trim().ShouldBe("Showing 0 to 0 of 0 entries");
    }

    [Fact]
    public void DataTable_Has_DefaultPageSize()
    {
        // Act
        var cut = Render<DataTable<string>>();

        // Assert
        cut.Instance.PageSize.ShouldBe(10);
    }

    [Fact]
    public void DataTable_Renders_EmptyMessage_WhenNoData()
    {
        // Arrange
        var columns = new List<DataTable<string>.DataTableColumn<string>>
        {
            new() { Title = "Name", PropertyName = "ToString" }
        };

        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.Items, new List<string>())
            .Add(p => p.Columns, columns)
            .Add(p => p.EmptyMessage, "No data"));

        // Assert
        var emptyCell = cut.Find(".datatable-empty");
        emptyCell.TextContent.ShouldBe("No data");
    }

    [Fact]
    public void DataTable_Renders_TableHeaders()
    {
        // Arrange
        var columns = new List<DataTable<string>.DataTableColumn<string>>
        {
            new() { Title = "Column 1", PropertyName = "ToString" },
            new() { Title = "Column 2", PropertyName = "Length" }
        };

        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var headers = cut.FindAll(".header-label");
        headers.Count.ShouldBe(2);
        headers[0].TextContent.ShouldBe("Column 1");
    }

    [Fact]
    public void DataTable_Marks_SortableColumns()
    {
        // Arrange
        var columns = new List<DataTable<string>.DataTableColumn<string>>
        {
            new() { Title = "Name", PropertyName = "ToString", IsSortable = true }
        };

        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var header = cut.Find("th");
        header.ClassList.ShouldContain("sortable");
    }

    [Fact]
    public void DataTable_Renders_SortableHeaderAsButton()
    {
        // Arrange
        var columns = new List<DataTable<TestRow>.DataTableColumn<TestRow>>
        {
            new() { Title = "Name", PropertyName = nameof(TestRow.Name), IsSortable = true }
        };

        // Act
        var cut = Render<DataTable<TestRow>>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var header = cut.Find("th.sortable");
        header.GetAttribute("aria-sort")!.ShouldBe("none");

        var sortButton = cut.Find(".datatable-sort-button");
        sortButton.TagName.ShouldBe("BUTTON");
        sortButton.GetAttribute("type")!.ShouldBe("button");
        sortButton.GetAttribute("aria-label")!.ShouldBe("Sort by Name");
    }

    [Fact]
    public void DataTable_ClickingSortableHeader_SortsRowsAndUpdatesSortState()
    {
        // Arrange
        var items = new List<TestRow>
        {
            new("Charlie", 30),
            new("Alice", 20),
            new("Bob", 40)
        };
        var columns = new List<DataTable<TestRow>.DataTableColumn<TestRow>>
        {
            new() { Title = "Name", PropertyName = nameof(TestRow.Name), IsSortable = true },
            new() { Title = "Age", PropertyName = nameof(TestRow.Age), IsSortable = false }
        };

        var cut = Render<DataTable<TestRow>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Columns, columns)
            .Add(p => p.ShowPagination, false));

        // Act
        cut.Find(".datatable-sort-button").Click();

        // Assert
        cut.Find("th.sortable").GetAttribute("aria-sort")!.ShouldBe("ascending");
        cut.Find(".datatable-sort-button").GetAttribute("aria-label")!.ShouldBe("Sort by Name descending");
        cut.FindAll("tbody tr td:first-child").Select(cell => cell.TextContent.Trim()).ToArray()
            .ShouldBe(new[] { "Alice", "Bob", "Charlie" });

        // Act
        cut.Find(".datatable-sort-button").Click();

        // Assert
        cut.Find("th.sortable").GetAttribute("aria-sort")!.ShouldBe("descending");
        cut.Find(".datatable-sort-button").GetAttribute("aria-label")!.ShouldBe("Sort by Name ascending");
        cut.FindAll("tbody tr td:first-child").Select(cell => cell.TextContent.Trim()).ToArray()
            .ShouldBe(new[] { "Charlie", "Bob", "Alice" });
    }

    [Fact]
    public void DataTable_Renders_Items()
    {
        // Arrange
        var items = new List<string> { "Item 1", "Item 2" };
        var columns = new List<DataTable<string>.DataTableColumn<string>>
        {
            new() { Title = "Name", PropertyName = "ToString" }
        };

        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Columns, columns));

        // Assert
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void DataTable_Applies_AdditionalAttributes()
    {
        // Act
        var cut = Render<DataTable<string>>(parameters => parameters
            .AddUnmatched("data-test", "datatable-value"));

        // Assert
        cut.Find(".vibe-datatable").GetAttribute("data-test")!.ShouldBe("datatable-value");
    }

    private sealed record TestRow(string Name, int Age);
}
