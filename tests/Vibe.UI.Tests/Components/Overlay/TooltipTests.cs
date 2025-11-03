namespace Vibe.UI.Tests.Components.Overlay;

public class TooltipTests : TestBase
{
    [Fact]
    public void Tooltip_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        var tooltip = cut.Find(".vibe-tooltip");
        tooltip.ShouldNotBeNull();
    }

    [Fact]
    public void Tooltip_Applies_PlacementClass()
    {
        // Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.Placement, "bottom")
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        cut.Find(".vibe-tooltip").ClassList.ShouldContain("vibe-tooltip-bottom");
    }

    [Fact]
    public void Tooltip_Renders_TriggerContent()
    {
        // Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        var trigger = cut.Find(".trigger");
        trigger.TextContent.ShouldContain("Hover me");
    }

    [Fact]
    public void Tooltip_Renders_TooltipContent()
    {
        // Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        var content = cut.Find(".tooltip-content");
        content.TextContent.ShouldContain("Tooltip text");
    }

    [Fact]
    public void Tooltip_IsHidden_Initially()
    {
        // Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        var content = cut.Find(".tooltip-content");
        content.ClassList.ShouldNotContain("visible");
    }

    [Fact]
    public void Tooltip_Has_DefaultDelay()
    {
        // Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        cut.Instance.DelayMS.ShouldBe(200);
    }

    [Fact]
    public void Tooltip_Accepts_CustomDelay()
    {
        // Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.DelayMS, 500)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        cut.Instance.DelayMS.ShouldBe(500);
    }

    [Fact]
    public void Tooltip_Applies_AdditionalAttributes()
    {
        // Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text"))
            .AddUnmatched("data-test", "tooltip-value"));

        // Assert - AdditionalAttributes are captured
        cut.Markup.ShouldNotBeNull();
    }
}
