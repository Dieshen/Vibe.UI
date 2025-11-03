namespace Vibe.UI.Tests.Components.Disclosure;

public class CarouselTests : TestBase
{
    [Fact]
    public void Carousel_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Carousel>();

        // Assert
        var carousel = cut.Find(".vibe-carousel");
        carousel.ShouldNotBeNull();
    }

    [Fact]
    public void Carousel_Renders_Viewport()
    {
        // Act
        var cut = RenderComponent<Carousel>();

        // Assert
        var viewport = cut.Find(".carousel-viewport");
        viewport.ShouldNotBeNull();
    }

    [Fact]
    public void Carousel_Renders_Container()
    {
        // Act
        var cut = RenderComponent<Carousel>();

        // Assert
        var container = cut.Find(".carousel-container");
        container.ShouldNotBeNull();
    }

    [Fact]
    public void Carousel_Shows_NavigationButtons_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<Carousel>(parameters => parameters
            .Add(p => p.ShowNavigation, true));

        // Assert
        var navigation = cut.FindAll(".carousel-button");
        navigation.ShouldNotBeEmpty();
    }

    [Fact]
    public void Carousel_Hides_NavigationButtons_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<Carousel>(parameters => parameters
            .Add(p => p.ShowNavigation, false));

        // Assert
        cut.FindAll(".carousel-navigation").ShouldBeEmpty();
    }

    [Fact]
    public void Carousel_Shows_Indicators_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<Carousel>(parameters => parameters
            .Add(p => p.ShowIndicators, true));

        // Assert
        cut.FindAll(".carousel-indicators").ShouldNotBeEmpty();
    }

    [Fact]
    public void Carousel_Hides_Indicators_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<Carousel>(parameters => parameters
            .Add(p => p.ShowIndicators, false));

        // Assert
        cut.FindAll(".carousel-indicators").ShouldBeEmpty();
    }

    [Fact]
    public void Carousel_Applies_DefaultActiveIndex()
    {
        // Act
        var cut = RenderComponent<Carousel>(parameters => parameters
            .Add(p => p.ActiveIndex, 0));

        // Assert
        var carousel = cut.Find(".vibe-carousel");
        carousel.ShouldNotBeNull();
    }

    [Fact]
    public void Carousel_Applies_HorizontalOrientation_ByDefault()
    {
        // Act
        var cut = RenderComponent<Carousel>();

        // Assert
        var container = cut.Find(".carousel-container");
        var style = container.GetAttribute("style");
        style.ShouldContain("translateX");
    }

    [Fact]
    public void Carousel_Applies_VerticalOrientation()
    {
        // Act
        var cut = RenderComponent<Carousel>(parameters => parameters
            .Add(p => p.Orientation, Carousel.CarouselOrientation.Vertical));

        // Assert
        var container = cut.Find(".carousel-container");
        var style = container.GetAttribute("style");
        style.ShouldContain("translateY");
    }
}
