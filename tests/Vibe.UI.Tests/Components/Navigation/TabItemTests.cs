namespace Vibe.UI.Tests.Components.Navigation;

public class TabItemTests : TestBase
{
    [Fact]
    public void TabItem_RendersTabPanelWithProvidedId()
    {
        var cut = RenderComponent<TabItem>(parameters => parameters
            .Add(p => p.Id, "overview")
            .AddChildContent("Overview content"));

        var panel = cut.Find(".vibe-tab-item");
        panel.GetAttribute("role").ShouldBe("tabpanel");
        panel.GetAttribute("id").ShouldBe("tabpanel-overview");
        panel.GetAttribute("aria-labelledby").ShouldBe("tab-overview");
        panel.TextContent.ShouldBe("Overview content");
    }

    [Fact]
    public void TabItem_GeneratesId_WhenMissing()
    {
        var cut = RenderComponent<TabItem>();

        cut.Instance.Id.ShouldNotBeNullOrWhiteSpace();
        cut.Find(".vibe-tab-item").GetAttribute("id").ShouldBe($"tabpanel-{cut.Instance.Id}");
    }

    [Theory]
    [InlineData(true, "display: block")]
    [InlineData(false, "display: none")]
    public void TabItem_UsesDisplayStyleForActiveState(bool isActive, string expectedStyle)
    {
        var cut = RenderComponent<TabItem>(parameters => parameters
            .Add(p => p.Id, "details")
            .Add(p => p.IsActive, isActive));

        cut.Find(".vibe-tab-item").GetAttribute("style").ShouldBe(expectedStyle);
    }

    [Fact]
    public void TabItem_PreservesMetadataAndAttributes()
    {
        var cut = RenderComponent<TabItem>(parameters => parameters
            .Add(p => p.Id, "settings")
            .Add(p => p.Header, "Settings")
            .Add(p => p.Icon, "gear")
            .Add(p => p.Disabled, true)
            .Add(p => p.Class, "settings-panel")
            .AddUnmatched("data-tab", "settings"));

        cut.Instance.Header.ShouldBe("Settings");
        cut.Instance.Icon.ShouldBe("gear");
        cut.Instance.Disabled.ShouldBeTrue();
        var panel = cut.Find(".vibe-tab-item");
        panel.ClassList.ShouldContain("settings-panel");
        panel.GetAttribute("data-tab").ShouldBe("settings");
    }
}
