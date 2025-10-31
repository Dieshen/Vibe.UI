namespace Vibe.UI.Tests.Components.Navigation;

public class BreadcrumbTests : TestContext
{
    public BreadcrumbTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Breadcrumb_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Breadcrumb>();

        // Assert
        cut.Find(".vibe-breadcrumb").Should().NotBeNull();
        cut.Find("nav[aria-label='Breadcrumb']").Should().NotBeNull();
        cut.Find("ol.breadcrumb-list").Should().NotBeNull();
    }

    [Fact]
    public void Breadcrumb_RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Breadcrumb>(parameters => parameters
            .AddChildContent("<li>Home</li><li>Products</li>"));

        // Assert
        cut.Markup.Should().Contain("<li>Home</li>");
        cut.Markup.Should().Contain("<li>Products</li>");
    }

    [Fact]
    public void Breadcrumb_HasDefaultSeparator()
    {
        // Arrange & Act
        var cut = RenderComponent<Breadcrumb>();

        // Assert
        cut.Instance.Separator.Should().Be("/");
    }

    [Fact]
    public void Breadcrumb_AppliesCustomSeparator()
    {
        // Arrange & Act
        var cut = RenderComponent<Breadcrumb>(parameters => parameters
            .Add(p => p.Separator, ">"));

        // Assert
        cut.Instance.Separator.Should().Be(">");
    }

    [Fact]
    public void Breadcrumb_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Breadcrumb>(parameters => parameters
            .Add(p => p.CssClass, "custom-breadcrumb"));

        // Assert
        cut.Find(".vibe-breadcrumb").ClassList.Should().Contain("custom-breadcrumb");
    }

    [Fact]
    public void Breadcrumb_SupportsAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderComponent<Breadcrumb>(parameters => parameters
            .AddUnmatched("data-testid", "my-breadcrumb"));

        // Assert
        cut.Find("nav").GetAttribute("data-testid").Should().Be("my-breadcrumb");
    }

    [Fact]
    public void Breadcrumb_RendersAsNav()
    {
        // Arrange & Act
        var cut = RenderComponent<Breadcrumb>();

        // Assert
        cut.Find("nav").Should().NotBeNull();
    }

    [Fact]
    public void Breadcrumb_ContainsOrderedList()
    {
        // Arrange & Act
        var cut = RenderComponent<Breadcrumb>();

        // Assert
        cut.Find("ol").Should().NotBeNull();
    }
}
