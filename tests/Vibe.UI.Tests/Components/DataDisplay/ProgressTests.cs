namespace Vibe.UI.Tests.Components.DataDisplay;

public class ProgressTests : TestContext
{
    public ProgressTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Progress_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>();

        // Assert
        cut.Find(".vibe-progress").Should().NotBeNull();
        cut.Find(".progress-indicator").Should().NotBeNull();
        cut.Find("[role='progressbar']").Should().NotBeNull();
    }

    [Fact]
    public void Progress_AppliesAriaAttributes()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 50));

        // Assert
        var progressBar = cut.Find("[role='progressbar']");
        progressBar.GetAttribute("aria-valuemin").Should().Be("0");
        progressBar.GetAttribute("aria-valuemax").Should().Be("100");
        progressBar.GetAttribute("aria-valuenow").Should().Be("50");
    }

    [Fact]
    public void Progress_DisplaysCorrectPercentage()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 75));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").Should().Contain("width: 75%");
    }

    [Fact]
    public void Progress_ClampsValueAt0()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, -10));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").Should().Contain("width: 0%");
    }

    [Fact]
    public void Progress_ClampsValueAt100()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 150));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").Should().Contain("width: 100%");
    }

    [Fact]
    public void Progress_ShowsIndeterminateAnimation_WhenSet()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.IndeterminateAnimation, true));

        // Assert
        cut.Find(".vibe-progress").ClassList.Should().Contain("animate");
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").Should().Contain("width: 100%");
    }

    [Fact]
    public void Progress_DoesNotAnimate_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>();

        // Assert
        cut.Find(".vibe-progress").ClassList.Should().NotContain("animate");
    }

    [Fact]
    public void Progress_AppliesDefaultVariant()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>();

        // Assert
        cut.Find(".vibe-progress").ClassList.Should().Contain("vibe-progress-default");
    }

    [Theory]
    [InlineData("default", "vibe-progress-default")]
    [InlineData("primary", "vibe-progress-primary")]
    [InlineData("success", "vibe-progress-success")]
    [InlineData("warning", "vibe-progress-warning")]
    [InlineData("error", "vibe-progress-error")]
    public void Progress_AppliesCorrectVariantClass(string variant, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Variant, variant));

        // Assert
        cut.Find(".vibe-progress").ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void Progress_Handles0Percent()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 0));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").Should().Contain("width: 0%");
    }

    [Fact]
    public void Progress_Handles100Percent()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.Value, 100));

        // Assert
        var indicator = cut.Find(".progress-indicator");
        indicator.GetAttribute("style").Should().Contain("width: 100%");
    }

    [Fact]
    public void Progress_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Progress>(parameters => parameters
            .Add(p => p.CssClass, "custom-progress"));

        // Assert
        cut.Find(".vibe-progress").ClassList.Should().Contain("custom-progress");
    }
}
