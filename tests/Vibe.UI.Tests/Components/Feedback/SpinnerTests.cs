namespace Vibe.UI.Tests.Components.Feedback;

public class SpinnerTests : TestContext
{
    public SpinnerTests() { this.AddVibeUIServices(); }

    [Fact]
    public void Spinner_RendersWithBasicStructure()
    {
        var cut = RenderComponent<Spinner>();
        cut.Find(".vibe-spinner").Should().NotBeNull();
        cut.Find("[role='status']").Should().NotBeNull();
        cut.Find(".sr-only").TextContent.Should().Be("Loading...");
    }

    [Theory]
    [InlineData(SpinnerSize.Small, "spinner-small")]
    [InlineData(SpinnerSize.Default, "spinner-default")]
    [InlineData(SpinnerSize.Large, "spinner-large")]
    public void Spinner_AppliesCorrectSizeClass(SpinnerSize size, string expectedClass)
    {
        var cut = RenderComponent<Spinner>(parameters => parameters.Add(p => p.Size, size));
        cut.Find(".vibe-spinner").ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void Spinner_ShowsLabel_WhenEnabled()
    {
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, "Loading data")
            .Add(p => p.ShowLabel, true));
        cut.Find(".spinner-label").TextContent.Should().Be("Loading data");
    }
}
