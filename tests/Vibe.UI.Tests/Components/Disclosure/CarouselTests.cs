namespace Vibe.UI.Tests.Components.Disclosure;

public class CarouselTests : TestContext
{
    public CarouselTests() { this.AddVibeUIServices(); }
    [Fact] public void Carousel_Renders() { var cut = RenderComponent<Carousel>(); cut.Markup.Should().NotBeEmpty(); }
}
