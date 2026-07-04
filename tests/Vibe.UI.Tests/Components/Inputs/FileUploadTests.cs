using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Vibe.UI.Tests.Components.Inputs;

public class FileUploadTests : TestBase
{
    [Fact]
    public void FileUpload_Renders_WithAccessibleDefaultProps()
    {
        // Act
        var cut = Render<FileUpload>();

        // Assert
        var upload = cut.Find(".vibe-file-upload");
        upload.GetAttribute("role").ShouldBe("group");
        upload.GetAttribute("aria-label").ShouldBe("File upload");
        upload.GetAttribute("aria-disabled").ShouldBe("false");

        var input = cut.Find("input[type='file']");
        input.ClassList.ShouldContain("file-upload-input");
        input.GetAttribute("id").ShouldStartWith("vibe-file-upload-");
        input.GetAttribute("aria-label").ShouldBe("File upload");
        input.GetAttribute("aria-describedby").ShouldBe(cut.Find(".file-upload-hint").GetAttribute("id"));
    }

    [Fact]
    public void FileUpload_Shows_EmptyState_WhenNoFiles()
    {
        // Act
        var cut = Render<FileUpload>();

        // Assert
        cut.Find(".file-upload-empty").ShouldNotBeNull();
        cut.Find(".file-upload-title").TextContent.ShouldBe("Drop files here or click to browse");
        cut.Find(".file-upload-hint").TextContent.ShouldBe("Supports: Images, PDFs, Documents");
        cut.Find(".file-upload-button").TextContent.ShouldContain("Browse Files");
    }

