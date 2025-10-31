namespace Vibe.UI.Tests.Components.Disclosure;

public class AccordionItemTests : TestContext
{
    public AccordionItemTests() { this.AddVibeUIServices(); }
    [Fact] public void AccordionItem_Renders() { var cut = RenderComponent<AccordionItem>(); cut.Markup.Should().NotBeEmpty(); }
}
