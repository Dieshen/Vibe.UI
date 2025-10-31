namespace Vibe.UI.Tests.Components.Form;

public class FormLabelTests : TestContext
{
    public FormLabelTests() { this.AddVibeUIServices(); }
    [Fact] public void FormLabel_Renders() { var cut = RenderComponent<FormLabel>(); cut.Markup.Should().NotBeEmpty(); }
}
