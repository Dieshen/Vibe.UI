using Vibe.UI.Services.Dialog;

namespace Vibe.UI.Tests.Components.Overlay;

public class DialogContainerTests : TestBase
{
    [Fact]
    public void DialogContainer_DoesNotRenderDialogInitially()
    {
        var cut = Render<DialogContainer>();

        cut.FindAll(".vibe-dialog-host").ShouldBeEmpty();
    }

    [Fact]
    public void DialogContainer_RendersDialogWhenServiceOpens()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<DialogContainer>();

        _ = service.ShowCustomAsync("Confirm", builder => builder.AddContent(0, "Confirm body"));

        cut.WaitForAssertion(() =>
        {
            var dialog = cut.Find(".vibe-dialog-container");
            dialog.GetAttribute("role").ShouldBe("dialog");
            dialog.GetAttribute("aria-modal").ShouldBe("true");
            dialog.GetAttribute("aria-labelledby").ShouldStartWith("vibe-dialog-title-");
            cut.Find(".vibe-dialog-title").TextContent.ShouldBe("Confirm");
            cut.Find(".vibe-dialog-body").TextContent.ShouldContain("Confirm body");
        });
    }

    [Fact]
    public void DialogContainer_CloseButtonClosesServiceDialog()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<DialogContainer>();
        var task = service.ShowCustomAsync("Closable", builder => builder.AddContent(0, "Body"));
        cut.WaitForAssertion(() => cut.Find(".vibe-dialog-close").ShouldNotBeNull());

        cut.Find(".vibe-dialog-close").Click();

        task.IsCompletedSuccessfully.ShouldBeTrue();
        cut.WaitForAssertion(() => cut.FindAll(".vibe-dialog-host").ShouldBeEmpty());
    }

    [Fact]
    public void DialogContainer_BackdropClickHonorsCloseSetting()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<DialogContainer>(parameters => parameters
            .Add(p => p.CloseOnBackdropClick, false));

        _ = service.ShowCustomAsync("Sticky", builder => builder.AddContent(0, "Body"));
        cut.WaitForAssertion(() => cut.Find(".vibe-dialog-overlay").ShouldNotBeNull());

        cut.Find(".vibe-dialog-overlay").Click();

        cut.Find(".vibe-dialog-host").ShouldNotBeNull();
    }

    [Fact]
    public void DialogContainer_HidesCloseButtonWhenDisabled()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<DialogContainer>(parameters => parameters
            .Add(p => p.ShowCloseButton, false));

        _ = service.ShowCustomAsync("No Close", builder => builder.AddContent(0, "Body"));

        cut.WaitForAssertion(() => cut.Find(".vibe-dialog-header").ShouldNotBeNull());
        cut.FindAll(".vibe-dialog-close").ShouldBeEmpty();
    }

    [Fact]
    public void DialogContainer_PreservesCustomClassAndAttributes()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<DialogContainer>(parameters => parameters
            .Add(p => p.Class, "global-dialog-host")
            .AddUnmatched("data-host", "dialog"));

        _ = service.ShowCustomAsync("Host", builder => builder.AddContent(0, "Body"));

        cut.WaitForAssertion(() =>
        {
            var host = cut.Find(".vibe-dialog-host");
            host.ClassList.ShouldContain("global-dialog-host");
            host.GetAttribute("data-host").ShouldBe("dialog");
        });
    }
}
