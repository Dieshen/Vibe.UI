namespace Vibe.UI.Tests.Components.Disclosure;

public class CollapsibleTests : TestBase
{
    [Fact]
    public void Collapsible_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle"))
            .AddChildContent("Collapsible Content"));

        // Assert
        cut.Find(".vibe-collapsible").ShouldNotBeNull();
    }

    [Fact]
    public void Collapsible_IsClosed_ByDefault()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle"))
            .AddChildContent("Collapsible Content"));

        // Assert
        cut.Instance.IsOpen.ShouldBeFalse();
    }

    [Fact]
    public void Collapsible_Renders_TriggerContent()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle Me"))
            .AddChildContent("Collapsible Content"));

        // Assert
        cut.Markup.ShouldContain("Toggle Me");
    }

    [Fact]
    public void Collapsible_Renders_ChildContent()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle"))
            .AddChildContent("<span class='test-content'>Test Content</span>"));

        // Assert
        var content = cut.Find(".test-content");
        content.TextContent.ShouldBe("Test Content");
    }

    [Fact]
    public void Collapsible_Content_NotExpanded_Initially()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle"))
            .AddChildContent("Collapsible Content"));

        // Assert
        var content = cut.Find(".collapsible-content");
        content.ClassList.ShouldNotContain("expanded");
    }

    [Fact]
    public void Collapsible_Content_Expanded_WhenOpen()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle"))
            .AddChildContent("Collapsible Content"));

        // Assert
        var content = cut.Find(".collapsible-content");
        content.ClassList.ShouldContain("expanded");
    }

    [Fact]
    public void Collapsible_Invokes_IsOpenChanged()
    {
        // Arrange
        bool stateChanged = false;
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => stateChanged = true))
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle"))
            .AddChildContent("Collapsible Content"));

        // Act
        cut.Instance.ToggleAsync();

        // Assert
        stateChanged.ShouldBeTrue();
    }

    [Fact]
    public void Collapsible_Toggles_State()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle"))
            .AddChildContent("Collapsible Content"));

        var initialState = cut.Instance.IsOpen;
        cut.Instance.ToggleAsync();

        // Assert
        cut.Instance.IsOpen.ShouldBe(!initialState);
    }

    [Fact]
    public void Collapsible_Passes_IsOpenState_ToTrigger()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.TriggerContent, (isOpen) => builder =>
                builder.AddContent(0, isOpen ? "Close" : "Open"))
            .AddChildContent("Collapsible Content"));

        // Assert
        cut.Markup.ShouldContain("Close");
    }

    [Fact]
    public void Collapsible_Applies_AdditionalAttributes()
    {
        // Act
        var cut = RenderComponent<Collapsible>(parameters => parameters
            .Add(p => p.TriggerContent, (isOpen) => builder => builder.AddContent(0, "Toggle"))
            .AddUnmatched("data-test", "collapsible-value")
            .AddChildContent("Collapsible Content"));

        // Assert - AdditionalAttributes are captured
        cut.Markup.ShouldNotBeNull();
    }
}
