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

    [Fact]
    public void DataTable_LabelsTableAndFocusableOverflowRegionByDefault()
    {
        var cut = Render<DataTable<TestRow>>(parameters => parameters
            .Add(p => p.Items, CreateRows(2))
            .Add(p => p.Columns, CreateColumns()));

        var table = cut.Find(".datatable-table");
        table.GetAttribute("aria-label").ShouldBe("Data table");

        var region = cut.Find(".datatable-container");
        region.GetAttribute("role").ShouldBe("region");
        region.GetAttribute("tabindex").ShouldBe("0");
        region.GetAttribute("aria-label").ShouldBe("Data table, scrollable table");

        var status = cut.Find(".datatable-status");
        region.GetAttribute("aria-describedby").ShouldBe(status.Id);
        status.GetAttribute("role").ShouldBe("status");
        status.GetAttribute("aria-live").ShouldBe("polite");
        status.GetAttribute("aria-atomic").ShouldBe("true");
    }

    [Fact]
    public void DataTable_UsesVisibleCaptionInsteadOfDuplicateAriaLabel()
    {
        var cut = Render<DataTable<TestRow>>(parameters => parameters
            .Add(p => p.Items, CreateRows(2))
            .Add(p => p.Columns, CreateColumns())
            .Add(p => p.Caption, "  Customer accounts  ")
            .Add(p => p.AriaLabel, "Ignored table label"));

        cut.Find("caption").TextContent.ShouldBe("Customer accounts");
        cut.Find(".datatable-table").GetAttribute("aria-label").ShouldBeNull();
        cut.Find(".datatable-container").GetAttribute("aria-label")
            .ShouldBe("Customer accounts, scrollable table");
        cut.Find(".vibe-pagination").GetAttribute("aria-label")
            .ShouldBe("Customer accounts pagination");
    }

    [Fact]
    public void DataTable_UsesCustomAriaLabelWhenCaptionIsMissing()
    {
        var cut = Render<DataTable<TestRow>>(parameters => parameters
            .Add(p => p.Items, CreateRows(2))
            .Add(p => p.Columns, CreateColumns())
            .Add(p => p.AriaLabel, "  Customer accounts  "));

        cut.FindAll("caption").ShouldBeEmpty();
        cut.Find(".datatable-table").GetAttribute("aria-label").ShouldBe("Customer accounts");
        cut.Find(".datatable-container").GetAttribute("aria-label")
            .ShouldBe("Customer accounts, scrollable table");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void DataTable_NormalizesInvalidPageSizeToOne(int pageSize)
    {
        var cut = Render<DataTable<TestRow>>(parameters => parameters
            .Add(p => p.Items, CreateRows(3))
            .Add(p => p.Columns, CreateColumns())
            .Add(p => p.PageSize, pageSize));

        cut.FindAll("tbody tr").Count.ShouldBe(1);
        cut.Find("tbody td").TextContent.Trim().ShouldBe("Row 1");
        cut.Find(".pagination-info").TextContent.Trim().ShouldBe("Showing 1 to 1 of 3 entries");
        cut.FindAll(".pagination-link").Count.ShouldBe(3);
    }

    [Fact]
    public void DataTable_ClampsCurrentPageWhenItemsShrink()
    {
        var cut = RenderPagedRows(12, 5);
        ClickPage(cut, 3);
        cut.Find(".pagination-info").TextContent.Trim().ShouldBe("Showing 11 to 12 of 12 entries");

        cut.Render(parameters => parameters.Add(p => p.Items, CreateRows(2)));

        cut.Find(".pagination-info").TextContent.Trim().ShouldBe("Showing 1 to 2 of 2 entries");
        cut.Find(".pagination-link.active").TextContent.Trim().ShouldBe("1");
        cut.FindAll("tbody tr").Count.ShouldBe(2);
    }

    [Fact]
    public void DataTable_ClampsCurrentPageWhenPageSizeChanges()
    {
        var cut = RenderPagedRows(12, 5);
        ClickPage(cut, 3);

        cut.Render(parameters => parameters.Add(p => p.PageSize, 10));

        cut.Find(".pagination-info").TextContent.Trim().ShouldBe("Showing 11 to 12 of 12 entries");
        cut.Find(".pagination-link.active").TextContent.Trim().ShouldBe("2");
        cut.FindAll("tbody tr").Count.ShouldBe(2);
    }

    [Fact]
    public void DataTable_FilterReturnsToFirstPageAndAnnouncesResultCount()
    {
        var cut = RenderPagedRows(12, 5);
        ClickPage(cut, 3);

        cut.Find(".search-input").Input("Row 1");

        cut.Find(".pagination-info").TextContent.Trim().ShouldBe("Showing 1 to 4 of 4 entries");
        cut.Find(".pagination-link.active").TextContent.Trim().ShouldBe("1");
        cut.Find(".datatable-status").TextContent.Trim().ShouldBe("4 entries. Showing 1 to 4.");
    }

    [Fact]
    public void DataTable_StatusUsesNonPaginatedResultCountWhenPaginationIsHidden()
    {
        var cut = Render<DataTable<TestRow>>(parameters => parameters
            .Add(p => p.Items, CreateRows(3))
            .Add(p => p.Columns, CreateColumns())
            .Add(p => p.ShowPagination, false));

        cut.Find(".datatable-status").TextContent.Trim().ShouldBe("3 entries.");
        cut.FindAll(".datatable-pagination").ShouldBeEmpty();
    }

    private IRenderedComponent<DataTable<TestRow>> RenderPagedRows(int count, int pageSize) =>
        Render<DataTable<TestRow>>(parameters => parameters
            .Add(p => p.Items, CreateRows(count))
            .Add(p => p.Columns, CreateColumns())
            .Add(p => p.PageSize, pageSize));

    private static void ClickPage(IRenderedComponent<DataTable<TestRow>> cut, int page)
    {
        cut.FindAll(".pagination-link")
            .Single(button => button.TextContent.Trim() == page.ToString())
            .Click();
    }

    private static List<TestRow> CreateRows(int count) =>
        Enumerable.Range(1, count).Select(index => new TestRow($"Row {index}", index)).ToList();

    private static List<DataTable<TestRow>.DataTableColumn<TestRow>> CreateColumns() =>
    [
        new() { Title = "Name", PropertyName = nameof(TestRow.Name) }
    ];

    private sealed record TestRow(string Name, int Age);
}
