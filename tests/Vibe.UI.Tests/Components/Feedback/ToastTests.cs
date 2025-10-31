namespace Vibe.UI.Tests.Components.Feedback;

public class ToastTests : TestContext
{
    public ToastTests() { this.AddVibeUIServices(); }

    [Fact]
    public void Toast_RendersWithBasicStructure()
    {
        var cut = RenderComponent<Toast>(parameters => parameters.Add(p => p.Description, "Message"));
        cut.Find(".vibe-toast").Should().NotBeNull();
        cut.Find("[role='alert']").Should().NotBeNull();
    }

    [Fact]
    public void Toast_RendersDescription()
    {
        var cut = RenderComponent<Toast>(parameters => parameters.Add(p => p.Description, "Test message"));
        cut.Find(".toast-description").TextContent.Should().Be("Test message");
    }

    [Fact]
    public void Toast_RendersTitle_WhenProvided()
    {
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Title, "Success")
            .Add(p => p.Description, "Done"));
        cut.Find(".toast-title").TextContent.Should().Be("Success");
    }

    [Fact]
    public void Toast_ShowsCloseButton_ByDefault()
    {
        var cut = RenderComponent<Toast>(parameters => parameters.Add(p => p.Description, "Message"));
        cut.Find(".toast-close").Should().NotBeNull();
    }

    [Fact]
    public void Toast_ShowsProgressBar_ByDefault()
    {
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, "Message")
            .Add(p => p.Duration, 5000));
        cut.Find(".toast-progress").Should().NotBeNull();
    }

    [Fact]
    public void Toast_AppliesVariantClass()
    {
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, "Message")
            .Add(p => p.Variant, "success"));
        cut.Find(".vibe-toast").ClassList.Should().Contain("toast-success");
    }
}
