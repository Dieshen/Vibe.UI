namespace Vibe.UI.Tests.Components.Feedback;

public class ToastTests : TestBase
{
    [Fact]
    public void Toast_Renders_WithAccessibleDefaultProps()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Test message")
            .Add(p => p.Duration, 0));

        var toast = cut.Find(".vibe-toast");
        toast.GetAttribute("role").ShouldBe("alert");
        toast.GetAttribute("aria-live").ShouldBe("assertive");
        toast.GetAttribute("aria-atomic").ShouldBe("true");
        toast.GetAttribute("data-state").ShouldBe("open");
        toast.HasAttribute("aria-label").ShouldBeFalse();
        toast.ClassList.ShouldContain("toast-default");
        toast.ClassList.ShouldContain("visible");
        toast.ClassList.ShouldContain("toast-visible");

        var closeButton = cut.Find(".toast-close");
        closeButton.GetAttribute("type").ShouldBe("button");
        closeButton.GetAttribute("aria-label").ShouldBe("Close");
        closeButton.GetAttribute("aria-disabled").ShouldBe("false");
        closeButton.QuerySelector("svg")!.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void Toast_PreservesAdditionalAttributesAndCustomClass()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Test message")
            .Add(p => p.Duration, 0)
            .Add(p => p.Class, "toast-stack-item")
            .AddUnmatched("data-testid", "toast-root"));

        var toast = cut.Find(".vibe-toast");
        toast.ClassList.ShouldContain("toast-stack-item");
        toast.GetAttribute("data-testid").ShouldBe("toast-root");
    }

    [Fact]
    public void Toast_UsesCustomLiveRegionSemanticsAndNormalizesInvalidValues()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Saved")
            .Add(p => p.Duration, 0)
            .Add(p => p.Role, "status")
            .Add(p => p.AriaLive, "polite"));

        var toast = cut.Find(".vibe-toast");
        toast.GetAttribute("role").ShouldBe("status");
        toast.GetAttribute("aria-live").ShouldBe("polite");

        cut.Render(parameters => parameters
            .Add(p => p.Description, "Saved")
            .Add(p => p.Duration, 0)
            .Add(p => p.Role, "presentation")
            .Add(p => p.AriaLive, "loud"));

        toast = cut.Find(".vibe-toast");
        toast.GetAttribute("role").ShouldBe("alert");
        toast.GetAttribute("aria-live").ShouldBe("assertive");
    }

    [Fact]
    public void Toast_Displays_TitleAndDescription_WhenProvided()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Title, " Success ")
            .Add(p => p.Description, " Operation completed successfully ")
            .Add(p => p.Duration, 0));

        cut.Find(".toast-title").TextContent.ShouldBe("Success");
        cut.Find(".toast-description").TextContent.ShouldBe("Operation completed successfully");
    }

    [Fact]
    public void Toast_OmitsEmptyTitleDescriptionAndIcon_AndProvidesFallbackLabel()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Title, " ")
            .Add(p => p.Description, null)
            .Add(p => p.Icon, " ")
            .Add(p => p.Duration, 0));

        var toast = cut.Find(".vibe-toast");
        toast.GetAttribute("aria-label").ShouldBe("Notification");
        cut.FindAll(".toast-title").ShouldBeEmpty();
        cut.FindAll(".toast-description").ShouldBeEmpty();
        cut.FindAll(".toast-icon").ShouldBeEmpty();
        cut.FindAll(".toast-message").ShouldBeEmpty();
    }

    [Fact]
    public void Toast_UsesCustomAriaLabel_WhenProvided()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Visible message")
            .Add(p => p.AriaLabel, "  Background sync failed  ")
            .Add(p => p.Duration, 0));

        cut.Find(".vibe-toast").GetAttribute("aria-label").ShouldBe("Background sync failed");
    }

    [Fact]
    public void Toast_Displays_IconAsDecorative_WhenProvided()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Icon, "!")
            .Add(p => p.Description, "Test")
            .Add(p => p.Duration, 0));

        var icon = cut.Find(".toast-icon");
        icon.TextContent.ShouldBe("!");
        icon.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void Toast_Applies_SafeVariantClass()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Variant, " Success ")
            .Add(p => p.Description, "Test")
            .Add(p => p.Duration, 0));

        cut.Find(".vibe-toast").ClassList.ShouldContain("toast-success");

        cut.Render(parameters => parameters
            .Add(p => p.Variant, "success injected")
            .Add(p => p.Description, "Test")
            .Add(p => p.Duration, 0));

        var toast = cut.Find(".vibe-toast");
        toast.ClassList.ShouldContain("toast-default");
        toast.ClassList.ShouldNotContain("injected");
    }

    [Fact]
    public void Toast_Hides_CloseButton_WhenDisabled()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Test")
            .Add(p => p.ShowCloseButton, false)
            .Add(p => p.Duration, 0));

        cut.FindAll(".toast-close").ShouldBeEmpty();
    }

    [Fact]
    public void Toast_Shows_ProgressBar_WhenEnabledWithPositiveDuration()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Test")
            .Add(p => p.ShowProgress, true)
            .Add(p => p.Duration, 2500));

        var progress = cut.Find(".toast-progress");
        progress.GetAttribute("aria-hidden").ShouldBe("true");
        cut.Find(".toast-progress-bar").GetAttribute("style").ShouldBe("animation-duration: 2500ms;");
    }

    [Fact]
    public async Task Toast_NonPositiveDuration_DisablesProgressAndAutoDismiss()
    {
        var closeCount = 0;
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Persistent")
            .Add(p => p.Duration, -1)
            .Add(p => p.OnClose, () => closeCount++));

        cut.FindAll(".toast-progress").ShouldBeEmpty();

        await Task.Delay(75);

        closeCount.ShouldBe(0);
        cut.Find(".vibe-toast").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact]
    public void Toast_HidesProgressBar_WhenShowProgressIsFalse()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Test")
            .Add(p => p.ShowProgress, false)
            .Add(p => p.Duration, 2500));

        cut.FindAll(".toast-progress").ShouldBeEmpty();
    }

    [Fact]
    public async Task Toast_CloseButtonInvokesOnCloseOnceAndCancelsAutoDismiss()
    {
        var closeCount = 0;
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Dismiss me")
            .Add(p => p.Duration, 25)
            .Add(p => p.OnClose, () => closeCount++));

        cut.Find(".toast-close").Click();

        closeCount.ShouldBe(1);
        var toast = cut.Find(".vibe-toast");
        toast.GetAttribute("data-state").ShouldBe("closed");
        toast.ClassList.ShouldContain("toast-hidden");
        toast.ClassList.ShouldNotContain("toast-visible");
        cut.Find(".toast-close").HasAttribute("disabled").ShouldBeTrue();

        await Task.Delay(100);

        closeCount.ShouldBe(1);
    }

    [Fact]
    public void Toast_AutoDismissInvokesOnCloseOnce()
    {
        var closeCount = 0;
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Auto dismiss")
            .Add(p => p.Duration, 20)
            .Add(p => p.OnClose, () => closeCount++));

        cut.WaitForAssertion(() => closeCount.ShouldBe(1), TimeSpan.FromSeconds(1));

        var toast = cut.Find(".vibe-toast");
        toast.GetAttribute("data-state").ShouldBe("closed");
        toast.ClassList.ShouldContain("toast-hidden");

        cut.Find(".toast-close").Click();
        closeCount.ShouldBe(1);
    }

    [Fact]
    public async Task Toast_DisposeCancelsAutoDismissWithoutInvokingOnClose()
    {
        var closeCount = 0;
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "Dispose me")
            .Add(p => p.Duration, 20)
            .Add(p => p.OnClose, () => closeCount++));

        cut.Instance.Dispose();

        await Task.Delay(100);

        closeCount.ShouldBe(0);
    }

    [Fact]
    public void Toast_ParameterContentChangeAfterClose_ReopensForReusedComponent()
    {
        var cut = Render<Toast>(parameters => parameters
            .Add(p => p.Description, "First")
            .Add(p => p.Duration, 0));

        cut.Find(".toast-close").Click();
        cut.Find(".vibe-toast").GetAttribute("data-state").ShouldBe("closed");

        cut.Render(parameters => parameters
            .Add(p => p.Title, "Updated")
            .Add(p => p.Description, "Second")
            .Add(p => p.Duration, 0));

        var toast = cut.Find(".vibe-toast");
        toast.GetAttribute("data-state").ShouldBe("open");
        toast.ClassList.ShouldContain("toast-visible");
        cut.Find(".toast-title").TextContent.ShouldBe("Updated");
        cut.Find(".toast-description").TextContent.ShouldBe("Second");
    }
}
