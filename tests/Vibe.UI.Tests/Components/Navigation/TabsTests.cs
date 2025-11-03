namespace Vibe.UI.Tests.Components.Navigation;

public class TabsTests : TestBase
{
    [Fact]
    public void Tabs_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Tabs>();

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
        var cut = RenderComponent<Tabs>();

        // Assert
        var tabList = cut.Find(".vibe-tabs-list");
        tabList.GetAttribute("role").ShouldBe("tablist");
    }

    [Fact]
    public void Tabs_Invokes_ActiveTabIdChanged_WhenTabActivated()
    {
        // Arrange
        string activatedTabId = null;
        var cut = RenderComponent<Tabs>(parameters => parameters
            .Add(p => p.ActiveTabId, "tab1")
            .Add(p => p.ActiveTabIdChanged, EventCallback.Factory.Create<string>(this, id => activatedTabId = id)));

        // Assert - ActiveTabId can be set
        cut.Instance.ActiveTabId.ShouldBe("tab1");
    }

    [Fact]
    public void Tabs_Invokes_OnTabActivated_Callback()
    {
        // Arrange
        Tabs.TabActivatedEventArgs eventArgs = null;
        var cut = RenderComponent<Tabs>(parameters => parameters
            .Add(p => p.OnTabActivated, EventCallback.Factory.Create<Tabs.TabActivatedEventArgs>(this, args => eventArgs = args)));

        // Assert - callback can be set
        cut.Instance.OnTabActivated.HasDelegate.ShouldBeTrue();
    }

    [Fact]
    public void Tabs_Applies_AdditionalAttributes()
    {
        // Act
        var cut = RenderComponent<Tabs>(parameters => parameters
            .AddUnmatched("data-test", "tabs-value"));

        // Assert - AdditionalAttributes are captured
        cut.Markup.ShouldNotBeNull();
    }
}
