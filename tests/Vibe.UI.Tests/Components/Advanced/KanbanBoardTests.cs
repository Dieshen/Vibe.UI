namespace Vibe.UI.Tests.Components.Advanced;

public class KanbanBoardTests : TestContext
{
    public KanbanBoardTests() { this.AddVibeUIServices(); }
    [Fact] public void KanbanBoard_Renders() { var cut = RenderComponent<KanbanBoard>(); cut.Markup.Should().NotBeEmpty(); }
}
