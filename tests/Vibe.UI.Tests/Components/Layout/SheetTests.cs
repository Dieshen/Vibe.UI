namespace Vibe.UI.Tests.Components.Layout;

public class SheetTests : TestBase
{
    [Fact]
    public void Sheet_DoesNotRender_WhenClosed()
    {
        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, false));

        // Assert
        cut.FindAll(".vibe-sheet").ShouldBeEmpty();
    }

    [Fact]
    public void Sheet_Renders_WhenOpen()
    {
        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var sheet = cut.Find(".vibe-sheet");
        sheet.ShouldNotBeNull();
    }

    [Fact]
    public void Sheet_Displays_Title()
    {
        // Arrange
        var title = "Sheet Title";

        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Title, title));

        // Assert
        var titleElement = cut.Find(".sheet-title");
        titleElement.TextContent.ShouldBe(title);
    }

    [Fact]
    public void Sheet_Displays_Description()
    {
        // Arrange
        var description = "Sheet Description";

        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Description, description));

        // Assert
        var descElement = cut.Find(".sheet-description");
        descElement.TextContent.ShouldBe(description);
    }

    [Fact]
    public void Sheet_Applies_SideClass()
    {
        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Side, Sheet.SheetSide.Left));

        // Assert
        var sheet = cut.Find(".vibe-sheet");
        sheet.ClassList.ShouldContain("sheet-left");
    }

    [Fact]
    public void Sheet_Shows_CloseButton_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, true));

        // Assert
        var closeButton = cut.Find(".sheet-close");
        closeButton.ShouldNotBeNull();
    }

    [Fact]
    public void Sheet_Hides_CloseButton_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, false));

        // Assert
        cut.FindAll(".sheet-close").ShouldBeEmpty();
    }

    [Fact]
    public void Sheet_Displays_ChildContent()
    {
        // Arrange
        var content = "Sheet Content";

        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var body = cut.Find(".sheet-body");
        body.TextContent.ShouldContain(content);
    }

    [Fact]
    public void Sheet_Displays_Footer_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Footer, builder => builder.AddContent(0, "Footer")));

        // Assert
        var footer = cut.Find(".sheet-footer");
        footer.ShouldNotBeNull();
    }

    [Fact]
    public void Sheet_Applies_SizeStyle()
    {
        // Act
        var cut = RenderComponent<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Size, Sheet.SheetSize.Large));

        // Assert
        var content = cut.Find(".sheet-content");
        content.GetAttribute("style").ShouldContain("--sheet-size");
    }
}
