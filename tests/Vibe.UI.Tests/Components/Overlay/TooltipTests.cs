namespace Vibe.UI.Tests.Components.Overlay;

public class TooltipTests : TestContext
{
    public TooltipTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Tooltip_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        cut.Find(".vibe-tooltip").Should().NotBeNull();
        cut.Find(".trigger").Should().NotBeNull();
        cut.Find(".tooltip-content").Should().NotBeNull();
    }

    [Fact]
    public void Tooltip_RendersTriggerContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Hover me")));

        // Assert
        cut.Find(".trigger").TextContent.Should().Be("Hover me");
    }

    [Fact]
    public void Tooltip_RendersContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Tooltip text")));

        // Assert
        cut.Find(".tooltip-content").TextContent.Should().Be("Tooltip text");
    }

    [Theory]
    [InlineData("top", "vibe-tooltip-top")]
    [InlineData("bottom", "vibe-tooltip-bottom")]
    [InlineData("left", "vibe-tooltip-left")]
    [InlineData("right", "vibe-tooltip-right")]
    public void Tooltip_AppliesCorrectPlacementClass(string placement, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.Placement, placement));

        // Assert
        cut.Find(".vibe-tooltip").ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void Tooltip_UsesTopPlacement_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Tooltip>();

        // Assert
        cut.Find(".vibe-tooltip").ClassList.Should().Contain("vibe-tooltip-top");
    }

    [Fact]
    public void Tooltip_HasDefaultDelay()
    {
        // Arrange & Act
        var cut = RenderComponent<Tooltip>();

        // Assert
        cut.Instance.DelayMS.Should().Be(200);
    }

    [Fact]
    public void Tooltip_AppliesCustomDelay()
    {
        // Arrange & Act
        var cut = RenderComponent<Tooltip>(parameters => parameters
            .Add(p => p.DelayMS, 500));

        // Assert
        cut.Instance.DelayMS.Should().Be(500);
    }
}
