namespace Vibe.UI.Tests.Components.Input;

public class MentionsTests : TestBase
{
    [Fact]
    public void Mentions_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Mentions>();

        // Assert
        cut.Find(".vibe-mentions").ShouldNotBeNull();
    }

    [Fact]
    public void Mentions_Renders_InputField()
    {
        // Act
        var cut = RenderComponent<Mentions>();

        // Assert
        cut.Find(".mentions-input").ShouldNotBeNull();
    }

    [Fact]
    public void Mentions_Has_DefaultPlaceholder()
    {
        // Act
        var cut = RenderComponent<Mentions>();

        // Assert
        var input = cut.Find(".mentions-input");
        input.GetAttribute("placeholder").ShouldBe("Type @ to mention...");
    }

    [Fact]
    public void Mentions_Accepts_CustomPlaceholder()
    {
        // Act
        var cut = RenderComponent<Mentions>(parameters => parameters
            .Add(p => p.Placeholder, "Custom placeholder"));

        // Assert
        var input = cut.Find(".mentions-input");
        input.GetAttribute("placeholder").ShouldBe("Custom placeholder");
    }

    [Fact]
    public void Mentions_Has_DefaultMentionPrefix()
    {
        // Act
        var cut = RenderComponent<Mentions>();

        // Assert
        cut.Instance.MentionPrefix.ShouldBe("@");
    }

    [Fact]
    public void Mentions_Accepts_CustomMentionPrefix()
    {
        // Act
        var cut = RenderComponent<Mentions>(parameters => parameters
            .Add(p => p.MentionPrefix, "#"));

        // Assert
        cut.Instance.MentionPrefix.ShouldBe("#");
    }

    [Fact]
    public void Mentions_Allows_Hashtags_ByDefault()
    {
        // Act
        var cut = RenderComponent<Mentions>();

        // Assert
        cut.Instance.AllowHashtags.ShouldBeTrue();
    }

    [Fact]
    public void Mentions_Disables_Input_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<Mentions>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var input = cut.Find(".mentions-input");
        input.HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".vibe-mentions").ClassList.ShouldContain("mentions-disabled");
    }

    [Fact]
    public void Mentions_Has_DefaultMaxSuggestions()
    {
        // Act
        var cut = RenderComponent<Mentions>();

        // Assert
        cut.Instance.MaxSuggestions.ShouldBe(5);
    }

    [Fact]
    public void Mentions_Has_AriaLabel()
    {
        // Act
        var cut = RenderComponent<Mentions>(parameters => parameters
            .Add(p => p.AriaLabel, "Mention users"));

        // Assert
        var input = cut.Find(".mentions-input");
        input.GetAttribute("aria-label").ShouldBe("Mention users");
    }

    [Fact]
    public void Mentions_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<Mentions>(parameters => parameters
            .Add(p => p.CssClass, "custom-mentions"));

        // Assert
        cut.Find(".vibe-mentions").ClassList.ShouldContain("custom-mentions");
    }
}
