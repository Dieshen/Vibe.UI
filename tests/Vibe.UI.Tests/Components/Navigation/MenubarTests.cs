namespace Vibe.UI.Tests.Components.Navigation;

using Vibe.UI.Components;
using static Vibe.UI.Components.Menubar;

public class MenubarTests : TestContext
{
    public MenubarTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Menubar_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Menubar>();

        // Assert
        cut.Find(".vibe-menubar").Should().NotBeNull();
        cut.Find("[role='menubar']").Should().NotBeNull();
    }

    [Fact]
    public void Menubar_RendersMenuItems()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "File menu") },
            new() { Id = "edit", Label = "Edit", Content = builder => builder.AddContent(0, "Edit menu") }
        };

        // Act
        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        // Assert
        var triggers = cut.FindAll(".menubar-trigger");
        triggers.Should().HaveCount(2);
        triggers[0].TextContent.Should().Be("File");
        triggers[1].TextContent.Should().Be("Edit");
    }

    [Fact]
    public void Menubar_OpensMenu_OnTriggerClick()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "File content") }
        };

        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        // Act
        cut.Find(".menubar-trigger").Click();

        // Assert
        cut.Find(".menubar-content").Should().NotBeNull();
        cut.Markup.Should().Contain("File content");
    }

    [Fact]
    public void Menubar_ClosesMenu_OnSecondClick()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "File content") }
        };

        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        // Act
        cut.Find(".menubar-trigger").Click(); // Open
        cut.Find(".menubar-trigger").Click(); // Close

        // Assert
        cut.FindAll(".menubar-content").Should().BeEmpty();
    }

    [Fact]
    public void Menubar_ShowsBackdrop_WhenMenuOpen()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "Content") }
        };

        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        // Act
        cut.Find(".menubar-trigger").Click();

        // Assert
        cut.Find(".menubar-backdrop").Should().NotBeNull();
    }

    [Fact]
    public void Menubar_ClosesMenu_OnBackdropClick()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "Content") }
        };

        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        cut.Find(".menubar-trigger").Click(); // Open

        // Act
        cut.Find(".menubar-backdrop").Click();

        // Assert
        cut.FindAll(".menubar-content").Should().BeEmpty();
    }

    [Fact]
    public void Menubar_AppliesAriaAttributes()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "Content") }
        };

        // Act
        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        // Assert
        var trigger = cut.Find(".menubar-trigger");
        trigger.GetAttribute("role").Should().Be("menuitem");
        trigger.GetAttribute("aria-haspopup").Should().Be("true");
        trigger.GetAttribute("aria-expanded").Should().Be("False");
    }

    [Fact]
    public void Menubar_UpdatesAriaExpanded_WhenMenuOpen()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "Content") }
        };

        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        // Act
        cut.Find(".menubar-trigger").Click();

        // Assert
        cut.Find(".menubar-trigger").GetAttribute("aria-expanded").Should().Be("True");
    }

    [Fact]
    public void Menubar_MarksActiveMenu()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "Content") }
        };

        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        // Act
        cut.Find(".menubar-trigger").Click();

        // Assert
        cut.Find(".menubar-trigger").ClassList.Should().Contain("active");
    }

    [Fact]
    public void Menubar_SwitchesMenus()
    {
        // Arrange
        var menus = new List<MenubarItem>
        {
            new() { Id = "file", Label = "File", Content = builder => builder.AddContent(0, "File content") },
            new() { Id = "edit", Label = "Edit", Content = builder => builder.AddContent(0, "Edit content") }
        };

        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.Menus, menus));

        // Act - Open File menu
        cut.FindAll(".menubar-trigger")[0].Click();
        cut.Markup.Should().Contain("File content");

        // Act - Open Edit menu
        cut.FindAll(".menubar-trigger")[1].Click();

        // Assert - Should now show Edit content, not File
        cut.Markup.Should().Contain("Edit content");
        cut.Markup.Should().NotContain("File content");
    }

    [Fact]
    public void Menubar_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Menubar>(parameters => parameters
            .Add(p => p.CssClass, "custom-menubar"));

        // Assert
        cut.Find(".vibe-menubar").ClassList.Should().Contain("custom-menubar");
    }
}
