namespace Vibe.UI.Tests.Components.Disclosure;

public class CarouselItemTests : TestBase
{
    [Fact]
    public void CarouselItem_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<CarouselItem>();

        // Assert
        var item = cut.Find(".carousel-item");
        item.ShouldNotBeNull();
    }

    [Fact]
    public void CarouselItem_Displays_ChildContent()
    {
        // Arrange
        var content = "Carousel Item Content";

        // Act
        var cut = RenderComponent<CarouselItem>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var item = cut.Find(".carousel-item");
        item.TextContent.ShouldContain(content);
    }

    [Fact]
    public void CarouselItem_HasCorrectCssClass()
    {
        // Act
        var cut = RenderComponent<CarouselItem>();

        // Assert
        var item = cut.Find(".carousel-item");
        item.ClassList.ShouldContain("carousel-item");
    }
}
