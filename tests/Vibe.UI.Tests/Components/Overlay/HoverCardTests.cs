namespace Vibe.UI.Tests.Components.Overlay;

public class HoverCardTests : TestBase
{
    [Fact]
    public void HoverCard_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<HoverCard>();

        // Assert
        var hoverCard = cut.Find(".vibe-hovercard");
        hoverCard.ShouldNotBeNull();
    }

    [Fact]
    public void HoverCard_Renders_Trigger()
    {
        // Act
        var cut = RenderComponent<HoverCard>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me")));

        // Assert
        var trigger = cut.Find(".hovercard-trigger");
        trigger.ShouldNotBeNull();
        trigger.TextContent.ShouldContain("Hover me");
    }

    [Fact]
    public void HoverCard_DoesNotShow_ContentInitially()
    {
        // Act
        var cut = RenderComponent<HoverCard>(parameters => parameters
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
        var cut = RenderComponent<HoverCard>(parameters => parameters
            .Add(p => p.Position, position)
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Assert
        var hoverCard = cut.Find(".vibe-hovercard");
        hoverCard.ShouldNotBeNull();
    }

    [Fact]
    public void HoverCard_Applies_DefaultPosition()
    {
        // Act
        var cut = RenderComponent<HoverCard>();

        // Assert
        var hoverCard = cut.Find(".vibe-hovercard");
        hoverCard.ShouldNotBeNull();
    }

    [Fact]
    public void HoverCard_HasDefaultOpenDelay()
    {
        // Act
        var cut = RenderComponent<HoverCard>();

        // Assert
        cut.Instance.OpenDelay.ShouldBe(300);
    }

    [Fact]
    public void HoverCard_HasDefaultCloseDelay()
    {
        // Act
        var cut = RenderComponent<HoverCard>();

        // Assert
        cut.Instance.CloseDelay.ShouldBe(200);
    }

    [Fact]
    public void HoverCard_Applies_CustomOpenDelay()
    {
        // Arrange
        var openDelay = 500;

        // Act
        var cut = RenderComponent<HoverCard>(parameters => parameters
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
        var cut = RenderComponent<HoverCard>(parameters => parameters
            .Add(p => p.CloseDelay, closeDelay));

        // Assert
        cut.Instance.CloseDelay.ShouldBe(closeDelay);
    }
}
