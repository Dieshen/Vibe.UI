namespace Vibe.UI.Tests.Components.DataDisplay;

public class TimelineTests : TestBase
{
    [Fact]
    public void Timeline_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Timeline>();

        // Assert
        cut.Find(".vibe-timeline").ShouldNotBeNull();
    }

    [Fact]
    public void Timeline_Has_DefaultLeftPosition()
    {
        // Act
        var cut = RenderComponent<Timeline>();

        // Assert
        cut.Find(".vibe-timeline").ClassList.ShouldContain("timeline-left");
    }

    [Fact]
    public void Timeline_Applies_PositionClass()
    {
        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Position, Timeline.TimelinePosition.Right));

        // Assert
        cut.Find(".vibe-timeline").ClassList.ShouldContain("timeline-right");
    }

    [Fact]
    public void Timeline_Renders_NoItems_WhenListIsNull()
    {
        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Items, (List<Timeline.TimelineItem>)null));

        // Assert
        cut.FindAll(".timeline-item").ShouldBeEmpty();
    }

    [Fact]
    public void Timeline_Renders_Items()
    {
        // Arrange
        var items = new List<Timeline.TimelineItem>
        {
            new() { Title = "Event 1", Description = "Description 1" },
            new() { Title = "Event 2", Description = "Description 2" }
        };

        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var timelineItems = cut.FindAll(".timeline-item");
        timelineItems.Count.ShouldBe(2);
    }

    [Fact]
    public void Timeline_Renders_ItemTitles()
    {
        // Arrange
        var items = new List<Timeline.TimelineItem>
        {
            new() { Title = "Event Title" }
        };

        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var title = cut.Find(".timeline-title");
        title.TextContent.ShouldBe("Event Title");
    }

    [Fact]
    public void Timeline_Renders_ItemDescriptions()
    {
        // Arrange
        var items = new List<Timeline.TimelineItem>
        {
            new() { Title = "Event", Description = "Event Description" }
        };

        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var description = cut.Find(".timeline-description");
        description.TextContent.ShouldBe("Event Description");
    }

    [Fact]
    public void Timeline_Renders_Marker()
    {
        // Arrange
        var items = new List<Timeline.TimelineItem>
        {
            new() { Title = "Event", Status = Timeline.TimelineStatus.Success }
        };

        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var marker = cut.Find(".timeline-marker");
        marker.ShouldNotBeNull();
        marker.ClassList.ShouldContain("marker-success");
    }

    [Fact]
    public void Timeline_Renders_Connector_BetweenItems()
    {
        // Arrange
        var items = new List<Timeline.TimelineItem>
        {
            new() { Title = "Event 1" },
            new() { Title = "Event 2" }
        };

        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var connectors = cut.FindAll(".timeline-connector");
        connectors.ShouldNotBeEmpty();
    }

    [Fact]
    public void Timeline_NoConnector_AfterLastItem()
    {
        // Arrange
        var items = new List<Timeline.TimelineItem>
        {
            new() { Title = "Event 1" },
            new() { Title = "Event 2" }
        };

        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var connectors = cut.FindAll(".timeline-connector");
        connectors.Count.ShouldBe(1); // Only 1 connector between 2 items
    }

    [Fact]
    public void Timeline_Renders_Timestamp_WhenProvided()
    {
        // Arrange
        var items = new List<Timeline.TimelineItem>
        {
            new() { Title = "Event", Timestamp = System.DateTime.Now }
        };

        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var timestamp = cut.Find(".timeline-time");
        timestamp.ShouldNotBeNull();
        timestamp.HasAttribute("datetime").ShouldBeTrue();
    }

    [Fact]
    public void Timeline_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<Timeline>(parameters => parameters
            .Add(p => p.CssClass, "custom-timeline"));

        // Assert
        cut.Find(".vibe-timeline").ClassList.ShouldContain("custom-timeline");
    }
}
