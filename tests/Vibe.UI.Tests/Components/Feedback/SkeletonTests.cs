namespace Vibe.UI.Tests.Components.Feedback;

public class SkeletonTests : TestBase
{
    [Fact]
    public void Skeleton_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Skeleton>();

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ShouldNotBeNull();
    }

    [Fact]
    public void Skeleton_Applies_CustomWidth()
    {
        // Arrange
        var width = "200px";

        // Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Width, width));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.GetAttribute("style").ShouldContain($"width: {width}");
    }

    [Fact]
    public void Skeleton_Applies_CustomHeight()
    {
        // Arrange
        var height = "50px";

        // Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Height, height));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.GetAttribute("style").ShouldContain($"height: {height}");
    }

    [Fact]
    public void Skeleton_Applies_RoundedClass()
    {
        // Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Rounded, true));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("rounded");
    }

    [Fact]
    public void Skeleton_Applies_CircleClass()
    {
        // Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Circle, true));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("circle");
    }

    [Fact]
    public void Skeleton_DoesNotApply_RoundedClass_WhenFalse()
    {
        // Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Rounded, false));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldNotContain("rounded");
    }

    [Fact]
    public void Skeleton_Applies_DefaultDimensions()
    {
        // Act
        var cut = RenderComponent<Skeleton>();

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        var style = skeleton.GetAttribute("style");
        style.ShouldContain("width: 100%");
        style.ShouldContain("height: 1rem");
    }

    [Fact]
    public void Skeleton_Applies_BothRoundedAndCircle()
    {
        // Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Rounded, true)
            .Add(p => p.Circle, true));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("rounded");
        skeleton.ClassList.ShouldContain("circle");
    }
}
