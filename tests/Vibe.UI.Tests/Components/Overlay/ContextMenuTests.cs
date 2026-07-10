using Microsoft.AspNetCore.Components.Web;

namespace Vibe.UI.Tests.Components.Overlay;

public class ContextMenuTests : TestBase
{
    [Fact]
    public void ContextMenu_RendersTriggerAndBaseClass()
    {
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Right click me")));

        var root = cut.Find(".vibe-context-menu");
        root.ClassList.ShouldContain("vibe-context-menu");
        root.GetAttribute("data-state").ShouldBe("closed");

        var trigger = cut.Find(".context-trigger");
        trigger.TextContent.ShouldContain("Right click me");
        trigger.GetAttribute("role").ShouldBe("button");
        trigger.GetAttribute("tabindex").ShouldBe("0");
        trigger.GetAttribute("aria-haspopup").ShouldBe("menu");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-disabled").ShouldBe("false");
        trigger.GetAttribute("aria-controls").ShouldNotBeNullOrWhiteSpace();
        trigger.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact]
    public void ContextMenu_DoesNotRenderContentInitially()
    {
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu content")));

        cut.FindAll(".context-content").ShouldBeEmpty();
        cut.FindAll(".context-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenu_OpensAtPointerPositionAndRendersMenuSemantics()
    {
        MouseEventArgs? openedArgs = null;
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<button>Rename</button>"))
            .Add(p => p.OnOpen, args => openedArgs = args));

        OpenWithContextMenu(cut, 24.5, 32);

        var root = cut.Find(".vibe-context-menu");
        root.ClassList.ShouldContain("context-open");
        root.GetAttribute("data-state").ShouldBe("open");

        var trigger = cut.Find(".context-trigger");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
        trigger.GetAttribute("data-state").ShouldBe("open");

        var content = cut.Find(".context-content");
        content.TextContent.ShouldContain("Rename");
        content.GetAttribute("id").ShouldBe(trigger.GetAttribute("aria-controls"));
        content.GetAttribute("role").ShouldBe("menu");
        content.GetAttribute("tabindex").ShouldBe("-1");
        content.GetAttribute("aria-label").ShouldBe("Context menu");
        content.GetAttribute("data-state").ShouldBe("open");
        content.GetAttribute("style").ShouldBe("left: 24.5px; top: 32px;");

        var backdrop = cut.Find(".context-backdrop");
        backdrop.GetAttribute("aria-hidden").ShouldBe("true");
        openedArgs.ShouldNotBeNull();
        openedArgs.ClientX.ShouldBe(24.5);
        openedArgs.ClientY.ShouldBe(32);
    }

    [Fact]
    public void ContextMenu_RepositionsOpenMenuWithoutRepeatingOpenCallback()
    {
        var openCount = 0;
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu content"))
            .Add(p => p.OnOpen, _ => openCount++));

        OpenWithContextMenu(cut, 8, 12);
        OpenWithContextMenu(cut, 44, 56);

        openCount.ShouldBe(1);
        cut.Find(".context-content").GetAttribute("style").ShouldBe("left: 44px; top: 56px;");
    }

    [Fact]
    public void ContextMenu_BackdropClosesAndInvokesOnCloseOnce()
    {
        var closeCount = 0;
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu content"))
            .Add(p => p.OnClose, () => closeCount++));

        cut.Find(".context-trigger").KeyDown("Escape");
        closeCount.ShouldBe(0);

        OpenWithContextMenu(cut);
        cut.Find(".context-backdrop").Click();

        closeCount.ShouldBe(1);
        cut.FindAll(".context-content").ShouldBeEmpty();
        cut.Find(".context-trigger").GetAttribute("aria-expanded").ShouldBe("false");

