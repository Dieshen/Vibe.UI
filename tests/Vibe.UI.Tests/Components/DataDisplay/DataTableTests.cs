namespace Vibe.UI.Tests.Components.DataDisplay;

public class DataTableTests : TestContext
{
    public DataTableTests() { this.AddVibeUIServices(); }
    [Fact] public void DataTable_Renders() { var cut = RenderComponent<DataTable>(); cut.Markup.Should().NotBeEmpty(); }
}
