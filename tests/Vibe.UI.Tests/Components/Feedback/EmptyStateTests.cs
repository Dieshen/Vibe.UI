namespace Vibe.UI.Tests.Components.Feedback;

public class EmptyStateTests : TestBase
{
    [Fact]
    public void EmptyState_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<EmptyState>();

        // Assert
        var emptyState = cut.Find(".vibe-empty-state");
        emptyState.ShouldNotBeNull();
        emptyState.GetAttribute("data-state").ShouldBe("empty");
    }

    [Fact]
    public void EmptyState_RendersAccessibleStatusByDefault()
    {
        // Act
        var cut = Render<EmptyState>();

        // Assert
        var emptyState = cut.Find(".vibe-empty-state");
        emptyState.GetAttribute("role").ShouldBe("status");
        emptyState.GetAttribute("aria-live").ShouldBe("polite");
        emptyState.GetAttribute("aria-atomic").ShouldBe("true");
        emptyState.GetAttribute("aria-label").ShouldBe("No content");
    }

    [Fact]
    public void EmptyState_UsesCustomAccessibilityAttributesAndNormalizesInvalidValues()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.Title, "No results")
            .Add(p => p.Role, "region")
            .Add(p => p.AriaLive, "assertive")
            .Add(p => p.AriaAtomic, false)
            .Add(p => p.AriaLabel, "Empty search results"));

        // Assert
        var emptyState = cut.Find(".vibe-empty-state");
        emptyState.GetAttribute("role").ShouldBe("region");
        emptyState.GetAttribute("aria-live").ShouldBe("assertive");
        emptyState.GetAttribute("aria-atomic").ShouldBe("false");
        emptyState.GetAttribute("aria-label").ShouldBe("Empty search results");

        // Act
        cut.Render(parameters => parameters
            .Add(p => p.Role, "button")
            .Add(p => p.AriaLive, "loud"));

        // Assert
        emptyState = cut.Find(".vibe-empty-state");
        emptyState.GetAttribute("role").ShouldBe("status");
        emptyState.GetAttribute("aria-live").ShouldBe("polite");
    }

    [Fact]
    public void EmptyState_Displays_Title()
    {
        // Arrange
        var title = "No results found";

        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert
        var titleElement = cut.Find(".empty-state-title");
        titleElement.TextContent.ShouldBe(title);
    }

    [Fact]
    public void EmptyState_TrimsTitleAndDescription_AndOmitsWhitespaceDescription()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.Title, "  No items  ")
            .Add(p => p.Description, "   "));

        // Assert
        cut.Find(".empty-state-title").TextContent.ShouldBe("No items");
        cut.FindAll(".empty-state-description").ShouldBeEmpty();
        cut.Find(".vibe-empty-state").HasAttribute("aria-label").ShouldBeFalse();
    }

    [Fact]
    public void EmptyState_Displays_Description()
    {
        // Arrange
        var description = "Try adjusting your search or filter criteria";

        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.Description, description));

        // Assert
        var descElement = cut.Find(".empty-state-description");
        descElement.TextContent.ShouldBe(description);
    }

    [Fact]
    public void EmptyState_Displays_Icon()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.Icon, builder => builder.AddContent(0, "📭")));

        // Assert
        var iconElement = cut.Find(".empty-state-icon");
        iconElement.ShouldNotBeNull();
        iconElement.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void EmptyState_CanExposeIconToAssistiveTechnology()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.IconAriaHidden, false)
            .Add(p => p.Icon, builder => builder.AddContent(0, "!")));

        // Assert
        cut.Find(".empty-state-icon").GetAttribute("aria-hidden").ShouldBe("false");
    }

    [Fact]
    public void EmptyState_Displays_ChildContent()
    {
        // Arrange
        var content = "Custom content";

        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var body = cut.Find(".empty-state-body");
        body.TextContent.ShouldContain(content);
    }

    [Fact]
    public void EmptyState_Displays_Action()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.Action, builder => builder.AddContent(0, "Add Item")));

        // Assert
        var actionElement = cut.Find(".empty-state-action");
        actionElement.ShouldNotBeNull();
    }

    [Fact]
    public void EmptyState_HidesTitle_WhenNotProvided()
    {
        // Act
        var cut = Render<EmptyState>();

        // Assert
        cut.FindAll(".empty-state-title").ShouldBeEmpty();
    }

    [Fact]
    public void EmptyState_HidesDescription_WhenNotProvided()
    {
        // Act
        var cut = Render<EmptyState>();

        // Assert
        cut.FindAll(".empty-state-description").ShouldBeEmpty();
    }

    [Fact]
    public void EmptyState_HidesIcon_WhenNotProvided()
    {
        // Act
        var cut = Render<EmptyState>();

        // Assert
        cut.FindAll(".empty-state-icon").ShouldBeEmpty();
    }

    [Fact]
    public void EmptyState_HidesAction_WhenNotProvided()
    {
        // Act
        var cut = Render<EmptyState>();

        // Assert
        cut.FindAll(".empty-state-action").ShouldBeEmpty();
    }

    [Fact]
    public void EmptyState_HidesTitleAndDescription_WhenWhitespace()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.Title, "   ")
            .Add(p => p.Description, "\t"));

        // Assert
        cut.FindAll(".empty-state-title").ShouldBeEmpty();
        cut.FindAll(".empty-state-description").ShouldBeEmpty();
        cut.Find(".vibe-empty-state").GetAttribute("aria-label").ShouldBe("No content");
    }

    [Fact]
    public void EmptyState_AppliesCustomClassesAndAdditionalAttributes()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters
            .Add(p => p.Class, "empty-state-shell")
            .Add(p => p.CssClass, "legacy-empty-class")
            .AddUnmatched("data-testid", "empty-state-root"));

        // Assert
        var emptyState = cut.Find(".vibe-empty-state");
        emptyState.ClassList.ShouldContain("empty-state-shell");
        emptyState.ClassList.ShouldContain("legacy-empty-class");
        emptyState.GetAttribute("data-testid").ShouldBe("empty-state-root");
    }
}