        cut.Find(".context-trigger").KeyDown("Escape");
        closeCount.ShouldBe(1);
    }

    [Fact]
    public void ContextMenu_LeftMouseDownOnTriggerClosesOpenMenu()
    {
        var closeCount = 0;
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu content"))
            .Add(p => p.OnClose, () => closeCount++));

        OpenWithContextMenu(cut);
        cut.Find(".context-trigger").TriggerEvent("onmousedown", new MouseEventArgs { Button = 0 });

        closeCount.ShouldBe(1);
        cut.FindAll(".context-content").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenu_RespectsCloseOnClickOutside()
    {
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.CloseOnClickOutside, false)
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu content")));

        OpenWithContextMenu(cut);

        cut.Find(".context-content").ShouldNotBeNull();
        cut.FindAll(".context-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenu_AppliesCustomMenuAriaLabel()
    {
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.MenuAriaLabel, "File actions")
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu content")));

        OpenWithContextMenu(cut);

        cut.Find(".context-content").GetAttribute("aria-label").ShouldBe("File actions");
    }

    [Fact]
    public void ContextMenu_OpensAndClosesWithKeyboard()
    {
        var openCount = 0;
        var closeCount = 0;
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Keyboard menu"))
            .Add(p => p.OnOpen, _ => openCount++)
            .Add(p => p.OnClose, () => closeCount++));

        cut.Find(".context-trigger").KeyDown("Enter");

        openCount.ShouldBe(1);
        cut.Find(".context-content").TextContent.ShouldContain("Keyboard menu");
        cut.Find(".context-content").GetAttribute("style").ShouldBe("left: 0px; top: 0px;");

        cut.Find(".context-content").KeyDown("Escape");

        closeCount.ShouldBe(1);
        cut.FindAll(".context-content").ShouldBeEmpty();

        cut.Find(".context-trigger").KeyDown(" ");
        openCount.ShouldBe(2);
        cut.Find(".context-trigger").KeyDown("Escape");
        closeCount.ShouldBe(2);
    }

    [Fact]
    public void ContextMenu_OpensWithContextMenuKeyboardShortcuts()
    {
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Keyboard menu")));

        cut.Find(".context-trigger").TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ContextMenu" });
        cut.Find(".context-content").ShouldNotBeNull();

        cut.Find(".context-content").KeyDown("Escape");
        cut.Find(".context-trigger").TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "F10", ShiftKey = true });

        cut.Find(".context-content").ShouldNotBeNull();
    }

    [Fact]
    public void ContextMenu_DisabledStatePreventsPointerAndKeyboardOpen()
    {
        var openCount = 0;
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.Content, builder => builder.AddContent(0, "Disabled menu"))
            .Add(p => p.OnOpen, _ => openCount++));

        var root = cut.Find(".vibe-context-menu");
        root.ClassList.ShouldContain("context-disabled");

        var trigger = cut.Find(".context-trigger");
        trigger.GetAttribute("tabindex").ShouldBe("-1");
        trigger.GetAttribute("aria-disabled").ShouldBe("true");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");

        OpenWithContextMenu(cut);
        cut.Find(".context-trigger").KeyDown("Enter");
        cut.Find(".context-trigger").TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "F10", ShiftKey = true });

        openCount.ShouldBe(0);
        cut.FindAll(".context-content").ShouldBeEmpty();
    }

    [Fact]
    public void ContextMenu_PreservesAdditionalAttributesAndCustomClass()
    {
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Class, "compact-context-menu")
            .AddUnmatched("data-testid", "context-menu")
            .AddUnmatched("aria-label", "File actions"));

        var root = cut.Find(".vibe-context-menu");
        root.ClassList.ShouldContain("compact-context-menu");
        root.GetAttribute("data-testid").ShouldBe("context-menu");
        root.GetAttribute("aria-label").ShouldBe("File actions");
    }

    [Fact]
    public void ContextMenu_RendersEmptyMenu_WhenContentIsNull()
    {
        var cut = Render<ContextMenu>();

        cut.Find(".context-trigger").TextContent.ShouldBe(string.Empty);
        OpenWithContextMenu(cut);

        var content = cut.Find(".context-content");
        content.TextContent.ShouldBe(string.Empty);
        content.GetAttribute("role").ShouldBe("menu");
    }

    [Fact]
    public void ContextMenu_ClampsInvalidPointerCoordinates()
    {
        var cut = Render<ContextMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu content")));

        OpenWithContextMenu(cut, -12, double.NaN);

        cut.Find(".context-content").GetAttribute("style").ShouldBe("left: 0px; top: 0px;");
    }

    private static void OpenWithContextMenu(IRenderedComponent<ContextMenu> cut, double clientX = 12, double clientY = 18)
    {
        cut.Find(".context-trigger").TriggerEvent("oncontextmenu", new MouseEventArgs
        {
            Button = 2,
            ClientX = clientX,
            ClientY = clientY
        });
    }
}
