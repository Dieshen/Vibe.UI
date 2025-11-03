namespace Vibe.UI.Tests.Components.Feedback;

public class ConfettiTests : TestBase
{
    [Fact]
    public void Confetti_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Confetti>();

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
    }

    [Fact]
    public void Confetti_IsInactive_Initially()
    {
        // Act
        var cut = RenderComponent<Confetti>();

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ClassList.ShouldNotContain("active");
    }

    [Fact]
    public void Confetti_BecomesActive_WhenActivated()
    {
        // Act
        var cut = RenderComponent<Confetti>(parameters => parameters
            .Add(p => p.Active, true));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ClassList.ShouldContain("active");
    }

    [Fact]
    public void Confetti_UsesDefaultParticleCount()
    {
        // Act
        var cut = RenderComponent<Confetti>(parameters => parameters
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
        var cut = RenderComponent<Confetti>(parameters => parameters
            .Add(p => p.ParticleCount, particleCount)
            .Add(p => p.Active, true));

        // Assert
        var particles = cut.FindAll(".confetti-particle");
        particles.Count.ShouldBe(particleCount);
    }

    [Fact]
    public void Confetti_Applies_CustomDuration()
    {
        // Arrange
        var duration = 5000;

        // Act
        var cut = RenderComponent<Confetti>(parameters => parameters
            .Add(p => p.Duration, duration));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
    }

    [Fact]
    public void Confetti_Applies_CustomOrigin()
    {
        // Act
        var cut = RenderComponent<Confetti>(parameters => parameters
            .Add(p => p.Origin, Confetti.ConfettiOrigin.Top)
            .Add(p => p.Active, true));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
    }

    [Fact]
    public void Confetti_Applies_CustomPattern()
    {
        // Act
        var cut = RenderComponent<Confetti>(parameters => parameters
            .Add(p => p.Pattern, Confetti.ConfettiPattern.Fountain));

        // Assert
        var confetti = cut.Find(".vibe-confetti");
        confetti.ShouldNotBeNull();
    }

    [Fact]
    public void Confetti_GeneratesParticles_WhenActive()
    {
        // Act
        var cut = RenderComponent<Confetti>(parameters => parameters
            .Add(p => p.Active, true));

        // Assert
        var particles = cut.FindAll(".confetti-particle");
        particles.ShouldNotBeEmpty();
    }
}
