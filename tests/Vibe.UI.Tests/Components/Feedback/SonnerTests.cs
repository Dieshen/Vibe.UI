namespace Vibe.UI.Tests.Components.Feedback;

public class SonnerTests : TestBase
{
    [Fact]
    public void Sonner_Renders_WithAccessibleDefaultProps()
    {
        var cut = Render<Sonner>();

        var sonner = cut.Find(".sonner-container");
        sonner.GetAttribute("role").ShouldBe("region");
        sonner.GetAttribute("aria-label").ShouldBe("Notifications");
        sonner.GetAttribute("aria-disabled").ShouldBe("false");
        sonner.GetAttribute("data-state").ShouldBe("empty");
        sonner.GetAttribute("data-position").ShouldBe("bottomright");
        sonner.GetAttribute("data-toast-count").ShouldBe("0");
        sonner.ClassList.ShouldContain("sonner-bottomright");
        sonner.ClassList.ShouldContain("sonner-rich");
        cut.FindAll(".sonner-toast").ShouldBeEmpty();
    }

    [Fact]
    public void Sonner_PreservesAdditionalAttributesAndCustomClasses()
    {
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.CssClass, "legacy-sonner")
            .Add(p => p.Class, "base-sonner")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "sonner-root"
            }));

        var sonner = cut.Find(".sonner-container");
        sonner.ClassList.ShouldContain("legacy-sonner");
        sonner.ClassList.ShouldContain("base-sonner");
        sonner.GetAttribute("data-testid").ShouldBe("sonner-root");
    }

    [Fact]
    public void Sonner_UsesCustomAriaLabelAndFallsBackWhenBlank()
    {
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.AriaLabel, "App notifications"));

        cut.Find(".sonner-container").GetAttribute("aria-label").ShouldBe("App notifications");

        cut.Render(parameters => parameters
            .Add(p => p.AriaLabel, " "));

        cut.Find(".sonner-container").GetAttribute("aria-label").ShouldBe("Notifications");
    }

    [Fact]
    public void Sonner_Applies_PositionClass()
    {
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.Position, Sonner.SonnerPosition.TopRight));

        var sonner = cut.Find(".sonner-container");
        sonner.ClassList.ShouldContain("sonner-topright");
        sonner.GetAttribute("data-position").ShouldBe("topright");
    }

    [Fact]
    public void Sonner_DoesNotApply_RichColorsClass_WhenDisabled()
    {
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.RichColors, false));

        cut.Find(".sonner-container").ClassList.ShouldNotContain("sonner-rich");
    }

    [Fact]
    public void Sonner_ShowsToast_WithLiveRegionStateAndCloseButton()
    {
        var cut = Render<Sonner>();

        cut.Instance.Toast(" Test message ");
        cut.Render();

        var root = cut.Find(".sonner-container");
        root.GetAttribute("data-state").ShouldBe("open");
        root.GetAttribute("data-toast-count").ShouldBe("1");

        var toast = cut.Find(".sonner-toast");
        toast.GetAttribute("role").ShouldBe("status");
        toast.GetAttribute("aria-live").ShouldBe("polite");
        toast.GetAttribute("aria-atomic").ShouldBe("true");
        toast.GetAttribute("data-state").ShouldBe("open");
        toast.GetAttribute("data-type").ShouldBe("default");
        toast.GetAttribute("data-dismissible").ShouldBe("true");
        toast.ClassList.ShouldContain("sonner-default");
        cut.Find(".sonner-description").TextContent.ShouldBe("Test message");

        var close = cut.Find(".sonner-close");
        close.GetAttribute("type").ShouldBe("button");
        close.GetAttribute("aria-label").ShouldBe("Dismiss notification");
        close.GetAttribute("aria-disabled").ShouldBe("false");
    }

    [Theory]
    [InlineData(Sonner.SonnerType.Success, "sonner-success", "status", "polite")]
    [InlineData(Sonner.SonnerType.Error, "sonner-error", "alert", "assertive")]
    [InlineData(Sonner.SonnerType.Warning, "sonner-warning", "alert", "assertive")]
    [InlineData(Sonner.SonnerType.Info, "sonner-info", "status", "polite")]
    [InlineData(Sonner.SonnerType.Loading, "sonner-loading", "status", "polite")]
    public void Sonner_AppliesVariantClassAndDefaultSemantics(
        Sonner.SonnerType type,
        string expectedClass,
        string expectedRole,
        string expectedLive)
    {
        var cut = Render<Sonner>();

        cut.Instance.Toast(type.ToString(), type);
        cut.Render();

        var toast = cut.Find(".sonner-toast");
        toast.ClassList.ShouldContain(expectedClass);
        toast.GetAttribute("data-type").ShouldBe(type.ToString().ToLowerInvariant());
        toast.GetAttribute("role").ShouldBe(expectedRole);
        toast.GetAttribute("aria-live").ShouldBe(expectedLive);
    }

    [Fact]
    public void Sonner_NormalizesInvalidTypeRoleAndAriaLive()
    {
        var cut = Render<Sonner>();

        cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Type = (Sonner.SonnerType)999,
            Role = "presentation",
            AriaLive = "loud",
            Duration = 0
        });
        cut.Render();

        var toast = cut.Find(".sonner-toast");
        toast.ClassList.ShouldContain("sonner-default");
        toast.GetAttribute("role").ShouldBe("status");
        toast.GetAttribute("aria-live").ShouldBe("polite");
    }

    [Fact]
    public void Sonner_RendersTitleIconActionAndInvokesAction_WhenInteractive()
    {
        var actionCount = 0;
        var cut = Render<Sonner>();

        cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Title = "  Build complete  ",
            Description = builder => builder.AddContent(0, "Ready"),
            Icon = builder => builder.AddMarkupContent(0, "<span class='custom-icon'>!</span>"),
            Action = new Sonner.SonnerAction
            {
                Label = "  Retry  ",
                OnClick = () => actionCount++
            },
            Duration = 0
        });
        cut.Render();

        cut.Find(".sonner-title").TextContent.ShouldBe("Build complete");
        cut.Find(".sonner-icon").GetAttribute("aria-hidden").ShouldBe("true");
        cut.Find(".custom-icon").TextContent.ShouldBe("!");

        var action = cut.Find(".sonner-action");
        action.GetAttribute("type").ShouldBe("button");
        action.GetAttribute("aria-disabled").ShouldBe("false");
        action.TextContent.Trim().ShouldBe("Retry");

        action.Click();

        actionCount.ShouldBe(1);
    }

    [Fact]
    public void Sonner_HandlesNullAndEmptyContentWithoutBlankInteractiveControls()
    {
        var cut = Render<Sonner>();

        cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Title = " ",
            Action = new Sonner.SonnerAction { Label = " " },
            Duration = 0
        });
        cut.Render();

        var toast = cut.Find(".sonner-toast");
        toast.GetAttribute("aria-label").ShouldBe("Notification");
        cut.FindAll(".sonner-title").ShouldBeEmpty();
        cut.FindAll(".sonner-description").ShouldBeEmpty();
        cut.FindAll(".sonner-action").ShouldBeEmpty();
        cut.Find(".sonner-close").GetAttribute("aria-label").ShouldBe("Dismiss notification");
    }

    [Fact]
    public void Sonner_NonDismissibleToast_HidesCloseButton()
    {
        var cut = Render<Sonner>();

        cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Title = "Pinned",
            Dismissible = false,
            Duration = 0
        });
        cut.Render();

        cut.Find(".sonner-toast").GetAttribute("data-dismissible").ShouldBe("false");
        cut.FindAll(".sonner-close").ShouldBeEmpty();
    }

    [Fact]
    public void Sonner_CloseButtonDismissesToastAndInvokesCallbackOnce()
    {
        var dismissed = new List<Sonner.SonnerDismissEventArgs>();
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.OnDismiss, dismissed.Add));

        var toastId = cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Title = "Dismiss me",
            Duration = 0
        });
        cut.Render();

        cut.Find(".sonner-close").Click();
        cut.Instance.RemoveToast(toastId);

        dismissed.Count.ShouldBe(1);
        dismissed[0].ToastId.ShouldBe(toastId);
        dismissed[0].Toast.Title.ShouldBe("Dismiss me");
        dismissed[0].Reason.ShouldBe(Sonner.SonnerDismissReason.User);
        cut.FindAll(".sonner-toast").ShouldBeEmpty();
        cut.Find(".sonner-container").GetAttribute("data-state").ShouldBe("empty");
    }

    [Fact]
    public void Sonner_ProgrammaticRemoveInvokesDismissCallback()
    {
        Sonner.SonnerDismissEventArgs? dismissed = null;
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.OnDismiss, args => dismissed = args));

        var toastId = cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Title = "Remove me",
            Duration = 0
        });

        cut.Instance.RemoveToast(toastId);
        cut.Render();

        dismissed.ShouldNotBeNull();
        dismissed.ToastId.ShouldBe(toastId);
        dismissed.Reason.ShouldBe(Sonner.SonnerDismissReason.Programmatic);
        cut.FindAll(".sonner-toast").ShouldBeEmpty();
    }

    [Fact]
    public void Sonner_RespectsMaxToastsAndReportsCapacityDismissal()
    {
        var dismissed = new List<Sonner.SonnerDismissEventArgs>();
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.MaxToasts, 3)
            .Add(p => p.OnDismiss, dismissed.Add));

        for (var index = 0; index < 5; index++)
        {
            cut.Instance.Toast($"Message {index}");
        }
        cut.Render();

        var toasts = cut.FindAll(".sonner-toast");
        toasts.Count.ShouldBe(3);
        dismissed.Count.ShouldBe(2);
        dismissed.ShouldAllBe(args => args.Reason == Sonner.SonnerDismissReason.Capacity);
    }

    [Fact]
    public void Sonner_ClampsInvalidMaxToastsToOne()
    {
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.MaxToasts, 0));

        cut.Instance.Toast("One");
        cut.Instance.Toast("Two");
        cut.Render();

        cut.FindAll(".sonner-toast").Single().TextContent.ShouldContain("Two");
        cut.Find(".sonner-container").GetAttribute("data-toast-count").ShouldBe("1");
    }

    [Fact]
    public void Sonner_AutoDismissInvokesTimeoutReason()
    {
        var dismissed = new List<Sonner.SonnerDismissEventArgs>();
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.DefaultDuration, 20)
            .Add(p => p.OnDismiss, dismissed.Add));

        cut.Instance.Toast("Auto dismiss");
        cut.Render();

        cut.WaitForAssertion(() =>
        {
            cut.FindAll(".sonner-toast").ShouldBeEmpty();
            dismissed.Count.ShouldBe(1);
            dismissed[0].Reason.ShouldBe(Sonner.SonnerDismissReason.Timeout);
        }, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Sonner_NonPositiveDurationDisablesAutoDismiss()
    {
        var dismissed = new List<Sonner.SonnerDismissEventArgs>();
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.DefaultDuration, -1)
            .Add(p => p.OnDismiss, dismissed.Add));

        cut.Instance.Toast("Persistent");
        cut.Render();

        await Task.Delay(250);

        cut.FindAll(".sonner-toast").Count.ShouldBe(1);
        dismissed.ShouldBeEmpty();
    }

    [Fact]
    public async Task Sonner_DisposeCancelsAutoDismissWithoutDismissCallback()
    {
        var dismissed = new List<Sonner.SonnerDismissEventArgs>();
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.DefaultDuration, 20)
            .Add(p => p.OnDismiss, dismissed.Add));

        cut.Instance.Toast("Dispose me");
        cut.Instance.Dispose();

        await Task.Delay(250);

        dismissed.ShouldBeEmpty();
    }

    [Fact]
    public void Sonner_DisabledSuppressesNewToastsAndAddsDisabledState()
    {
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.Disabled, true));

        var toastId = cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Title = "Suppressed",
            Duration = 0
        });
        cut.Render();

        toastId.ShouldBe(string.Empty);
        var root = cut.Find(".sonner-container");
        root.ClassList.ShouldContain("sonner-disabled");
        root.GetAttribute("aria-disabled").ShouldBe("true");
        root.GetAttribute("data-state").ShouldBe("empty");
        cut.FindAll(".sonner-toast").ShouldBeEmpty();
    }

    [Fact]
    public void Sonner_ReadOnlyDisablesActionAndDismissWithoutSuppressingDisplay()
    {
        var actionCount = 0;
        var dismissCount = 0;
        var cut = Render<Sonner>(parameters => parameters
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnDismiss, _ => dismissCount++));

        cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Title = "Read only",
            Action = new Sonner.SonnerAction
            {
                Label = "Retry",
                OnClick = () => actionCount++
            },
            Duration = 0
        });
        cut.Render();

        cut.Find(".sonner-container").ClassList.ShouldContain("sonner-readonly");

        var action = cut.Find(".sonner-action");
        action.HasAttribute("disabled").ShouldBeTrue();
        action.GetAttribute("aria-disabled").ShouldBe("true");
        action.Click();

        var close = cut.Find(".sonner-close");
        close.HasAttribute("disabled").ShouldBeTrue();
        close.GetAttribute("aria-disabled").ShouldBe("true");
        close.Click();

        actionCount.ShouldBe(0);
        dismissCount.ShouldBe(0);
        cut.FindAll(".sonner-toast").Count.ShouldBe(1);
    }

    [Fact]
    public void Sonner_ToastsStateSnapshotIsCloned()
    {
        var cut = Render<Sonner>();

        var toastId = cut.Instance.ShowToast(new Sonner.SonnerToast
        {
            Title = "Original",
            Duration = 0
        });

        var snapshot = cut.Instance.Toasts;
        snapshot.Count.ShouldBe(1);
        snapshot[0].Id.ShouldBe(toastId);
        snapshot[0].Title = "Mutated";

        cut.Instance.Toasts[0].Title.ShouldBe("Original");
    }

    [Fact]
    public void Sonner_PromiseUpdatesLoadingToastToSuccess()
    {
        var taskCompletionSource = new TaskCompletionSource<int>();
        var cut = Render<Sonner>();

        cut.Instance.Promise(taskCompletionSource.Task, "Saving", "Saved", "Failed");
        cut.Render();

        var loading = cut.Find(".sonner-toast");
        loading.ClassList.ShouldContain("sonner-loading");
        loading.TextContent.ShouldContain("Saving");

        taskCompletionSource.SetResult(1);

        cut.WaitForAssertion(() =>
        {
            var toast = cut.Find(".sonner-toast");
            toast.ClassList.ShouldContain("sonner-success");
            toast.GetAttribute("role").ShouldBe("status");
            toast.TextContent.ShouldContain("Saved");
        }, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Sonner_PromiseUpdatesLoadingToastToError()
    {
        var taskCompletionSource = new TaskCompletionSource<int>();
        var cut = Render<Sonner>();

        cut.Instance.Promise(taskCompletionSource.Task, "Saving", "Saved", "Failed");
        cut.Render();

        taskCompletionSource.SetException(new InvalidOperationException("Nope"));

        cut.WaitForAssertion(() =>
        {
            var toast = cut.Find(".sonner-toast");
            toast.ClassList.ShouldContain("sonner-error");
            toast.GetAttribute("role").ShouldBe("alert");
            toast.GetAttribute("aria-live").ShouldBe("assertive");
            toast.TextContent.ShouldContain("Failed");
        }, TimeSpan.FromSeconds(1));
    }
}
