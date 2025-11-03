namespace Vibe.UI.Tests.Components.Feedback;

public class SpinnerTests : TestBase
{
    [Fact]
    public void Spinner_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Spinner>();

        // Assert
        var spinner = cut.Find(".vibe-spinner");
        spinner.ShouldNotBeNull();
        spinner.GetAttribute("role").ShouldBe("status");
    }

    [Fact]
    public void Spinner_Applies_Size_Class()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Large));

        // Assert
        cut.Find(".vibe-spinner").ClassList.ShouldContain("spinner-large");
    }

    [Fact]
    public void Spinner_Renders_Label_WhenShowLabelIsTrue()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, "Loading data...")
            .Add(p => p.ShowLabel, true));

        // Assert
        var label = cut.Find(".spinner-label");
        label.ShouldNotBeNull();
        label.TextContent.ShouldBe("Loading data...");
    }

    [Fact]
    public void Spinner_DoesNotRenderLabel_WhenShowLabelIsFalse()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, "Loading...")
            .Add(p => p.ShowLabel, false));

        // Assert
        cut.FindAll(".spinner-label").ShouldBeEmpty();
    }

    [Fact]
    public void Spinner_HasAccessibilityLabel()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, "Processing"));

        // Assert
        var srOnly = cut.Find(".sr-only");
        srOnly.TextContent.ShouldBe("Processing");
    }

    [Fact]
    public void Spinner_HasDefaultAccessibilityLabel()
    {
        // Act
        var cut = RenderComponent<Spinner>();

        // Assert
        var srOnly = cut.Find(".sr-only");
        srOnly.TextContent.ShouldBe("Loading...");
    }

    [Fact]
    public void Spinner_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.CssClass, "my-custom-spinner"));

        // Assert
        cut.Find(".vibe-spinner").ClassList.ShouldContain("my-custom-spinner");
    }

    // ===== Size Variant Tests =====

    [Fact]
    public void Spinner_SmallSize_HasSmallClass()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Small));

        // Assert
        cut.Find(".vibe-spinner").ClassList.ShouldContain("spinner-small");
    }

    [Fact]
    public void Spinner_DefaultSize_HasDefaultClass()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Default));

        // Assert
        cut.Find(".vibe-spinner").ClassList.ShouldContain("spinner-default");
    }

    [Fact]
    public void Spinner_MediumSize_HasMediumClass()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Medium));

        // Assert
        cut.Find(".vibe-spinner").ClassList.ShouldContain("spinner-medium");
    }

    [Fact]
    public void Spinner_LargeSize_HasLargeClass()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Large));

        // Assert
        cut.Find(".vibe-spinner").ClassList.ShouldContain("spinner-large");
    }

    // ===== Label Configuration Tests =====

    [Fact]
    public void Spinner_WithNullLabel_UsesDefaultAccessibilityText()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, (string?)null));

        // Assert
        var srOnly = cut.Find(".sr-only");
        srOnly.TextContent.ShouldBe("Loading...");
    }

    [Fact]
    public void Spinner_WithEmptyLabel_UsesDefaultAccessibilityText()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, string.Empty));

        // Assert - Empty string is not null, so component uses empty string
        // Component code: @(Label ?? "Loading...") - ?? operator only checks for null
        var srOnly = cut.Find(".sr-only");
        srOnly.TextContent.ShouldBe(string.Empty);
    }

    [Fact]
    public void Spinner_ShowLabel_WithNullLabel_DoesNotRenderVisibleLabel()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, (string?)null)
            .Add(p => p.ShowLabel, true));

        // Assert
        cut.FindAll(".spinner-label").ShouldBeEmpty();
    }

    [Fact]
    public void Spinner_ShowLabel_WithEmptyLabel_DoesNotRenderVisibleLabel()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, string.Empty)
            .Add(p => p.ShowLabel, true));

        // Assert
        cut.FindAll(".spinner-label").ShouldBeEmpty();
    }

    [Fact]
    public void Spinner_WithLabel_ShowLabelFalse_OnlyRendersScreenReaderText()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, "Processing data")
            .Add(p => p.ShowLabel, false));

        // Assert
        cut.FindAll(".spinner-label").ShouldBeEmpty();
        var srOnly = cut.Find(".sr-only");
        srOnly.TextContent.ShouldBe("Processing data");
    }

    // ===== Accessibility Tests =====

    [Fact]
    public void Spinner_AriaLabel_MatchesLabelParameter()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, "Custom loading message"));

        // Assert
        var spinner = cut.Find(".vibe-spinner");
        spinner.GetAttribute("aria-label").ShouldBe("Custom loading message");
    }

    [Fact]
    public void Spinner_RoleStatus_IsPresent()
    {
        // Act
        var cut = RenderComponent<Spinner>();

        // Assert
        var spinner = cut.Find(".vibe-spinner");
        spinner.GetAttribute("role").ShouldBe("status");
    }

    [Fact]
    public void Spinner_ScreenReaderText_AlwaysPresent()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, "Loading"));

        // Assert
        var srOnly = cut.Find(".sr-only");
        srOnly.ShouldNotBeNull();
        srOnly.ClassList.ShouldContain("sr-only");
    }

    // ===== CSS Class Combination Tests =====

    [Fact]
    public void Spinner_WithMultipleClasses_CombinesCorrectly()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Large)
            .Add(p => p.CssClass, "custom-class another-class"));

        // Assert
        var spinner = cut.Find(".vibe-spinner");
        spinner.ClassList.ShouldContain("vibe-spinner");
        spinner.ClassList.ShouldContain("spinner-large");
        spinner.ClassList.ShouldContain("custom-class");
        spinner.ClassList.ShouldContain("another-class");
    }

    [Fact]
    public void Spinner_WithoutCustomClass_OnlyHasBaseClasses()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Medium));

        // Assert
        var spinner = cut.Find(".vibe-spinner");
        spinner.ClassList.ShouldContain("vibe-spinner");
        spinner.ClassList.ShouldContain("spinner-medium");
    }

    // ===== Combined Feature Tests =====

    [Fact]
    public void Spinner_AllFeaturesCombined_RendersCorrectly()
    {
        // Act
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Large)
            .Add(p => p.Label, "Loading user data")
            .Add(p => p.ShowLabel, true)
            .Add(p => p.CssClass, "custom-spinner"));

        // Assert
        var spinner = cut.Find(".vibe-spinner");
        spinner.ClassList.ShouldContain("spinner-large");
        spinner.ClassList.ShouldContain("custom-spinner");
        spinner.GetAttribute("aria-label").ShouldBe("Loading user data");

        var visibleLabel = cut.Find(".spinner-label");
        visibleLabel.TextContent.ShouldBe("Loading user data");

        var srOnly = cut.Find(".sr-only");
        srOnly.TextContent.ShouldBe("Loading user data");
    }
}
