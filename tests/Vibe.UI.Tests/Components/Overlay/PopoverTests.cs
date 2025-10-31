namespace Vibe.UI.Tests.Components.Overlay;

public class PopoverTests : TestContext
{
    public PopoverTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Popover_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Click me"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        // Assert
        cut.Find(".vibe-popover").Should().NotBeNull();
        cut.Find(".popover-trigger").Should().NotBeNull();
    }

    [Fact]
    public void Popover_RendersTriggerContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Click me")));

        // Assert
        cut.Find(".popover-trigger").TextContent.Should().Be("Click me");
    }

    [Fact]
    public void Popover_DoesNotShowContent_Initially()
    {
        // Arrange & Act
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        // Assert
        cut.FindAll(".popover-content").Should().BeEmpty();
    }

    [Fact]
    public void Popover_ShowsContent_OnTriggerClick()
    {
        // Arrange
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Popover content")));

        // Act
        cut.Find(".popover-trigger").Click();

        // Assert
        cut.Find(".popover-content").Should().NotBeNull();
        cut.Markup.Should().Contain("Popover content");
    }

    [Fact]
    public void Popover_ShowsBackdrop_WhenOpen()
    {
        // Arrange
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Act
        cut.Find(".popover-trigger").Click();

        // Assert
        cut.Find(".popover-backdrop").Should().NotBeNull();
    }

    [Fact]
    public void Popover_ClosesOnBackdropClick_ByDefault()
    {
        // Arrange
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        cut.Find(".popover-trigger").Click();

        // Act
        cut.Find(".popover-backdrop").Click();

        // Assert
        cut.FindAll(".popover-content").Should().BeEmpty();
    }

    [Fact]
    public void Popover_AppliesPositionClass()
    {
        // Arrange
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Position, "top")
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Act
        cut.Find(".popover-trigger").Click();

        // Assert
        cut.Find(".popover-content").ClassList.Should().Contain("popover-top");
    }

    [Fact]
    public void Popover_AppliesAlignmentClass()
    {
        // Arrange
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Align, "start")
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Act
        cut.Find(".popover-trigger").Click();

        // Assert
        cut.Find(".popover-content").ClassList.Should().Contain("popover-start");
    }

    [Fact]
    public void Popover_UsesBottomPosition_ByDefault()
    {
        // Arrange
        var cut = RenderComponent<Popover>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Content")));

        // Act
        cut.Find(".popover-trigger").Click();

        // Assert
        cut.Find(".popover-content").ClassList.Should().Contain("popover-bottom");
    }
}
