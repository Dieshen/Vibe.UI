namespace Vibe.UI.Tests.Components.Overlay;

public class AlertDialogTests : TestBase
{
    [Fact]
    public void AlertDialog_DoesNotRender_WhenClosed()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters.Add(p => p.IsOpen, false).AddChildContent("Delete item?")
        );

        cut.FindAll(".vibe-alert-dialog").ShouldBeEmpty();
    }

    [Fact]
    public void AlertDialog_RendersModalAttributes_WhenOpen()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.Title, "Delete item")
                .Add(p => p.Description, "This action cannot be undone.")
                .AddChildContent("Delete item?")
        );

        var dialog = cut.Find(".vibe-alert-dialog");
        dialog.GetAttribute("role").ShouldBe("alertdialog");
        dialog.GetAttribute("aria-modal").ShouldBe("true");
        dialog.GetAttribute("tabindex").ShouldBe("-1");

        var title = cut.Find(".vibe-alert-dialog-title");
        var description = cut.Find(".vibe-alert-dialog-description");
        title.TextContent.ShouldBe("Delete item");
        description.TextContent.ShouldBe("This action cannot be undone.");
        dialog.GetAttribute("aria-labelledby").ShouldBe(title.GetAttribute("id"));
        dialog.GetAttribute("aria-describedby").ShouldBe(description.GetAttribute("id"));
    }

    [Fact]
    public void AlertDialog_RendersBodyAndFooterContent()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(
                    p => p.Footer,
                    builder => builder.AddMarkupContent(0, "<button>Confirm</button>")
                )
                .AddChildContent("<p class='dialog-body-copy'>Delete item?</p>")
        );

        cut.Find(".vibe-alert-dialog-body").InnerHtml.ShouldContain("dialog-body-copy");
        cut.Find(".vibe-alert-dialog-footer").TextContent.ShouldContain("Confirm");
    }

    [Fact]
    public void AlertDialog_OmitsHeader_WhenTitleIsEmpty()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.Title, string.Empty)
                .Add(p => p.Description, "This action cannot be undone.")
                .AddChildContent("Delete item?")
        );

        var dialog = cut.Find(".vibe-alert-dialog");
        cut.FindAll(".vibe-alert-dialog-header").ShouldBeEmpty();
        dialog.GetAttribute("aria-labelledby").ShouldBeNull();
        dialog.GetAttribute("aria-describedby").ShouldBeNull();
    }

    [Fact]
    public void AlertDialog_OmitsDescription_WhenDescriptionIsEmpty()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.Title, "Delete item")
                .Add(p => p.Description, string.Empty)
                .AddChildContent("Delete item?")
        );

        var dialog = cut.Find(".vibe-alert-dialog");
        var title = cut.Find(".vibe-alert-dialog-title");
        cut.FindAll(".vibe-alert-dialog-description").ShouldBeEmpty();
        dialog.GetAttribute("aria-labelledby").ShouldBe(title.GetAttribute("id"));
        dialog.GetAttribute("aria-describedby").ShouldBeNull();
    }

    [Fact]
    public void AlertDialog_CloseButtonInvokesIsOpenChanged()
    {
        bool? changedValue = null;
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.ShowCloseButton, true)
                .Add(p => p.IsOpenChanged, value => changedValue = value)
                .AddChildContent("Delete item?")
        );

        cut.Find(".vibe-alert-dialog-close").Click();

        changedValue.ShouldBe(false);
    }

    [Fact]
    public void AlertDialog_BackdropInvokesIsOpenChanged_WhenEnabled()
    {
        bool? changedValue = null;
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.CloseOnBackdropClick, true)
                .Add(p => p.IsOpenChanged, value => changedValue = value)
                .AddChildContent("Delete item?")
        );

        cut.Find(".vibe-alert-dialog-backdrop").Click();

        changedValue.ShouldBe(false);
    }

    [Fact]
    public void AlertDialog_BackdropDoesNotClose_WhenDisabled()
    {
        bool callbackInvoked = false;
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.CloseOnBackdropClick, false)
                .Add(p => p.IsOpenChanged, _ => callbackInvoked = true)
                .AddChildContent("Delete item?")
        );

        cut.Find(".vibe-alert-dialog-backdrop").Click();

        callbackInvoked.ShouldBeFalse();
    }

    [Fact]
    public void AlertDialog_EscapeInvokesIsOpenChanged_WhenEnabled()
    {
        bool? changedValue = null;
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.CloseOnEscape, true)
                .Add(p => p.IsOpenChanged, value => changedValue = value)
                .AddChildContent("Delete item?")
        );

        cut.Find(".vibe-alert-dialog").KeyDown("Escape");

        changedValue.ShouldBe(false);
    }

    [Fact]
    public void AlertDialog_EscapeDoesNotClose_WhenDisabled()
    {
        bool callbackInvoked = false;
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.CloseOnEscape, false)
                .Add(p => p.IsOpenChanged, _ => callbackInvoked = true)
                .AddChildContent("Delete item?")
        );

        cut.Find(".vibe-alert-dialog").KeyDown("Escape");

        callbackInvoked.ShouldBeFalse();
    }

    [Fact]
    public void AlertDialog_OmitsCloseButton_WhenAllCloseControlsAreDisabled()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.CloseOnBackdropClick, false)
                .Add(p => p.ShowCloseButton, false)
                .AddChildContent("Delete item?")
        );

        cut.FindAll(".vibe-alert-dialog-close").ShouldBeEmpty();
    }

    [Fact]
    public void AlertDialog_DefaultsExposeADismissAffordance()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters.Add(p => p.IsOpen, true).AddChildContent("Delete item?")
        );

        cut.FindAll(".vibe-alert-dialog-close").ShouldHaveSingleItem();
        cut.Instance.CloseOnBackdropClick.ShouldBeTrue();
        cut.Instance.ShowCloseButton.ShouldBeTrue();
    }

    [Fact]
    public void AlertDialog_CloseButtonUsesSharedIcon()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.ShowCloseButton, true)
                .AddChildContent("Delete item?")
        );

        cut.Find(".vibe-alert-dialog-close svg.vibe-icon").ShouldNotBeNull();
        cut.Markup.ShouldNotContain("&times;");
    }

    [Fact]
    public async Task AlertDialog_DocumentEscapeCallbackClosesWhenEnabled()
    {
        bool? changedValue = null;
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.IsOpenChanged, value => changedValue = value)
                .AddChildContent("Delete item?")
        );

        await cut.InvokeAsync(cut.Instance.HandleDialogEscape);

        changedValue.ShouldBe(false);
    }

    [Fact]
    public void AlertDialog_PreservesAdditionalAttributes()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(
                    p => p.AdditionalAttributes,
                    new Dictionary<string, object>
                    {
                        ["data-testid"] = "alert-dialog",
                        ["aria-label"] = "Confirm delete",
                    }
                )
                .AddChildContent("Delete item?")
        );

        var dialog = cut.Find(".vibe-alert-dialog");
        dialog.GetAttribute("data-testid").ShouldBe("alert-dialog");
        dialog.GetAttribute("aria-label").ShouldBe("Confirm delete");
        dialog.GetAttribute("aria-labelledby").ShouldBeNull();
    }

    [Fact]
    public void AlertDialog_AppliesCustomClass()
    {
        var cut = Render<AlertDialog>(parameters =>
            parameters
                .Add(p => p.IsOpen, true)
                .Add(p => p.Class, "danger-dialog")
                .AddChildContent("Delete item?")
        );

        cut.Find(".vibe-alert-dialog").ClassList.ShouldContain("danger-dialog");
    }
}
