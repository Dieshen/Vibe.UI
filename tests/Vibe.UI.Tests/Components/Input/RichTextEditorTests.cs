namespace Vibe.UI.Tests.Components.Input;

public class RichTextEditorTests : TestContext
{
    public RichTextEditorTests() { this.AddVibeUIServices(); }
    [Fact] public void RichTextEditor_Renders() { var cut = RenderComponent<RichTextEditor>(); cut.Markup.Should().NotBeEmpty(); }
}
