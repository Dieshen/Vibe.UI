namespace Vibe.UI.Tests.Components.Utility;

public class ScrollAreaTests : TestBase
{
    [Fact]
    public void ScrollArea_RendersViewportContentAndBaseClass()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .AddChildContent("<p class='scroll-copy'>Scrollable content</p>"));

        cut.Find(".vibe-scroll-area").ShouldNotBeNull();
        cut.Find(".scroll-viewport").ShouldNotBeNull();
        cut.Find(".scroll-content .scroll-copy").TextContent.ShouldBe("Scrollable content");
    }

    [Fact]
    public void ScrollArea_AppliesHeightStyle()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.Height, " 320px "));

        cut.Find(".vibe-scroll-area").GetAttribute("style").ShouldNotBeNull().ShouldContain("height: 320px");
    }

    [Fact]
    public void ScrollArea_OmitsHeightStyle_WhenHeightIsEmpty()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.Height, string.Empty));

        (cut.Find(".vibe-scroll-area").GetAttribute("style") ?? string.Empty).ShouldBe(string.Empty);
    }

    [Fact]
    public void ScrollArea_RendersScrollbarByDefault()
    {
        var cut = Render<ScrollArea>();

        cut.Find(".scrollbar.vertical").ShouldNotBeNull();
        cut.Find(".scrollbar-thumb").ShouldNotBeNull();
    }

    [Fact]
    public void ScrollArea_OmitsScrollbar_WhenShowScrollbarIsFalse()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.ShowScrollbar, false));

        cut.FindAll(".scrollbar").ShouldBeEmpty();
    }

    [Fact]
    public void ScrollArea_OmitsScrollbar_WhenHideScrollbarsIsTrue()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.HideScrollbars, true));

        cut.FindAll(".scrollbar").ShouldBeEmpty();
    }

    [Fact]
    public void ScrollArea_AutoHideRendersScrollbarVisibleInitially()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.AutoHide, true));

        cut.Find(".scrollbar").ClassList.ShouldContain("visible");
        cut.Find(".vibe-scroll-area").ClassList.ShouldContain("auto-hide");
    }

    [Fact]
    public void ScrollArea_ScrollUpdatesThumbStyle()
    {
        var cut = Render<ScrollArea>();

        cut.Find(".scroll-viewport").TriggerEvent("onscroll", EventArgs.Empty);

        var thumb = cut.Find(".scrollbar-thumb");
        var style = thumb.GetAttribute("style").ShouldNotBeNull();
        style.ShouldContain("height: 30%;");
        style.ShouldContain("top: 0%;");
    }

    [Fact]
    public void ScrollArea_AutoHideHidesScrollbarAfterConfiguredDelay()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.AutoHide, true)
            .Add(p => p.AutoHideDelay, 1));

        cut.Find(".scroll-viewport").TriggerEvent("onscroll", EventArgs.Empty);

        cut.WaitForAssertion(() =>
        {
            cut.Find(".scrollbar").ClassList.ShouldNotContain("visible");
            cut.Find(".vibe-scroll-area").GetAttribute("data-state").ShouldBe("hidden");
        });
    }

    [Fact]
    public void ScrollArea_AppliesCustomClass()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.Class, "content-scroll-area")
            .Add(p => p.CssClass, "legacy-scroll-area")
            .AddUnmatched("data-testid", "scroll-area"));

        var scrollArea = cut.Find(".vibe-scroll-area");
        scrollArea.ClassList.ShouldContain("content-scroll-area");
        scrollArea.ClassList.ShouldContain("legacy-scroll-area");
        scrollArea.GetAttribute("data-testid").ShouldBe("scroll-area");
    }

    [Fact]
    public void ScrollArea_MergesAdditionalClassAndStyle()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.Class, "content-scroll-area")
            .Add(p => p.Height, "320px")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-scroll-area",
                ["style"] = "max-width: 20rem;"
            }));

        var scrollArea = cut.Find(".vibe-scroll-area");
        scrollArea.ClassList.ShouldContain("content-scroll-area");
        scrollArea.ClassList.ShouldContain("attribute-scroll-area");

        var style = scrollArea.GetAttribute("style")!;
        style.ShouldContain("max-width: 20rem");
        style.ShouldContain("height: 320px");
    }

    [Fact]
    public void ScrollArea_WithAriaLabelAddsRegionSemantics()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.AriaLabel, "Activity log"));

        var scrollArea = cut.Find(".vibe-scroll-area");
        scrollArea.GetAttribute("role").ShouldBe("region");
        scrollArea.GetAttribute("aria-label").ShouldBe("Activity log");
    }

    [Fact]
    public void ScrollArea_WithCallerProvidedSemanticsPreservesCallerValues()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.AriaLabel, "Parameter label")
            .Add(p => p.Role, "region")
            .AddUnmatched("role", "feed")
            .AddUnmatched("aria-label", "Caller label"));

        var scrollArea = cut.Find(".vibe-scroll-area");
        scrollArea.GetAttribute("role").ShouldBe("feed");
        scrollArea.GetAttribute("aria-label").ShouldBe("Caller label");
    }

    [Fact]
    public void ScrollArea_FocusableAddsViewportTabIndex()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.Focusable, true));

        cut.Find(".scroll-viewport").GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact]
    public void ScrollArea_HideScrollbarsAddsStateClassAndData()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.HideScrollbars, true));

        var scrollArea = cut.Find(".vibe-scroll-area");
        scrollArea.ClassList.ShouldContain("hide-scrollbar");
        scrollArea.GetAttribute("data-scrollbar").ShouldBe("hidden");
    }

    [Fact]
    public async Task ScrollArea_DisposeCancelsPendingAutoHide()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.AutoHide, true)
            .Add(p => p.AutoHideDelay, 10));

        cut.Find(".scroll-viewport").TriggerEvent("onscroll", EventArgs.Empty);
        await cut.Instance.DisposeAsync();
        await Task.Delay(30);

        cut.Find(".scrollbar").ClassList.ShouldContain("visible");
    }
}
