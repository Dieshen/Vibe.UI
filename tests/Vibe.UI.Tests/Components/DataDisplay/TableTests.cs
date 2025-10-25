namespace Vibe.UI.Tests.Components.DataDisplay;

public class TableTests : TestContext
{
    public TableTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Table_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>();

        // Assert
        cut.Find(".vibe-table").Should().NotBeNull();
        cut.Find("table.table-root").Should().NotBeNull();
        cut.Find("tbody.table-body").Should().NotBeNull();
    }

    [Fact]
    public void Table_RendersBodyContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .AddChildContent("<tr><td>Cell 1</td><td>Cell 2</td></tr>"));

        // Assert
        cut.Markup.Should().Contain("<tr><td>Cell 1</td><td>Cell 2</td></tr>");
    }

    [Fact]
    public void Table_RendersHeader_WhenProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.HeaderContent, builder => builder.AddContent(0, "<tr><th>Header 1</th></tr>")));

        // Assert
        cut.Find("thead.table-header").Should().NotBeNull();
        cut.Markup.Should().Contain("<th>Header 1</th>");
    }

    [Fact]
    public void Table_DoesNotRenderHeader_WhenNotProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>();

        // Assert
        cut.FindAll("thead.table-header").Should().BeEmpty();
    }

    [Fact]
    public void Table_RendersFooter_WhenProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.FooterContent, builder => builder.AddContent(0, "<tr><td>Footer</td></tr>")));

        // Assert
        cut.Find("tfoot.table-footer").Should().NotBeNull();
        cut.Markup.Should().Contain("Footer");
    }

    [Fact]
    public void Table_DoesNotRenderFooter_WhenNotProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>();

        // Assert
        cut.FindAll("tfoot.table-footer").Should().BeEmpty();
    }

    [Fact]
    public void Table_AppliesBorderedClass_WhenSet()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.Bordered, true));

        // Assert
        cut.Find(".vibe-table").ClassList.Should().Contain("bordered");
    }

    [Fact]
    public void Table_AppliesStripedClass_WhenSet()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.Striped, true));

        // Assert
        cut.Find(".vibe-table").ClassList.Should().Contain("striped");
    }

    [Fact]
    public void Table_AppliesCompactClass_WhenSet()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.Compact, true));

        // Assert
        cut.Find(".vibe-table").ClassList.Should().Contain("compact");
    }

    [Fact]
    public void Table_AppliesHoverClass_WhenSet()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.Hover, true));

        // Assert
        cut.Find(".vibe-table").ClassList.Should().Contain("hover");
    }

    [Fact]
    public void Table_CombinesMultipleStyles()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.Bordered, true)
            .Add(p => p.Striped, true)
            .Add(p => p.Hover, true));

        // Assert
        var table = cut.Find(".vibe-table");
        table.ClassList.Should().Contain("bordered");
        table.ClassList.Should().Contain("striped");
        table.ClassList.Should().Contain("hover");
    }

    [Fact]
    public void Table_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.CssClass, "custom-table"));

        // Assert
        cut.Find(".vibe-table").ClassList.Should().Contain("custom-table");
    }

    [Fact]
    public void Table_RendersCompleteTable_WithAllSections()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>(parameters => parameters
            .Add(p => p.HeaderContent, builder => builder.AddContent(0, "<tr><th>Name</th><th>Age</th></tr>"))
            .AddChildContent("<tr><td>John</td><td>30</td></tr><tr><td>Jane</td><td>25</td></tr>")
            .Add(p => p.FooterContent, builder => builder.AddContent(0, "<tr><td colspan='2'>Total: 2 rows</td></tr>")));

        // Assert
        cut.Find("thead").Should().NotBeNull();
        cut.Find("tbody").Should().NotBeNull();
        cut.Find("tfoot").Should().NotBeNull();
        cut.Markup.Should().Contain("Name");
        cut.Markup.Should().Contain("John");
        cut.Markup.Should().Contain("Total: 2 rows");
    }

    [Fact]
    public void Table_DoesNotApplyStyleClasses_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Table>();

        // Assert
        var table = cut.Find(".vibe-table");
        table.ClassList.Should().NotContain("bordered");
        table.ClassList.Should().NotContain("striped");
        table.ClassList.Should().NotContain("compact");
        table.ClassList.Should().NotContain("hover");
    }
}
