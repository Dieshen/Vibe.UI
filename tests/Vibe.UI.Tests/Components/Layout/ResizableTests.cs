namespace Vibe.UI.Tests.Components.Layout;

public class ResizableTests : TestBase
{
    [Fact]
    public void Resizable_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Resizable>();

        // Assert
        var resizable = cut.Find(".vibe-resizable");
        resizable.ShouldNotBeNull();
    }

    [Fact]
    public void Resizable_Renders_Panel()
    {
        // Act
        var cut = Render<Resizable>();

        // Assert
        var panel = cut.Find(".resizable-panel");
        panel.ShouldNotBeNull();
    }

    [Fact]
    public void Resizable_Renders_Handle()
    {
        // Act
        var cut = Render<Resizable>();

        // Assert
        var handle = cut.Find(".resizable-handle");
        handle.ShouldNotBeNull();
    }

    [Fact]
    public void Resizable_Applies_HorizontalDirection_ByDefault()
    {
        // Act
        var cut = Render<Resizable>();

        // Assert
        var handle = cut.Find(".resizable-handle");
        handle.ClassList.ShouldContain("resizable-handle-horizontal");
    }

    [Fact]
    public void Resizable_Applies_VerticalDirection()
    {
        // Act
        var cut = Render<Resizable>(parameters => parameters
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
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.DefaultWidth, defaultWidth));

        // Assert
        var resizable = cut.Find(".vibe-resizable");
        resizable.GetAttribute("style")!.ShouldContain("width: 400px");
    }

    [Fact]
    public void Resizable_Applies_DefaultHeight()
    {
        // Arrange
        var defaultHeight = 300.0;

        // Act
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.Direction, Resizable.ResizableDirection.Vertical)
            .Add(p => p.DefaultHeight, defaultHeight));

        // Assert
        var resizable = cut.Find(".vibe-resizable");
        resizable.GetAttribute("style")!.ShouldContain("height: 300px");
    }

    [Fact]
    public void Resizable_Displays_ChildContent()
    {
        // Arrange
        var content = "Resizable Content";

        // Act
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var panel = cut.Find(".resizable-panel");
        panel.TextContent.ShouldContain(content);
    }

    [Fact]
    public void Resizable_Renders_HandleBar()
    {
        // Act
        var cut = Render<Resizable>();

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
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.MinWidth, minWidth)
            .Add(p => p.MaxWidth, maxWidth));

        // Assert
        var resizable = cut.Find(".vibe-resizable");
        resizable.ShouldNotBeNull();
    }

    [Fact]
    public void Resizable_MergesClassStyleAndRootAttributes()
    {
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.Class, "resize-shell")
            .Add(p => p.DefaultWidth, 360)
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-resize",
                ["style"] = "min-width: 10rem;",
                ["data-testid"] = "resizable"
            }));

        var resizable = cut.Find(".vibe-resizable");
        resizable.ClassList.ShouldContain("resize-shell");
        resizable.ClassList.ShouldContain("attribute-resize");
        resizable.GetAttribute("data-testid").ShouldBe("resizable");

        var style = resizable.GetAttribute("style")!;
        style.ShouldContain("min-width: 10rem");
        style.ShouldContain("width: 360px");
    }

    [Fact]
    public void Resizable_WithInvalidDirection_FallsBackToHorizontal()
    {
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.Direction, (Resizable.ResizableDirection)999));

        var handle = cut.Find(".resizable-handle");
        handle.ClassList.ShouldContain("resizable-handle-horizontal");
        handle.GetAttribute("aria-orientation").ShouldBe("vertical");
        cut.Find(".vibe-resizable").GetAttribute("style")!.ShouldContain("width: 300px");
    }

    [Fact]
    public void Resizable_ClampsInitialWidthAndExposesAccessibleSize()
    {
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.DefaultWidth, 25)
            .Add(p => p.MinWidth, 100)
            .Add(p => p.MaxWidth, 200));

        var resizable = cut.Find(".vibe-resizable");
        resizable.GetAttribute("style")!.ShouldContain("width: 100px");

        var handle = cut.Find(".resizable-handle");
        handle.GetAttribute("role").ShouldBe("separator");
        handle.GetAttribute("aria-valuenow").ShouldBe("100");
        handle.GetAttribute("aria-valuemin").ShouldBe("100");
        handle.GetAttribute("aria-valuemax").ShouldBe("200");
        handle.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact]
    public async Task Resizable_HandleDragEnd_InvokesSizeCallback()
    {
        double? changedSize = null;
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.MinWidth, 100)
            .Add(p => p.MaxWidth, 500)
            .Add(p => p.OnSizeChange, value => changedSize = value));

        await cut.InvokeAsync(() => cut.Instance.HandleDragMove(450, 0));
        await cut.InvokeAsync(() => cut.Instance.HandleDragEnd());

        changedSize.ShouldBe(450);
    }

    [Fact]
    public void Resizable_KeyboardResize_UpdatesSizeAndInvokesCallback()
    {
        double? changedSize = null;
        var cut = Render<Resizable>(parameters => parameters
            .Add(p => p.DefaultWidth, 300)
            .Add(p => p.OnSizeChange, value => changedSize = value));

        cut.Find(".resizable-handle").KeyDown("ArrowRight");

        changedSize.ShouldBe(310);
        cut.Find(".vibe-resizable").GetAttribute("style")!.ShouldContain("width: 310px");
    }
}
