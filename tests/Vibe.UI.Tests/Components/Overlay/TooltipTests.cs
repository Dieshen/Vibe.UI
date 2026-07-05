using Microsoft.AspNetCore.Components.Web;

namespace Vibe.UI.Tests.Components.Overlay;

public class TooltipTests : TestBase
{
    [Fact]
    public void Tooltip_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        var tooltip = cut.Find(".vibe-tooltip");
        tooltip.ShouldNotBeNull();
        tooltip.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact]
    public void Tooltip_Applies_PlacementClass()
    {
        // Act
        var cut = Render<Tooltip>(parameters => parameters
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
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        var trigger = cut.Find(".trigger");
        trigger.TextContent.ShouldContain("Hover me");
        trigger.GetAttribute("tabindex").ShouldBe("0");
        trigger.GetAttribute("aria-disabled").ShouldBe("false");
        trigger.GetAttribute("aria-describedby").ShouldStartWith("vibe-tooltip-");
        trigger.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact]
    public void Tooltip_Renders_TooltipContent()
    {
        // Act
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        var content = cut.Find(".tooltip-content");
        content.TextContent.ShouldContain("Tooltip text");
        content.GetAttribute("role").ShouldBe("tooltip");
        content.GetAttribute("aria-hidden").ShouldBe("true");
        content.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact]
    public void Tooltip_IsHidden_Initially()
    {
        // Act
        var cut = Render<Tooltip>(parameters => parameters
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
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        cut.Instance.DelayMS.ShouldBe(200);
    }

    [Fact]
    public void Tooltip_Accepts_CustomDelay()
    {
        // Act
        var cut = Render<Tooltip>(parameters => parameters
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
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text"))
            .AddUnmatched("data-test", "tooltip-value"));

        // Assert
        cut.Find(".vibe-tooltip").GetAttribute("data-test").ShouldBe("tooltip-value");
    }

    [Fact]
    public void Tooltip_FallsBackToTopPlacement_WhenPlacementIsInvalid()
    {
        // Act
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.Placement, "sideways")
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        var tooltip = cut.Find(".vibe-tooltip");
        tooltip.ClassList.ShouldContain("vibe-tooltip-top");
        tooltip.ClassList.ShouldNotContain("vibe-tooltip-sideways");
    }

    [Fact]
    public void Tooltip_ShowsAfterDelay_AndHidesOnMouseLeave()
    {
        // Arrange
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.DelayMS, 1)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Act
        cut.Find(".vibe-tooltip").TriggerEvent("onmouseenter", new MouseEventArgs());

        // Assert
        cut.WaitForAssertion(() =>
        {
            var content = cut.Find(".tooltip-content");
            content.ClassList.ShouldContain("visible");
            content.GetAttribute("aria-hidden").ShouldBe("false");
            cut.Find(".trigger").GetAttribute("data-state").ShouldBe("open");
        });

        // Act
        cut.Find(".vibe-tooltip").TriggerEvent("onmouseleave", new MouseEventArgs());

        // Assert
        cut.WaitForAssertion(() =>
        {
            var hiddenContent = cut.Find(".tooltip-content");
            hiddenContent.ClassList.ShouldNotContain("visible");
            hiddenContent.GetAttribute("aria-hidden").ShouldBe("true");
        });
    }

    [Fact]
    public void Tooltip_FocusShowsAndEscapeHides()
    {
        // Arrange
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.DelayMS, 1)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Act
        cut.Find(".vibe-tooltip").TriggerEvent("onfocusin", new FocusEventArgs());

        // Assert
        cut.WaitForAssertion(() => cut.Find(".tooltip-content").ClassList.ShouldContain("visible"));

        // Act
        cut.Find(".trigger").TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Escape" });

        // Assert
        cut.WaitForAssertion(() => cut.Find(".tooltip-content").ClassList.ShouldNotContain("visible"));
    }

    [Fact]
    public void Tooltip_DisabledTriggerDoesNotShow()
    {
        // Arrange
        var cut = Render<Tooltip>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.DelayMS, 1)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Act
        var trigger = cut.Find(".trigger");
        cut.Find(".vibe-tooltip").TriggerEvent("onmouseenter", new MouseEventArgs());

        // Assert
        trigger.GetAttribute("aria-disabled").ShouldBe("true");
        trigger.GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".tooltip-content").ClassList.ShouldNotContain("visible");
    }
}
