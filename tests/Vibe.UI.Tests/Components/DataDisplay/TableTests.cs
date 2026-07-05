namespace Vibe.UI.Tests.Components.DataDisplay;

public class TableTests : TestBase
{
    [Fact]
    public void Table_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".vibe-table").ShouldNotBeNull();
    }

    [Fact]
    public void Table_Renders_TableElement()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".table-root").ShouldNotBeNull();
    }

    [Fact]
    public void Table_Renders_Tbody()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".table-body").ShouldNotBeNull();
    }

    [Fact]
    public void Table_Renders_Header_WhenProvided()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.HeaderContent, builder => builder.AddContent(0, "<tr><th>Header</th></tr>"))
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        var thead = cut.Find(".table-header");
        thead.ShouldNotBeNull();
    }

    [Fact]
    public void Table_Renders_Footer_WhenProvided()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.FooterContent, builder => builder.AddContent(0, "<tr><td>Footer</td></tr>"))
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        var tfoot = cut.Find(".table-footer");
        tfoot.ShouldNotBeNull();
    }

    [Fact]
    public void Table_Applies_BorderedClass()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.Bordered, true)
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".vibe-table").ClassList.ShouldContain("bordered");
    }

    [Fact]
    public void Table_Applies_StripedClass()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.Striped, true)
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".vibe-table").ClassList.ShouldContain("striped");
    }

    [Fact]
    public void Table_Applies_CompactClass()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.Compact, true)
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".vibe-table").ClassList.ShouldContain("compact");
    }

    [Fact]
    public void Table_Applies_HoverClass()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.Hover, true)
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".vibe-table").ClassList.ShouldContain("hover");
    }

    [Fact]
    public void Table_Renders_ChildContent()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .AddChildContent("<tr><td class='test-cell'>Test Content</td></tr>"));

        // Assert
        var cell = cut.Find(".test-cell");
        cell.TextContent.ShouldBe("Test Content");
    }

    [Fact]
    public void Table_Applies_AdditionalAttributes()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .AddUnmatched("data-test", "table-value")
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".vibe-table").GetAttribute("data-test")!.ShouldBe("table-value");
    }

    [Fact]
    public void Table_Applies_CustomClass()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.Class, "orders-table")
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".vibe-table").ClassList.ShouldContain("orders-table");
    }

    [Fact]
    public void Table_RendersCaption_WhenProvided()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.Caption, "  User information  ")
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".table-caption").TextContent.ShouldBe("User information");
        cut.Find(".table-root").HasAttribute("aria-label").ShouldBeFalse();
    }

    [Fact]
    public void Table_AppliesAriaLabel_WhenCaptionIsNotProvided()
    {
        // Act
        var cut = Render<Table>(parameters => parameters
            .Add(p => p.AriaLabel, "User information")
            .AddChildContent("<tr><td>Cell</td></tr>"));

        // Assert
        cut.Find(".table-root").GetAttribute("aria-label")!.ShouldBe("User information");
    }

    [Fact]
    public void Table_RendersEmptyBody_WhenChildContentIsNull()
    {
        // Act
        var cut = Render<Table>();

        // Assert
        cut.Find(".table-root").ShouldNotBeNull();
        cut.Find(".table-body").Children.Length.ShouldBe(0);
    }
}
