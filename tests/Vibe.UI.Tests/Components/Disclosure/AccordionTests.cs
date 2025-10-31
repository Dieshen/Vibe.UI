namespace Vibe.UI.Tests.Components.Disclosure;

public class AccordionTests : TestContext
{
    public AccordionTests() { this.AddVibeUIServices(); }

    [Fact]
    public void Accordion_RendersWithBasicStructure()
    {
        var cut = RenderComponent<Accordion>();
        cut.Find(".vibe-accordion").Should().NotBeNull();
    }

    [Fact]
    public void Accordion_HasSingleType_ByDefault()
    {
        var cut = RenderComponent<Accordion>();
        cut.Instance.Type.Should().Be(Accordion.AccordionType.Single);
    }

    [Fact]
    public void Accordion_SupportsMultipleType()
    {
        var cut = RenderComponent<Accordion>(parameters => parameters
            .Add(p => p.Type, Accordion.AccordionType.Multiple));
        cut.Instance.Type.Should().Be(Accordion.AccordionType.Multiple);
    }

    [Fact]
    public void Accordion_CollapsesOthers_ByDefault()
    {
        var cut = RenderComponent<Accordion>();
        cut.Instance.CollapseOthers.Should().BeTrue();
    }

    [Fact]
    public void Accordion_AppliesCustomCssClass()
    {
        var cut = RenderComponent<Accordion>(parameters => parameters.Add(p => p.CssClass, "custom-accordion"));
        cut.Find(".vibe-accordion").ClassList.Should().Contain("custom-accordion");
    }
}
