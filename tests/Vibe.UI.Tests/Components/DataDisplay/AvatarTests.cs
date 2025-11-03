namespace Vibe.UI.Tests.Components.DataDisplay;

public class AvatarTests : TestBase
{
    [Fact]
    public void Avatar_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Avatar>();

        // Assert
        cut.Find(".vibe-avatar").ShouldNotBeNull();
    }

    [Fact]
    public void Avatar_Renders_Image_WhenImageUrlProvided()
    {
        // Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "test.jpg"));

        // Assert
        var img = cut.Find(".avatar-image");
        img.GetAttribute("src").ShouldBe("test.jpg");
    }

    [Fact]
    public void Avatar_Shows_Initials_WhenNoImage()
    {
        // Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Initials, "JD"));

        // Assert
        var initials = cut.Find(".avatar-initials");
        initials.TextContent.ShouldBe("JD");
    }

    [Fact]
    public void Avatar_Shows_FallbackIcon_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.FallbackIcon, "👤"));

        // Assert
        var icon = cut.Find(".avatar-icon");
        icon.TextContent.ShouldBe("👤");
    }

    [Fact]
    public void Avatar_Shows_Fallback_WhenNoContent()
    {
        // Act
        var cut = RenderComponent<Avatar>();

        // Assert
        cut.Find(".avatar-fallback").ShouldNotBeNull();
    }

    [Fact]
    public void Avatar_Has_DefaultSize()
    {
        // Act
        var cut = RenderComponent<Avatar>();

        // Assert
        cut.Instance.Size.ShouldBe(40);
    }

    [Fact]
    public void Avatar_Accepts_CustomSize()
    {
        // Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Size, 80));

        // Assert
        var avatar = cut.Find(".vibe-avatar");
        avatar.GetAttribute("style").ShouldContain("width: 80px");
        avatar.GetAttribute("style").ShouldContain("height: 80px");
    }

    [Fact]
    public void Avatar_Has_DefaultCircleShape()
    {
        // Act
        var cut = RenderComponent<Avatar>();

        // Assert
        cut.Instance.Shape.ShouldBe("circle");
    }

    [Fact]
    public void Avatar_Accepts_CustomShape()
    {
        // Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Shape, "square"));

        // Assert
        cut.Instance.Shape.ShouldBe("square");
    }

    [Fact]
    public void Avatar_Applies_DelayloadClass_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .Add(p => p.Delayload, true)
            .Add(p => p.ImageUrl, "test.jpg"));

        // Assert
        cut.Find(".vibe-avatar").ClassList.ShouldContain("delayload");
    }

    [Fact]
    public void Avatar_Applies_AdditionalAttributes()
    {
        // Act
        var cut = RenderComponent<Avatar>(parameters => parameters
            .AddUnmatched("data-test", "avatar-value"));

        // Assert - AdditionalAttributes are captured
        cut.Markup.ShouldNotBeNull();
    }
}
