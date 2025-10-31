namespace Vibe.UI.Tests.Components.Disclosure;

public class CarouselItemTests : TestContext
{
    public CarouselItemTests() { this.AddVibeUIServices(); }
    [Fact] public void CarouselItem_Renders() { var cut = RenderComponent<CarouselItem>(); cut.Markup.Should().NotBeEmpty(); }
}
