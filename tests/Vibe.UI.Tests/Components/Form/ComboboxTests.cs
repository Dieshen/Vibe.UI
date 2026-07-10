namespace Vibe.UI.Tests.Components.Form;

public class ComboboxTests : TestBase
{
    [Fact]
    public void Combobox_RendersDefaultInputAndBaseClass()
    {
        var cut = Render<Combobox>();

        var root = cut.Find(".vibe-combobox");
        root.ShouldNotBeNull();

        var input = cut.Find("input.combobox-input");
        input.GetAttribute("placeholder").ShouldBe("Select an option...");
        input.GetAttribute("autocomplete").ShouldBe("off");
    }

    [Fact]
    public void Combobox_AppliesCustomClass()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Class, "custom-combobox"));

        cut.Find(".vibe-combobox").ClassList.ShouldContain("custom-combobox");
    }

    [Fact]
    public void Combobox_ForwardsRootAttributesAndInputAccessibility()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Id, "country")
            .Add(p => p.AriaLabel, "Country")
            .Add(p => p.HelperText, "Choose a country")
            .AddUnmatched("data-testid", "country-combobox"));

        var root = cut.Find(".vibe-combobox");
        root.GetAttribute("data-testid").ShouldBe("country-combobox");

        var input = cut.Find("input");
        input.GetAttribute("id").ShouldBe("country");
        input.GetAttribute("role").ShouldBe("combobox");
        input.GetAttribute("aria-label").ShouldBe("Country");
        input.GetAttribute("aria-haspopup").ShouldBe("listbox");
        input.GetAttribute("aria-expanded").ShouldBe("false");
        input.GetAttribute("aria-controls").ShouldBe("country-listbox");
        input.GetAttribute("aria-describedby").ShouldBe("country-helper");
        cut.Find(".combobox-helper").TextContent.ShouldBe("Choose a country");
    }

    [Fact]
    public void Combobox_ExposesListboxAndActiveOptionSemantics_OnFocus()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Id, "choice")
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();

        var input = cut.Find("input");
        input.GetAttribute("aria-expanded").ShouldBe("true");
        input.GetAttribute("aria-activedescendant").ShouldBe("choice-option-0");

        var listbox = cut.Find(".combobox-content");
        listbox.GetAttribute("id").ShouldBe("choice-listbox");
        listbox.GetAttribute("role").ShouldBe("listbox");

        var option = cut.Find(".combobox-option");
        option.GetAttribute("id").ShouldBe("choice-option-0");
        option.GetAttribute("role").ShouldBe("option");
        option.GetAttribute("aria-selected").ShouldBe("true");
    }

    [Fact]
    public void Combobox_OpensAndRendersEnabledOptions_OnFocus()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();

        var options = cut.FindAll(".combobox-option");
        options.Count.ShouldBe(3);
        options.Select(option => option.TextContent.Trim()).ShouldBe(new[] { "Alpha", "Beta", "Gamma" });
    }

    [Fact]
    public void Combobox_FiltersOptions_OnInput()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();
        cut.Find("input").Input("ga");

        var option = cut.Find(".combobox-option");
        option.TextContent.Trim().ShouldBe("Gamma");
        option.ClassList.ShouldContain("selected");
    }

    [Fact]
    public void Combobox_RendersEmptyState_WhenNoOptionsMatch()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();
        cut.Find("input").Input("missing");

        cut.Find(".combobox-empty").TextContent.ShouldBe("No results found.");
        cut.FindAll(".combobox-option").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_RendersCustomEmptyText()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.EmptyText, "No matches"));

        cut.Find("input").Focus();
        cut.Find("input").Input("missing");

        cut.Find(".combobox-empty").TextContent.ShouldBe("No matches");
    }

    [Fact]
    public void Combobox_SelectsOptionAndClosesDropdown()
    {
        string? changedValue = null;
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find("input").Focus();
        cut.FindAll(".combobox-option")[1].Click();

        changedValue.ShouldBe("beta");
        cut.Find("input").GetAttribute("value").ShouldBe("Beta");
        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_DisabledPreventsOpeningAndChange()
    {
        string? changedValue = null;
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.Disabled, true)
            .Add(p => p.ValueChanged, value => changedValue = value));

        var input = cut.Find("input");
        input.HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".vibe-combobox").ClassList.ShouldContain("disabled");

        input.Focus();
        input.Input("Alpha");
        input.KeyDown("Enter");

        changedValue.ShouldBeNull();
        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_ReadOnlyPreventsOpeningAndChange()
    {
        string? changedValue = null;
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.ReadOnly, true)
            .Add(p => p.AllowFreeText, true)
            .Add(p => p.ValueChanged, value => changedValue = value));

        var input = cut.Find("input");
        input.HasAttribute("readonly").ShouldBeTrue();
        input.GetAttribute("aria-readonly").ShouldBe("true");
        cut.Find(".vibe-combobox").ClassList.ShouldContain("readonly");

        input.Focus();
        input.Input("Custom");
        input.KeyDown("Enter");

        changedValue.ShouldBeNull();
        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_ErrorMessageOverridesHelperTextAndLinksInput()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Id, "city")
            .Add(p => p.HelperText, "Choose a city")
            .Add(p => p.ErrorMessage, "City is required")
            .Add(p => p.AriaDescribedBy, "external-help"));

        var input = cut.Find("input");
        input.GetAttribute("aria-invalid").ShouldBe("true");
        input.GetAttribute("aria-describedby").ShouldBe("external-help city-error");

        cut.Find(".vibe-combobox").ClassList.ShouldContain("has-error");
        cut.Find(".combobox-error").TextContent.ShouldBe("City is required");
        cut.FindAll(".combobox-helper").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_AllowsNullOptions()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, null!));

        cut.Find("input").Focus();

        cut.Find(".combobox-empty").TextContent.ShouldBe("No results found.");
        cut.FindAll(".combobox-option").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_ClearsDisplayedValue_WhenValueBecomesNull()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.Value, "gamma"));

        cut.Find("input").GetAttribute("value").ShouldBe("Gamma");

        cut.Render(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.Value, null));

        cut.Find("input").GetAttribute("value").ShouldBe(string.Empty);
    }

    [Fact]
    public void Combobox_InitialValueUsesMatchingOptionLabel()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.Value, "gamma"));

        cut.Find("input").GetAttribute("value").ShouldBe("Gamma");
    }

    [Fact]
    public void Combobox_AllowFreeTextUsesUnmatchedValue()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.Value, "Custom")
            .Add(p => p.AllowFreeText, true));

        cut.Find("input").GetAttribute("value").ShouldBe("Custom");
    }

    [Fact]
    public void Combobox_EnterSelectsHighlightedOption()
    {
        string? changedValue = null;
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find("input").Focus();
        cut.Find("input").KeyDown("Enter");

        changedValue.ShouldBe("alpha");
        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_EnterSubmitsFreeText_WhenOpenWithoutHighlightedOption()
    {
        string? changedValue = null;
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.AllowFreeText, true)
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find("input").Input("Custom");
        cut.Find("input").KeyDown("Enter");

        changedValue.ShouldBe("Custom");
        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_EscapeClosesDropdown()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();
        cut.Find(".combobox-content").ShouldNotBeNull();

        cut.Find("input").KeyDown("Escape");

        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_BackdropClosesDropdown()
    {
        var cut = Render<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();
        cut.Find(".combobox-backdrop").Click();

        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    private static List<Combobox.ComboboxOption> CreateOptions() =>
    [
        new() { Label = "Alpha", Value = "alpha" },
        new() { Label = "Beta", Value = "beta" },
        new() { Label = "Hidden", Value = "hidden", Disabled = true },
        new() { Label = "Gamma", Value = "gamma" }
    ];
}
