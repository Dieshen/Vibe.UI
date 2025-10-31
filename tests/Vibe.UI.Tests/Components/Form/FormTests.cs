namespace Vibe.UI.Tests.Components.Form;

public class FormTests : TestContext
{
    public FormTests() { this.AddVibeUIServices(); }
    [Fact] public void Form_Renders() { var cut = RenderComponent<Form>(); cut.Markup.Should().NotBeEmpty(); }
}
