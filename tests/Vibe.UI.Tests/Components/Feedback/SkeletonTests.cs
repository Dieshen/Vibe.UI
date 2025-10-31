namespace Vibe.UI.Tests.Components.Feedback;

public class SkeletonTests : TestContext
{
    public SkeletonTests() { this.AddVibeUIServices(); }

    [Fact]
    public void Skeleton_RendersWithBasicStructure()
    {
        var cut = RenderComponent<Skeleton>();
        cut.Find(".vibe-skeleton").Should().NotBeNull();
    }

    [Fact]
    public void Skeleton_AppliesDefaultDimensions()
    {
        var cut = RenderComponent<Skeleton>();
        var style = cut.Find(".vibe-skeleton").GetAttribute("style");
        style.Should().Contain("width: 100%");
        style.Should().Contain("height: 1rem");
    }

    [Fact]
    public void Skeleton_AppliesCustomDimensions()
    {
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Width, "200px")
            .Add(p => p.Height, "50px"));
        var style = cut.Find(".vibe-skeleton").GetAttribute("style");
        style.Should().Contain("width: 200px");
        style.Should().Contain("height: 50px");
    }

    [Fact]
    public void Skeleton_AppliesRoundedClass()
    {
        var cut = RenderComponent<Skeleton>(parameters => parameters.Add(p => p.Rounded, true));
        cut.Find(".vibe-skeleton").ClassList.Should().Contain("rounded");
    }

    [Fact]
    public void Skeleton_AppliesCircleClass()
    {
        var cut = RenderComponent<Skeleton>(parameters => parameters.Add(p => p.Circle, true));
        cut.Find(".vibe-skeleton").ClassList.Should().Contain("circle");
    }
}
