namespace Vibe.UI.Tests.Components.Inputs;

public class FileUploadTests : TestBase
{
    [Fact]
    public void FileUpload_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<FileUpload>();

        // Assert
        var upload = cut.Find(".vibe-file-upload");
        upload.ShouldNotBeNull();
    }

    [Fact]
    public void FileUpload_Shows_EmptyState_WhenNoFiles()
    {
        // Act
        var cut = Render<FileUpload>();

        // Assert
        var empty = cut.Find(".file-upload-empty");
        empty.ShouldNotBeNull();
    }

    [Fact]
    public void FileUpload_Displays_DropText()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.DropText, "Drag and drop files here"));

        // Assert
        var text = cut.Find(".file-upload-title");
        text.TextContent.ShouldBe("Drag and drop files here");
    }

    [Fact]
    public void FileUpload_Displays_HintText()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.HintText, "Maximum file size: 10MB"));

        // Assert
        var hint = cut.Find(".file-upload-hint");
        hint.TextContent.ShouldBe("Maximum file size: 10MB");
    }

    [Fact]
    public void FileUpload_Renders_HiddenFileInput()
    {
        // Act
        var cut = Render<FileUpload>();

        // Assert
        var input = cut.Find("input[type='file']");
        input.ClassList.ShouldContain("file-upload-input");
        input.GetAttribute("id").ShouldStartWith("vibe-file-upload-");
    }

    [Fact]
    public void FileUpload_Supports_MultipleFiles()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Multiple, true));

        // Assert
        var input = cut.Find("input[type='file']");
        input.HasAttribute("multiple").ShouldBeTrue();
    }

    [Fact]
    public void FileUpload_Applies_AcceptAttribute()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Accept, "image/*,.pdf"));

        // Assert
        var input = cut.Find("input[type='file']");
        input.GetAttribute("accept")!.ShouldBe("image/*,.pdf");
    }

    [Fact]
    public void FileUpload_Renders_BrowseButton()
    {
        // Act
        var cut = Render<FileUpload>();

        // Assert
        var button = cut.Find(".file-upload-button");
        button.TextContent.ShouldContain("Browse Files");
        button.Click();
    }

    [Fact]
    public void FileUpload_EmptyStateIsKeyboardAccessible()
    {
        var cut = Render<FileUpload>();

        var empty = cut.Find(".file-upload-empty");
        empty.GetAttribute("role").ShouldBe("button");
        empty.GetAttribute("tabindex").ShouldBe("0");
        empty.GetAttribute("aria-controls").ShouldBe(cut.Find("input[type='file']").GetAttribute("id"));

        empty.KeyDown("Enter");
        empty.KeyDown(" ");
    }

    [Fact]
    public void FileUpload_RemoveButtonHasAccessibleLabel()
    {
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Files, new List<FileUpload.UploadedFile>
            {
                new() { Name = "report.pdf", Size = 1024 }
            }));

        cut.Find(".file-item-remove").GetAttribute("aria-label").ShouldBe("Remove report.pdf");
    }

    [Fact]
    public void FileUpload_Applies_CustomCssClass()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.CssClass, "custom-upload"));

        // Assert
        cut.Find(".vibe-file-upload").ClassList.ShouldContain("custom-upload");
    }
}
