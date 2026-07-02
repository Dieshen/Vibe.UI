namespace Vibe.UI.Tests.Components.Utility;

public class KbdTests : TestBase
{
    [Fact]
    public void Kbd_RendersKeyboardElementWithDefaultSize()
    {
        var cut = RenderComponent<Kbd>(parameters => parameters
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
        var cut = RenderComponent<Kbd>(parameters => parameters
            .Add(p => p.Size, size));

        cut.Find(".vibe-kbd").ClassList.ShouldContain(expectedClass);
    }

    [Fact]
    public void Kbd_AppliesCustomCssClassAndAttributes()
    {
        var cut = RenderComponent<Kbd>(parameters => parameters
            .Add(p => p.CssClass, "shortcut-key")
            .AddUnmatched("aria-label", "Control key")
            .AddChildContent("Ctrl"));

        var kbd = cut.Find(".vibe-kbd");
        kbd.ClassList.ShouldContain("shortcut-key");
        kbd.GetAttribute("aria-label").ShouldBe("Control key");
    }
}
