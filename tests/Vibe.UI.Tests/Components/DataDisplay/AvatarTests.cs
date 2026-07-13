namespace Vibe.UI.Tests.Components.DataDisplay;

public class AvatarTests : TestBase
{
    [Fact]
    public void Avatar_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Avatar>();

        // Assert
        cut.Find(".vibe-avatar").ShouldNotBeNull();
    }

    [Fact]
    public void Avatar_Renders_Image_WhenImageUrlProvided()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "test.jpg"));

        // Assert
        var img = cut.Find(".avatar-image");
        img.GetAttribute("src")!.ShouldBe("test.jpg");
    }

    [Fact]
    public void Avatar_Renders_Image_WhenSrcAliasProvided()
    {
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Src, "profile.jpg"));

        cut.Find(".avatar-image").GetAttribute("src")!.ShouldBe("profile.jpg");
    }

    [Fact]
    public void Avatar_ImageUrl_TakesPrecedenceOverSrcAlias()
    {
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "canonical.jpg")
            .Add(p => p.Src, "alias.jpg"));

        cut.Find(".avatar-image").GetAttribute("src")!.ShouldBe("canonical.jpg");
    }

    [Fact]
    public void Avatar_Shows_Initials_WhenNoImage()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Initials, "JD"));

        // Assert
        var initials = cut.Find(".avatar-initials");
        initials.TextContent.ShouldBe("JD");
    }

    [Fact]
    public void Avatar_Shows_FallbackIcon_WhenProvided()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.FallbackIcon, "👤"));

        // Assert
        var icon = cut.Find(".avatar-icon");
        icon.TextContent.ShouldBe("👤");
    }

    [Fact]
    public void Avatar_Shows_RenderFragmentFallback_WhenProvided()
    {
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.FallbackContent, builder =>
                builder.AddMarkupContent(0, "<svg class='user-icon'></svg>")));

        cut.Find(".avatar-icon .user-icon").ShouldNotBeNull();
    }

    [Fact]
    public void Avatar_Shows_Fallback_WhenNoContent()
    {
        // Act
        var cut = Render<Avatar>();

        // Assert
        cut.Find(".avatar-fallback").ShouldNotBeNull();
    }

    [Fact]
    public void Avatar_Has_DefaultSize()
    {
        // Act
        var cut = Render<Avatar>();

        // Assert
        cut.Instance.Size.ShouldBe(40);
    }

    [Fact]
    public void Avatar_Accepts_CustomSize()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Size, 80));

        // Assert
        var avatar = cut.Find(".vibe-avatar");
        avatar.GetAttribute("style")!.ShouldContain("width: 80px");
        avatar.GetAttribute("style")!.ShouldContain("height: 80px");
    }

    [Fact]
    public void Avatar_Has_DefaultCircleShape()
    {
        // Act
        var cut = Render<Avatar>();

        // Assert
        cut.Instance.Shape.ShouldBe("circle");
    }

    [Fact]
    public void Avatar_Accepts_CustomShape()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Shape, "square"));

        // Assert
        cut.Instance.Shape.ShouldBe("square");
        cut.Find(".vibe-avatar").ClassList.ShouldContain("square");
    }

    [Fact]
    public void Avatar_Applies_DelayloadClass_WhenEnabled()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Delayload, true)
            .Add(p => p.ImageUrl, "test.jpg"));

        // Assert
        cut.Find(".vibe-avatar").ClassList.ShouldContain("delayload");
    }

    [Fact]
    public void Avatar_Applies_AdditionalAttributes()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .AddUnmatched("data-test", "avatar-value")
            .AddUnmatched("title", "Profile avatar"));

        // Assert
        var avatar = cut.Find(".vibe-avatar");
        avatar.GetAttribute("data-test")!.ShouldBe("avatar-value");
        avatar.GetAttribute("title")!.ShouldBe("Profile avatar");
    }

    [Fact]
    public void Avatar_Applies_CustomClass()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Class, "custom-avatar"));

        // Assert
        cut.Find(".vibe-avatar").ClassList.ShouldContain("custom-avatar");
    }

    [Fact]
    public void Avatar_Uses_AccessibleLabel_FromAlt()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Alt, "Jane Doe"));

        // Assert
        var avatar = cut.Find(".vibe-avatar");
        avatar.GetAttribute("role")!.ShouldBe("img");
        avatar.GetAttribute("aria-label")!.ShouldBe("Jane Doe");
    }

    [Fact]
    public void Avatar_DoesNotOverride_ProvidedAriaLabel()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Alt, "Internal label")
            .AddUnmatched("aria-label", "Provided label"));

        // Assert
        cut.Find(".vibe-avatar").GetAttribute("aria-label")!.ShouldBe("Provided label");
    }

    [Fact]
    public void Avatar_TreatsWhitespaceContent_AsEmpty()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "   ")
            .Add(p => p.Initials, "  AB  "));

        // Assert
        cut.FindAll(".avatar-image").ShouldBeEmpty();
        cut.Find(".avatar-initials").TextContent.ShouldBe("AB");
    }

    [Fact]
    public void Avatar_Uses_DefaultSize_WhenCustomSizeIsInvalid()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.Size, -1));

        // Assert
        var avatar = cut.Find(".vibe-avatar");
        avatar.GetAttribute("style")!.ShouldContain("width: 40px");
        avatar.GetAttribute("style")!.ShouldContain("height: 40px");
    }

    [Fact]
    public void Avatar_Image_IsDecorative_WhenRootProvidesAccessibleName()
    {
        // Act
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "profile.jpg")
            .Add(p => p.Alt, "Profile image"));

        // Assert
        var avatar = cut.Find(".vibe-avatar");
        var img = cut.Find(".avatar-image");
        avatar.GetAttribute("aria-label")!.ShouldBe("Profile image");
        img.GetAttribute("alt")!.ShouldBe(string.Empty);
        img.GetAttribute("aria-hidden")!.ShouldBe("true");
    }

    [Fact]
    public void Avatar_ImageError_InvokesCallback_AndShowsFallback()
    {
        // Arrange
        string? failedImageUrl = null;
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "missing.jpg")
            .Add(p => p.Initials, "JD")
            .Add(p => p.OnImageError, EventCallback.Factory.Create<string?>(this, value => failedImageUrl = value)));

        // Act
        cut.Find(".avatar-image").TriggerEvent("onerror", EventArgs.Empty);

        // Assert
        failedImageUrl.ShouldBe("missing.jpg");
        cut.FindAll(".avatar-image").ShouldBeEmpty();
        cut.Find(".avatar-initials").TextContent.ShouldBe("JD");
    }

    [Fact]
    public void Avatar_ImageErrorState_Resets_WhenImageUrlChanges()
    {
        // Arrange
        var cut = Render<Avatar>(parameters => parameters
            .Add(p => p.ImageUrl, "missing.jpg")
            .Add(p => p.Initials, "JD"));
        cut.Find(".avatar-image").TriggerEvent("onerror", EventArgs.Empty);
        cut.FindAll(".avatar-image").ShouldBeEmpty();

        // Act
        cut.Render(parameters => parameters
            .Add(p => p.ImageUrl, "profile.jpg")
            .Add(p => p.Initials, "JD"));

        // Assert
        cut.Find(".avatar-image").GetAttribute("src")!.ShouldBe("profile.jpg");
    }
}
