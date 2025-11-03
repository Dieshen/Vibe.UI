namespace Vibe.UI.Tests.Components.Layout;

public class ResizableTests : TestBase
{
    [Fact]
    public void Resizable_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Resizable>();

        // Assert
        var resizable = cut.Find(".vibe-resizable");
        resizable.ShouldNotBeNull();
    }

    [Fact]
    public void Resizable_Renders_Panel()
    {
        // Act
        var cut = RenderComponent<Resizable>();

        // Assert
        var panel = cut.Find(".resizable-panel");
        panel.ShouldNotBeNull();
    }

    [Fact]
    public void Resizable_Renders_Handle()
    {
        // Act
        var cut = RenderComponent<Resizable>();

        // Assert
        var handle = cut.Find(".resizable-handle");
        handle.ShouldNotBeNull();
    }

    [Fact]
    public void Resizable_Applies_HorizontalDirection_ByDefault()
    {
        // Act
        var cut = RenderComponent<Resizable>();

        // Assert
        var handle = cut.Find(".resizable-handle");
        handle.ClassList.ShouldContain("resizable-handle-horizontal");
    }

    [Fact]
    public void Resizable_Applies_VerticalDirection()
    {
        // Act
        var cut = RenderComponent<Resizable>(parameters => parameters
            .Add(p => p.Direction, Resizable.ResizableDirection.Vertical));

        // Assert
        var handle = cut.Find(".resizable-handle");
        handle.ClassList.ShouldContain("resizable-handle-vertical");
    }

    [Fact]
    public void Resizable_Applies_DefaultWidth()
    {
        // Arrange
        var defaultWidth = 400.0;

        // Act
        var cut = RenderComponent<Resizable>(parameters => parameters
            .Add(p => p.DefaultWidth, defaultWidth));

        // Assert
        var resizable = cut.Find(".vibe-resizable");
        resizable.GetAttribute("style").ShouldContain("width: 400px");
    }

    [Fact]
    public void Resizable_Applies_DefaultHeight()
    {
        // Arrange
        var defaultHeight = 300.0;

        // Act
        var cut = RenderComponent<Resizable>(parameters => parameters
            .Add(p => p.Direction, Resizable.ResizableDirection.Vertical)
            .Add(p => p.DefaultHeight, defaultHeight));

        // Assert
        var resizable = cut.Find(".vibe-resizable");
        resizable.GetAttribute("style").ShouldContain("height: 300px");
    }

    [Fact]
    public void Resizable_Displays_ChildContent()
    {
        // Arrange
        var content = "Resizable Content";

        // Act
        var cut = RenderComponent<Resizable>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var panel = cut.Find(".resizable-panel");
        panel.TextContent.ShouldContain(content);
    }

    [Fact]
    public void Resizable_Renders_HandleBar()
    {
        // Act
        var cut = RenderComponent<Resizable>();

        // Assert
        var handleBar = cut.Find(".resizable-handle-bar");
        handleBar.ShouldNotBeNull();
    }

    [Fact]
    public void Resizable_Applies_MinMaxConstraints()
    {
        // Arrange
        var minWidth = 150.0;
        var maxWidth = 600.0;

        // Act
        var cut = RenderComponent<Resizable>(parameters => parameters
            .Add(p => p.MinWidth, minWidth)
            .Add(p => p.MaxWidth, maxWidth));

        // Assert
        var resizable = cut.Find(".vibe-resizable");
        resizable.ShouldNotBeNull();
    }
}
