namespace Vibe.UI.Tests.Components.DataDisplay;

public class AvatarTests : TestContext
{
    public AvatarTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Avatar_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>();

        // Assert
        cut.Find(".vibe-avatar").Should().NotBeNull();
    }

    [Fact]
    public void Avatar_RendersImage_WhenImageUrlProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "https://example.com/avatar.jpg"));

        // Assert
        var img = cut.Find("img.avatar-image");
        img.Should().NotBeNull();
        img.GetAttribute("src").Should().Be("https://example.com/avatar.jpg");
    }

    [Fact]
    public void Avatar_RendersInitials_WhenNoImage()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Initials, "JD"));

        // Assert
        var initials = cut.Find(".avatar-initials");
        initials.Should().NotBeNull();
        initials.TextContent.Should().Be("JD");
    }

    [Fact]
    public void Avatar_RendersFallbackIcon_WhenNoImageOrInitials()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.FallbackIcon, "👤"));

        // Assert
        var icon = cut.Find(".avatar-icon");
        icon.Should().NotBeNull();
        icon.TextContent.Should().Be("👤");
    }

    [Fact]
    public void Avatar_RendersFallback_WhenNothingProvided()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>();

        // Assert
        cut.Find(".avatar-fallback").Should().NotBeNull();
    }

    [Fact]
    public void Avatar_PrefersImage_OverInitials()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "https://example.com/avatar.jpg")
            .Add(p => p.Initials, "JD"));

        // Assert
        cut.FindAll("img.avatar-image").Should().NotBeEmpty();
        cut.FindAll(".avatar-initials").Should().BeEmpty();
    }

    [Fact]
    public void Avatar_FallsBackToInitials_WhenImageFails()
    {
        // Arrange
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "https://example.com/broken.jpg")
            .Add(p => p.Initials, "JD"));

        // Act - Trigger image error
        cut.Find("img").Error();

        // Assert
        cut.FindAll("img.avatar-image").Should().BeEmpty();
        cut.Find(".avatar-initials").TextContent.Should().Be("JD");
    }

    [Fact]
    public void Avatar_AppliesDefaultSize()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>();

        // Assert
        var style = cut.Find(".vibe-avatar").GetAttribute("style");
        style.Should().Contain("width: 40px");
        style.Should().Contain("height: 40px");
    }

    [Fact]
    public void Avatar_AppliesCustomSize()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Size, 64));

        // Assert
        var style = cut.Find(".vibe-avatar").GetAttribute("style");
        style.Should().Contain("width: 64px");
        style.Should().Contain("height: 64px");
    }

    [Fact]
    public void Avatar_AppliesDelayloadClass_WhenSet()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Delayload, true));

        // Assert
        cut.Find(".vibe-avatar").ClassList.Should().Contain("delayload");
    }

    [Fact]
    public void Avatar_DoesNotApplyDelayload_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>();

        // Assert
        cut.Find(".vibe-avatar").ClassList.Should().NotContain("delayload");
    }

    [Fact]
    public void Avatar_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.CssClass, "custom-avatar"));

        // Assert
        cut.Find(".vibe-avatar").ClassList.Should().Contain("custom-avatar");
    }

    [Fact]
    public void Avatar_SupportsCircleShape()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Shape, "circle"));

        // Assert
        cut.Instance.Shape.Should().Be("circle");
    }

    [Fact]
    public void Avatar_SupportsSquareShape()
    {
        // Arrange & Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Shape, "square"));

        // Assert
        cut.Instance.Shape.Should().Be("square");
    }
}
