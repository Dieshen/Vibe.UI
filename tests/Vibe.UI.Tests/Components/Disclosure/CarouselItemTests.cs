namespace Vibe.UI.Tests.Components.Disclosure;

public class CarouselItemTests : TestBase
{
    [Fact]
    public void CarouselItem_Renders_WithDefaultProps()
    {
        var cut = Render<CarouselItem>();

        var item = cut.Find(".carousel-item");
        item.ShouldNotBeNull();
        item.GetAttribute("role").ShouldBe("group");
        item.GetAttribute("aria-roledescription").ShouldBe("slide");
    }

    [Fact]
    public void CarouselItem_Displays_ChildContent()
    {
        var content = "Carousel Item Content";

        var cut = Render<CarouselItem>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        cut.Find(".carousel-item").TextContent.ShouldContain(content);
    }

    [Fact]
    public void CarouselItem_RendersWithoutChildContent()
    {
        var cut = Render<CarouselItem>();

        cut.Find(".carousel-item").TextContent.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void CarouselItem_PreservesCustomClassAndAttributes()
    {
        var cut = Render<CarouselItem>(parameters => parameters
            .Add(p => p.Class, "featured-slide")
            .AddUnmatched("data-slide", "hero"));

        var item = cut.Find(".carousel-item");
        item.ClassList.ShouldContain("featured-slide");
        item.GetAttribute("data-slide").ShouldBe("hero");
    }

    [Fact]
    public void CarouselItem_RegistersWithParentCarousel()
    {
        var cut = Render<Carousel>(parameters => parameters
            .Add(p => p.ChildContent, BuildCarouselItems("One", "Two")));

        cut.FindAll(".carousel-item").Count.ShouldBe(2);
        cut.Find(".carousel-viewport").GetAttribute("aria-label").ShouldBe("Slide 1 of 2");
        cut.FindAll(".carousel-indicator").Count.ShouldBe(2);
    }

    private static RenderFragment BuildCarouselItems(params string[] items)
    {
        return builder =>
        {
            var sequence = 0;
            foreach (var item in items)
            {
                builder.OpenComponent<CarouselItem>(sequence++);
                builder.AddAttribute(sequence++, nameof(CarouselItem.ChildContent), (RenderFragment)(contentBuilder =>
                    contentBuilder.AddContent(0, item)));
                builder.CloseComponent();
            }
        };
    }
}
