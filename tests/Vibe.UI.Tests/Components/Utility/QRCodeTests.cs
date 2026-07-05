namespace Vibe.UI.Tests.Components.Utility;

public class QRCodeTests : TestBase
{
    [Fact]
    public void QRCode_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "https://example.com"));

        // Assert
        var qrcode = cut.Find(".vibe-qrcode");
        qrcode.ShouldNotBeNull();
        qrcode.GetAttribute("data-state").ShouldBe("ready");
        qrcode.GetAttribute("data-level").ShouldBe("m");
        cut.Find("img.qr-image").GetAttribute("alt").ShouldBe("QR code");
    }

    [Fact]
    public void QRCode_Displays_QRContainer()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test"));

        // Assert
        var container = cut.Find(".qr-container");
        container.ShouldNotBeNull();
    }

    [Fact]
    public void QRCode_Applies_CustomSize()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.Size, 300));

        // Assert
        var container = cut.Find(".qr-container");
        container.GetAttribute("style")!.ShouldContain("width: 300px");
        container.GetAttribute("style")!.ShouldContain("height: 300px");
    }

    [Theory]
    [InlineData(0, "1px")]
    [InlineData(-10, "1px")]
    [InlineData(5000, "2048px")]
    public void QRCode_ClampsUnsafeSizes(int size, string expectedSize)
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.Size, size));

        // Assert
        var style = cut.Find(".qr-container").GetAttribute("style")!;
        style.ShouldContain($"width: {expectedSize}");
        style.ShouldContain($"height: {expectedSize}");
    }

    [Fact]
    public void QRCode_Shows_Value_WhenEnabled()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "https://example.com")
            .Add(p => p.ShowValue, true));

        // Assert
        var value = cut.Find(".qr-value");
        value.TextContent.ShouldBe("https://example.com");
    }

    [Fact]
    public void QRCode_TrimsValueForDisplayAndGeneration()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "  https://example.com  ")
            .Add(p => p.ShowValue, true));

        // Assert
        cut.Find(".qr-value").TextContent.ShouldBe("https://example.com");
        cut.Find("img.qr-image").GetAttribute("src")!.ShouldStartWith("data:image/svg+xml;base64,");
    }

    [Fact]
    public void QRCode_Hides_Value_WhenDisabled()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "https://example.com")
            .Add(p => p.ShowValue, false));

        // Assert
        cut.FindAll(".qr-value").ShouldBeEmpty();
    }

    [Fact]
    public void QRCode_Shows_DownloadButton_WhenAllowed()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.AllowDownload, true));

        // Assert
        var download = cut.Find(".qr-download-btn");
        download.GetAttribute("href")!.ShouldStartWith("data:image/svg+xml;base64,");
        download.GetAttribute("download").ShouldBe("qrcode.png");
    }

    [Fact]
    public void QRCode_Hides_DownloadButton_WhenDisabled()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
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
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, string.Empty)
            .Add(p => p.EmptyContent, emptyMarkup));

        // Assert
        var empty = cut.Find(".empty");
        empty.ShouldNotBeNull();
        empty.TextContent.ShouldContain("No QR code");
    }

    [Fact]
    public void QRCode_DisplaysDefaultEmptyState_WhenNoValueOrEmptyContent()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "  "));

        // Assert
        var qrcode = cut.Find(".vibe-qrcode");
        qrcode.GetAttribute("data-state").ShouldBe("empty");
        cut.Find(".qr-empty").GetAttribute("role").ShouldBe("status");
        cut.Find(".qr-empty").TextContent.ShouldBe("No QR code value");
        cut.FindAll("img.qr-image").ShouldBeEmpty();
    }

    [Fact]
    public void QRCode_Applies_CustomCssClass()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.CssClass, "custom-qr")
            .Add(p => p.Class, "outer-qr")
            .AddUnmatched("data-testid", "qr"));

        // Assert
        var qrcode = cut.Find(".vibe-qrcode");
        qrcode.ClassList.ShouldContain("custom-qr");
        qrcode.ClassList.ShouldContain("outer-qr");
        qrcode.GetAttribute("data-testid").ShouldBe("qr");
    }

    [Fact]
    public void QRCode_UsesCustomImageAltText()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.AriaLabel, "Link to setup guide"));

        // Assert
        cut.Find("img.qr-image").GetAttribute("alt").ShouldBe("Link to setup guide");
    }

    [Fact]
    public void QRCode_InvalidLevelFallsBackToMedium()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.Level, (QRCode.QRCodeLevel)999));

        // Assert
        cut.Find(".vibe-qrcode").GetAttribute("data-level").ShouldBe("m");
    }

    [Fact]
    public void QRCode_SanitizesSvgPaintValues()
    {
        // Act
        var cut = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test")
            .Add(p => p.ForegroundColor, "red\" onload=\"alert(1)")
            .Add(p => p.BackgroundColor, "url(javascript:alert(1))"));

        // Assert
        var svg = DecodeSvgDataUrl(cut.Find("img.qr-image").GetAttribute("src")!);
        svg.ShouldContain("fill='#000000'");
        svg.ShouldContain("fill='#FFFFFF'");
        svg.ShouldNotContain("onload");
        svg.ShouldNotContain("javascript");
    }

    [Fact]
    public void QRCode_GeneratesDeterministicSvgForSameValue()
    {
        // Act
        var first = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test"));
        var second = Render<QRCode>(parameters => parameters
            .Add(p => p.Value, "test"));

        // Assert
        first.Find("img.qr-image").GetAttribute("src").ShouldBe(second.Find("img.qr-image").GetAttribute("src"));
    }

    private static string DecodeSvgDataUrl(string dataUrl)
    {
        const string prefix = "data:image/svg+xml;base64,";
        dataUrl.ShouldStartWith(prefix);

        var payload = dataUrl[prefix.Length..];
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
    }
}
