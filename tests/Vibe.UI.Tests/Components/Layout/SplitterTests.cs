namespace Vibe.UI.Tests.Components.Layout;

public class SplitterTests : TestBase
{
    [Fact]
    public void Splitter_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Splitter>();

        // Assert
        var splitter = cut.Find(".vibe-splitter");
        splitter.ShouldNotBeNull();
    }

    [Fact]
    public void Splitter_Applies_OrientationClass()
    {
        // Act
        var cut = Render<Splitter>(parameters => parameters
            .Add(p => p.Orientation, Splitter.SplitterOrientation.Vertical));

        // Assert
        var splitter = cut.Find(".vibe-splitter");
        splitter.ClassList.ShouldContain("splitter-vertical");
    }

    [Fact]
    public void Splitter_Renders_TwoPanes()
    {
        // Act
        var cut = Render<Splitter>();

        // Assert
        var panes = cut.FindAll(".splitter-pane");
        panes.Count.ShouldBe(2);
    }

    [Fact]
    public void Splitter_Renders_Divider()
    {
        // Act
        var cut = Render<Splitter>();

        // Assert
        var divider = cut.Find(".splitter-divider");
        divider.ShouldNotBeNull();
        divider.GetAttribute("role").ShouldBe("separator");
        divider.GetAttribute("aria-orientation").ShouldBe("vertical");
        divider.GetAttribute("aria-valuenow").ShouldBe("50");
    }

    [Fact]
    public void Splitter_Displays_FirstPaneContent()
    {
        // Arrange
        var content = "First Pane";

        // Act
        var cut = Render<Splitter>(parameters => parameters
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
        var cut = Render<Splitter>(parameters => parameters
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
        var cut = Render<Splitter>(parameters => parameters
            .Add(p => p.InitialSize, initialSize));

        // Assert
        var firstPane = cut.Find(".splitter-pane-first");
        firstPane.GetAttribute("style")!.ShouldContain("60");
    }

    [Fact]
    public void Splitter_Applies_HorizontalOrientation_ByDefault()
    {
        // Act
        var cut = Render<Splitter>();

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
        var cut = Render<Splitter>(parameters => parameters
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
        var cut = Render<Splitter>();

        // Assert
        var handle = cut.Find(".divider-handle");
        handle.ShouldNotBeNull();
    }

    [Fact]
    public void Splitter_MergesClassStyleAndRootAttributes()
    {
        var cut = Render<Splitter>(parameters => parameters
            .Add(p => p.Class, "workspace")
            .Add(p => p.CssClass, "dense")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-splitter",
                ["style"] = "min-height: 20rem;",
                ["data-testid"] = "splitter"
            }));

        var splitter = cut.Find(".vibe-splitter");
        splitter.ClassList.ShouldContain("workspace");
        splitter.ClassList.ShouldContain("dense");
        splitter.ClassList.ShouldContain("attribute-splitter");
        splitter.GetAttribute("style").ShouldBe("min-height: 20rem");
        splitter.GetAttribute("data-testid").ShouldBe("splitter");
    }

    [Fact]
    public void Splitter_WithInvalidOrientation_FallsBackToHorizontal()
    {
        var cut = Render<Splitter>(parameters => parameters
            .Add(p => p.Orientation, (Splitter.SplitterOrientation)999));

        var splitter = cut.Find(".vibe-splitter");
        splitter.ClassList.ShouldContain("splitter-horizontal");
        splitter.ClassList.ShouldNotContain("splitter-999");
        cut.Find(".splitter-pane-first").GetAttribute("style")!.ShouldContain("width: 50%");
    }

    [Fact]
    public void Splitter_ClampsInitialSizeAndAppliesDividerSize()
    {
        var cut = Render<Splitter>(parameters => parameters
            .Add(p => p.InitialSize, 150)
            .Add(p => p.MinSize, 20)
            .Add(p => p.MaxSize, 80)
            .Add(p => p.DividerSize, 12));

        cut.Find(".splitter-pane-first").GetAttribute("style")!.ShouldContain("width: 80%");

        var divider = cut.Find(".splitter-divider");
        divider.GetAttribute("style").ShouldBe("width: 12px;");
        divider.GetAttribute("aria-valuenow").ShouldBe("80");
        divider.GetAttribute("aria-valuemin").ShouldBe("20");
        divider.GetAttribute("aria-valuemax").ShouldBe("80");
        divider.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact]
    public async Task Splitter_SetSize_ClampsAndInvokesCallback()
    {
        var changedSizes = new List<double>();
        var cut = Render<Splitter>(parameters => parameters
            .Add(p => p.MinSize, 20)
            .Add(p => p.MaxSize, 80)
            .Add(p => p.OnSizeChanged, changedSizes.Add));

        await cut.InvokeAsync(() => cut.Instance.SetSize(90));

        changedSizes.ShouldBe(new[] { 80d });
        cut.Find(".splitter-pane-first").GetAttribute("style")!.ShouldContain("width: 80%");
    }

    [Fact]
    public void Splitter_KeyboardResize_UpdatesSizeAndInvokesCallback()
    {
        double? changedSize = null;
        var cut = Render<Splitter>(parameters => parameters
            .Add(p => p.OnSizeChanged, value => changedSize = value));

        cut.Find(".splitter-divider").KeyDown("ArrowRight");

        changedSize.ShouldBe(51);
        cut.Find(".splitter-pane-first").GetAttribute("style")!.ShouldContain("width: 51%");
    }

    [Fact]
    public void Splitter_VerticalOrientation_UsesHorizontalDividerSemantics()
    {
        var cut = Render<Splitter>(parameters => parameters
            .Add(p => p.Orientation, Splitter.SplitterOrientation.Vertical)
            .Add(p => p.DividerSize, 8));

        var divider = cut.Find(".splitter-divider");
        divider.GetAttribute("aria-orientation").ShouldBe("horizontal");
        divider.GetAttribute("style").ShouldBe("height: 8px;");
        cut.Find(".splitter-pane-first").GetAttribute("style")!.ShouldContain("height: 50%");
    }
}
