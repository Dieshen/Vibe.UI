namespace Vibe.UI.Tests.Components.Feedback;

public class SonnerTests : TestBase
{
    [Fact]
    public void Sonner_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Sonner>();

        // Assert
        var sonner = cut.Find(".sonner-container");
        sonner.ShouldNotBeNull();
    }

    [Fact]
    public void Sonner_Applies_PositionClass()
    {
        // Act
        var cut = RenderComponent<Sonner>(parameters => parameters
            .Add(p => p.Position, Sonner.SonnerPosition.TopRight));

        // Assert
        var sonner = cut.Find(".sonner-container");
        sonner.ClassList.ShouldContain("sonner-topright");
    }

    [Fact]
    public void Sonner_Applies_RichColorsClass_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<Sonner>(parameters => parameters
            .Add(p => p.RichColors, true));

        // Assert
        var sonner = cut.Find(".sonner-container");
        sonner.ClassList.ShouldContain("sonner-rich");
    }

    [Fact]
    public void Sonner_DoesNotApply_RichColorsClass_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<Sonner>(parameters => parameters
            .Add(p => p.RichColors, false));

        // Assert
        var sonner = cut.Find(".sonner-container");
        sonner.ClassList.ShouldNotContain("sonner-rich");
    }

    [Fact]
    public void Sonner_InitiallyEmpty()
    {
        // Act
        var cut = RenderComponent<Sonner>();

        // Assert
        var toasts = cut.FindAll(".sonner-toast");
        toasts.ShouldBeEmpty();
    }

    [Fact]
    public void Sonner_ShowsToast_AfterToastMethodCalled()
    {
        // Act
        var cut = RenderComponent<Sonner>();
        cut.Instance.Toast("Test message");
        cut.Render();

        // Assert
        var toasts = cut.FindAll(".sonner-toast");
        toasts.ShouldNotBeEmpty();
    }

    [Fact]
    public void Sonner_ShowsSuccessToast()
    {
        // Act
        var cut = RenderComponent<Sonner>();
        cut.Instance.Success("Success message");
        cut.Render();

        // Assert
        var toast = cut.Find(".sonner-success");
        toast.ShouldNotBeNull();
    }

    [Fact]
    public void Sonner_ShowsErrorToast()
    {
        // Act
        var cut = RenderComponent<Sonner>();
        cut.Instance.Error("Error message");
        cut.Render();

        // Assert
        var toast = cut.Find(".sonner-error");
        toast.ShouldNotBeNull();
    }

    [Fact]
    public void Sonner_ShowsWarningToast()
    {
        // Act
        var cut = RenderComponent<Sonner>();
        cut.Instance.Warning("Warning message");
        cut.Render();

        // Assert
        var toast = cut.Find(".sonner-warning");
        toast.ShouldNotBeNull();
    }

    [Fact]
    public void Sonner_ShowsInfoToast()
    {
        // Act
        var cut = RenderComponent<Sonner>();
        cut.Instance.Info("Info message");
        cut.Render();

        // Assert
        var toast = cut.Find(".sonner-info");
        toast.ShouldNotBeNull();
    }

    [Fact]
    public void Sonner_ShowsLoadingToast()
    {
        // Act
        var cut = RenderComponent<Sonner>();
        cut.Instance.Loading("Loading...");
        cut.Render();

        // Assert
        var toast = cut.Find(".sonner-loading");
        toast.ShouldNotBeNull();
    }

    [Fact]
    public void Sonner_RespectsMaxToasts()
    {
        // Arrange
        var maxToasts = 3;

        // Act
        var cut = RenderComponent<Sonner>(parameters => parameters
            .Add(p => p.MaxToasts, maxToasts));

        for (int i = 0; i < 5; i++)
        {
            cut.Instance.Toast($"Message {i}");
        }
        cut.Render();

        // Assert
        var toasts = cut.FindAll(".sonner-toast");
        toasts.Count.ShouldBeLessThanOrEqualTo(maxToasts);
    }
}
