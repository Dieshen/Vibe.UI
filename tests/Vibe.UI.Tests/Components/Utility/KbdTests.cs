namespace Vibe.UI.Tests.Components.Utility;

public class KbdTests : TestBase
{
    [Fact]
    public void Kbd_RendersKeyboardElementWithDefaultSize()
    {
        var cut = Render<Kbd>(parameters => parameters
            .AddChildContent("Ctrl"));

        var kbd = cut.Find("kbd.vibe-kbd");
        kbd.TextContent.ShouldBe("Ctrl");
        kbd.ClassList.ShouldContain("kbd-default");
    }

    [Theory]
    [InlineData(Kbd.KbdSize.Small, "kbd-small")]
    [InlineData(Kbd.KbdSize.Large, "kbd-large")]
    public void Kbd_AppliesConfiguredSize(Kbd.KbdSize size, string expectedClass)
    {
        var cut = Render<Kbd>(parameters => parameters
            .Add(p => p.Size, size));

        cut.Find(".vibe-kbd").ClassList.ShouldContain(expectedClass);
    }

    [Fact]
    public void Kbd_AppliesCustomCssClassAndAttributes()
    {
        var cut = Render<Kbd>(parameters => parameters
            .Add(p => p.CssClass, "shortcut-key")
            .Add(p => p.Class, "outer-key")
            .AddUnmatched("aria-label", "Control key")
            .AddChildContent("Ctrl"));

        var kbd = cut.Find(".vibe-kbd");
        kbd.ClassList.ShouldContain("shortcut-key");
        kbd.ClassList.ShouldContain("outer-key");
        kbd.GetAttribute("aria-label").ShouldBe("Control key");
    }

    [Fact]
    public void Kbd_InvalidSizeFallsBackToDefaultSize()
    {
        var cut = Render<Kbd>(parameters => parameters
            .Add(p => p.Size, (Kbd.KbdSize)999)
            .AddChildContent("Esc"));

        var kbd = cut.Find(".vibe-kbd");
        kbd.ClassList.ShouldContain("kbd-default");
        kbd.ClassList.ShouldNotContain("kbd-999");
    }

    [Fact]
    public void Kbd_NullContentRendersEmptyKeyboardElement()
    {
        var cut = Render<Kbd>();

        var kbd = cut.Find("kbd.vibe-kbd");
        kbd.TextContent.ShouldBe(string.Empty);
        kbd.ClassList.ShouldContain("kbd-default");
    }
}
