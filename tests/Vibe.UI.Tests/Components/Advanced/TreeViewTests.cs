namespace Vibe.UI.Tests.Components.Advanced;

public class TreeViewTests : TestBase
{
    [Fact]
    public void TreeView_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<TreeView>();

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ShouldNotBeNull();
        tree.GetAttribute("role").ShouldBe("tree");
    }

    [Fact]
    public void TreeView_Renders_Items()
    {
        // Arrange
        var items = new List<TreeView.TreeNode>
        {
            new() { Id = "1", Label = "Item 1" },
            new() { Id = "2", Label = "Item 2" }
        };

        // Act
        var cut = RenderComponent<TreeView>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        cut.Markup.ShouldContain("Item 1");
        cut.Markup.ShouldContain("Item 2");
    }

    [Fact]
    public void TreeView_Renders_NestedItems()
    {
        // Arrange
        var items = new List<TreeView.TreeNode>
        {
            new()
            {
                Id = "1",
                Label = "Parent",
                Children = new()
                {
                    new() { Id = "1-1", Label = "Child 1" },
                    new() { Id = "1-2", Label = "Child 2" }
                }
            }
        };

        // Act
        var cut = RenderComponent<TreeView>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        cut.Markup.ShouldContain("Parent");
        cut.Markup.ShouldContain("Child 1");
        cut.Markup.ShouldContain("Child 2");
    }

    [Fact]
    public void TreeView_Supports_MultiSelect()
    {
        // Arrange
        var items = new List<TreeView.TreeNode>
        {
            new() { Id = "1", Label = "Item 1" },
            new() { Id = "2", Label = "Item 2" }
        };

        // Act
        var cut = RenderComponent<TreeView>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.MultiSelect, true));

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ShouldNotBeNull();
    }

    [Fact]
    public void TreeView_Shows_Checkboxes_WhenEnabled()
    {
        // Arrange
        var items = new List<TreeView.TreeNode>
        {
            new() { Id = "1", Label = "Item 1" }
        };

        // Act
        var cut = RenderComponent<TreeView>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.ShowCheckboxes, true));

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ShouldNotBeNull();
    }

    [Fact]
    public void TreeView_Handles_SelectedValue()
    {
        // Arrange
        var items = new List<TreeView.TreeNode>
        {
            new() { Id = "1", Label = "Item 1" },
            new() { Id = "2", Label = "Item 2" }
        };

        // Act
        var cut = RenderComponent<TreeView>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.SelectedValue, "1"));

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ShouldNotBeNull();
    }

    [Fact]
    public void TreeView_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<TreeView>(parameters => parameters
            .Add(p => p.CssClass, "custom-tree"));

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ClassList.ShouldContain("custom-tree");
    }

    [Fact]
    public void TreeView_Handles_EmptyItems()
    {
        // Act
        var cut = RenderComponent<TreeView>(parameters => parameters
            .Add(p => p.Items, new List<TreeView.TreeNode>()));

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ShouldNotBeNull();
    }
}
