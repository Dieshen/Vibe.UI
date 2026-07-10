using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Vibe.UI.Tests.Components.Form;

public class FormFieldTests : TestBase
{
    public class FieldValidationModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void FormField_RendersChildContentAndBaseClass()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .AddChildContent("<input id=\"email\" />"));

        cut.Find(".vibe-form-field").ShouldNotBeNull();
        cut.Find(".form-field-input input").GetAttribute("id").ShouldBe("email");
    }

    [Fact]
    public void FormField_RendersLabelAndDescription_WhenProvided()
    {
        var cut = Render<FormField<string>>(parameters => parameters
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
        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.Label, "Email")
            .AddChildContent("<input />"));

        var labelFor = cut.Find(".form-field-label").GetAttribute("for");
        labelFor.ShouldNotBeNullOrWhiteSpace();
        labelFor.ShouldStartWith("form-field-");
    }

    [Fact]
    public void FormField_OmitsOptionalLabelAndDescription_WhenNotProvided()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .AddChildContent("<input />"));

        cut.FindAll(".form-field-label").ShouldBeEmpty();
        cut.FindAll(".form-field-description").ShouldBeEmpty();
    }

    [Fact]
    public void FormField_AppliesHasErrorClass_WhenHasError()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.HasError, true)
            .AddChildContent("<input />"));

        cut.Find(".form-field-container").ClassList.ShouldContain("has-error");
    }

    [Fact]
    public void FormField_ForwardsAdditionalAttributesAndClass()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.Class, "stacked")
            .AddUnmatched("data-testid", "email-field")
            .AddChildContent("<input />"));

        var root = cut.Find(".vibe-form-field");
        root.ClassList.ShouldContain("stacked");
        root.GetAttribute("data-testid").ShouldBe("email-field");
        root.GetAttribute("role").ShouldBe("group");
    }

    [Fact]
    public void FormField_LinksLabelDescriptionAndExplicitError()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.Id, "email")
            .Add(p => p.Label, "Email")
            .Add(p => p.Description, "Use your work email")
            .Add(p => p.ErrorMessage, "Email is required")
            .Add(p => p.Required, true)
            .AddChildContent("<input id=\"email\" />"));

        var root = cut.Find(".vibe-form-field");
        root.GetAttribute("aria-labelledby").ShouldBe("email-label");
        root.GetAttribute("aria-describedby").ShouldBe("email-description email-error");
        root.GetAttribute("aria-invalid").ShouldBe("true");
        root.GetAttribute("aria-required").ShouldBe("true");

        cut.Find(".form-field-label").GetAttribute("id").ShouldBe("email-label");
        cut.Find(".form-field-description").GetAttribute("id").ShouldBe("email-description");
        cut.Find(".form-validation-message").GetAttribute("id").ShouldBe("email-error");
        cut.Find(".form-validation-message").GetAttribute("role").ShouldBe("alert");
    }

    [Fact]
    public void FormField_PreservesCallerDescribedBy_WhenAddingDescriptionAndError()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.Id, "email")
            .Add(p => p.Description, "Help")
            .Add(p => p.ErrorMessage, "Error")
            .AddUnmatched("aria-describedby", "external-help")
            .AddChildContent("<input />"));

        cut.Find(".vibe-form-field")
            .GetAttribute("aria-describedby")
            .ShouldBe("external-help email-description email-error");
    }

    [Fact]
    public void FormField_RendersEditContextValidationMessages()
    {
        var model = new FieldValidationModel();
        Expression<Func<string>> validationFor = () => model.Name;

        var cut = Render(builder =>
        {
            builder.OpenComponent<Vibe.UI.Components.Form<FieldValidationModel>>(0);
            builder.AddAttribute(1, "Model", model);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<FormField<string>>(0);
                childBuilder.AddAttribute(1, "Id", "name");
                childBuilder.AddAttribute(2, "Label", "Name");
                childBuilder.AddAttribute(3, "ValidationFor", validationFor);
                childBuilder.AddAttribute(4, "ChildContent", (RenderFragment)(inputBuilder =>
                    inputBuilder.AddMarkupContent(0, "<input id=\"name\" />")));
                childBuilder.CloseComponent();
                childBuilder.AddMarkupContent(5, "<button type=\"submit\">Submit</button>");
            }));
            builder.CloseComponent();
        });

        cut.Find("form").Submit();

        var root = cut.Find(".vibe-form-field");
        root.GetAttribute("aria-invalid").ShouldBe("true");
        root.GetAttribute("aria-describedby").ShouldBe("name-error");
        cut.Find(".form-field-container").ClassList.ShouldContain("has-error");
        cut.Find(".form-validation-message").TextContent.ShouldContain("Name is required");
    }

    [Fact]
    public void FormField_DoesNotRenderValidationMessageOutsideEditContext()
    {
        var model = new FieldValidationModel();
        Expression<Func<string>> validationFor = () => model.Name;

        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.ValidationFor, validationFor)
            .AddChildContent("<input id=\"name\" />"));

        cut.Find(".vibe-form-field").ShouldNotBeNull();
        cut.FindAll(".form-validation-message").ShouldBeEmpty();
    }
}
