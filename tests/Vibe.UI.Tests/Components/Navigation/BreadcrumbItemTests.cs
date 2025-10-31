namespace Vibe.UI.Tests.Components.Navigation;

public class BreadcrumbItemTests : TestContext
{
    public BreadcrumbItemTests() { this.AddVibeUIServices(); }
    [Fact] public void BreadcrumbItem_Renders() { var cut = RenderComponent<BreadcrumbItem>(); cut.Markup.Should().NotBeEmpty(); }
}
