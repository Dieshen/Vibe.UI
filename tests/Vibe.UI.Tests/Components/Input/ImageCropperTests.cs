namespace Vibe.UI.Tests.Components.Input;

public class ImageCropperTests : TestBase
{
    [Fact]
    public void ImageCropper_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg"));

        // Assert
        cut.Find(".vibe-image-cropper").ShouldNotBeNull();
    }

    [Fact]
    public void ImageCropper_Shows_EmptyContent_WhenNoImage()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.EmptyContent, builder => builder.AddContent(0, "No image")));

        // Assert
        cut.Find(".cropper-empty").ShouldNotBeNull();
        cut.Markup.ShouldContain("No image");
    }

    [Fact]
    public void ImageCropper_Renders_Image_WhenProvided()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg"));

        // Assert
        var img = cut.Find(".cropper-image");
        img.GetAttribute("src").ShouldBe("test.jpg");
    }

    [Fact]
    public void ImageCropper_Shows_Controls_ByDefault()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.ShowControls, true));

        // Assert
        cut.Find(".cropper-controls").ShouldNotBeNull();
    }

    [Fact]
    public void ImageCropper_Hides_Controls_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.ShowControls, false));

        // Assert
        cut.FindAll(".cropper-controls").ShouldBeEmpty();
    }

    [Fact]
    public void ImageCropper_Renders_CropBox()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg"));

        // Assert
        cut.Find(".crop-box").ShouldNotBeNull();
    }

    [Fact]
    public void ImageCropper_Has_DefaultAspectRatioOptions()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg"));

        // Assert
        cut.Instance.AspectRatioOptions.ShouldNotBeEmpty();
        cut.Instance.AspectRatioOptions.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ImageCropper_Invokes_OnCropped_Callback()
    {
        // Arrange
        ImageCropper.CroppedImageData croppedData = null;
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.OnCropped, EventCallback.Factory.Create<ImageCropper.CroppedImageData>(this, data => croppedData = data)));

        // Act
        var cropButton = cut.FindAll(".control-btn").Last();
        cropButton.Click();

        // Assert
        croppedData.ShouldNotBeNull();
    }

    [Fact]
    public void ImageCropper_Renders_RotateButtons()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.ShowControls, true));

        // Assert
        var buttons = cut.FindAll(".control-btn");
        buttons.Count.ShouldBeGreaterThan(4);
    }

    [Fact]
    public void ImageCropper_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.CssClass, "custom-cropper"));

        // Assert
        cut.Find(".vibe-image-cropper").ClassList.ShouldContain("custom-cropper");
    }
}
