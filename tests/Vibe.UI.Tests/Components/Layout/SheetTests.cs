namespace Vibe.UI.Tests.Components.Layout;

public class SheetTests : TestBase
{
    private static readonly TimeSpan TransitionTimeout = TimeSpan.FromSeconds(5);

    [Fact]
    public void Sheet_DoesNotRender_WhenClosed()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, false));

        // Assert
        cut.FindAll(".vibe-sheet").ShouldBeEmpty();
    }

    [Fact]
    public void Sheet_Renders_WhenOpen()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var sheet = cut.Find(".vibe-sheet");
        sheet.ShouldNotBeNull();
    }

    [Fact]
    public void Sheet_Hides_Overlay_WhenShowOverlayFalse()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowOverlay, false));

        // Assert
        cut.Find(".sheet-content").ShouldNotBeNull();
        cut.FindAll(".sheet-overlay").ShouldBeEmpty();
    }

    [Fact]
    public void Sheet_Displays_Title()
    {
        // Arrange
        var title = "Sheet Title";

        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Title, title));

        // Assert
        var titleElement = cut.Find(".sheet-title");
        titleElement.TextContent.ShouldBe(title);
    }

    [Fact]
    public void Sheet_Displays_Description()
    {
        // Arrange
        var description = "Sheet Description";

        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Description, description));

        // Assert
        var descElement = cut.Find(".sheet-description");
        descElement.TextContent.ShouldBe(description);
    }

    [Fact]
    public void Sheet_Wires_TitleAndDescription_AccessibilityAttributes()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Title, "Sheet Title")
            .Add(p => p.Description, "Sheet Description"));

        // Assert
        var dialog = cut.Find(".sheet-content");
        var title = cut.Find(".sheet-title");
        var description = cut.Find(".sheet-description");

        dialog.GetAttribute("role").ShouldBe("dialog");
        dialog.GetAttribute("aria-modal").ShouldBe("true");
        dialog.GetAttribute("tabindex").ShouldBe("-1");
        dialog.GetAttribute("aria-labelledby").ShouldBe(title.GetAttribute("id"));
        dialog.GetAttribute("aria-describedby").ShouldBe(description.GetAttribute("id"));
    }

    [Fact]
    public void Sheet_Applies_SideClass()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Side, Sheet.SheetSide.Left));

        // Assert
        var sheet = cut.Find(".vibe-sheet");
        sheet.ClassList.ShouldContain("sheet-left");
    }

    [Fact]
    public void Sheet_Shows_CloseButton_WhenEnabled()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, true));

        // Assert
        var closeButton = cut.Find(".sheet-close");
        closeButton.ShouldNotBeNull();
    }

    [Fact]
    public void Sheet_CloseButtonInvokesIsOpenChanged()
    {
        // Arrange
        bool? changedValue = null;
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.IsOpenChanged, value => changedValue = value));

        // Act
        cut.Find(".sheet-close").Click();

        // Assert
        cut.WaitForAssertion(() => changedValue.ShouldBe(false), TransitionTimeout);
    }

    [Fact]
    public void Sheet_Hides_CloseButton_WhenDisabled()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, false));

        // Assert
        cut.FindAll(".sheet-close").ShouldBeEmpty();
    }

    [Fact]
    public void Sheet_OverlayClickInvokesIsOpenChanged_WhenEnabled()
    {
        // Arrange
        bool? changedValue = null;
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOverlayClick, true)
            .Add(p => p.IsOpenChanged, value => changedValue = value));

        // Act
        cut.Find(".sheet-overlay").Click();

        // Assert
        cut.WaitForAssertion(() => changedValue.ShouldBe(false), TransitionTimeout);
    }

    [Fact]
    public void Sheet_OverlayClickDoesNotClose_WhenDisabled()
    {
        // Arrange
        var callbackInvoked = false;
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOverlayClick, false)
            .Add(p => p.IsOpenChanged, _ => callbackInvoked = true));

        // Act
        cut.Find(".sheet-overlay").Click();

        // Assert
        callbackInvoked.ShouldBeFalse();
    }

    [Fact]
    public void Sheet_EscapeInvokesIsOpenChanged_WhenEnabled()
    {
        // Arrange
        bool? changedValue = null;
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnEscape, true)
            .Add(p => p.IsOpenChanged, value => changedValue = value));

        // Act
        cut.Find(".sheet-content").KeyDown("Escape");

        // Assert
        cut.WaitForAssertion(() => changedValue.ShouldBe(false), TransitionTimeout);
    }

    [Fact]
    public void Sheet_EscapeDoesNotClose_WhenDisabled()
    {
        // Arrange
        var callbackInvoked = false;
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnEscape, false)
            .Add(p => p.IsOpenChanged, _ => callbackInvoked = true));

        // Act
        cut.Find(".sheet-content").KeyDown("Escape");

        // Assert
        callbackInvoked.ShouldBeFalse();
    }

    [Fact]
    public void Sheet_Displays_ChildContent()
    {
        // Arrange
        var content = "Sheet Content";

        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var body = cut.Find(".sheet-body");
        body.TextContent.ShouldContain(content);
    }

    [Fact]
    public void Sheet_Displays_Footer_WhenProvided()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Footer, builder => builder.AddContent(0, "Footer")));

        // Assert
        var footer = cut.Find(".sheet-footer");
        footer.ShouldNotBeNull();
    }

    [Fact]
    public void Sheet_Applies_SizeStyle()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Size, Sheet.SheetSize.Large));

        // Assert
        var content = cut.Find(".sheet-content");
        content.GetAttribute("style")!.ShouldContain("--sheet-size");
    }

    [Fact]
    public void Sheet_PreservesAdditionalAttributes_OnDialogElement()
    {
        // Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "sheet-dialog",
                ["aria-label"] = "Custom sheet label"
            }));

        // Assert
        var dialog = cut.Find(".sheet-content");
        dialog.GetAttribute("data-testid").ShouldBe("sheet-dialog");
        dialog.GetAttribute("aria-label").ShouldBe("Custom sheet label");
    }
}
