namespace Vibe.UI.Tests.Components.Feedback;

public class EmptyStateTests : TestBase
{
    [Fact]
    public void EmptyState_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<EmptyState>();

        // Assert
        var emptyState = cut.Find(".vibe-empty-state");
        emptyState.ShouldNotBeNull();
    }

    [Fact]
    public void EmptyState_Displays_Title()
    {
        // Arrange
        var title = "No results found";

        // Act
        var cut = RenderComponent<EmptyState>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert
        var titleElement = cut.Find(".empty-state-title");
        titleElement.TextContent.ShouldBe(title);
    }

    [Fact]
    public void EmptyState_Displays_Description()
    {
        // Arrange
        var description = "Try adjusting your search or filter criteria";

        // Act
        var cut = RenderComponent<EmptyState>(parameters => parameters
            .Add(p => p.Description, description));

        // Assert
        var descElement = cut.Find(".empty-state-description");
        descElement.TextContent.ShouldBe(description);
    }

    [Fact]
    public void EmptyState_Displays_Icon()
    {
        // Act
        var cut = RenderComponent<EmptyState>(parameters => parameters
            .Add(p => p.Icon, builder => builder.AddContent(0, "📭")));

        // Assert
        var iconElement = cut.Find(".empty-state-icon");
        iconElement.ShouldNotBeNull();
    }

    [Fact]
    public void EmptyState_Displays_ChildContent()
    {
        // Arrange
        var content = "Custom content";

        // Act
        var cut = RenderComponent<EmptyState>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var body = cut.Find(".empty-state-body");
        body.TextContent.ShouldContain(content);
    }

    [Fact]
    public void EmptyState_Displays_Action()
    {
        // Act
        var cut = RenderComponent<EmptyState>(parameters => parameters
            .Add(p => p.Action, builder => builder.AddContent(0, "Add Item")));

        // Assert
        var actionElement = cut.Find(".empty-state-action");
        actionElement.ShouldNotBeNull();
    }

    [Fact]
    public void EmptyState_HidesTitle_WhenNotProvided()
    {
        // Act
        var cut = RenderComponent<EmptyState>();

        // Assert
        cut.FindAll(".empty-state-title").ShouldBeEmpty();
    }

    [Fact]
    public void EmptyState_HidesDescription_WhenNotProvided()
    {
        // Act
        var cut = RenderComponent<EmptyState>();

        // Assert
        cut.FindAll(".empty-state-description").ShouldBeEmpty();
    }

    [Fact]
    public void EmptyState_HidesIcon_WhenNotProvided()
    {
        // Act
        var cut = RenderComponent<EmptyState>();

        // Assert
        cut.FindAll(".empty-state-icon").ShouldBeEmpty();
    }

    [Fact]
    public void EmptyState_HidesAction_WhenNotProvided()
    {
        // Act
        var cut = RenderComponent<EmptyState>();

        // Assert
        cut.FindAll(".empty-state-action").ShouldBeEmpty();
    }
}
