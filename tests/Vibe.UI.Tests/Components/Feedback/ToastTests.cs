namespace Vibe.UI.Tests.Components.Feedback;

public class ToastTests : TestBase
{
    [Fact]
    public void Toast_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, "Test message"));

        // Assert
        var toast = cut.Find(".vibe-toast");
        toast.ShouldNotBeNull();
        toast.GetAttribute("role").ShouldBe("alert");
    }

    [Fact]
    public void Toast_Displays_Title()
    {
        // Arrange
        var title = "Success";

        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.Description, "Test"));

        // Assert
        var titleElement = cut.Find(".toast-title");
        titleElement.TextContent.ShouldBe(title);
    }

    [Fact]
    public void Toast_Displays_Description()
    {
        // Arrange
        var description = "Operation completed successfully";

        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, description));

        // Assert
        var descElement = cut.Find(".toast-description");
        descElement.TextContent.ShouldBe(description);
    }

    [Fact]
    public void Toast_Applies_VariantClass()
    {
        // Arrange
        var variant = "success";

        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Variant, variant)
            .Add(p => p.Description, "Test"));

        // Assert
        var toast = cut.Find(".vibe-toast");
        toast.ClassList.ShouldContain("toast-success");
    }

    [Fact]
    public void Toast_Shows_CloseButton_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, "Test")
            .Add(p => p.ShowCloseButton, true));

        // Assert
        var closeButton = cut.Find(".toast-close");
        closeButton.ShouldNotBeNull();
    }

    [Fact]
    public void Toast_Hides_CloseButton_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, "Test")
            .Add(p => p.ShowCloseButton, false));

        // Assert
        cut.FindAll(".toast-close").ShouldBeEmpty();
    }

    [Fact]
    public void Toast_Shows_ProgressBar_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, "Test")
            .Add(p => p.ShowProgress, true)
            .Add(p => p.Duration, 5000));

        // Assert
        var progress = cut.Find(".toast-progress");
        progress.ShouldNotBeNull();
    }

    [Fact]
    public void Toast_InvokesOnClose_WhenCloseButtonClicked()
    {
        // Arrange
        var closeCalled = false;
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, "Test")
            .Add(p => p.ShowCloseButton, true)
            .Add(p => p.OnClose, () => closeCalled = true));

        // Act
        var closeButton = cut.Find(".toast-close");
        closeButton.Click();

        // Assert
        closeCalled.ShouldBeTrue();
    }

    [Fact]
    public void Toast_IsVisible_Initially()
    {
        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Description, "Test"));

        // Assert
        var toast = cut.Find(".vibe-toast");
        toast.ClassList.ShouldContain("visible");
    }

    [Fact]
    public void Toast_Displays_Icon_WhenProvided()
    {
        // Arrange
        var icon = "✓";

        // Act
        var cut = RenderComponent<Toast>(parameters => parameters
            .Add(p => p.Icon, icon)
            .Add(p => p.Description, "Test"));

        // Assert
        var iconElement = cut.Find(".toast-icon");
        iconElement.TextContent.ShouldBe(icon);
    }
}
