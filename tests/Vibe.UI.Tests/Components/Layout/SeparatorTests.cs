namespace Vibe.UI.Tests.Components.Layout;

public class SeparatorTests : TestBase
{
    [Fact]
    public void Separator_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Separator>();

        // Assert
        var separator = cut.Find(".vibe-separator");
        separator.ShouldNotBeNull();
        separator.GetAttribute("role")!.ShouldBe("separator");
        separator.GetAttribute("aria-orientation")!.ShouldBe("horizontal");
        separator.ClassList.ShouldContain("separator-horizontal");
    }

    [Fact]
    public void Separator_Applies_Vertical_Orientation()
    {
        // Act
        var cut = Render<Separator>(parameters => parameters
            .Add(p => p.Orientation, "vertical"));

        // Assert
        var separator = cut.Find(".vibe-separator");
        separator.ClassList.ShouldContain("separator-vertical");
        separator.GetAttribute("aria-orientation")!.ShouldBe("vertical");
    }

    [Fact]
    public void Separator_HasSeparatorRole()
    {
        // Act
        var cut = Render<Separator>();

        // Assert
        cut.Find("[role='separator']").ShouldNotBeNull();
    }

    [Fact]
    public void Separator_Horizontal_IsDefaultOrientation()
    {
        // Act
        var cut = Render<Separator>();

        // Assert
        var separator = cut.Find(".vibe-separator");
        separator.ClassList.ShouldContain("separator-horizontal");
        separator.ClassList.ShouldNotContain("separator-vertical");
    }

    [Fact]
    public void Separator_WithCustomClass_AppliesCorrectly()
    {
        // Act
        var cut = Render<Separator>(parameters => parameters
            .Add(p => p.Class, "custom-separator-class"));

        // Assert
        var separator = cut.Find(".vibe-separator");
        separator.ClassList.ShouldContain("custom-separator-class");
    }

    [Fact]
    public void Separator_WithAdditionalAttributes_MergesRootAttributes()
    {
        var cut = Render<Separator>(parameters => parameters
            .Add(p => p.Class, "custom-separator-class")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["class"] = "attribute-separator",
                ["style"] = "margin-block: 0;",
                ["role"] = "presentation",
                ["aria-hidden"] = "true",
                ["data-testid"] = "separator"
            }));

        var separator = cut.Find(".vibe-separator");
        separator.ClassList.ShouldContain("custom-separator-class");
        separator.ClassList.ShouldContain("attribute-separator");
        separator.GetAttribute("style").ShouldBe("margin-block: 0;");
        separator.GetAttribute("role").ShouldBe("presentation");
        separator.GetAttribute("aria-hidden").ShouldBe("true");
        separator.GetAttribute("data-testid").ShouldBe("separator");
    }

    [Fact]
    public void Separator_Decorative_AppliesCorrectly()
    {
        // Act
        var cut = Render<Separator>(parameters => parameters
            .Add(p => p.Decorative, true));

        // Assert
        var separator = cut.Find(".vibe-separator");
        separator.ShouldNotBeNull();
        separator.GetAttribute("role").ShouldBeNull();
        separator.GetAttribute("aria-orientation").ShouldBeNull();
        separator.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void Separator_NonDecorativeIsDefault()
    {
        // Act
        var cut = Render<Separator>();

        // Assert
        var separator = cut.Find(".vibe-separator");
        separator.ShouldNotBeNull();
    }

    [Fact]
    public void Separator_WithInvalidOrientation_DefaultsToHorizontal()
    {
        // Act
        var cut = Render<Separator>(parameters => parameters
            .Add(p => p.Orientation, "invalid"));

        // Assert
        var separator = cut.Find(".vibe-separator");
        separator.ClassList.ShouldContain("separator-horizontal");
        separator.ClassList.ShouldNotContain("separator-invalid");
        separator.GetAttribute("aria-orientation").ShouldBe("horizontal");
    }

    [Fact]
    public void Separator_WithEmptyOrientation_DefaultsToHorizontal()
    {
        // Act
        var cut = Render<Separator>(parameters => parameters
            .Add(p => p.Orientation, ""));

        // Assert
        var separator = cut.Find(".vibe-separator");
        separator.ClassList.ShouldContain("separator-horizontal");
        separator.GetAttribute("aria-orientation").ShouldBe("horizontal");
    }

    [Fact]
    public void Separator_BothOrientations_CanBeRenderedSeparately()
    {
        // Act
        var cutHorizontal = Render<Separator>(parameters => parameters
            .Add(p => p.Orientation, "horizontal"));

        var cutVertical = Render<Separator>(parameters => parameters
            .Add(p => p.Orientation, "vertical"));

        // Assert
        cutHorizontal.Find(".separator-horizontal").ShouldNotBeNull();
        cutVertical.Find(".separator-vertical").ShouldNotBeNull();
    }

    [Fact]
    public void Separator_MultipleInstances_RenderIndependently()
    {
        // Act
        var cut1 = Render<Separator>(parameters => parameters
            .Add(p => p.Orientation, "horizontal"));

        var cut2 = Render<Separator>(parameters => parameters
            .Add(p => p.Orientation, "vertical"));

        // Assert
        cut1.Find(".separator-horizontal").ShouldNotBeNull();
        cut2.Find(".separator-vertical").ShouldNotBeNull();
    }
}
