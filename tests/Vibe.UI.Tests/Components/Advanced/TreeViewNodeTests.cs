namespace Vibe.UI.Tests.Components.Advanced;

public class TreeViewNodeTests : TestBase
{
    [Fact]
    public void TreeViewNode_RendersLeafNode()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, new TreeView.TreeNode { Id = "leaf", Label = "Leaf" }));

        var node = cut.Find(".tree-view-node");
        node.GetAttribute("role").ShouldBe("treeitem");
        node.GetAttribute("aria-expanded").ShouldBe("false");
        cut.Find(".tree-node-label").TextContent.ShouldBe("Leaf");
        cut.Find(".tree-node-spacer").ShouldNotBeNull();
    }

    [Fact]
    public void TreeViewNode_RendersCollapsedChildren()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, CreateParentNode()));

        cut.Find(".tree-node-toggle").GetAttribute("aria-label").ShouldBe("Expand");
        var group = cut.Find(".tree-node-children");
        group.GetAttribute("role").ShouldBe("group");
        group.ClassList.ShouldContain("collapsed");
        group.GetAttribute("style").ShouldBe("display: none;");
    }

    [Fact]
    public void TreeViewNode_RendersExpandedChildren()
    {
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, CreateParentNode())
            .Add(p => p.ExpandedNodes, new HashSet<string> { "parent" }));

        cut.Find(".tree-view-node").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find(".tree-node-toggle").GetAttribute("aria-label").ShouldBe("Collapse");
        cut.Find(".tree-node-toggle").ClassList.ShouldContain("expanded");
        cut.Find(".tree-node-children").ClassList.ShouldContain("expanded");
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
    public void TreeViewNode_DoesNotInvokeClickForDisabledNode()
    {
        var clicked = false;
        var cut = Render<TreeViewNode>(parameters => parameters
            .Add(p => p.Item, new TreeView.TreeNode { Id = "leaf", Label = "Leaf", IsDisabled = true })
            .Add(p => p.OnNodeClick, EventCallback.Factory.Create<TreeView.TreeNode>(this, _ => clicked = true)));

        cut.Find(".tree-node-content").Click();

        clicked.ShouldBeFalse();
        cut.Find(".tree-node-label").ClassList.ShouldContain("disabled");
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
    }

    private static TreeView.TreeNode CreateParentNode() => new()
    {
        Id = "parent",
        Label = "Parent",
        Children =
        [
            new() { Id = "child", Label = "Child" }
        ]
    };
}
