namespace Vibe.UI.Tests.Components.Form;

public class FormMessageTests : TestBase
{
    [Fact]
    public void FormMessage_RendersChildContentAndDefaultVariant()
    {
        var cut = RenderComponent<FormMessage>(parameters => parameters
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
        var cut = RenderComponent<FormMessage>(parameters => parameters
            .Add(p => p.Variant, variant)
            .AddChildContent("Message"));

        cut.Find("p").ClassList.ShouldContain(expectedClass);
    }

    [Fact]
    public void FormMessage_RendersIconMarkup_WhenIconProvided()
    {
        var cut = RenderComponent<FormMessage>(parameters => parameters
            .Add(p => p.Icon, "<svg aria-hidden=\"true\"></svg>")
            .AddChildContent("Error"));

        var icon = cut.Find(".form-message-icon");
        icon.InnerHtml.ShouldContain("<svg");
        cut.Find("p").TextContent.ShouldContain("Error");
    }

    [Fact]
    public void FormMessage_OmitsIcon_WhenIconIsNullOrEmpty()
    {
        var withoutIcon = RenderComponent<FormMessage>(parameters => parameters
            .Add(p => p.Icon, null)
            .AddChildContent("Message"));
        var withEmptyIcon = RenderComponent<FormMessage>(parameters => parameters
            .Add(p => p.Icon, "")
            .AddChildContent("Message"));

        withoutIcon.FindAll(".form-message-icon").ShouldBeEmpty();
        withEmptyIcon.FindAll(".form-message-icon").ShouldBeEmpty();
    }

    [Fact]
    public void FormMessage_ForwardsAdditionalAttributes()
    {
        var cut = RenderComponent<FormMessage>(parameters => parameters
            .AddUnmatched("role", "alert")
            .AddChildContent("Error"));

        cut.Find("p").GetAttribute("role").ShouldBe("alert");
    }
}
