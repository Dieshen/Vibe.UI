namespace Vibe.UI.Tests.Components.Overlay;

public class DrawerTests : TestBase
{
    [Fact]
    public void Drawer_DoesNotRender_WhenClosed()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, false));

        // Assert
        cut.FindAll(".vibe-drawer").ShouldBeEmpty();
    }

    [Fact]
    public void Drawer_Renders_WhenOpen()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var drawer = cut.Find(".vibe-drawer");
        drawer.ShouldNotBeNull();
        drawer.GetAttribute("data-state").ShouldBe("open");
    }

    [Fact]
    public void Drawer_Displays_ChildContent()
    {
        // Arrange
        var content = "Drawer Content";

        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ChildContent, builder => builder.AddContent(0, content)));

        // Assert
        var body = cut.Find(".drawer-body");
        body.TextContent.ShouldContain(content);
    }

    [Fact]
    public void Drawer_Applies_SideClass()
    {
        // Arrange
        var side = "left";

        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Side, side));

        // Assert
        var drawer = cut.Find(".vibe-drawer");
        drawer.ClassList.ShouldContain($"drawer-{side}");
    }

    [Fact]
    public void Drawer_Applies_DefaultSide()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var drawer = cut.Find(".vibe-drawer");
        drawer.ClassList.ShouldContain("drawer-right");
    }

    [Fact]
    public void Drawer_FallsBackToRightSide_WhenSideIsInvalid()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Side, "invalid"));

        // Assert
        var drawer = cut.Find(".vibe-drawer");
        drawer.ClassList.ShouldContain("drawer-right");
        drawer.ClassList.ShouldNotContain("drawer-invalid");
    }

    [Fact]
    public void Drawer_Shows_CloseButton_WhenEnabled()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, true));

        // Assert
        var closeButton = cut.Find(".drawer-close");
        closeButton.ShouldNotBeNull();
        closeButton.GetAttribute("type").ShouldBe("button");
        closeButton.GetAttribute("aria-label").ShouldBe("Close");
    }

    [Fact]
    public void Drawer_Hides_CloseButton_WhenDisabled()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ShowCloseButton, false));

        // Assert
        cut.FindAll(".drawer-close").ShouldBeEmpty();
    }

    [Fact]
    public void Drawer_Renders_Overlay()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var overlay = cut.Find(".drawer-overlay");
        overlay.ShouldNotBeNull();
    }

    [Fact]
    public void Drawer_Renders_Content()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true));

        // Assert
        var content = cut.Find(".drawer-content");
        content.ShouldNotBeNull();
        content.GetAttribute("role").ShouldBe("dialog");
        content.GetAttribute("aria-modal").ShouldBe("true");
        content.GetAttribute("aria-label").ShouldBe("Drawer");
        content.GetAttribute("tabindex").ShouldBe("-1");
    }

    [Fact]
    public void Drawer_AppliesDialogLabellingAttributes()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.AriaLabel, "Navigation")
            .Add(p => p.AriaLabelledBy, "drawer-title")
            .Add(p => p.AriaDescribedBy, "drawer-description"));

        // Assert
        var content = cut.Find(".drawer-content");
        content.GetAttribute("aria-labelledby").ShouldBe("drawer-title");
        content.GetAttribute("aria-describedby").ShouldBe("drawer-description");
        content.GetAttribute("aria-label").ShouldBeNull();
    }

    [Fact]
    public void Drawer_AppliesAriaLabel_WhenNoLabelledByIsProvided()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.AriaLabel, "Navigation"));

        // Assert
        cut.Find(".drawer-content").GetAttribute("aria-label").ShouldBe("Navigation");
    }

    [Fact]
    public void Drawer_PreservesCustomClassAndAttributes_OnRoot()
    {
        // Act
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.Class, "drawer-wide")
            .AddUnmatched("data-testid", "drawer-root"));

        // Assert
        var drawer = cut.Find(".vibe-drawer");
        drawer.ClassList.ShouldContain("drawer-wide");
        drawer.GetAttribute("data-testid").ShouldBe("drawer-root");
    }

    [Fact]
    public void Drawer_CloseButtonInvokesIsOpenChanged_WithoutMutatingParameter()
    {
        // Arrange
        bool? changedValue = null;
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.IsOpenChanged, value => changedValue = value));

        // Act
        cut.Find(".drawer-close").Click();

        // Assert
        cut.WaitForAssertion(() => changedValue.ShouldBe(false));
        cut.Instance.IsOpen.ShouldBeTrue();
        cut.FindAll(".vibe-drawer").ShouldBeEmpty();
    }

    [Fact]
    public void Drawer_OverlayClickInvokesIsOpenChanged_WhenEnabled()
    {
        // Arrange
        bool? changedValue = null;
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOverlayClick, true)
            .Add(p => p.IsOpenChanged, value => changedValue = value));

        // Act
        cut.Find(".drawer-overlay").Click();

        // Assert
        cut.WaitForAssertion(() => changedValue.ShouldBe(false));
    }

    [Fact]
    public void Drawer_OverlayClickDoesNotClose_WhenDisabled()
    {
        // Arrange
        var callbackInvoked = false;
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnOverlayClick, false)
            .Add(p => p.IsOpenChanged, _ => callbackInvoked = true));

        // Act
        cut.Find(".drawer-overlay").Click();

        // Assert
        callbackInvoked.ShouldBeFalse();
        cut.Find(".vibe-drawer").ShouldNotBeNull();
    }

    [Fact]
    public void Drawer_EscapeInvokesIsOpenChanged_WhenEnabled()
    {
        // Arrange
        bool? changedValue = null;
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnEscape, true)
            .Add(p => p.IsOpenChanged, value => changedValue = value));

        // Act
        cut.Find(".vibe-drawer").KeyDown("Escape");

        // Assert
        cut.WaitForAssertion(() => changedValue.ShouldBe(false));
    }

    [Fact]
    public void Drawer_EscapeDoesNotClose_WhenDisabled()
    {
        // Arrange
        var callbackInvoked = false;
        var cut = Render<Drawer>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.CloseOnEscape, false)
            .Add(p => p.IsOpenChanged, _ => callbackInvoked = true));

        // Act
        cut.Find(".vibe-drawer").KeyDown("Escape");

        // Assert
        callbackInvoked.ShouldBeFalse();
        cut.Find(".vibe-drawer").ShouldNotBeNull();
    }
}