    [Fact]
    public void FileUpload_Renders_CustomDropAndHintText()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.DropText, "Drag and drop files here")
            .Add(p => p.HintText, "Maximum file size: 10MB"));

        // Assert
        cut.Find(".file-upload-title").TextContent.ShouldBe("Drag and drop files here");
        cut.Find(".file-upload-hint").TextContent.ShouldBe("Maximum file size: 10MB");
    }

    [Fact]
    public void FileUpload_Applies_InputAttributes()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Multiple, true)
            .Add(p => p.Accept, "image/*,.pdf")
            .Add(p => p.AriaLabel, "Upload attachments"));

        // Assert
        var input = cut.Find("input[type='file']");
        input.HasAttribute("multiple").ShouldBeTrue();
        input.GetAttribute("accept").ShouldBe("image/*,.pdf");
        input.GetAttribute("aria-label").ShouldBe("Upload attachments");
    }

    [Fact]
    public void FileUpload_Applies_CustomCssClass()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.CssClass, "custom-upload")
            .Add(p => p.Class, "base-class"));

        // Assert
        var upload = cut.Find(".vibe-file-upload");
        upload.ClassList.ShouldContain("custom-upload");
        upload.ClassList.ShouldContain("base-class");
    }

    [Fact]
    public void FileUpload_SelectingValidFile_AddsFileAndInvokesCallbacks()
    {
        // Arrange
        var changedFiles = new List<FileUpload.UploadedFile>();
        var addedFiles = new List<FileUpload.UploadedFile>();
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Accept, ".pdf")
            .Add(p => p.FilesChanged, files => changedFiles = files)
            .Add(p => p.OnFileAdded, addedFiles.Add));

        // Act
        UploadFiles(cut, TextFile("hello", "C:\\fakepath\\report.pdf", "application/octet-stream"));

        // Assert
        cut.WaitForAssertion(() =>
        {
            cut.Find(".file-item-name").TextContent.ShouldBe("report.pdf");
            cut.Find(".file-item-size").TextContent.ShouldBe("5 B");
            cut.Find(".file-upload-add").TextContent.ShouldContain("Add More Files");

            changedFiles.Count.ShouldBe(1);
            addedFiles.Count.ShouldBe(1);
            addedFiles[0].Name.ShouldBe("report.pdf");
            addedFiles[0].Size.ShouldBe(5);
            addedFiles[0].ContentType.ShouldBe("application/octet-stream");
            addedFiles[0].Data.ShouldNotBeNull();
            addedFiles[0].Data!.Length.ShouldBe(5);
        });
    }

    [Fact]
    public void FileUpload_MultipleFalse_AddsOnlyFirstSelectedFile()
    {
        // Arrange
        var changedFiles = new List<FileUpload.UploadedFile>();
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Multiple, false)
            .Add(p => p.FilesChanged, files => changedFiles = files));

        // Act
        UploadFiles(
            cut,
            TextFile("first", "first.txt", "text/plain"),
            TextFile("second", "second.txt", "text/plain"));

        // Assert
        cut.WaitForAssertion(() =>
        {
            changedFiles.Count.ShouldBe(1);
            changedFiles[0].Name.ShouldBe("first.txt");
            cut.FindAll(".file-upload-item").Count.ShouldBe(1);
            cut.Markup.ShouldNotContain("second.txt");
        });
    }

    [Fact]
    public void FileUpload_RejectsFile_WhenOverMaxFileSize()
    {
        // Arrange
        var addedCount = 0;
        var changedCount = 0;
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.MaxFileSize, 4)
            .Add(p => p.FilesChanged, _ => changedCount++)
            .Add(p => p.OnFileAdded, _ => addedCount++));

        // Act
        UploadFiles(cut, TextFile("hello", "large.txt", "text/plain"));

        // Assert
        cut.WaitForAssertion(() =>
        {
            cut.Find(".vibe-file-upload").ClassList.ShouldContain("file-upload-error");
            cut.Find("[role='alert']").TextContent.ShouldContain("large.txt is larger than the 4 B limit.");
            cut.FindAll(".file-upload-item").ShouldBeEmpty();
            addedCount.ShouldBe(0);
            changedCount.ShouldBe(0);
        });
    }

    [Fact]
    public void FileUpload_RejectsFile_WhenAcceptDoesNotMatch()
    {
        // Arrange
        var addedCount = 0;
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Accept, "image/*,.pdf")
            .Add(p => p.OnFileAdded, _ => addedCount++));

        // Act
        UploadFiles(cut, TextFile("notes", "notes.txt", "text/plain"));

        // Assert
        cut.WaitForAssertion(() =>
        {
            cut.Find("[role='alert']").TextContent.ShouldContain("notes.txt is not an accepted file type.");
            cut.FindAll(".file-upload-item").ShouldBeEmpty();
            addedCount.ShouldBe(0);
        });
    }

    [Fact]
    public void FileUpload_AcceptsWildcardMimeType()
    {
        // Arrange
        var changedFiles = new List<FileUpload.UploadedFile>();
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Accept, "image/*")
            .Add(p => p.FilesChanged, files => changedFiles = files));

        // Act
        UploadFiles(cut, TextFile("png", "avatar.png", "image/png"));

        // Assert
        cut.WaitForAssertion(() =>
        {
            changedFiles.Count.ShouldBe(1);
            changedFiles[0].Name.ShouldBe("avatar.png");
        });
    }

    [Fact]
    public void FileUpload_EnforcesMaxFiles_AndStillAddsAllowedFiles()
    {
        // Arrange
        var addedFiles = new List<FileUpload.UploadedFile>();
        var changedFiles = new List<FileUpload.UploadedFile>();
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.MaxFiles, 1)
            .Add(p => p.FilesChanged, files => changedFiles = files)
            .Add(p => p.OnFileAdded, addedFiles.Add));

        // Act
        UploadFiles(
            cut,
            TextFile("one", "one.txt", "text/plain"),
            TextFile("two", "two.txt", "text/plain"));

        // Assert
        cut.WaitForAssertion(() =>
        {
            changedFiles.Count.ShouldBe(1);
            addedFiles.Count.ShouldBe(1);
            cut.Find(".file-upload-limit").TextContent.ShouldBe("Maximum 1 file selected");
            cut.Find("[role='alert']").TextContent.ShouldBe("Maximum 1 file selected");
            cut.Markup.ShouldContain("one.txt");
            cut.Markup.ShouldNotContain("two.txt");
        });
    }

    [Fact]
    public void FileUpload_RemoveButton_RemovesFileAndInvokesCallbacks()
    {
        // Arrange
        var removedFiles = new List<FileUpload.UploadedFile>();
        var changedFiles = new List<FileUpload.UploadedFile>();
        var report = new FileUpload.UploadedFile { Name = "report.pdf", Size = 1024, ContentType = "application/pdf" };
        var photo = new FileUpload.UploadedFile { Name = "photo.png", Size = 2048, ContentType = "image/png" };
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Files, new List<FileUpload.UploadedFile> { report, photo })
            .Add(p => p.FilesChanged, files => changedFiles = files)
            .Add(p => p.OnFileRemoved, removedFiles.Add));

        // Act
        cut.Find("[aria-label='Remove report.pdf']").Click();

        // Assert
        cut.WaitForAssertion(() =>
        {
            removedFiles.ShouldBe(new[] { report });
            changedFiles.ShouldBe(new[] { photo });
            cut.Markup.ShouldNotContain("report.pdf");
            cut.Markup.ShouldContain("photo.png");
        });
    }

    [Fact]
    public void FileUpload_DisabledState_DisablesSelectionAndRemoval()
    {
        // Arrange
        var addedCount = 0;
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnFileAdded, _ => addedCount++));

        // Act
        UploadFiles(cut, TextFile("hello", "disabled.txt", "text/plain"));

        // Assert
        cut.Find(".vibe-file-upload").ClassList.ShouldContain("vibe-file-upload-disabled");
        cut.Find(".vibe-file-upload").GetAttribute("aria-disabled").ShouldBe("true");
        cut.Find("input[type='file']").HasAttribute("disabled").ShouldBeTrue();
        cut.FindAll(".file-upload-item").ShouldBeEmpty();
        addedCount.ShouldBe(0);

        var withFile = Render<FileUpload>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.Files, new List<FileUpload.UploadedFile>
            {
                new() { Name = "report.pdf", Size = 1024 }
            }));

        withFile.FindAll(".file-item-remove").ShouldBeEmpty();
    }

    [Fact]
    public void FileUpload_DragEvents_ToggleDraggingClass()
    {
        // Arrange
        var cut = Render<FileUpload>();

        // Act
        cut.Find(".vibe-file-upload").TriggerEvent("ondragenter", new DragEventArgs());

        // Assert
        cut.Find(".vibe-file-upload").ClassList.ShouldContain("file-upload-dragging");

        // Act
        cut.Find(".vibe-file-upload").TriggerEvent("ondragleave", new DragEventArgs());

        // Assert
        cut.Find(".vibe-file-upload").ClassList.ShouldNotContain("file-upload-dragging");

        // Act
        cut.Find(".vibe-file-upload").TriggerEvent("ondragenter", new DragEventArgs());
        cut.Find(".vibe-file-upload").TriggerEvent("ondrop", new DragEventArgs());

        // Assert
        cut.Find(".vibe-file-upload").ClassList.ShouldNotContain("file-upload-dragging");
    }

    [Fact]
    public void FileUpload_Disabled_DoesNotEnterDraggingState()
    {
        // Arrange
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Act
        cut.Find(".vibe-file-upload").TriggerEvent("ondragenter", new DragEventArgs());

        // Assert
        cut.Find(".vibe-file-upload").ClassList.ShouldNotContain("file-upload-dragging");
    }

    [Fact]
    public void FileUpload_ErrorMessageParameter_RendersAlertAndDescribesGroup()
    {
        // Act
        var cut = Render<FileUpload>(parameters => parameters
            .Add(p => p.ErrorMessage, "Upload failed."));

        // Assert
        var alert = cut.Find("[role='alert']");
        alert.TextContent.ShouldBe("Upload failed.");
        cut.Find(".vibe-file-upload").ClassList.ShouldContain("file-upload-error");
        cut.Find(".vibe-file-upload").GetAttribute("aria-describedby").ShouldBe(alert.GetAttribute("id"));
        var alertId = alert.GetAttribute("id");
        alertId.ShouldNotBeNull();
        var describedBy = cut.Find("input[type='file']").GetAttribute("aria-describedby");
        describedBy.ShouldNotBeNull();
        describedBy.ShouldContain(alertId);
    }

    private static void UploadFiles(IRenderedComponent<FileUpload> cut, params InputFileContent[] files)
    {
        cut.FindComponent<InputFile>().UploadFiles(files);
    }

    private static InputFileContent TextFile(string content, string fileName, string contentType) =>
        InputFileContent.CreateFromText(content, fileName, DateTimeOffset.UtcNow, contentType);
}
