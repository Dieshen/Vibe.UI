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
            .Add(p => p.Height, "320px"));

        cut.Find(".vibe-scroll-area").GetAttribute("style").ShouldNotBeNull().ShouldContain("height: 320px;");
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
    public void ScrollArea_AppliesCustomClass()
    {
        var cut = Render<ScrollArea>(parameters => parameters
            .Add(p => p.Class, "content-scroll-area"));

        cut.Find(".vibe-scroll-area").ClassList.ShouldContain("content-scroll-area");
    }
}
