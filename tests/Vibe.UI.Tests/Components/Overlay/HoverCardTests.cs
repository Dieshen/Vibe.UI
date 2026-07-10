using Microsoft.AspNetCore.Components.Web;

namespace Vibe.UI.Tests.Components.Overlay;

public class HoverCardTests : TestBase
{
    [Fact]
    public void HoverCard_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<HoverCard>();

        // Assert
        var hoverCard = cut.Find(".vibe-hovercard");
        hoverCard.ShouldNotBeNull();
    }

    [Fact]
    public void HoverCard_Renders_Trigger()
    {
        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me")));

        // Assert
        var trigger = cut.Find(".hovercard-trigger");
        trigger.ShouldNotBeNull();
        trigger.TextContent.ShouldContain("Hover me");
        trigger.GetAttribute("role").ShouldBe("button");
        trigger.GetAttribute("tabindex").ShouldBe("0");
        trigger.GetAttribute("aria-haspopup").ShouldBe("dialog");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-disabled").ShouldBe("false");
    }

    [Fact]
    public void HoverCard_DoesNotShow_ContentInitially()
    {
        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Assert
        cut.FindAll(".hovercard-content").ShouldBeEmpty();
    }

    [Fact]
    public void HoverCard_Applies_PositionClass()
    {
        // Arrange
        var position = "top";

        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.Position, position)
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Assert
        var hoverCard = cut.Find(".vibe-hovercard");
        hoverCard.ShouldNotBeNull();
    }

    [Fact]
    public void HoverCard_FallsBackToDefaultPosition_WhenPositionIsInvalid()
    {
        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Position, "sideways")
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Assert
        var content = cut.Find(".hovercard-content");
        content.ClassList.ShouldContain("hovercard-bottom");
        content.ClassList.ShouldNotContain("hovercard-sideways");
    }

    [Fact]
    public void HoverCard_Applies_DefaultPosition()
    {
        // Act
        var cut = Render<HoverCard>();

        // Assert
        var hoverCard = cut.Find(".vibe-hovercard");
        hoverCard.ShouldNotBeNull();
    }

    [Fact]
    public void HoverCard_HasDefaultOpenDelay()
    {
        // Act
        var cut = Render<HoverCard>();

        // Assert
        cut.Instance.OpenDelay.ShouldBe(300);
    }

    [Fact]
    public void HoverCard_HasDefaultCloseDelay()
    {
        // Act
        var cut = Render<HoverCard>();

        // Assert
        cut.Instance.CloseDelay.ShouldBe(200);
    }

    [Fact]
    public void HoverCard_Applies_CustomOpenDelay()
    {
        // Arrange
        var openDelay = 500;

        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.OpenDelay, openDelay));

        // Assert
        cut.Instance.OpenDelay.ShouldBe(openDelay);
    }

    [Fact]
    public void HoverCard_Applies_CustomCloseDelay()
    {
        // Arrange
        var closeDelay = 100;

        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.CloseDelay, closeDelay));

        // Assert
        cut.Instance.CloseDelay.ShouldBe(closeDelay);
    }

    [Fact]
    public void HoverCard_PreservesAdditionalAttributes_OnRoot()
    {
        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.Class, "profile-preview")
            .AddUnmatched("data-testid", "hovercard-root"));

        // Assert
        var root = cut.Find(".vibe-hovercard");
        root.ClassList.ShouldContain("profile-preview");
        root.GetAttribute("data-testid").ShouldBe("hovercard-root");
    }

    [Fact]
    public void HoverCard_RendersControlledOpenState()
    {
        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.AriaLabel, "Profile preview")
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Assert
        var trigger = cut.Find(".hovercard-trigger");
        var content = cut.Find(".hovercard-content");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
        content.GetAttribute("role").ShouldBe("dialog");
        content.GetAttribute("aria-label").ShouldBe("Profile preview");
        content.GetAttribute("data-state").ShouldBe("open");
        cut.Find(".vibe-hovercard").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact]
    public void HoverCard_DisabledDoesNotOverrideControlledOpenState()
    {
        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Assert
        cut.Find(".hovercard-trigger").GetAttribute("aria-disabled").ShouldBe("true");
        cut.Find(".hovercard-content").TextContent.ShouldContain("Content");
    }

    [Fact]
    public void HoverCard_AppliesDialogLabellingAttributes()
    {
        // Act
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.AriaLabel, "Ignored label")
            .Add(p => p.AriaLabelledBy, "hovercard-title")
            .Add(p => p.AriaDescribedBy, "hovercard-description")
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Assert
        var content = cut.Find(".hovercard-content");
        content.GetAttribute("aria-labelledby").ShouldBe("hovercard-title");
        content.GetAttribute("aria-describedby").ShouldBe("hovercard-description");
        content.GetAttribute("aria-label").ShouldBeNull();
    }

    [Fact]
    public void HoverCard_OpensAndClosesAfterConfiguredDelays()
    {
        // Arrange
        var changedValues = new List<bool>();
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.OpenDelay, 1)
            .Add(p => p.CloseDelay, 1)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Content"))
            .Add(p => p.IsOpenChanged, changedValues.Add));

        // Act
        cut.Find(".vibe-hovercard").TriggerEvent("onmouseenter", new MouseEventArgs());

        // Assert
        cut.WaitForAssertion(() => cut.Find(".hovercard-content").TextContent.ShouldContain("Content"));
        changedValues.ShouldBe([true]);

        // Act
        cut.Find(".vibe-hovercard").TriggerEvent("onmouseleave", new MouseEventArgs());

        // Assert
        cut.WaitForAssertion(() => cut.FindAll(".hovercard-content").ShouldBeEmpty());
        changedValues.ShouldBe([true, false]);
    }

    [Fact]
    public void HoverCard_FocusOpensAndEscapeCloses()
    {
        // Arrange
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.OpenDelay, 1)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Act
        cut.Find(".vibe-hovercard").TriggerEvent("onfocusin", new FocusEventArgs());

        // Assert
        cut.WaitForAssertion(() => cut.Find(".hovercard-content").ShouldNotBeNull());

        // Act
        cut.Find(".hovercard-trigger").TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Escape" });

        // Assert
        cut.WaitForAssertion(() => cut.FindAll(".hovercard-content").ShouldBeEmpty());
    }

    [Fact]
    public void HoverCard_DisabledTriggerDoesNotOpen()
    {
        // Arrange
        var changed = false;
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OpenDelay, 1)
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Content"))
            .Add(p => p.IsOpenChanged, _ => changed = true));

        // Act
        var trigger = cut.Find(".hovercard-trigger");
        cut.Find(".vibe-hovercard").TriggerEvent("onmouseenter", new MouseEventArgs());
        trigger.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        // Assert
        trigger.GetAttribute("aria-disabled").ShouldBe("true");
        trigger.GetAttribute("tabindex").ShouldBe("-1");
        changed.ShouldBeFalse();
        cut.FindAll(".hovercard-content").ShouldBeEmpty();
    }
}
