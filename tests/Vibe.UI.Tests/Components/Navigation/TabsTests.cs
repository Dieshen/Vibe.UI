namespace Vibe.UI.Tests.Components.Navigation;

public class TabsTests : TestBase
{
    [Fact]
    public void Tabs_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Tabs>();

        // Assert
        var tabs = cut.Find(".vibe-tabs");
        tabs.ShouldNotBeNull();
        cut.Find(".vibe-tabs-list").ShouldNotBeNull();
        cut.Find(".vibe-tabs-content").ShouldNotBeNull();
    }

    [Fact]
    public void Tabs_Renders_TabList_WithRole()
    {
        // Act
        var cut = Render<Tabs>();

        // Assert
        var tabList = cut.Find(".vibe-tabs-list");
        tabList.GetAttribute("role")!.ShouldBe("tablist");
    }

    [Fact]
    public void Tabs_RendersDeclarativeTabItems_AsTriggersAndPanels()
    {
        var cut = Render<Tabs>(parameters => parameters
            .AddChildContent(CreateTabsContent()));

        WaitForTabs(cut, 3);

        var tabs = cut.FindAll("[role='tab']");
        tabs[0].TextContent.Trim().ShouldBe("Overview");
        tabs[0].GetAttribute("id").ShouldBe("vibe-tab-overview");
        tabs[0].GetAttribute("aria-controls").ShouldBe("vibe-tabpanel-overview");
        tabs[0].GetAttribute("aria-selected").ShouldBe("true");
        tabs[0].GetAttribute("tabindex").ShouldBe("0");

        var overviewPanel = cut.Find("#vibe-tabpanel-overview");
        overviewPanel.GetAttribute("role").ShouldBe("tabpanel");
        overviewPanel.GetAttribute("aria-labelledby").ShouldBe("vibe-tab-overview");
        overviewPanel.HasAttribute("hidden").ShouldBeFalse();
        overviewPanel.TextContent.ShouldContain("Overview content");

        var detailsPanel = cut.Find("#vibe-tabpanel-details");
        detailsPanel.HasAttribute("hidden").ShouldBeTrue();
        detailsPanel.TextContent.ShouldContain("Details content");
    }

    [Fact]
    public void Tabs_InvokesCallbacks_WhenTabActivatedByClick()
    {
        string? activeTabId = null;
        Tabs.TabActivatedEventArgs? eventArgs = null;
        var cut = Render<Tabs>(parameters => parameters
            .Add(p => p.ActiveTabIdChanged, EventCallback.Factory.Create<string?>(this, id => activeTabId = id))
            .Add(p => p.OnTabActivated, EventCallback.Factory.Create<Tabs.TabActivatedEventArgs>(this, args => eventArgs = args))
            .AddChildContent(CreateTabsContent()));

        WaitForTabs(cut, 3);

        cut.Find("#vibe-tab-details").Click();

        activeTabId.ShouldBe("details");
        eventArgs.ShouldNotBeNull();
        eventArgs.TabId.ShouldBe("details");
        eventArgs.Title.ShouldNotBeNull();
        cut.Find("#vibe-tab-details").GetAttribute("aria-selected").ShouldBe("true");
        cut.Find("#vibe-tabpanel-details").HasAttribute("hidden").ShouldBeFalse();
    }

    [Fact]
    public void Tabs_ActivatesNextEnabledTab_WhenNavigatingWithKeyboard()
    {
        Tabs.TabActivatedEventArgs? eventArgs = null;
        var cut = Render<Tabs>(parameters => parameters
            .Add(p => p.OnTabActivated, EventCallback.Factory.Create<Tabs.TabActivatedEventArgs>(this, args => eventArgs = args))
            .AddChildContent(CreateTabsContent(disableDetails: true)));

        WaitForTabs(cut, 3);

        cut.Find("#vibe-tab-overview").KeyDown("ArrowRight");

        cut.Find("#vibe-tab-settings").GetAttribute("aria-selected").ShouldBe("true");
        cut.Find("#vibe-tabpanel-settings").HasAttribute("hidden").ShouldBeFalse();
        eventArgs.ShouldNotBeNull();
        eventArgs.TabId.ShouldBe("settings");

        cut.Find("#vibe-tab-settings").KeyDown("ArrowLeft");

        cut.Find("#vibe-tab-overview").GetAttribute("aria-selected").ShouldBe("true");
        cut.Find("#vibe-tabpanel-overview").HasAttribute("hidden").ShouldBeFalse();
    }

    [Fact]
    public void Tabs_UsesDefaultActiveTab_WhenProvided()
    {
        var cut = Render<Tabs>(parameters => parameters
            .AddChildContent(CreateTabsContent(defaultDetails: true)));

        WaitForTabs(cut, 3);

        cut.Find("#vibe-tab-details").GetAttribute("aria-selected").ShouldBe("true");
        cut.Find("#vibe-tabpanel-details").HasAttribute("hidden").ShouldBeFalse();
        cut.Find("#vibe-tab-overview").GetAttribute("tabindex").ShouldBe("-1");
    }

    [Fact]
    public void Tabs_DefaultActiveDoesNotOverrideUserSelectionAfterRerender()
    {
        var cut = Render<Tabs>(parameters => parameters
            .AddChildContent(CreateTabsContent(defaultDetails: true)));

        WaitForTabs(cut, 3);

        cut.Find("#vibe-tab-settings").Click();

        cut.Find("#vibe-tab-settings").GetAttribute("aria-selected").ShouldBe("true");
        cut.Find("#vibe-tab-details").GetAttribute("aria-selected").ShouldBe("false");

        cut.Render(parameters => parameters
            .AddChildContent(CreateTabsContent(defaultDetails: true)));

        cut.Find("#vibe-tab-settings").GetAttribute("aria-selected").ShouldBe("true");
        cut.Find("#vibe-tabpanel-settings").HasAttribute("hidden").ShouldBeFalse();
    }

    [Fact]
    public void Tabs_SkipsDisabledTabs_ForInitialActivationAndClicks()
    {
        string? activeTabId = null;
        var cut = Render<Tabs>(parameters => parameters
            .Add(p => p.ActiveTabIdChanged, EventCallback.Factory.Create<string?>(this, id => activeTabId = id))
            .AddChildContent(CreateTabsContent(disableOverview: true)));

        WaitForTabs(cut, 3);

        var overviewTrigger = cut.Find("#vibe-tab-overview");
        overviewTrigger.HasAttribute("disabled").ShouldBeTrue();
        overviewTrigger.GetAttribute("aria-disabled").ShouldBe("true");
        overviewTrigger.GetAttribute("aria-selected").ShouldBe("false");

        cut.Find("#vibe-tab-details").GetAttribute("aria-selected").ShouldBe("true");
        cut.Find("#vibe-tabpanel-details").HasAttribute("hidden").ShouldBeFalse();

        overviewTrigger.Click();

        activeTabId.ShouldBeNull();
        cut.Find("#vibe-tab-details").GetAttribute("aria-selected").ShouldBe("true");
    }

    [Fact]
    public void Tabs_Applies_AdditionalAttributes()
    {
        // Act
        var cut = Render<Tabs>(parameters => parameters
            .AddUnmatched("data-test", "tabs-value"));

        // Assert - AdditionalAttributes are captured
        cut.Find(".vibe-tabs").GetAttribute("data-test").ShouldBe("tabs-value");
    }

    [Fact]
    public void Tabs_Applies_TabItemClassAndAttributes_ToRenderedPanel()
    {
        var cut = Render<Tabs>(parameters => parameters
            .AddChildContent(CreateTabsContent()));

        WaitForTabs(cut, 3);

        var settingsPanel = cut.Find("#vibe-tabpanel-settings");
        settingsPanel.ClassList.ShouldContain("vibe-tab-item");
        settingsPanel.ClassList.ShouldContain("settings-panel");
        settingsPanel.GetAttribute("data-tab").ShouldBe("settings");
    }

    private static RenderFragment CreateTabsContent(
        bool disableOverview = false,
        bool disableDetails = false,
        bool defaultDetails = false)
    {
        return builder =>
        {
            builder.OpenComponent<TabItem>(0);
            builder.AddAttribute(1, "Id", "overview");
            builder.AddAttribute(2, "Header", "Overview");
            builder.AddAttribute(3, "Disabled", disableOverview);
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Overview content")));
            builder.CloseComponent();

            builder.OpenComponent<TabItem>(5);
            builder.AddAttribute(6, "Id", "details");
            builder.AddAttribute(7, "Header", "Details");
            builder.AddAttribute(8, "Disabled", disableDetails);
            builder.AddAttribute(9, "DefaultActive", defaultDetails);
            builder.AddAttribute(10, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Details content")));
            builder.CloseComponent();

            builder.OpenComponent<TabItem>(11);
            builder.AddAttribute(12, "Id", "settings");
            builder.AddAttribute(13, "Header", "Settings");
            builder.AddAttribute(14, "Class", "settings-panel");
            builder.AddAttribute(15, "data-tab", "settings");
            builder.AddAttribute(16, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Settings content")));
            builder.CloseComponent();
        };
    }

    private static void WaitForTabs(IRenderedComponent<Tabs> cut, int expectedCount)
    {
        cut.WaitForAssertion(() => cut.FindAll("[role='tab']").Count.ShouldBe(expectedCount));
    }
}
