namespace Vibe.UI.Tests.Components.Form;

public class FormMessageTests : TestContext
{
    public FormMessageTests() { this.AddVibeUIServices(); }
    [Fact] public void FormMessage_Renders() { var cut = RenderComponent<FormMessage>(); cut.Markup.Should().NotBeEmpty(); }
}
