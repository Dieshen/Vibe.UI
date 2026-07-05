namespace Vibe.UI.Tests.Components.Feedback;

public class ConfettiTests : TestBase
{
    [Fact]
    public void Confetti_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Confetti>();

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
        confetti.GetAttribute("data-state").ShouldBe("idle");
        confetti.GetAttribute("data-particle-count").ShouldBe("50");
    }

    [Fact]
    public void Confetti_IsInactive_Initially()
    {
        // Act
        var cut = Render<Confetti>();

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ClassList.ShouldNotContain("active");
    }

    [Fact]
    public void Confetti_Default_IsDecorativeForAssistiveTechnology()
    {
        // Act
        var cut = Render<Confetti>();

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.GetAttribute("aria-hidden").ShouldBe("true");
        confetti.HasAttribute("role").ShouldBeFalse();
        confetti.HasAttribute("aria-live").ShouldBeFalse();
        confetti.HasAttribute("aria-label").ShouldBeFalse();
    }

    [Fact]
    public void Confetti_Announce_AddsStatusLiveRegionSemantics()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Announce, true)
            .Add(p => p.AriaLabel, "Celebration complete"));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.GetAttribute("aria-hidden").ShouldBe("false");
        confetti.GetAttribute("role").ShouldBe("status");
        confetti.GetAttribute("aria-live").ShouldBe("polite");
        confetti.GetAttribute("aria-label").ShouldBe("Celebration complete");
    }

    [Fact]
    public void Confetti_BecomesActive_WhenActivated()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Active, true));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ClassList.ShouldContain("active");
    }

    [Fact]
    public void Confetti_UsesDefaultParticleCount()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Active, true));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
    }

    [Fact]
    public void Confetti_Applies_CustomParticleCount()
    {
        // Arrange
        var particleCount = 100;

        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.ParticleCount, particleCount)
            .Add(p => p.Active, true));

        // Assert
        var particles = cut.FindAll(".confetti-particle");
        particles.Count.ShouldBe(particleCount);
    }

    [Fact]
    public void Confetti_Clamps_NegativeParticleCount()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.ParticleCount, -10)
            .Add(p => p.Active, true));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.GetAttribute("data-particle-count").ShouldBe("0");
        cut.FindAll(".confetti-particle").ShouldBeEmpty();
    }

    [Fact]
    public void Confetti_Applies_CustomDuration()
    {
        // Arrange
        var duration = 5000;

        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Duration, duration));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
    }

    [Fact]
    public void Confetti_Applies_CustomOrigin()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Origin, Confetti.ConfettiOrigin.Top)
            .Add(p => p.Active, true));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
        confetti.GetAttribute("data-origin").ShouldBe("top");
    }

    [Fact]
    public void Confetti_Applies_CustomPattern()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Pattern, Confetti.ConfettiPattern.Fountain));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
        confetti.GetAttribute("data-pattern").ShouldBe("fountain");
    }

    [Fact]
    public void Confetti_GeneratesParticles_WhenActive()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Active, true));

        // Assert
        var particles = cut.FindAll(".confetti-particle");
        particles.ShouldNotBeEmpty();
    }

    [Fact]
    public void Confetti_FallsBackToDefaultColors_WhenColorsAreNullOrUnsafe()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Active, true)
            .Add(p => p.ParticleCount, 1)
            .Add(p => p.Colors, [null!, " ", "red; background: url(javascript:alert(1))"]));

        // Assert
        var style = cut.Find(".confetti-particle").GetAttribute("style");
        style.ShouldNotBeNull();
        style!.ShouldContain("--confetti-color: #");
        style.ShouldNotContain("javascript");
        style.ShouldNotContain("background:");
    }

    [Fact]
    public void Confetti_AppliesCustomClassesAndAdditionalAttributes()
    {
        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Class, "confetti-shell")
            .Add(p => p.CssClass, "legacy-confetti-class")
            .AddUnmatched("data-testid", "confetti-root"));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ClassList.ShouldContain("confetti-shell");
        confetti.ClassList.ShouldContain("legacy-confetti-class");
        confetti.GetAttribute("data-testid").ShouldBe("confetti-root");
    }

    [Fact]
    public void Confetti_NonPositiveDuration_CompletesWithoutDelay()
    {
        // Arrange
        var completeCount = 0;

        // Act
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Active, true)
            .Add(p => p.Duration, -1)
            .Add(p => p.OnComplete, () => completeCount++));

        // Assert
        cut.WaitForAssertion(() => completeCount.ShouldBe(1));
        var confetti = cut.Find(".vibe-confetti");
        confetti.ClassList.ShouldNotContain("active");
        confetti.GetAttribute("data-state").ShouldBe("idle");
    }

    [Fact]
    public async Task Confetti_DeactivationCancelsPendingCompletion()
    {
        // Arrange
        var completeCount = 0;
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Active, true)
            .Add(p => p.Duration, 25)
            .Add(p => p.OnComplete, () => completeCount++));

        // Act
        cut.Render(parameters => parameters
            .Add(p => p.Active, false)
            .Add(p => p.Duration, 25)
            .Add(p => p.OnComplete, () => completeCount++));
        await Task.Delay(75);

        // Assert
        completeCount.ShouldBe(0);
        cut.Find(".vibe-confetti").ClassList.ShouldNotContain("active");
    }

    [Fact]
    public async Task Confetti_DisposeCancelsPendingCompletion()
    {
        // Arrange
        var completeCount = 0;
        var cut = Render<Confetti>(parameters => parameters
            .Add(p => p.Active, true)
            .Add(p => p.Duration, 25)
            .Add(p => p.OnComplete, () => completeCount++));

        // Act
        await cut.Instance.DisposeAsync();
        await Task.Delay(75);

        // Assert
        completeCount.ShouldBe(0);
    }
}
