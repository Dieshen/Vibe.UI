namespace Vibe.UI.Tests.Components.Layout;

public class SplitterTests : TestBase
{
    [Fact]
    public void Splitter_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Splitter>();

        // Assert
        var splitter = cut.Find(".vibe-splitter");
        splitter.ShouldNotBeNull();
    }

    [Fact]
    public void Splitter_Applies_OrientationClass()
    {
        // Act
        var cut = RenderComponent<Splitter>(parameters => parameters
            .Add(p => p.Orientation, Splitter.SplitterOrientation.Vertical));

        // Assert
        var splitter = cut.Find(".vibe-splitter");
        splitter.ClassList.ShouldContain("splitter-vertical");
    }

    [Fact]
    public void Splitter_Renders_TwoPanes()
    {
        // Act
        var cut = RenderComponent<Splitter>();

        // Assert
        var panes = cut.FindAll(".splitter-pane");
        panes.Count.ShouldBe(2);
    }

    [Fact]
    public void Splitter_Renders_Divider()
    {
        // Act
        var cut = RenderComponent<Splitter>();

        // Assert
        var divider = cut.Find(".splitter-divider");
        divider.ShouldNotBeNull();
    }

    [Fact]
    public void Splitter_Displays_FirstPaneContent()
    {
        // Arrange
        var content = "First Pane";

        // Act
        var cut = RenderComponent<Splitter>(parameters => parameters
            .Add(p => p.FirstPane, builder => builder.AddContent(0, content)));

        // Assert
        var firstPane = cut.Find(".splitter-pane-first");
        firstPane.TextContent.ShouldContain(content);
    }

    [Fact]
    public void Splitter_Displays_SecondPaneContent()
    {
        // Arrange
        var content = "Second Pane";

        // Act
        var cut = RenderComponent<Splitter>(parameters => parameters
            .Add(p => p.SecondPane, builder => builder.AddContent(0, content)));

        // Assert
        var secondPane = cut.Find(".splitter-pane-second");
        secondPane.TextContent.ShouldContain(content);
    }

    [Fact]
    public void Splitter_Applies_InitialSize()
    {
        // Arrange
        var initialSize = 60.0;

        // Act
        var cut = RenderComponent<Splitter>(parameters => parameters
            .Add(p => p.InitialSize, initialSize));

        // Assert
        var firstPane = cut.Find(".splitter-pane-first");
        firstPane.GetAttribute("style").ShouldContain("60");
    }

    [Fact]
    public void Splitter_Applies_HorizontalOrientation_ByDefault()
    {
        // Act
        var cut = RenderComponent<Splitter>();

        // Assert
        var splitter = cut.Find(".vibe-splitter");
        splitter.ClassList.ShouldContain("splitter-horizontal");
    }

    [Fact]
    public void Splitter_Applies_MinMaxConstraints()
    {
        // Arrange
        var minSize = 20.0;
        var maxSize = 80.0;

        // Act
        var cut = RenderComponent<Splitter>(parameters => parameters
            .Add(p => p.MinSize, minSize)
            .Add(p => p.MaxSize, maxSize));

        // Assert
        var splitter = cut.Find(".vibe-splitter");
        splitter.ShouldNotBeNull();
    }

    [Fact]
    public void Splitter_Renders_DividerHandle()
    {
        // Act
        var cut = RenderComponent<Splitter>();

        // Assert
        var handle = cut.Find(".divider-handle");
        handle.ShouldNotBeNull();
    }
}
