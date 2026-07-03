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
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Id", "one");
                builder.AddAttribute(2, "Header", (RenderFragment)(header => header.AddContent(0, "One")));
                builder.AddAttribute(3, "Content", (RenderFragment)(content => content.AddContent(0, "One content")));
                builder.CloseComponent();
                builder.OpenComponent<AccordionItem>(4);
                builder.AddAttribute(5, "Id", "two");
                builder.AddAttribute(6, "Header", (RenderFragment)(header => header.AddContent(0, "Two")));
                builder.AddAttribute(7, "Content", (RenderFragment)(content => content.AddContent(0, "Two content")));
                builder.CloseComponent();
            }));

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
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Id", "one");
                builder.AddAttribute(2, "Header", (RenderFragment)(header => header.AddContent(0, "One")));
                builder.AddAttribute(3, "Content", (RenderFragment)(content => content.AddContent(0, "One content")));
                builder.CloseComponent();
                builder.OpenComponent<AccordionItem>(4);
                builder.AddAttribute(5, "Id", "two");
                builder.AddAttribute(6, "Header", (RenderFragment)(header => header.AddContent(0, "Two")));
                builder.AddAttribute(7, "Content", (RenderFragment)(content => content.AddContent(0, "Two content")));
                builder.CloseComponent();
            }));

        foreach (var trigger in cut.FindAll(".vibe-accordion-item-trigger"))
        {
            trigger.Click();
        }

        cut.FindAll(".vibe-accordion-item-expanded").Count.ShouldBe(2);
    }

    [Fact]
    public void Accordion_RaisesStateChangedEvent()
    {
        Accordion.AccordionItemEventArgs? args = null;
        var cut = Render<Accordion>(parameters => parameters
            .Add(p => p.OnItemStateChanged, EventCallback.Factory.Create<Accordion.AccordionItemEventArgs>(this, value => args = value))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Id", "item-a");
                builder.AddAttribute(2, "Header", (RenderFragment)(header => header.AddContent(0, "Item A")));
                builder.AddAttribute(3, "Content", (RenderFragment)(content => content.AddContent(0, "A content")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-accordion-item-trigger").Click();

        args.ShouldNotBeNull();
        args.ItemId.ShouldBe("item-a");
        args.IsExpanded.ShouldBeTrue();
    }
}
