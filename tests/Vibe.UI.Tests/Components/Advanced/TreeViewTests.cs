namespace Vibe.UI.Tests.Components.Advanced;

public class TreeViewTests : TestBase
{
    [Fact]
    public void TreeView_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<TreeView>();

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ShouldNotBeNull();
        tree.GetAttribute("role").ShouldBe("tree");
        tree.GetAttribute("aria-multiselectable").ShouldBe("false");
        cut.FindAll("[role='treeitem']").ShouldBeEmpty();
    }

    [Fact]
    public void TreeView_RendersRootItems_WithTreeitemMetadata()
    {
        // Arrange
        var items = new List<TreeView.TreeNode>
        {
            new() { Id = "1", Label = "Item 1" },
            new() { Id = "2", Label = "Item 2" }
        };

        // Act
        var cut = Render<TreeView>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.SelectedValue, "2"));

        // Assert
        var treeItems = cut.FindAll("[role='treeitem']");
        treeItems.Count.ShouldBe(2);
        treeItems[0].GetAttribute("aria-level").ShouldBe("1");
        treeItems[0].GetAttribute("aria-posinset").ShouldBe("1");
        treeItems[0].GetAttribute("aria-setsize").ShouldBe("2");
        treeItems[0].GetAttribute("aria-selected").ShouldBe("false");
        treeItems[0].HasAttribute("aria-expanded").ShouldBeFalse();
        treeItems[1].GetAttribute("aria-posinset").ShouldBe("2");
        treeItems[1].GetAttribute("aria-selected").ShouldBe("true");
        cut.Markup.ShouldContain("Item 1");
        cut.Markup.ShouldContain("Item 2");
    }

    [Fact]
    public void TreeView_InvokesCallbacks_AndUpdatesSingleSelection_WhenNodeClicked()
    {
        // Arrange
        string? selectedValue = null;
        TreeView.TreeNode? clickedNode = null;
        var items = new List<TreeView.TreeNode>
        {
            new() { Id = "1", Label = "Item 1" },
            new() { Id = "2", Label = "Item 2" }
        };

        var cut = Render<TreeView>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.SelectedValueChanged, EventCallback.Factory.Create<string?>(this, value => selectedValue = value))
            .Add(p => p.OnNodeClick, EventCallback.Factory.Create<TreeView.TreeNode>(this, value => clickedNode = value)));

        // Act
        cut.FindAll(".tree-node-content")[1].Click();

        // Assert
        selectedValue.ShouldBe("2");
        clickedNode.ShouldBeSameAs(items[1]);
        cut.FindAll("[role='treeitem']")[1].GetAttribute("aria-selected").ShouldBe("true");
    }

    [Fact]
    public void TreeView_TogglesMultiSelection_WhenMultiSelectIsEnabled()
    {
        // Arrange
        var items = new List<TreeView.TreeNode>
        {
            new() { Id = "1", Label = "Item 1" },
            new() { Id = "2", Label = "Item 2" }
        };

        var cut = Render<TreeView>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.MultiSelect, true));

        // Act
        cut.FindAll(".tree-node-content")[0].Click();
        cut.FindAll(".tree-node-content")[1].Click();

        // Assert
        cut.Find(".vibe-tree-view").GetAttribute("aria-multiselectable").ShouldBe("true");
        cut.FindAll("[role='treeitem']")[0].GetAttribute("aria-selected").ShouldBe("true");
        cut.FindAll("[role='treeitem']")[1].GetAttribute("aria-selected").ShouldBe("true");

        // Act
        cut.FindAll(".tree-node-content")[0].Click();

        // Assert
        cut.FindAll("[role='treeitem']")[0].GetAttribute("aria-selected").ShouldBe("false");
        cut.FindAll("[role='treeitem']")[1].GetAttribute("aria-selected").ShouldBe("true");
    }

    [Fact]
    public void TreeView_TogglesExpansion_AndAnnotatesNestedItems()
    {
        // Arrange
        TreeView.TreeNode? expandedNode = null;
        var items = new List<TreeView.TreeNode>
        {
            new()
            {
                Id = "parent",
                Label = "Parent",
                Children =
                [
                    new() { Id = "child-1", Label = "Child 1" },
                    new() { Id = "child-2", Label = "Child 2" }
                ]
            }
        };

        var cut = Render<TreeView>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.OnNodeExpand, EventCallback.Factory.Create<TreeView.TreeNode>(this, value => expandedNode = value)));

        // Assert
        cut.Find("[role='treeitem']").GetAttribute("aria-expanded").ShouldBe("false");
        cut.Find("[role='group']").HasAttribute("hidden").ShouldBeTrue();

        // Act
        cut.Find(".tree-node-toggle").Click();

        // Assert
        expandedNode.ShouldBeSameAs(items[0]);
        cut.Find("[role='treeitem']").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find("[role='group']").HasAttribute("hidden").ShouldBeFalse();

        var treeItems = cut.FindAll("[role='treeitem']");
        treeItems.Count.ShouldBe(3);
        treeItems[1].GetAttribute("aria-level").ShouldBe("2");
        treeItems[1].GetAttribute("aria-posinset").ShouldBe("1");
        treeItems[1].GetAttribute("aria-setsize").ShouldBe("2");
        treeItems[2].GetAttribute("aria-posinset").ShouldBe("2");

        // Act
        cut.Find(".tree-node-toggle").Click();

        // Assert
        cut.Find("[role='treeitem']").GetAttribute("aria-expanded").ShouldBe("false");
        cut.Find("[role='group']").HasAttribute("hidden").ShouldBeTrue();
    }

    [Fact]
    public void TreeView_DoesNotInvokeCallbacks_ForDisabledNodeInteractions()
    {
        // Arrange
        var clicked = false;
        var expanded = false;
        string? selectedValue = null;
        var items = new List<TreeView.TreeNode>
        {
            new()
            {
                Id = "disabled-parent",
                Label = "Disabled parent",
                IsDisabled = true,
                Children =
                [
                    new() { Id = "child", Label = "Child" }
                ]
            }
        };

        var cut = Render<TreeView>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.ShowCheckboxes, true)
            .Add(p => p.SelectedValueChanged, EventCallback.Factory.Create<string?>(this, value => selectedValue = value))
            .Add(p => p.OnNodeClick, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => clicked = true))
            .Add(p => p.OnNodeExpand, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => expanded = true)));

        // Act
        cut.Find(".tree-node-content").Click();
        cut.Find(".tree-node-toggle").Click();
        cut.Find("input.tree-node-checkbox").Change(true);

        // Assert
        clicked.ShouldBeFalse();
        expanded.ShouldBeFalse();
        selectedValue.ShouldBeNull();
        cut.Find("[role='treeitem']").GetAttribute("aria-disabled").ShouldBe("true");
        cut.Find("[role='treeitem']").GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".tree-node-toggle").HasAttribute("disabled").ShouldBeTrue();
        cut.Find("input.tree-node-checkbox").HasAttribute("disabled").ShouldBeTrue();
        cut.Find("[role='group']").HasAttribute("hidden").ShouldBeTrue();
    }

    [Fact]
    public void TreeView_Applies_CustomCssClass()
    {
        // Act
        var cut = Render<TreeView>(parameters => parameters
            .Add(p => p.CssClass, "custom-tree"));

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ClassList.ShouldContain("custom-tree");
    }

    [Fact]
    public void TreeView_ForwardsAdditionalAttributes_ToRoot()
    {
        // Act
        var cut = Render<TreeView>(parameters => parameters
            .AddUnmatched("data-testid", "tree-root"));

        // Assert
        cut.Find(".vibe-tree-view").GetAttribute("data-testid").ShouldBe("tree-root");
    }

    [Fact]
    public void TreeView_Handles_EmptyItems()
    {
        // Act
        var cut = Render<TreeView>(parameters => parameters
            .Add(p => p.Items, new List<TreeView.TreeNode>()));

        // Assert
        var tree = cut.Find(".vibe-tree-view");
        tree.ShouldNotBeNull();
        cut.FindAll("[role='treeitem']").ShouldBeEmpty();
    }
}
