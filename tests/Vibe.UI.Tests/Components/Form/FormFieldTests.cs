namespace Vibe.UI.Tests.Components.Form;

public class FormFieldTests : TestContext
{
    public FormFieldTests() { this.AddVibeUIServices(); }
    [Fact] public void FormField_Renders() { var cut = RenderComponent<FormField>(); cut.Markup.Should().NotBeEmpty(); }
}
