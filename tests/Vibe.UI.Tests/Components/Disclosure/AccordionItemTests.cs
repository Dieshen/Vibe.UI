namespace Vibe.UI.Tests.Components.Disclosure;

public class AccordionItemTests : TestBase
{
    [Fact]
    public void AccordionItem_RendersHeaderAndContent()
    {
        var cut = Render<Accordion>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Id", "general");
                builder.AddAttribute(2, "Header", (RenderFragment)(header => header.AddContent(0, "General")));
                builder.AddAttribute(3, "Content", (RenderFragment)(content => content.AddMarkupContent(0, "<p>General content</p>")));
                builder.CloseComponent();
            }));

        var item = cut.Find(".vibe-accordion-item");
        item.TextContent.ShouldContain("General");
        item.InnerHtml.ShouldContain("General content");
        var trigger = cut.Find(".vibe-accordion-item-trigger");
        var content = cut.Find(".vibe-accordion-item-content");

        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-controls").ShouldBe(content.GetAttribute("id"));
        trigger.GetAttribute("aria-disabled").ShouldBe("false");
        trigger.HasAttribute("disabled").ShouldBeFalse();
        content.GetAttribute("role").ShouldBe("region");
        content.GetAttribute("aria-labelledby").ShouldBe(trigger.GetAttribute("id"));
        content.HasAttribute("hidden").ShouldBeTrue();
    }

    [Fact]
    public void AccordionItem_TogglesExpandedState()
    {
        var cut = Render<Accordion>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Id", "details");
                builder.AddAttribute(2, "Header", (RenderFragment)(header => header.AddContent(0, "Details")));
                builder.AddAttribute(3, "Content", (RenderFragment)(content => content.AddContent(0, "Details content")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-accordion-item-trigger").Click();

        var item = cut.Find(".vibe-accordion-item");
        item.ClassList.ShouldContain("vibe-accordion-item-expanded");
        cut.Find(".vibe-accordion-item-trigger").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find(".vibe-accordion-item-content").GetAttribute("style").ShouldBeEmpty();
        cut.Find(".vibe-accordion-item-content").HasAttribute("hidden").ShouldBeFalse();
    }

    [Fact]
    public void AccordionItem_DisabledSuppressesToggle()
    {
        var cut = Render<Accordion>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Id", "locked");
                builder.AddAttribute(2, "Disabled", true);
                builder.AddAttribute(3, "Header", (RenderFragment)(header => header.AddContent(0, "Locked")));
                builder.AddAttribute(4, "Content", (RenderFragment)(content => content.AddContent(0, "Locked content")));
                builder.CloseComponent();
            }));

        cut.Find(".vibe-accordion-item-trigger").Click();

        var item = cut.Find(".vibe-accordion-item");
        item.ClassList.ShouldContain("vibe-accordion-item-disabled");
        item.ClassList.ShouldNotContain("vibe-accordion-item-expanded");
        var trigger = cut.Find(".vibe-accordion-item-trigger");
        trigger.HasAttribute("disabled").ShouldBeTrue();
        trigger.GetAttribute("aria-disabled").ShouldBe("true");
        var content = cut.Find(".vibe-accordion-item-content");
        content.GetAttribute("style").ShouldBe("display: none;");
        content.HasAttribute("hidden").ShouldBeTrue();
    }

    [Fact]
    public void AccordionItem_ExpandedByDefaultStartsOpen()
    {
        var cut = Render<Accordion>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Id", "open");
                builder.AddAttribute(2, "ExpandedByDefault", true);
                builder.AddAttribute(3, "Header", (RenderFragment)(header => header.AddContent(0, "Open")));
                builder.AddAttribute(4, "Content", (RenderFragment)(content => content.AddContent(0, "Open content")));
                builder.CloseComponent();
            }));

        cut.WaitForAssertion(() => cut.Find(".vibe-accordion-item").ClassList.ShouldContain("vibe-accordion-item-expanded"));
    }

    [Fact]
    public void AccordionItem_PreservesCustomClassAndAttributes()
    {
        var cut = Render<Accordion>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Id", "custom");
                builder.AddAttribute(2, "Class", "custom-item");
                builder.AddAttribute(3, "data-item", "custom");
                builder.AddAttribute(4, "Header", (RenderFragment)(header => header.AddContent(0, "Custom")));
                builder.CloseComponent();
            }));

        var item = cut.Find(".vibe-accordion-item");
        item.ClassList.ShouldContain("custom-item");
        item.GetAttribute("data-item").ShouldBe("custom");
    }
}
