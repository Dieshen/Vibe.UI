namespace Vibe.UI.Tests.Components.Input;

public class ImageCropperTests : TestContext
{
    public ImageCropperTests() { this.AddVibeUIServices(); }
    [Fact] public void ImageCropper_Renders() { var cut = RenderComponent<ImageCropper>(); cut.Markup.Should().NotBeEmpty(); }
}
