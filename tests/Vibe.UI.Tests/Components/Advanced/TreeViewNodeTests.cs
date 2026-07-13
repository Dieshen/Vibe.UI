namespace Vibe.UI.Tests.Components.Advanced;

public class TreeViewNodeTests : TestBase
{
    [Fact]
    public void TreeViewNode_RendersNothing_WhenItemIsNull()
    {
        var cut = Render<TreeViewNode>();

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void TreeViewNode_RendersLeafNode()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, new TreeView.TreeNode { Id = "leaf", Label = "Leaf" })
            .Add(p => p.Level, 2)
            .Add(p => p.PositionInSet, 3)
            .Add(p => p.SetSize, 5));

        var node = cut.Find(".tree-view-node");
        node.GetAttribute("role").ShouldBe("treeitem");
        node.HasAttribute("aria-expanded").ShouldBeFalse();
        node.GetAttribute("aria-selected").ShouldBe("false");
        node.GetAttribute("aria-disabled").ShouldBe("false");
        node.GetAttribute("aria-level").ShouldBe("2");
        node.GetAttribute("aria-posinset").ShouldBe("3");
        node.GetAttribute("aria-setsize").ShouldBe("5");
        node.GetAttribute("tabindex").ShouldBe("0");
        cut.Find(".tree-node-label").TextContent.ShouldBe("Leaf");
        cut.Find(".tree-node-spacer").GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void TreeViewNode_RendersCollapsedChildren()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, CreateParentNode()));

        cut.Find(".tree-view-node").GetAttribute("aria-expanded").ShouldBe("false");
        cut.Find(".tree-node-toggle").GetAttribute("aria-label").ShouldBe("Expand Parent");
        cut.Find(".tree-node-toggle svg").GetAttribute("aria-hidden").ShouldBe("true");

        var group = cut.Find(".tree-node-children");
        group.GetAttribute("role").ShouldBe("group");
        group.ClassList.ShouldContain("collapsed");
        group.HasAttribute("hidden").ShouldBeTrue();
        group.HasAttribute("style").ShouldBeFalse();
    }

    [Fact]
    public void TreeViewNode_RendersExpandedChildren()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, CreateParentNode())
            .Add(p => p.ExpandedNodes, new HashSet<string> { "parent" }));

        cut.Find(".tree-view-node").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find(".tree-node-toggle").GetAttribute("aria-label").ShouldBe("Collapse Parent");
        cut.Find(".tree-node-toggle").ClassList.ShouldContain("expanded");
        cut.Find(".tree-node-children").ClassList.ShouldContain("expanded");
        cut.Find(".tree-node-children").HasAttribute("hidden").ShouldBeFalse();
    }

    [Fact]
    public void TreeViewNode_AnnotatesNestedChildren_WithLevelAndSetPosition()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, CreateParentNode())
            .Add(p => p.ExpandedNodes, new HashSet<string> { "parent" })
            .Add(p => p.Level, 3)
            .Add(p => p.PositionInSet, 2)
            .Add(p => p.SetSize, 4));

        var treeItems = cut.FindAll("[role='treeitem']");
        treeItems.Count.ShouldBe(3);
        treeItems[0].GetAttribute("aria-level").ShouldBe("3");
        treeItems[0].GetAttribute("aria-posinset").ShouldBe("2");
        treeItems[0].GetAttribute("aria-setsize").ShouldBe("4");
        treeItems[1].GetAttribute("aria-level").ShouldBe("4");
        treeItems[1].GetAttribute("aria-posinset").ShouldBe("1");
        treeItems[1].GetAttribute("aria-setsize").ShouldBe("2");
        treeItems[2].GetAttribute("aria-posinset").ShouldBe("2");
    }

    [Fact]
    public void TreeViewNode_TreatsEmptyChildren_AsLeaf()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, new TreeView.TreeNode
            {
                Id = "empty-parent",
                Label = "Empty parent",
                Children = new()
            }));

        var node = cut.Find("[role='treeitem']");
        node.HasAttribute("aria-expanded").ShouldBeFalse();
        cut.FindAll(".tree-node-toggle").ShouldBeEmpty();
        cut.FindAll("[role='group']").ShouldBeEmpty();
    }

    [Fact]
    public void TreeViewNode_MarksSelectedNode()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, new TreeView.TreeNode { Id = "leaf", Label = "Leaf" })
            .Add(p => p.SelectedValue, "leaf"));

        cut.Find(".tree-view-node").GetAttribute("aria-selected").ShouldBe("true");
        cut.Find(".tree-view-node").ClassList.ShouldContain("selected");
        cut.Find(".tree-node-content").ClassList.ShouldContain("selected");
        cut.Find(".tree-node-label").ClassList.ShouldContain("selected");
    }

    [Fact]
    public void TreeViewNode_InvokesClickForEnabledNode()
    {
        TreeView.TreeNode? clicked = null;
        var node = new TreeView.TreeNode { Id = "leaf", Label = "Leaf" };
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, node)
            .Add(p => p.OnNodeClick, EventCallback.Factory.Create<TreeView.TreeNode>(this, value => clicked = value)));

        cut.Find(".tree-node-content").Click();

        clicked.ShouldBeSameAs(node);
    }

    [Fact]
    public void TreeViewNode_InvokesClick_WhenActivatedWithKeyboard()
    {
        var clickCount = 0;
        var node = new TreeView.TreeNode { Id = "leaf", Label = "Leaf" };
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, node)
            .Add(p => p.OnNodeClick, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => clickCount++)));

        cut.Find(".tree-view-node").KeyDown("Enter");
        cut.Find(".tree-view-node").KeyDown(" ");

        clickCount.ShouldBe(2);
    }

    [Fact]
    public void TreeViewNode_ToggleInvokesExpandOnly()
    {
        TreeView.TreeNode? expanded = null;
        var clicked = false;
        var node = CreateParentNode();
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, node)
            .Add(p => p.OnNodeClick, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => clicked = true))
            .Add(p => p.OnNodeExpand, EventCallback.Factory.Create<TreeView.TreeNode>(this, value => expanded = value)));

        cut.Find(".tree-node-toggle").Click();

        expanded.ShouldBeSameAs(node);
        clicked.ShouldBeFalse();
    }

    [Fact]
    public void TreeViewNode_ArrowKeys_InvokeExpandForSupportedState()
    {
        var expandCount = 0;
        var node = CreateParentNode();
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, node)
            .Add(p => p.OnNodeExpand, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => expandCount++)));

        cut.Find(".tree-view-node").KeyDown("ArrowRight");
        cut.Find(".tree-view-node").KeyDown("ArrowLeft");

        expandCount.ShouldBe(1);

        cut.Render(parameters => parameters
            .Add(p => p.Item, node)
            .Add(p => p.ExpandedNodes, new HashSet<string> { "parent" })
            .Add(p => p.OnNodeExpand, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => expandCount++)));

        cut.Find(".tree-view-node").KeyDown("ArrowLeft");

        expandCount.ShouldBe(2);
    }

    [Fact]
    public void TreeViewNode_DoesNotInvokeCallbacksForDisabledNode()
    {
        var clicked = false;
        var expanded = false;
        var node = CreateParentNode(disabled: true);
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, node)
            .Add(p => p.ShowCheckboxes, true)
            .Add(p => p.OnNodeClick, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => clicked = true))
            .Add(p => p.OnNodeExpand, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => expanded = true)));

        cut.Find(".tree-node-content").Click();
        cut.Find(".tree-view-node").KeyDown("Enter");
        cut.Find(".tree-view-node").KeyDown("ArrowRight");
        cut.Find(".tree-node-toggle").Click();
        cut.Find("input.tree-node-checkbox").Change(true);

        clicked.ShouldBeFalse();
        expanded.ShouldBeFalse();
        cut.Find(".tree-view-node").GetAttribute("aria-disabled").ShouldBe("true");
        cut.Find(".tree-view-node").GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".tree-node-content").ClassList.ShouldContain("disabled");
        cut.Find(".tree-node-label").ClassList.ShouldContain("disabled");
        cut.Find(".tree-node-toggle").HasAttribute("disabled").ShouldBeTrue();
        cut.Find("input.tree-node-checkbox").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void TreeViewNode_CheckboxChangeInvokesClickForEnabledNode()
    {
        TreeView.TreeNode? clicked = null;
        var node = new TreeView.TreeNode { Id = "leaf", Label = "Leaf" };
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, node)
            .Add(p => p.ShowCheckboxes, true)
            .Add(p => p.OnNodeClick, EventCallback.Factory.Create<TreeView.TreeNode>(this, value => clicked = value)));

        cut.Find("input.tree-node-checkbox").Change(true);

        clicked.ShouldBeSameAs(node);
        cut.Find("input.tree-node-checkbox").GetAttribute("aria-label").ShouldBe("Select Leaf");
    }

    [Fact]
    public void TreeViewNode_RendersSelectedCheckboxAndIcon()
    {
        RenderFragment icon = builder => builder.AddContent(0, "*");
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, new TreeView.TreeNode { Id = "leaf", Label = "Leaf", Icon = icon })
            .Add(p => p.ShowCheckboxes, true)
            .Add(p => p.SelectedNodes, new HashSet<string> { "leaf" }));

        cut.Find("input.tree-node-checkbox").HasAttribute("checked").ShouldBeTrue();
        cut.Find(".tree-node-icon").TextContent.ShouldBe("*");
        cut.Find(".tree-node-label").ClassList.ShouldContain("selected");
        cut.Find(".tree-view-node").GetAttribute("aria-selected").ShouldBe("true");
    }

    [Fact]
    public void TreeViewNode_RendersSharedIcon_WhenIconNameIsProvided()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, new TreeView.TreeNode
            {
                Id = "documents",
                Label = "Documents",
                IconName = "folder"
            }));

        var icon = cut.Find(".tree-node-icon svg");
        icon.GetAttribute("aria-hidden").ShouldBe("true");
        icon.GetAttribute("width").ShouldBe("16");
        cut.FindAll(".tree-node-icon path").ShouldNotBeEmpty();
    }

    private static TreeView.TreeNode CreateParentNode(bool disabled = false) => new()
    {
        Id = "parent",
        Label = "Parent",
        IsDisabled = disabled,
        Children =
        [
            new() { Id = "child-1", Label = "Child 1" },
            new() { Id = "child-2", Label = "Child 2" }
        ]
    };
}
