namespace Vibe.UI.Tests.Components.Feedback;

public class SkeletonTests : TestBase
{
    [Fact]
    public void Skeleton_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Skeleton>();

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
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Width, width));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.GetAttribute("style")!.ShouldContain($"width: {width}");
    }

    [Fact]
    public void Skeleton_Applies_CustomHeight()
    {
        // Arrange
        var height = "50px";

        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Height, height));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.GetAttribute("style")!.ShouldContain($"height: {height}");
    }

    [Fact]
    public void Skeleton_Applies_RoundedClass()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Rounded, true));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("rounded");
    }

    [Fact]
    public void Skeleton_Applies_CircleClass()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Circle, true));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("circle");
    }

    [Fact]
    public void Skeleton_DoesNotApply_RoundedClass_WhenFalse()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Rounded, false));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldNotContain("rounded");
    }

    [Fact]
    public void Skeleton_Applies_DefaultDimensions()
    {
        // Act
        var cut = Render<Skeleton>();

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        var style = skeleton.GetAttribute("style");
        style!.ShouldContain("width: 100%");
        style!.ShouldContain("height: 1rem");
    }

    [Fact]
    public void Skeleton_Applies_BothRoundedAndCircle()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Rounded, true)
            .Add(p => p.Circle, true));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("rounded");
        skeleton.ClassList.ShouldContain("circle");
    }

    [Fact]
    public void Skeleton_Default_IsDecorativeForAssistiveTechnology()
    {
        // Act
        var cut = Render<Skeleton>();

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.GetAttribute("aria-hidden")!.ShouldBe("true");
        skeleton.HasAttribute("role").ShouldBeFalse();
        skeleton.HasAttribute("aria-live").ShouldBeFalse();
    }

    [Fact]
    public void Skeleton_Announce_AddsLoadingStatusSemantics()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Announce, true)
            .Add(p => p.AriaLabel, "Loading dashboard"));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.GetAttribute("aria-hidden")!.ShouldBe("false");
        skeleton.GetAttribute("role")!.ShouldBe("status");
        skeleton.GetAttribute("aria-live")!.ShouldBe("polite");
        skeleton.GetAttribute("aria-label")!.ShouldBe("Loading dashboard");
        skeleton.GetAttribute("aria-busy")!.ShouldBe("true");
    }

    [Fact]
    public void Skeleton_AnimatedFalse_AppliesStaticClassAndBusyFalse()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Announce, true)
            .Add(p => p.Animated, false));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("vibe-skeleton-static");
        skeleton.ClassList.ShouldNotContain("vibe-skeleton-animated");
        skeleton.GetAttribute("aria-busy")!.ShouldBe("false");
    }

    [Fact]
    public void Skeleton_AppliesNamespacedShapeClasses()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Rounded, true)
            .Add(p => p.Circle, true));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("vibe-skeleton-rounded");
        skeleton.ClassList.ShouldContain("vibe-skeleton-circle");
    }

    [Fact]
    public void Skeleton_BlankDimensions_FallBackToDefaults()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Width, null!)
            .Add(p => p.Height, "   "));

        // Assert
        var style = cut.Find(".vibe-skeleton").GetAttribute("style");
        style!.ShouldContain("width: 100%");
        style!.ShouldContain("height: 1rem");
    }

    [Fact]
    public void Skeleton_AppliesCustomClassAndAdditionalAttributes()
    {
        // Act
        var cut = Render<Skeleton>(parameters => parameters
            .Add(p => p.Class, "custom-skeleton")
            .AddUnmatched("data-testid", "skeleton-root"));

        // Assert
        var skeleton = cut.Find(".vibe-skeleton");
        skeleton.ClassList.ShouldContain("custom-skeleton");
        skeleton.GetAttribute("data-testid")!.ShouldBe("skeleton-root");
    }
}
