namespace Vibe.UI.Tests.Components.Navigation;

using Vibe.UI.Components;

public class TabsTests : TestContext
{
    public TabsTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Tabs_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Tabs>();

        // Assert
        cut.Find(".vibe-tabs").Should().NotBeNull();
        cut.Find(".vibe-tabs-list[role='tablist']").Should().NotBeNull();
        cut.Find(".vibe-tabs-content").Should().NotBeNull();
    }

    [Fact]
    public void Tabs_RendersTabButtons()
    {
        // Arrange
        var cut = RenderComponent<Tabs>();

        // Manually add tabs to the internal collection using reflection
        var tabsField = cut.Instance.GetType().GetField("_tabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tabs = (List<Tabs.TabItem>)tabsField.GetValue(cut.Instance);

        tabs.Add(new Tabs.TabItem("tab1",
            builder => builder.AddContent(0, "Tab 1"),
            builder => builder.AddContent(0, "Content 1"),
            false, false));
        tabs.Add(new Tabs.TabItem("tab2",
            builder => builder.AddContent(0, "Tab 2"),
            builder => builder.AddContent(0, "Content 2"),
            false, false));

        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.ActiveTabId, "tab1"));

        // Assert
        var buttons = cut.FindAll("button.vibe-tabs-trigger");
        buttons.Should().HaveCount(2);
    }

    [Fact]
    public void Tabs_MarksActiveTab()
    {
        // Arrange
        var cut = RenderComponent<Tabs>();
        var tabsField = cut.Instance.GetType().GetField("_tabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tabs = (List<Tabs.TabItem>)tabsField.GetValue(cut.Instance);

        tabs.Add(new Tabs.TabItem("tab1", builder => builder.AddContent(0, "Tab 1"),
            builder => builder.AddContent(0, "Content 1"), false, false));
        tabs.Add(new Tabs.TabItem("tab2", builder => builder.AddContent(0, "Tab 2"),
            builder => builder.AddContent(0, "Content 2"), false, false));

        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.ActiveTabId, "tab1"));

        // Assert
        var activeButton = cut.Find(".vibe-tabs-trigger-active");
        activeButton.Should().NotBeNull();
        activeButton.GetAttribute("aria-selected").Should().Be("True");
    }

    [Fact]
    public void Tabs_ShowsActiveTabPanel()
    {
        // Arrange
        var cut = RenderComponent<Tabs>();
        var tabsField = cut.Instance.GetType().GetField("_tabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tabs = (List<Tabs.TabItem>)tabsField.GetValue(cut.Instance);

        tabs.Add(new Tabs.TabItem("tab1", builder => builder.AddContent(0, "Tab 1"),
            builder => builder.AddContent(0, "Panel 1"), false, false));

        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.ActiveTabId, "tab1"));

        // Assert
        cut.FindAll(".vibe-tabs-panel-active").Should().NotBeEmpty();
    }

    [Fact]
    public void Tabs_DisablesTabButton_WhenDisabled()
    {
        // Arrange
        var cut = RenderComponent<Tabs>();
        var tabsField = cut.Instance.GetType().GetField("_tabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tabs = (List<Tabs.TabItem>)tabsField.GetValue(cut.Instance);

        tabs.Add(new Tabs.TabItem("tab1", builder => builder.AddContent(0, "Disabled Tab"),
            builder => builder.AddContent(0, "Content"), true, false));

        cut.Render();

        // Assert
        var button = cut.Find("button.vibe-tabs-trigger");
        button.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Tabs_AppliesAriaAttributes()
    {
        // Arrange
        var cut = RenderComponent<Tabs>();
        var tabsField = cut.Instance.GetType().GetField("_tabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tabs = (List<Tabs.TabItem>)tabsField.GetValue(cut.Instance);

        tabs.Add(new Tabs.TabItem("tab1", builder => builder.AddContent(0, "Tab 1"),
            builder => builder.AddContent(0, "Content 1"), false, false));

        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.ActiveTabId, "tab1"));

        // Assert
        var button = cut.Find("button[role='tab']");
        button.Should().NotBeNull();

        var panel = cut.Find(".vibe-tabs-panel[role='tabpanel']");
        panel.Should().NotBeNull();
        panel.GetAttribute("id").Should().Be("tab1-panel");
    }

    [Fact]
    public void Tabs_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Tabs>(parameters => parameters
            .Add(p => p.CssClass, "custom-tabs"));

        // Assert
        cut.Find(".vibe-tabs").ClassList.Should().Contain("custom-tabs");
    }

    [Fact]
    public void Tabs_RendersMultiplePanels()
    {
        // Arrange
        var cut = RenderComponent<Tabs>();
        var tabsField = cut.Instance.GetType().GetField("_tabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tabs = (List<Tabs.TabItem>)tabsField.GetValue(cut.Instance);

        tabs.Add(new Tabs.TabItem("tab1", builder => builder.AddContent(0, "Tab 1"),
            builder => builder.AddContent(0, "Content 1"), false, false));
        tabs.Add(new Tabs.TabItem("tab2", builder => builder.AddContent(0, "Tab 2"),
            builder => builder.AddContent(0, "Content 2"), false, false));
        tabs.Add(new Tabs.TabItem("tab3", builder => builder.AddContent(0, "Tab 3"),
            builder => builder.AddContent(0, "Content 3"), false, false));

        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.ActiveTabId, "tab1"));

        // Assert
        cut.FindAll(".vibe-tabs-panel").Should().HaveCount(3);
    }
}
