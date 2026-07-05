namespace Vibe.UI.Tests.Components.Form;

public class FormMessageTests : TestBase
{
    [Fact]
    public void FormMessage_RendersChildContentAndDefaultVariant()
    {
        var cut = Render<FormMessage>(parameters => parameters
            .AddChildContent("Saved successfully"));

        var message = cut.Find("p");
        message.TextContent.ShouldBe("Saved successfully");
        message.ClassList.ShouldContain("vibe-form-message");
        message.ClassList.ShouldContain("form-message-default");
    }

    [Theory]
    [InlineData("error", "form-message-error")]
    [InlineData("warning", "form-message-warning")]
    [InlineData("success", "form-message-success")]
    [InlineData("info", "form-message-info")]
    public void FormMessage_AppliesVariantClass(string variant, string expectedClass)
    {
        var cut = Render<FormMessage>(parameters => parameters
            .Add(p => p.Variant, variant)
            .AddChildContent("Message"));

        cut.Find("p").ClassList.ShouldContain(expectedClass);
    }

    [Fact]
    public void FormMessage_RendersIconMarkup_WhenIconProvided()
    {
        var cut = Render<FormMessage>(parameters => parameters
            .Add(p => p.Icon, "<svg aria-hidden=\"true\"></svg>")
            .AddChildContent("Error"));

        var icon = cut.Find(".form-message-icon");
        icon.InnerHtml.ShouldContain("<svg");
        icon.GetAttribute("aria-hidden").ShouldBe("true");
        cut.Find("p").TextContent.ShouldContain("Error");
    }

    [Fact]
    public void FormMessage_OmitsIcon_WhenIconIsNullOrEmpty()
    {
        var withoutIcon = Render<FormMessage>(parameters => parameters
            .Add(p => p.Icon, null)
            .AddChildContent("Message"));
        var withEmptyIcon = Render<FormMessage>(parameters => parameters
            .Add(p => p.Icon, "")
            .AddChildContent("Message"));

        withoutIcon.FindAll(".form-message-icon").ShouldBeEmpty();
        withEmptyIcon.FindAll(".form-message-icon").ShouldBeEmpty();
    }

    [Fact]
    public void FormMessage_ForwardsAdditionalAttributes()
    {
        var cut = Render<FormMessage>(parameters => parameters
            .AddUnmatched("role", "alert")
            .AddChildContent("Error"));

        cut.Find("p").GetAttribute("role").ShouldBe("alert");
    }

    [Fact]
    public void FormMessage_AddsAlertSemantics_ForErrorVariant()
    {
        var cut = Render<FormMessage>(parameters => parameters
            .Add(p => p.Variant, "error")
            .AddChildContent("Email is required"));

        var message = cut.Find("p");
        message.GetAttribute("role").ShouldBe("alert");
        message.GetAttribute("aria-live").ShouldBe("assertive");
    }

    [Fact]
    public void FormMessage_PreservesCallerRole_WhenErrorVariant()
    {
        var cut = Render<FormMessage>(parameters => parameters
            .Add(p => p.Variant, "error")
            .AddUnmatched("role", "status")
            .AddUnmatched("aria-live", "polite")
            .AddChildContent("Saved"));

        var message = cut.Find("p");
        message.GetAttribute("role").ShouldBe("status");
        message.GetAttribute("aria-live").ShouldBe("polite");
    }

    [Fact]
    public void FormMessage_MergesClassParameterAndUnmatchedClass()
    {
        var cut = Render<FormMessage>(parameters => parameters
            .Add(p => p.Class, "from-parameter")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "class", "from-attributes" }
            })
            .AddChildContent("Message"));

        var message = cut.Find("p");
        message.ClassList.ShouldContain("vibe-form-message");
        message.ClassList.ShouldContain("form-message-default");
        message.ClassList.ShouldContain("from-parameter");
        message.ClassList.ShouldContain("from-attributes");
    }

    [Theory]
    [InlineData(null, "form-message-default")]
    [InlineData("", "form-message-default")]
    [InlineData("  Error  ", "form-message-error")]
    [InlineData("custom state", "form-message-custom-state")]
    public void FormMessage_NormalizesVariantClass(string? variant, string expectedClass)
    {
        var cut = Render<FormMessage>(parameters => parameters
            .Add(p => p.Variant, variant)
            .AddChildContent("Message"));

        cut.Find("p").ClassList.ShouldContain(expectedClass);
    }
}
