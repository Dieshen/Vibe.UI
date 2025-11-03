namespace Vibe.UI.Tests.Components.Overlay;

public class ContextMenuTests : TestBase
{
    [Fact]
    public void ContextMenu_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<ContextMenu>();

        // Assert
        var contextMenu = cut.Find(".vibe-context-menu");
        contextMenu.ShouldNotBeNull();
    }

    [Fact]
    public void ContextMenu_Renders_Trigger()
    {
        // Act
        var cut = RenderComponent<ContextMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Right click me")));

        // Assert
        var trigger = cut.Find(".context-trigger");
        trigger.ShouldNotBeNull();
        trigger.TextContent.ShouldContain("Right click me");
    }

    [Fact]
    public void ContextMenu_DoesNotShow_ContentInitially()
    {
        // Act
        var cut = RenderComponent<ContextMenu>();

        // Assert
        cut.FindAll(".context-content").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenu_Renders_Content_WhenProvided()
    {
        // Act
        var cut = RenderComponent<ContextMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu Content")));

        // Assert
        var contextMenu = cut.Find(".vibe-context-menu");
        contextMenu.ShouldNotBeNull();
    }

    [Fact]
    public void ContextMenu_DoesNotOpen_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<ContextMenu>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        cut.FindAll(".context-content").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenu_HasCorrectCssClass()
    {
        // Act
        var cut = RenderComponent<ContextMenu>();

        // Assert
        var contextMenu = cut.Find(".vibe-context-menu");
        contextMenu.ClassList.ShouldContain("vibe-context-menu");
    }
}
