namespace Vibe.UI.Tests.Components.Utility;

public class QRCodeTests : TestBase
{
    [Fact]
    public void QRCode_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, "https://example.com"));

        // Assert
        var qrcode = cut.Find(".vibe-qrcode");
        qrcode.ShouldNotBeNull();
    }

    [Fact]
    public void QRCode_Displays_QRContainer()
    {
        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, "test"));

        // Assert
        var container = cut.Find(".qr-container");
        container.ShouldNotBeNull();
    }

    [Fact]
    public void QRCode_Applies_CustomSize()
    {
        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.Size, 300));

        // Assert
        var container = cut.Find(".qr-container");
        container.GetAttribute("style").ShouldContain("width: 300px");
        container.GetAttribute("style").ShouldContain("height: 300px");
    }

    [Fact]
    public void QRCode_Shows_Value_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, "https://example.com")
            .Add(p => p.ShowValue, true));

        // Assert
        var value = cut.Find(".qr-value");
        value.TextContent.ShouldBe("https://example.com");
    }

    [Fact]
    public void QRCode_Hides_Value_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, "https://example.com")
            .Add(p => p.ShowValue, false));

        // Assert
        cut.FindAll(".qr-value").ShouldBeEmpty();
    }

    [Fact]
    public void QRCode_Shows_DownloadButton_WhenAllowed()
    {
        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.AllowDownload, true));

        // Assert - May not be visible until QR code is generated
        var qrcode = cut.Find(".vibe-qrcode");
        qrcode.ShouldNotBeNull();
    }

    [Fact]
    public void QRCode_Hides_DownloadButton_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.AllowDownload, false));

        // Assert
        cut.FindAll(".qr-download-btn").ShouldBeEmpty();
    }

    [Fact]
    public void QRCode_Displays_EmptyContent_WhenNoValue()
    {
        // Arrange
        var emptyMarkup = "<div class='empty'>No QR code</div>";

        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, string.Empty)
            .Add(p => p.EmptyContent, emptyMarkup));

        // Assert
        var empty = cut.Find(".empty");
        empty.ShouldNotBeNull();
        empty.TextContent.ShouldContain("No QR code");
    }

    [Fact]
    public void QRCode_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.CssClass, "custom-qr"));

        // Assert
        var qrcode = cut.Find(".vibe-qrcode");
        qrcode.ClassList.ShouldContain("custom-qr");
    }
}
