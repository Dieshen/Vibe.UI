namespace Vibe.UI.Tests.Components.Input;

public class FileUploadTests : TestBase
{
    [Fact]
    public void FileUpload_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<FileUpload>();

        // Assert
        var upload = cut.Find(".vibe-file-upload");
        upload.ShouldNotBeNull();
    }

    [Fact]
    public void FileUpload_Shows_EmptyState_WhenNoFiles()
    {
        // Act
        var cut = RenderComponent<FileUpload>();

        // Assert
        var empty = cut.Find(".file-upload-empty");
        empty.ShouldNotBeNull();
    }

    [Fact]
    public void FileUpload_Displays_DropText()
    {
        // Act
        var cut = RenderComponent<FileUpload>(parameters => parameters
            .Add(p => p.DropText, "Drag and drop files here"));

        // Assert
        var text = cut.Find(".file-upload-title");
        text.TextContent.ShouldBe("Drag and drop files here");
    }

    [Fact]
    public void FileUpload_Displays_HintText()
    {
        // Act
        var cut = RenderComponent<FileUpload>(parameters => parameters
            .Add(p => p.HintText, "Maximum file size: 10MB"));

        // Assert
        var hint = cut.Find(".file-upload-hint");
        hint.TextContent.ShouldBe("Maximum file size: 10MB");
    }

    [Fact]
    public void FileUpload_Renders_HiddenFileInput()
    {
        // Act
        var cut = RenderComponent<FileUpload>();

        // Assert
        var input = cut.Find("input[type='file']");
        input.GetAttribute("style").ShouldContain("display: none");
    }

    [Fact]
    public void FileUpload_Supports_MultipleFiles()
    {
        // Act
        var cut = RenderComponent<FileUpload>(parameters => parameters
            .Add(p => p.Multiple, true));

        // Assert
        var input = cut.Find("input[type='file']");
        input.HasAttribute("multiple").ShouldBeTrue();
    }

    [Fact]
    public void FileUpload_Applies_AcceptAttribute()
    {
        // Act
        var cut = RenderComponent<FileUpload>(parameters => parameters
            .Add(p => p.Accept, "image/*,.pdf"));

        // Assert
        var input = cut.Find("input[type='file']");
        input.GetAttribute("accept").ShouldBe("image/*,.pdf");
    }

    [Fact]
    public void FileUpload_Renders_BrowseButton()
    {
        // Act
        var cut = RenderComponent<FileUpload>();

        // Assert
        var button = cut.Find(".file-upload-button");
        button.TextContent.ShouldContain("Browse Files");
    }

    [Fact]
    public void FileUpload_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<FileUpload>(parameters => parameters
            .Add(p => p.CssClass, "custom-upload"));

        // Assert
        cut.Find(".vibe-file-upload").ClassList.ShouldContain("custom-upload");
    }
}
