namespace Vibe.UI.Tests.Components.Disclosure;

public class AccordionTests : TestBase
{
    [Fact]
    public void Accordion_RendersBaseClassAndChildContent()
    {
        var cut = Render<Accordion>(parameters => parameters
            .AddChildContent("<div class='accordion-child'>Child</div>"));

        var accordion = cut.Find(".vibe-accordion");
        accordion.InnerHtml.ShouldContain("accordion-child");
    }

    [Fact]
    public void Accordion_RendersWithoutChildContent()
    {
        var cut = Render<Accordion>();

        var accordion = cut.Find(".vibe-accordion");
        accordion.InnerHtml.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Accordion_PreservesCustomClassAndAttributes()
    {
        var cut = Render<Accordion>(parameters => parameters
            .Add(p => p.Class, "settings-accordion")
            .AddUnmatched("data-accordion", "settings"));

        var accordion = cut.Find(".vibe-accordion");
        accordion.ClassList.ShouldContain("settings-accordion");
        accordion.GetAttribute("data-accordion").ShouldBe("settings");
    }

    [Fact]
    public void Accordion_SingleModeCollapsesOtherItems()
    {
        var cut = Render<Accordion>(parameters => parameters
            .AddChildContent(BuildAccordionItems(
                ("one", "One", "One content"),
                ("two", "Two", "Two content"))));

        var triggers = cut.FindAll(".vibe-accordion-item-trigger");
        triggers[0].Click();
        triggers[1].Click();

        var items = cut.FindAll(".vibe-accordion-item");
        items[0].ClassList.ShouldNotContain("vibe-accordion-item-expanded");
        items[1].ClassList.ShouldContain("vibe-accordion-item-expanded");
    }

    [Fact]
    public void Accordion_MultipleModeKeepsExpandedItems()
    {
        var cut = Render<Accordion>(parameters => parameters
            .Add(p => p.Type, Accordion.AccordionType.Multiple)
            .AddChildContent(BuildAccordionItems(
                ("one", "One", "One content"),
                ("two", "Two", "Two content"))));

        foreach (var trigger in cut.FindAll(".vibe-accordion-item-trigger"))
        {
            trigger.Click();
        }

        cut.FindAll(".vibe-accordion-item-expanded").Count.ShouldBe(2);
    }

    [Fact]
    public void Accordion_SingleModeCanKeepItemsExpanded_WhenCollapseOthersDisabled()
    {
        var cut = Render<Accordion>(parameters => parameters
            .Add(p => p.CollapseOthers, false)
            .AddChildContent(BuildAccordionItems(
                ("one", "One", "One content"),
                ("two", "Two", "Two content"))));

        foreach (var trigger in cut.FindAll(".vibe-accordion-item-trigger"))
        {
            trigger.Click();
        }

        cut.FindAll(".vibe-accordion-item-expanded").Count.ShouldBe(2);
    }

    [Fact]
    public void Accordion_InvalidTypeFallsBackToSingleMode()
    {
        var cut = Render<Accordion>(parameters => parameters
            .Add(p => p.Type, (Accordion.AccordionType)999)
            .AddChildContent(BuildAccordionItems(
                ("one", "One", "One content"),
                ("two", "Two", "Two content"))));

        var triggers = cut.FindAll(".vibe-accordion-item-trigger");
        triggers[0].Click();
        triggers[1].Click();

        cut.FindAll(".vibe-accordion-item-expanded").Count.ShouldBe(1);
        cut.FindAll(".vibe-accordion-item")[0].ClassList.ShouldNotContain("vibe-accordion-item-expanded");
        cut.FindAll(".vibe-accordion-item")[1].ClassList.ShouldContain("vibe-accordion-item-expanded");
    }

    [Fact]
    public void Accordion_RaisesStateChangedEvent_ForExpandAndCollapse()
    {
        var events = new List<Accordion.AccordionItemEventArgs>();
        var cut = Render<Accordion>(parameters => parameters
            .Add(p => p.OnItemStateChanged, EventCallback.Factory.Create<Accordion.AccordionItemEventArgs>(this, events.Add))
            .AddChildContent(BuildAccordionItems(("item-a", "Item A", "A content"))));

        var trigger = cut.Find(".vibe-accordion-item-trigger");
        trigger.Click();
        trigger.Click();

        events.Count.ShouldBe(2);
        events[0].ItemId.ShouldBe("item-a");
        events[0].IsExpanded.ShouldBeTrue();
        events[1].ItemId.ShouldBe("item-a");
        events[1].IsExpanded.ShouldBeFalse();
        cut.Find(".vibe-accordion-item").ClassList.ShouldNotContain("vibe-accordion-item-expanded");
    }

    private static RenderFragment BuildAccordionItems(params (string Id, string Header, string Content)[] items)
    {
        return builder =>
        {
            var sequence = 0;
            foreach (var item in items)
            {
                builder.OpenComponent<AccordionItem>(sequence++);
                builder.AddAttribute(sequence++, nameof(AccordionItem.Id), item.Id);
                builder.AddAttribute(sequence++, nameof(AccordionItem.Header), (RenderFragment)(header =>
                    header.AddContent(0, item.Header)));
                builder.AddAttribute(sequence++, nameof(AccordionItem.Content), (RenderFragment)(content =>
                    content.AddContent(0, item.Content)));
                builder.CloseComponent();
            }
        };
    }
}
