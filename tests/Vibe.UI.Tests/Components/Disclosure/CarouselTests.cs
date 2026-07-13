using Microsoft.AspNetCore.Components.Web;

namespace Vibe.UI.Tests.Components.Disclosure;

public class CarouselTests : TestBase
{
    [Fact]
    public void Carousel_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Carousel>();

        // Assert
        var carousel = cut.Find(".vibe-carousel");
        carousel.ShouldNotBeNull();
        carousel.GetAttribute("role").ShouldBe("region");
        carousel.GetAttribute("aria-roledescription").ShouldBe("carousel");
        carousel.GetAttribute("aria-label").ShouldBe("Carousel");
        carousel.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact]
    public void Carousel_ForwardsAdditionalAttributes_AndUsesCustomAriaLabel()
    {
        // Act
        var cut = Render<Carousel>(parameters => parameters
            .Add(p => p.AriaLabel, "Featured projects")
            .AddUnmatched("data-testid", "carousel-root"));

        // Assert
        var carousel = cut.Find(".vibe-carousel");
        carousel.GetAttribute("aria-label").ShouldBe("Featured projects");
        carousel.GetAttribute("data-testid").ShouldBe("carousel-root");
    }

    [Fact]
    public void Carousel_Renders_Viewport()
    {
        // Act
        var cut = RenderCarouselWithItems("One", "Two");

        // Assert
        var viewport = cut.Find(".carousel-viewport");
        viewport.ShouldNotBeNull();
        viewport.GetAttribute("role").ShouldBe("group");
        viewport.GetAttribute("aria-label").ShouldBe("Slide 1 of 2");
        viewport.GetAttribute("aria-live").ShouldBe("polite");
    }

    [Fact]
    public void Carousel_Renders_Container()
    {
        // Act
        var cut = Render<Carousel>();

        // Assert
        var container = cut.Find(".carousel-container");
        container.ShouldNotBeNull();
    }

    [Fact]
    public void Carousel_Shows_NavigationButtons_WhenEnabled()
    {
        // Act
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.ShowNavigation, true));

        // Assert
        var navigation = cut.FindAll(".carousel-button");
        navigation.Count.ShouldBe(2);
        navigation[0].GetAttribute("aria-label").ShouldBe("Previous slide, slide 2");
        navigation[1].GetAttribute("aria-label").ShouldBe("Next slide, slide 2");
        navigation[0].QuerySelector("svg")!.GetAttribute("aria-hidden").ShouldBe("true");
        navigation[1].QuerySelector("svg")!.GetAttribute("focusable").ShouldBe("false");
    }

    [Fact]
    public void Carousel_Hides_NavigationButtons_WhenDisabled()
    {
        // Act
        var cut = Render<Carousel>(parameters => parameters
            .Add(p => p.ShowNavigation, false));

        // Assert
        cut.FindAll(".carousel-navigation").ShouldBeEmpty();
    }

    [Fact]
    public void Carousel_Shows_Indicators_WhenEnabled()
    {
        // Act
        var cut = RenderCarouselWithItems(new[] { "One", "Two", "Three" }, parameters => parameters
            .Add(p => p.ShowIndicators, true));

        // Assert
        var indicators = cut.FindAll(".carousel-indicator");
        indicators.Count.ShouldBe(3);
        indicators[0].GetAttribute("aria-current").ShouldBe("true");
        indicators[0].GetAttribute("aria-label").ShouldBe("Go to slide 1 of 3, current slide");
        indicators[1].GetAttribute("aria-label").ShouldBe("Go to slide 2 of 3");
    }

    [Fact]
    public void Carousel_Hides_Indicators_WhenDisabled()
    {
        // Act
        var cut = Render<Carousel>(parameters => parameters
            .Add(p => p.ShowIndicators, false));

        // Assert
        cut.FindAll(".carousel-indicators").ShouldBeEmpty();
    }

    [Fact]
    public void Carousel_Applies_DefaultActiveIndex()
    {
        // Act
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.ActiveIndex, 0));

        // Assert
        var container = cut.Find(".carousel-container");
        container.GetAttribute("style")!.ShouldContain("translateX(0%)");
    }

    [Fact]
    public void Carousel_Applies_HorizontalOrientation_ByDefault()
    {
        // Act
        var cut = RenderCarouselWithItems("One", "Two");

        // Assert
        var container = cut.Find(".carousel-container");
        var style = container.GetAttribute("style");
        style!.ShouldContain("translateX");
    }

    [Fact]
    public void Carousel_Applies_VerticalOrientation()
    {
        // Act
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.Orientation, Carousel.CarouselOrientation.Vertical));

        // Assert
        var container = cut.Find(".carousel-container");
        var style = container.GetAttribute("style");
        style!.ShouldContain("translateY");
    }

    [Fact]
    public void Carousel_NavigationButtons_RemainEnabledAtBoundaries_WhenLooping()
    {
        // Arrange
        var changedIndex = -1;
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.Loop, true)
            .Add(p => p.ActiveIndexChanged, index => changedIndex = index));

        // Act
        cut.Find(".carousel-prev-button").Click();

        // Assert
        changedIndex.ShouldBe(1);
        cut.Find(".carousel-container").GetAttribute("style")!.ShouldContain("translateX(-100%)");
        cut.Find(".carousel-prev-button").HasAttribute("disabled").ShouldBeFalse();
    }

    [Fact]
    public void Carousel_DisablesBoundaryButtons_WhenLoopingDisabled()
    {
        // Act
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.Loop, false)
            .Add(p => p.ActiveIndex, 0));

        // Assert
        cut.Find(".carousel-prev-button").HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".carousel-prev-button").GetAttribute("aria-disabled").ShouldBe("true");
        cut.Find(".carousel-next-button").HasAttribute("disabled").ShouldBeFalse();
    }

    [Fact]
    public void Carousel_IndicatorClick_ChangesSlideAndInvokesCallback()
    {
        // Arrange
        var changedIndex = -1;
        var cut = RenderCarouselWithItems(new[] { "One", "Two", "Three" }, parameters => parameters
            .Add(p => p.ActiveIndexChanged, index => changedIndex = index));

        // Act
        cut.FindAll(".carousel-indicator")[2].Click();

        // Assert
        changedIndex.ShouldBe(2);
        cut.Find(".carousel-container").GetAttribute("style")!.ShouldContain("translateX(-200%)");
        cut.FindAll(".carousel-indicator")[2].GetAttribute("aria-current").ShouldBe("true");
    }

    [Fact]
    public void Carousel_KeyboardNavigation_ChangesSlides()
    {
        // Arrange
        var changedIndex = -1;
        var cut = RenderCarouselWithItems(new[] { "One", "Two", "Three" }, parameters => parameters
            .Add(p => p.ActiveIndexChanged, index => changedIndex = index));
        var carousel = cut.Find(".vibe-carousel");

        // Act
        carousel.KeyDown("ArrowRight");
        carousel.KeyDown("End");

        // Assert
        changedIndex.ShouldBe(2);
        cut.Find(".carousel-container").GetAttribute("style")!.ShouldContain("translateX(-200%)");
    }

    [Fact]
    public void Carousel_VerticalKeyboardNavigation_UsesVerticalArrows()
    {
        // Arrange
        var changedIndex = -1;
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.Orientation, Carousel.CarouselOrientation.Vertical)
            .Add(p => p.ActiveIndexChanged, index => changedIndex = index));

        // Act
        cut.Find(".vibe-carousel").KeyDown("ArrowDown");

        // Assert
        changedIndex.ShouldBe(1);
        cut.Find(".carousel-container").GetAttribute("style")!.ShouldContain("translateY(-100%)");
    }

    [Fact]
    public void Carousel_DisabledState_SuppressesNavigationAndCallbacks()
    {
        // Arrange
        var changed = false;
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ActiveIndexChanged, _ => changed = true));

        // Act
        cut.Find(".vibe-carousel").KeyDown("ArrowRight");
        cut.Find(".carousel-next-button").Click();
        cut.FindAll(".carousel-indicator")[1].Click();

        // Assert
        changed.ShouldBeFalse();
        var carousel = cut.Find(".vibe-carousel");
        carousel.ClassList.ShouldContain("vibe-carousel-disabled");
        carousel.GetAttribute("aria-disabled").ShouldBe("true");
        carousel.GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".carousel-next-button").HasAttribute("disabled").ShouldBeTrue();
        cut.FindAll(".carousel-indicator").All(indicator => indicator.HasAttribute("disabled")).ShouldBeTrue();
    }

    [Fact]
    public void Carousel_ClampsActiveIndex_ToAvailableItems()
    {
        // Act
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.ActiveIndex, 25));

        // Assert
        cut.Find(".carousel-container").GetAttribute("style")!.ShouldContain("translateX(-100%)");
        cut.Find(".carousel-viewport").GetAttribute("aria-label").ShouldBe("Slide 2 of 2");
        cut.FindAll(".carousel-indicator")[1].GetAttribute("aria-current").ShouldBe("true");
    }

    [Fact]
    public void Carousel_HomeAndEndKeys_MoveToBoundarySlides()
    {
        // Arrange
        var cut = RenderCarouselWithItems(new[] { "One", "Two", "Three" }, parameters => parameters
            .Add(p => p.ActiveIndex, 1));
        var carousel = cut.Find(".vibe-carousel");

        // Act
        carousel.KeyDown("End");
        var endStyle = cut.Find(".carousel-container").GetAttribute("style");
        carousel.KeyDown("Home");

        // Assert
        endStyle!.ShouldContain("translateX(-200%)");
        cut.Find(".carousel-container").GetAttribute("style")!.ShouldContain("translateX(0%)");
    }

    [Fact]
    public void Carousel_NavigationUsesSharedIcons()
    {
        var cut = RenderCarouselWithItems("One", "Two");

        cut.FindAll(".carousel-button svg.vibe-icon").Count.ShouldBe(2);
        cut.FindAll(".carousel-button svg:not(.vibe-icon)").ShouldBeEmpty();
    }

    [Fact]
    public void Carousel_AutoPlayRendersUserPauseControl()
    {
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.AutoPlay, true));

        var pause = cut.Find(".carousel-autoplay-button");
        pause.GetAttribute("aria-label").ShouldBe("Pause automatic slide rotation");
        pause.GetAttribute("aria-pressed").ShouldBe("false");
        pause.ParentElement?.ClassList.ShouldContain("carousel-controls");
        cut.Find(".carousel-indicators").ParentElement?.ClassList.ShouldContain("carousel-controls");
        cut.FindAll(".carousel-viewport .carousel-indicators").ShouldBeEmpty();
        pause.Click();

        var resume = cut.Find(".carousel-autoplay-button");
        resume.GetAttribute("aria-label").ShouldBe("Resume automatic slide rotation");
        resume.GetAttribute("aria-pressed").ShouldBe("true");
    }

    [Fact]
    public void Carousel_PointerSwipeChangesSlide()
    {
        var changedIndex = -1;
        var cut = RenderCarouselWithItems(new[] { "One", "Two" }, parameters => parameters
            .Add(p => p.ActiveIndexChanged, index => changedIndex = index));
        var viewport = cut.Find(".carousel-viewport");

        viewport.TriggerEvent("onpointerdown", new PointerEventArgs { PointerId = 7, Button = 0, ClientX = 220, ClientY = 80 });
        viewport.TriggerEvent("onpointermove", new PointerEventArgs { PointerId = 7, ClientX = 140, ClientY = 80 });
        viewport.TriggerEvent("onpointerup", new PointerEventArgs { PointerId = 7, ClientX = 140, ClientY = 80 });

        changedIndex.ShouldBe(1);
        cut.Find(".carousel-container").GetAttribute("style")!.ShouldContain("translateX(-100%)");
    }

    private IRenderedComponent<Carousel> RenderCarouselWithItems(params string[] items)
    {
        return RenderCarouselWithItems(items, null);
    }

    private IRenderedComponent<Carousel> RenderCarouselWithItems(
        string[] items,
        Action<ComponentParameterCollectionBuilder<Carousel>>? configure)
    {
        return Render<Carousel>(parameters =>
        {
            configure?.Invoke(parameters);
            parameters.Add(p => p.ChildContent, BuildCarouselItems(items));
        });
    }

    private static RenderFragment BuildCarouselItems(IReadOnlyList<string> items)
    {
        return builder =>
        {
            var sequence = 0;
            foreach (var item in items)
            {
                builder.OpenComponent<CarouselItem>(sequence++);
                builder.AddAttribute(sequence++, "ChildContent", (RenderFragment)(contentBuilder =>
                    contentBuilder.AddContent(0, item)));
                builder.CloseComponent();
            }
        };
    }
}
