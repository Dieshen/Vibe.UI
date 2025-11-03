namespace Vibe.UI.Tests.Components.Overlay;

public class ContextMenuItemTests : TestBase
{
    [Fact]
    public void ContextMenuItem_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<ContextMenuItem>();

        // Assert
        var menuItem = cut.Find(".vibe-context-menu-item");
        menuItem.ShouldNotBeNull();
    }

    [Fact]
    public void ContextMenuItem_Displays_ChildContent()
    {
        // Arrange
        var content = "Menu Item";

        // Act
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var itemContent = cut.Find(".context-item-content");
        itemContent.TextContent.ShouldBe(content);
    }

    [Fact]
    public void ContextMenuItem_Displays_Icon_WhenProvided()
    {
        // Arrange
        var icon = "<svg>icon</svg>";

        // Act
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.Icon, icon)
            .Add(p => p.ChildContent, builder => builder.AddContent(0, "Item")));

        // Assert
        var iconElement = cut.Find(".context-item-icon");
        iconElement.ShouldNotBeNull();
    }

    [Fact]
    public void ContextMenuItem_Displays_Shortcut_WhenProvided()
    {
        // Arrange
        var shortcut = "Ctrl+S";

        // Act
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.Shortcut, shortcut)
            .Add(p => p.ChildContent, builder => builder.AddContent(0, "Save")));

        // Assert
        var shortcutElement = cut.Find(".context-item-shortcut");
        shortcutElement.TextContent.ShouldBe(shortcut);
    }

    [Fact]
    public void ContextMenuItem_Applies_DisabledClass_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var menuItem = cut.Find(".vibe-context-menu-item");
        menuItem.ClassList.ShouldContain("disabled");
    }

    [Fact]
    public void ContextMenuItem_DoesNotApply_DisabledClass_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.Disabled, false));

        // Assert
        var menuItem = cut.Find(".vibe-context-menu-item");
        menuItem.ClassList.ShouldNotContain("disabled");
    }

    [Fact]
    public void ContextMenuItem_InvokesOnItemClick_WhenClicked()
    {
        // Arrange
        var clicked = false;
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.OnItemClick, args => clicked = true));

        // Act
        var menuItem = cut.Find(".vibe-context-menu-item");
        menuItem.Click();

        // Assert
        clicked.ShouldBeTrue();
    }

    [Fact]
    public void ContextMenuItem_DoesNotInvokeOnItemClick_WhenDisabled()
    {
        // Arrange
        var clicked = false;
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnItemClick, args => clicked = true));

        // Act
        var menuItem = cut.Find(".vibe-context-menu-item");
        menuItem.Click();

        // Assert
        clicked.ShouldBeFalse();
    }

    [Fact]
    public void ContextMenuItem_HidesIcon_WhenNotProvided()
    {
        // Act
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, "Item")));

        // Assert
        cut.FindAll(".context-item-icon").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenuItem_HidesShortcut_WhenNotProvided()
    {
        // Act
        var cut = RenderComponent<ContextMenuItem>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, "Item")));

        // Assert
        cut.FindAll(".context-item-shortcut").ShouldBeEmpty();
    }
}
