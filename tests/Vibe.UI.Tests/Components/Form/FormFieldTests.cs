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
            .Add(p => p.ChildContent, RenderInput()));

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
            .Add(p => p.ChildContent, RenderInput()));

        var labelFor = cut.Find(".form-field-label").GetAttribute("for");
        labelFor.ShouldNotBeNullOrWhiteSpace();
        labelFor.ShouldStartWith("form-field-");
        cut.Find("input").GetAttribute("id").ShouldBe(labelFor);
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
        root.HasAttribute("role").ShouldBeFalse();
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
            .Add(p => p.ChildContent, RenderInput()));

        var root = cut.Find(".vibe-form-field");
        root.HasAttribute("aria-describedby").ShouldBeFalse();
        root.HasAttribute("aria-invalid").ShouldBeFalse();
        root.HasAttribute("aria-required").ShouldBeFalse();

        var input = cut.Find("input");
        input.GetAttribute("id").ShouldBe("email");
        input.GetAttribute("aria-describedby").ShouldBe("email-description email-error");
        input.GetAttribute("aria-errormessage").ShouldBe("email-error");
        input.GetAttribute("aria-invalid").ShouldBe("true");
        input.GetAttribute("aria-required").ShouldBe("true");
        input.HasAttribute("required").ShouldBeTrue();

        cut.Find(".form-field-label").GetAttribute("id").ShouldBe("email-label");
        cut.Find(".form-field-required").TextContent.ShouldBe("*");
        cut.Find(".form-field-description").GetAttribute("id").ShouldBe("email-description");
        cut.Find(".form-validation-message").GetAttribute("id").ShouldBe("email-error");
        cut.Find(".form-validation-message").GetAttribute("role").ShouldBe("alert");
    }

    [Fact]
    public void FormField_PreservesInputDescribedBy_WhenAddingDescriptionAndError()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.Id, "email")
            .Add(p => p.Description, "Help")
            .Add(p => p.ErrorMessage, "Error")
            .Add(p => p.ChildContent, RenderInput(new Dictionary<string, object>
            {
                ["aria-describedby"] = "external-help"
            })));

        cut.Find("input")
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
                childBuilder.AddAttribute(4, "ChildContent", RenderInput());
                childBuilder.CloseComponent();
                childBuilder.AddMarkupContent(5, "<button type=\"submit\">Submit</button>");
            }));
            builder.CloseComponent();
        });

        cut.Find("form").Submit();

        var input = cut.Find("input");
        input.GetAttribute("aria-invalid").ShouldBe("true");
        input.GetAttribute("aria-describedby").ShouldBe("name-error");
        input.GetAttribute("aria-errormessage").ShouldBe("name-error");
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

    [Fact]
    public void FormField_CascadesRelationshipsToSelect()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.Id, "country")
            .Add(p => p.Label, "Country")
            .Add(p => p.Description, "Choose your billing country")
            .Add(p => p.ErrorMessage, "Country is required")
            .Add(p => p.Required, true)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenComponent<Select>(0);
                builder.AddAttribute(1, nameof(Select.ChildContent), (RenderFragment)(options =>
                    options.AddMarkupContent(0, "<option value='us'>United States</option>")));
                builder.CloseComponent();
            }));

        var select = cut.Find("select");
        select.GetAttribute("id").ShouldBe("country");
        select.GetAttribute("aria-describedby").ShouldBe("country-description country-error");
        select.GetAttribute("aria-errormessage").ShouldBe("country-error");
        select.GetAttribute("aria-invalid").ShouldBe("true");
        select.HasAttribute("required").ShouldBeTrue();
    }

    [Fact]
    public void FormField_CascadesRelationshipsToTextArea()
    {
        var cut = Render<FormField<string>>(parameters => parameters
            .Add(p => p.Id, "notes")
            .Add(p => p.Label, "Notes")
            .Add(p => p.Description, "Add delivery instructions")
            .Add(p => p.Required, true)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenComponent<TextArea>(0);
                builder.CloseComponent();
            }));

        var textArea = cut.Find("textarea");
        textArea.GetAttribute("id").ShouldBe("notes");
        textArea.GetAttribute("aria-describedby").ShouldBe("notes-description");
        textArea.GetAttribute("aria-required").ShouldBe("true");
        textArea.HasAttribute("required").ShouldBeTrue();
    }

    private static RenderFragment RenderInput(IReadOnlyDictionary<string, object>? attributes = null)
    {
        return builder =>
        {
            builder.OpenComponent<Input>(0);
            if (attributes is not null)
            {
                builder.AddAttribute(1, nameof(Input.AdditionalAttributes), attributes);
            }
            builder.CloseComponent();
        };
    }
}
