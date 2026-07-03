namespace Vibe.UI.Tests.Components.Overlay;

public class ContextMenuItemTests : TestBase
{
    [Fact]
    public void ContextMenuItem_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<ContextMenuItem>();

        // Assert
        var menuItem = cut.Find(".vibe-context-menu-item");
        menuItem.ShouldNotBeNull();
        menuItem.GetAttribute("role").ShouldBe("menuitem");
        menuItem.GetAttribute("tabindex").ShouldBe("0");
        menuItem.GetAttribute("aria-disabled").ShouldBe("false");
    }

    [Fact]
    public void ContextMenuItem_Displays_ChildContent()
    {
        // Arrange
        var content = "Menu Item";

        // Act
        var cut = Render<ContextMenuItem>(parameters => parameters
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
        var cut = Render<ContextMenuItem>(parameters => parameters
            .Add(p => p.Icon, icon)
            .Add(p => p.ChildContent, builder => builder.AddContent(0, "Item")));

        // Assert
        var iconElement = cut.Find(".context-item-icon");
        iconElement.ShouldNotBeNull();
        iconElement.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void ContextMenuItem_Displays_Shortcut_WhenProvided()
    {
        // Arrange
        var shortcut = "Ctrl+S";

        // Act
        var cut = Render<ContextMenuItem>(parameters => parameters
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
        var cut = Render<ContextMenuItem>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var menuItem = cut.Find(".vibe-context-menu-item");
        menuItem.ClassList.ShouldContain("disabled");
        menuItem.GetAttribute("tabindex").ShouldBe("-1");
        menuItem.GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact]
    public void ContextMenuItem_DoesNotApply_DisabledClass_WhenEnabled()
    {
        // Act
        var cut = Render<ContextMenuItem>(parameters => parameters
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
        var cut = Render<ContextMenuItem>(parameters => parameters
            .Add(p => p.OnItemClick, args => clicked = true));

        // Act
        var menuItem = cut.Find(".vibe-context-menu-item");
        menuItem.Click();

        // Assert
        clicked.ShouldBeTrue();
    }

    [Fact]
    public void ContextMenuItem_InvokesOnItemClick_WhenActivatedByKeyboard()
    {
        // Arrange
        var clicked = false;
        var cut = Render<ContextMenuItem>(parameters => parameters
            .Add(p => p.OnItemClick, args => clicked = true));

        // Act
        cut.Find(".vibe-context-menu-item").KeyDown("Enter");

        // Assert
        clicked.ShouldBeTrue();
    }

    [Fact]
    public void ContextMenuItem_DoesNotInvokeOnItemClick_WhenDisabledAndActivatedByKeyboard()
    {
        // Arrange
        var clicked = false;
        var cut = Render<ContextMenuItem>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnItemClick, args => clicked = true));

        // Act
        cut.Find(".vibe-context-menu-item").KeyDown("Enter");
        cut.Find(".vibe-context-menu-item").KeyDown(" ");

        // Assert
        clicked.ShouldBeFalse();
    }

    [Fact]
    public void ContextMenuItem_DoesNotInvokeOnItemClick_WhenDisabled()
    {
        // Arrange
        var clicked = false;
        var cut = Render<ContextMenuItem>(parameters => parameters
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
        var cut = Render<ContextMenuItem>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, "Item")));

        // Assert
        cut.FindAll(".context-item-icon").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenuItem_HidesShortcut_WhenNotProvided()
    {
        // Act
        var cut = Render<ContextMenuItem>(parameters => parameters
            .Add(p => p.ChildContent, builder => builder.AddContent(0, "Item")));

        // Assert
        cut.FindAll(".context-item-shortcut").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenuItem_PreservesAdditionalAttributes()
    {
        // Act
        var cut = Render<ContextMenuItem>(parameters => parameters
            .AddUnmatched("data-testid", "context-menu-item")
            .AddUnmatched("aria-label", "Rename item"));

        // Assert
        var item = cut.Find(".vibe-context-menu-item");
        item.GetAttribute("data-testid").ShouldBe("context-menu-item");
        item.GetAttribute("aria-label").ShouldBe("Rename item");
    }
}
