namespace Vibe.UI.Tests.Components.Advanced;

public class TreeViewTests : TestContext
{
    public TreeViewTests() { this.AddVibeUIServices(); }
    [Fact] public void TreeView_Renders() { var cut = RenderComponent<TreeView>(); cut.Markup.Should().NotBeEmpty(); }
}
