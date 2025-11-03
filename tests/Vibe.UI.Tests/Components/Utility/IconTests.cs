namespace Vibe.UI.Tests.Components.Utility;

public class IconTests : TestBase
{
    [Fact]
    public void Icon_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart"));

        // Assert
        var icon = cut.Find(".vibe-icon");
        icon.ShouldNotBeNull();
        icon.TagName.ShouldBe("svg");
    }

    [Fact]
    public void Icon_Applies_DefaultSize()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "star"));

        // Assert
        var icon = cut.Find("svg");
        icon.GetAttribute("width").ShouldBe("24");
        icon.GetAttribute("height").ShouldBe("24");
    }

    [Fact]
    public void Icon_Applies_CustomSize()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart")
            .Add(p => p.Size, 32));

        // Assert
        var icon = cut.Find("svg");
        icon.GetAttribute("width").ShouldBe("32");
        icon.GetAttribute("height").ShouldBe("32");
    }

    [Fact]
    public void Icon_Applies_CustomWidth()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart")
            .Add(p => p.Width, 48));

        // Assert
        var icon = cut.Find("svg");
        icon.GetAttribute("width").ShouldBe("48");
    }

    [Fact]
    public void Icon_Applies_CustomHeight()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart")
            .Add(p => p.Height, 64));

        // Assert
        var icon = cut.Find("svg");
        icon.GetAttribute("height").ShouldBe("64");
    }

    [Fact]
    public void Icon_Applies_CustomColor()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart")
            .Add(p => p.Color, "#ff0000"));

        // Assert
        var icon = cut.Find("svg");
        icon.GetAttribute("style").ShouldContain("color: #ff0000");
    }

    [Fact]
    public void Icon_Applies_CustomFill()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart")
            .Add(p => p.Fill, "red"));

        // Assert
        var icon = cut.Find("svg");
        icon.GetAttribute("fill").ShouldBe("red");
    }

    [Fact]
    public void Icon_Applies_CustomStroke()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart")
            .Add(p => p.Stroke, "blue"));

        // Assert
        var icon = cut.Find("svg");
        icon.GetAttribute("stroke").ShouldBe("blue");
    }

    [Fact]
    public void Icon_Applies_CustomStrokeWidth()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart")
            .Add(p => p.StrokeWidth, 3));

        // Assert
        var icon = cut.Find("svg");
        icon.GetAttribute("stroke-width").ShouldBe("3");
    }

    [Fact]
    public void Icon_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.Name, "heart")
            .Add(p => p.CssClass, "custom-icon"));

        // Assert
        var icon = cut.Find(".vibe-icon");
        icon.ClassList.ShouldContain("custom-icon");
    }

    [Fact]
    public void Icon_Renders_CustomSvg()
    {
        // Arrange
        var customSvg = "<svg class='custom'><circle r='10'/></svg>";

        // Act
        var cut = RenderComponent<Icon>(parameters => parameters
            .Add(p => p.CustomSvg, customSvg));

        // Assert
        cut.Markup.ShouldContain("custom");
    }
}
