namespace Vibe.UI.Tests.Components.Navigation;

public class TabItemTests : TestBase
{
    [Fact]
    public void TabItem_RendersTabPanelWithProvidedId()
    {
        var cut = Render<TabItem>(parameters => parameters
            .Add(p => p.Id, "overview")
            .AddChildContent("Overview content"));

        var panel = cut.Find(".vibe-tab-item");
        panel.GetAttribute("role").ShouldBe("tabpanel");
        panel.GetAttribute("id").ShouldBe("tabpanel-overview");
        panel.GetAttribute("aria-labelledby").ShouldBe("tab-overview");
        panel.HasAttribute("hidden").ShouldBeTrue();
        panel.TextContent.ShouldBe("Overview content");
    }

    [Fact]
    public void TabItem_GeneratesId_WhenMissing()
    {
        var cut = Render<TabItem>();

        var panelId = cut.Find(".vibe-tab-item").GetAttribute("id");
        panelId.ShouldNotBeNullOrWhiteSpace();
        panelId.ShouldStartWith("tabpanel-");
        panelId.Length.ShouldBeGreaterThan("tabpanel-".Length);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void TabItem_UsesHiddenAttributeForActiveState(bool isActive, bool expectedHidden)
    {
        var cut = Render<TabItem>(parameters => parameters
            .Add(p => p.Id, "details")
            .Add(p => p.IsActive, isActive));

        cut.Find(".vibe-tab-item").HasAttribute("hidden").ShouldBe(expectedHidden);
    }

    [Fact]
    public void TabItem_PreservesMetadataAndAttributes()
    {
        var cut = Render<TabItem>(parameters => parameters
            .Add(p => p.Id, "settings")
            .Add(p => p.Header, "Settings")
            .Add(p => p.Icon, "gear")
            .Add(p => p.Disabled, true)
            .Add(p => p.DefaultActive, true)
            .Add(p => p.Class, "settings-panel")
            .AddUnmatched("data-tab", "settings"));

        cut.Instance.Header.ShouldBe("Settings");
        cut.Instance.Icon.ShouldBe("gear");
        cut.Instance.Disabled.ShouldBeTrue();
        cut.Instance.DefaultActive.ShouldBeTrue();
        var panel = cut.Find(".vibe-tab-item");
        panel.ClassList.ShouldContain("settings-panel");
        panel.GetAttribute("data-tab").ShouldBe("settings");
    }

    [Fact]
    public void TabItem_DoesNotRenderOwnPanel_WhenRegisteredWithTabs()
    {
        var cut = Render<Tabs>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<TabItem>(0);
                builder.AddAttribute(1, "Id", "overview");
                builder.AddAttribute(2, "Header", "Overview");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Overview content")));
                builder.CloseComponent();
            }));

        cut.WaitForAssertion(() => cut.FindAll(".vibe-tabs-panel").Count.ShouldBe(1));
        cut.FindAll(".vibe-tab-item").Count.ShouldBe(1);
        cut.Find(".vibe-tab-item").GetAttribute("id").ShouldBe("vibe-tabpanel-overview");
    }
}
