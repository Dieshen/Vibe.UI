namespace Vibe.UI.Tests.Components.Form;

public class FormFieldTests : TestBase
{
    [Fact]
    public void FormField_RendersChildContentAndBaseClass()
    {
        var cut = RenderComponent<FormField<string>>(parameters => parameters
            .AddChildContent("<input id=\"email\" />"));

        cut.Find(".vibe-form-field").ShouldNotBeNull();
        cut.Find(".form-field-input input").GetAttribute("id").ShouldBe("email");
    }

    [Fact]
    public void FormField_RendersLabelAndDescription_WhenProvided()
    {
        var cut = RenderComponent<FormField<string>>(parameters => parameters
            .Add(p => p.Id, "email")
            .Add(p => p.Label, "Email")
            .Add(p => p.Description, "Use your work email")
            .AddChildContent("<input id=\"email\" />"));

        var label = cut.Find(".form-field-label");
        label.TextContent.ShouldBe("Email");
        label.GetAttribute("for").ShouldBe("email");
        cut.Find(".form-field-description").TextContent.ShouldBe("Use your work email");
    }

    [Fact]
    public void FormField_GeneratesId_WhenIdIsMissing()
    {
        var cut = RenderComponent<FormField<string>>(parameters => parameters
            .Add(p => p.Label, "Email")
            .AddChildContent("<input />"));

        var labelFor = cut.Find(".form-field-label").GetAttribute("for");
        labelFor.ShouldNotBeNullOrWhiteSpace();
        labelFor.ShouldStartWith("form-field-");
    }

    [Fact]
    public void FormField_OmitsOptionalLabelAndDescription_WhenNotProvided()
    {
        var cut = RenderComponent<FormField<string>>(parameters => parameters
            .AddChildContent("<input />"));

        cut.FindAll(".form-field-label").ShouldBeEmpty();
        cut.FindAll(".form-field-description").ShouldBeEmpty();
    }

    [Fact]
    public void FormField_AppliesHasErrorClass_WhenHasError()
    {
        var cut = RenderComponent<FormField<string>>(parameters => parameters
            .Add(p => p.HasError, true)
            .AddChildContent("<input />"));

        cut.Find(".form-field-container").ClassList.ShouldContain("has-error");
    }
}
