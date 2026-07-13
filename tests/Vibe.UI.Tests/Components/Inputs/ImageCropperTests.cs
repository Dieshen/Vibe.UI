using Microsoft.AspNetCore.Components.Web;

namespace Vibe.UI.Tests.Components.Inputs;

public class ImageCropperTests : TestBase
{
    [Fact]
    public void ImageCropper_Renders_WithAccessibleDefaultProps()
    {
        // Act
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, " test.jpg ")
            .Add(p => p.ImageAlt, "Selected profile image")
            .Add(p => p.AriaLabel, "Profile image cropper"));

        // Assert
        var cropper = cut.Find(".vibe-image-cropper");
        cropper.GetAttribute("role").ShouldBe("group");
        cropper.GetAttribute("aria-label").ShouldBe("Profile image cropper");
        cropper.GetAttribute("aria-disabled").ShouldBe("false");
        cropper.GetAttribute("aria-describedby")!.ShouldContain("vibe-image-cropper-instructions");
        cropper.ClassList.ShouldContain("vibe-image-cropper-with-image");

        var image = cut.Find(".cropper-image");
        image.GetAttribute("src").ShouldBe("test.jpg");
        image.GetAttribute("alt").ShouldBe("Selected profile image");
        image.GetAttribute("draggable").ShouldBe("false");

        var cropBox = cut.Find(".crop-box");
        cropBox.GetAttribute("role").ShouldBe("group");
        cropBox.GetAttribute("tabindex").ShouldBe("0");
        cropBox.GetAttribute("aria-describedby")!.ShouldContain("vibe-image-cropper-status");

        var controls = cut.Find(".cropper-controls");
        controls.GetAttribute("role").ShouldBe("toolbar");
        controls.GetAttribute("aria-label").ShouldBe("Image crop controls");
        cut.Find("button[aria-label='Set aspect ratio Free']").GetAttribute("aria-pressed").ShouldBe("true");
    }

    [Fact]
    public void ImageCropper_Renders_DefaultEmptyState_WhenNoImage()
    {
        // Act
        var cut = Render<ImageCropper>();

        // Assert
        var cropper = cut.Find(".vibe-image-cropper");
        cropper.ClassList.ShouldContain("vibe-image-cropper-empty");
        cropper.GetAttribute("aria-describedby")!.ShouldContain("vibe-image-cropper-empty");

        var empty = cut.Find(".cropper-empty");
        empty.GetAttribute("role").ShouldBe("status");
        empty.TextContent.ShouldBe("No image selected");
        cut.FindAll(".cropper-canvas").ShouldBeEmpty();
    }

    [Fact]
    public void ImageCropper_Shows_EmptyContent_ForWhitespaceImageSource()
    {
        // Act
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "   ")
            .Add(p => p.EmptyContent, builder => builder.AddContent(0, "Choose an image")));

        // Assert
        cut.Find(".cropper-empty").TextContent.ShouldContain("Choose an image");
        cut.FindAll(".cropper-image").ShouldBeEmpty();
    }

    [Fact]
    public void ImageCropper_Applies_CustomCssClass_ClassAndAdditionalAttributes()
    {
        // Act
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.CssClass, "custom-cropper")
            .Add(p => p.Class, "base-class")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "image-cropper" }
            }));

        // Assert
        var cropper = cut.Find(".vibe-image-cropper");
        cropper.ClassList.ShouldContain("custom-cropper");
        cropper.ClassList.ShouldContain("base-class");
        cropper.GetAttribute("data-testid").ShouldBe("image-cropper");
    }

    [Fact]
    public void ImageCropper_Hides_Controls_WhenShowControlsFalse()
    {
        // Act
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.ShowControls, false));

        // Assert
        cut.Find(".crop-box").ShouldNotBeNull();
        cut.FindAll(".cropper-controls").ShouldBeEmpty();
    }

    [Fact]
    public void ImageCropper_DisabledState_DisablesControlsAndSuppressesCrop()
    {
        // Arrange
        var croppedCount = 0;
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.Disabled, true)
            .Add(p => p.OnCropped, _ => croppedCount++));

        // Act
        cut.Find("button[aria-label='Crop image']").Click();

        // Assert
        var cropper = cut.Find(".vibe-image-cropper");
        cropper.ClassList.ShouldContain("vibe-image-cropper-disabled");
        cropper.GetAttribute("aria-disabled").ShouldBe("true");
        cut.Find(".crop-box").GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".zoom-slider").HasAttribute("disabled").ShouldBeTrue();
        cut.FindAll(".control-btn").ShouldAllBe(button => button.HasAttribute("disabled"));
        croppedCount.ShouldBe(0);
    }

    [Fact]
    public void ImageCropper_ReadOnlyState_DisablesInteractionWithoutMarkingRootDisabled()
    {
        // Arrange
        var croppedCount = 0;
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnCropped, _ => croppedCount++));

        var cropBox = cut.Find(".crop-box");
        var initialStyle = cropBox.GetAttribute("style");

        // Act
        cropBox.KeyDown("ArrowRight");
        cut.Find("button[aria-label='Crop image']").Click();

        // Assert
        var cropper = cut.Find(".vibe-image-cropper");
        cropper.ClassList.ShouldContain("vibe-image-cropper-readonly");
        cropper.GetAttribute("aria-disabled").ShouldBe("false");
        cut.Find(".crop-box").GetAttribute("tabindex").ShouldBe("-1");
        cut.Find(".crop-box").GetAttribute("style").ShouldBe(initialStyle);
        croppedCount.ShouldBe(0);
    }

    [Fact]
    public void ImageCropper_CropCallback_IncludesTransformStateAndClampedZoom()
    {
        // Arrange
        ImageCropper.CroppedImageData? croppedData = null;
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.OnCropped, data => croppedData = data));

        // Act
        ClickButton(cut, "Rotate left");
        ClickButton(cut, "Flip horizontally");
        ClickButton(cut, "Flip vertically");
        cut.Find(".zoom-slider").Input("3.8");
        ClickButton(cut, "Crop image");

        // Assert
        croppedData.ShouldNotBeNull();
        croppedData.ImageSource.ShouldBe("test.jpg");
        croppedData.Zoom.ShouldBe(3);
        croppedData.Rotation.ShouldBe(270);
        croppedData.FlipHorizontal.ShouldBeTrue();
        croppedData.FlipVertical.ShouldBeTrue();
        croppedData.CropArea.X.ShouldBe(10);
        croppedData.CropArea.Y.ShouldBe(10);
        croppedData.CropArea.Width.ShouldBe(80);
        croppedData.CropArea.Height.ShouldBe(80);
    }

    [Fact]
    public void ImageCropper_InvalidZoomInput_PreservesPreviousZoom()
    {
        // Arrange
        ImageCropper.CroppedImageData? croppedData = null;
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.OnCropped, data => croppedData = data));

        // Act
        ClickButton(cut, "Zoom in");
        cut.Find(".zoom-slider").Input("not-a-number");
        ClickButton(cut, "Crop image");

        // Assert
        croppedData.ShouldNotBeNull();
        croppedData.Zoom.ShouldBe(1.1, 0.0001);
    }

    [Fact]
    public void ImageCropper_Reset_RestoresDefaultTransformState()
    {
        // Arrange
        var cropped = new List<ImageCropper.CroppedImageData>();
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.OnCropped, cropped.Add));

        // Act
        ClickButton(cut, "Rotate right");
        ClickButton(cut, "Zoom in");
        ClickButton(cut, "Flip horizontally");
        ClickButton(cut, "Reset cropper");
        ClickButton(cut, "Crop image");

        // Assert
        cropped.Count.ShouldBe(1);
        cropped[0].Zoom.ShouldBe(1);
        cropped[0].Rotation.ShouldBe(0);
        cropped[0].FlipHorizontal.ShouldBeFalse();
        cropped[0].FlipVertical.ShouldBeFalse();
    }

    [Fact]
    public void ImageCropper_AspectRatioSelection_AdjustsCropAreaAndRaisesChanged()
    {
        // Arrange
        var changedRatios = new List<double>();
        ImageCropper.CroppedImageData? croppedData = null;
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.AspectRatioChanged, changedRatios.Add)
            .Add(p => p.OnCropped, data => croppedData = data));

        // Act
        ClickButton(cut, "Set aspect ratio 16:9");
        ClickButton(cut, "Crop image");

        // Assert
        changedRatios.Count.ShouldBe(1);
        changedRatios[0].ShouldBe(16.0 / 9.0, 0.0001);
        cut.Find("button[aria-label='Set aspect ratio 16:9']").GetAttribute("aria-pressed").ShouldBe("true");

        croppedData.ShouldNotBeNull();
        croppedData.AspectRatio.ShouldBe(16.0 / 9.0, 0.0001);
        croppedData.CropArea.Width.ShouldBe(80);
        croppedData.CropArea.Height.ShouldBe(45, 0.0001);
    }

    [Fact]
    public void ImageCropper_KeyboardArrows_MoveAndResizeCropArea()
    {
        // Arrange
        ImageCropper.CroppedImageData? croppedData = null;
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.OnCropped, data => croppedData = data));

        var cropBox = cut.Find(".crop-box");

        // Act
        cropBox.KeyDown("ArrowRight");
        cropBox.KeyDown("ArrowDown");
        cropBox.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true });
        ClickButton(cut, "Crop image");

        // Assert
        croppedData.ShouldNotBeNull();
        croppedData.CropArea.X.ShouldBe(11);
        croppedData.CropArea.Y.ShouldBe(11);
        croppedData.CropArea.Width.ShouldBe(81);
        croppedData.CropArea.Height.ShouldBe(80);
    }

    [Fact]
    public void ImageCropper_KeyboardShortcuts_ZoomAndReset()
    {
        // Arrange
        var cropped = new List<ImageCropper.CroppedImageData>();
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.OnCropped, cropped.Add));

        var cropBox = cut.Find(".crop-box");

        // Act
        cropBox.KeyDown("+");
        ClickButton(cut, "Crop image");
        cropBox.KeyDown("R");
        ClickButton(cut, "Crop image");

        // Assert
        cropped.Count.ShouldBe(2);
        cropped[0].Zoom.ShouldBe(1.1, 0.0001);
        cropped[1].Zoom.ShouldBe(1);
    }

    [Fact]
    public void ImageCropper_CropPayload_UsesSnapshotCropArea()
    {
        // Arrange
        var cropped = new List<ImageCropper.CroppedImageData>();
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.OnCropped, cropped.Add));

        // Act
        ClickButton(cut, "Crop image");
        cropped[0].CropArea.X = 99;
        cropped[0].CropArea.Width = 1;
        ClickButton(cut, "Crop image");

        // Assert
        cropped.Count.ShouldBe(2);
        cropped[1].CropArea.X.ShouldBe(10);
        cropped[1].CropArea.Width.ShouldBe(80);
    }

    [Fact]
    public void ImageCropper_NullAspectRatioOptions_DoNotRenderAspectButtons()
    {
        // Act
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.AspectRatioOptions, null!));

        // Assert
        cut.FindAll("button[aria-label^='Set aspect ratio']").ShouldBeEmpty();
        cut.Find(".cropper-controls").ShouldNotBeNull();
    }

    [Fact]
    public void ImageCropper_InvalidAspectRatios_AreTreatedAsFreeAndSkippedInOptions()
    {
        // Arrange
        ImageCropper.CroppedImageData? croppedData = null;
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.AspectRatio, double.NaN)
            .Add(p => p.AspectRatioOptions, new List<ImageCropper.AspectRatioOption>
            {
                new() { Label = "Invalid", Ratio = double.NaN },
                new() { Label = "Missing ratio", Ratio = -1 },
                new() { Label = "Square", Ratio = 1 }
            })
            .Add(p => p.OnCropped, data => croppedData = data));

        // Act
        ClickButton(cut, "Crop image");

        // Assert
        cut.FindAll("button[aria-label='Set aspect ratio Invalid']").ShouldBeEmpty();
        cut.FindAll("button[aria-label='Set aspect ratio Missing ratio']").ShouldBeEmpty();
        cut.Find("button[aria-label='Set aspect ratio Square']").ShouldNotBeNull();

        croppedData.ShouldNotBeNull();
        croppedData.AspectRatio.ShouldBe(0);
        croppedData.CropArea.Width.ShouldBe(80);
        croppedData.CropArea.Height.ShouldBe(80);
    }

    [Fact]
    public async Task ImageCropper_PointerCallbackUpdatesAndClampsCropArea()
    {
        ImageCropper.CroppedImageData? croppedData = null;
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg")
            .Add(p => p.OnCropped, data => croppedData = data));

        await cut.InvokeAsync(() => cut.Instance.HandlePointerCrop(25, 15, 60, 55));
        ClickButton(cut, "Crop image");

        croppedData.ShouldNotBeNull();
        croppedData.CropArea.X.ShouldBe(25);
        croppedData.CropArea.Y.ShouldBe(15);
        croppedData.CropArea.Width.ShouldBe(60);
        croppedData.CropArea.Height.ShouldBe(55);

        await cut.InvokeAsync(() => cut.Instance.HandlePointerCrop(95, 95, 40, 40));
        ClickButton(cut, "Crop image");
        croppedData.CropArea.X.ShouldBe(60);
        croppedData.CropArea.Y.ShouldBe(60);
    }

    [Fact]
    public void ImageCropper_UsesPointerHandlesAndSharedControlIcons()
    {
        var cut = Render<ImageCropper>(parameters => parameters
            .Add(p => p.ImageSource, "test.jpg"));

        var handles = cut.FindAll("[data-crop-handle]");
        handles.Count.ShouldBe(8);
        handles.Select(handle => handle.GetAttribute("data-crop-handle")).ShouldBeUnique();
        cut.FindAll(".cropper-controls svg.vibe-icon").Count.ShouldBe(6);
        cut.Markup.ShouldNotContain("↶");
        cut.Markup.ShouldNotContain("↷");
        cut.Markup.ShouldNotContain("⇄");
        cut.Markup.ShouldNotContain("⇅");
    }

    private static void ClickButton(IRenderedComponent<ImageCropper> cut, string ariaLabel)
    {
        cut.Find($"button[aria-label='{ariaLabel}']").Click();
    }
}
