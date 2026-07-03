namespace Vibe.UI.Tests.Components.Form;

public class FormLabelTests : TestBase
{
    [Fact]
    public void FormLabel_RendersChildContentAndBaseClass()
    {
        var cut = Render<FormLabel>(parameters => parameters
            .AddChildContent("Email"));

        var label = cut.Find("label");
        label.TextContent.ShouldBe("Email");
        label.ClassList.ShouldContain("vibe-form-label");
    }

    [Fact]
    public void FormLabel_AssociatesWithControl_WhenForProvided()
    {
        var cut = Render<FormLabel>(parameters => parameters
            .Add(p => p.For, "email-input")
            .AddChildContent("Email"));

        cut.Find("label").GetAttribute("for").ShouldBe("email-input");
    }

    [Fact]
    public void FormLabel_OmitsForAttribute_WhenForIsNull()
    {
        var cut = Render<FormLabel>(parameters => parameters
            .Add(p => p.For, null)
            .AddChildContent("Email"));

        cut.Find("label").HasAttribute("for").ShouldBeFalse();
    }

    [Fact]
    public void FormLabel_RendersRequiredIndicator_WhenRequired()
    {
        var cut = Render<FormLabel>(parameters => parameters
            .Add(p => p.Required, true)
            .AddChildContent("Email"));

        var indicator = cut.Find(".required-indicator");
        indicator.TextContent.ShouldBe("*");
        cut.Find("label").TextContent.ShouldContain("Email");
    }

    [Fact]
    public void FormLabel_ForwardsAdditionalAttributes()
    {
        var cut = Render<FormLabel>(parameters => parameters
            .AddUnmatched("data-testid", "email-label")
            .AddChildContent("Email"));

        cut.Find("label").GetAttribute("data-testid").ShouldBe("email-label");
    }
}
