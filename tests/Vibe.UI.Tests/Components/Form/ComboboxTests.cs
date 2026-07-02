namespace Vibe.UI.Tests.Components.Form;

public class ComboboxTests : TestBase
{
    [Fact]
    public void Combobox_RendersDefaultInputAndBaseClass()
    {
        var cut = RenderComponent<Combobox>();

        var root = cut.Find(".vibe-combobox");
        root.ShouldNotBeNull();

        var input = cut.Find("input.combobox-input");
        input.GetAttribute("placeholder").ShouldBe("Select an option...");
        input.GetAttribute("autocomplete").ShouldBe("off");
    }

    [Fact]
    public void Combobox_AppliesCustomClass()
    {
        var cut = RenderComponent<Combobox>(parameters => parameters
            .Add(p => p.Class, "custom-combobox"));

        cut.Find(".vibe-combobox").ClassList.ShouldContain("custom-combobox");
    }

    [Fact]
    public void Combobox_OpensAndRendersEnabledOptions_OnFocus()
    {
        var cut = RenderComponent<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();

        var options = cut.FindAll(".combobox-option");
        options.Count.ShouldBe(3);
        options.Select(option => option.TextContent.Trim()).ShouldBe(new[] { "Alpha", "Beta", "Gamma" });
    }

    [Fact]
    public void Combobox_FiltersOptions_OnInput()
    {
        var cut = RenderComponent<Combobox>(parameters => parameters
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
        var cut = RenderComponent<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();
        cut.Find("input").Input("missing");

        cut.Find(".combobox-empty").TextContent.ShouldBe("No results found.");
        cut.FindAll(".combobox-option").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_SelectsOptionAndClosesDropdown()
    {
        string? changedValue = null;
        var cut = RenderComponent<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find("input").Focus();
        cut.FindAll(".combobox-option")[1].Click();

        changedValue.ShouldBe("beta");
        cut.Find("input").GetAttribute("value").ShouldBe("Beta");
        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_InitialValueUsesMatchingOptionLabel()
    {
        var cut = RenderComponent<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.Value, "gamma"));

        cut.Find("input").GetAttribute("value").ShouldBe("Gamma");
    }

    [Fact]
    public void Combobox_AllowFreeTextUsesUnmatchedValue()
    {
        var cut = RenderComponent<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions())
            .Add(p => p.Value, "Custom")
            .Add(p => p.AllowFreeText, true));

        cut.Find("input").GetAttribute("value").ShouldBe("Custom");
    }

    [Fact]
    public void Combobox_EnterSelectsHighlightedOption()
    {
        string? changedValue = null;
        var cut = RenderComponent<Combobox>(parameters => parameters
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
        var cut = RenderComponent<Combobox>(parameters => parameters
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
        var cut = RenderComponent<Combobox>(parameters => parameters
            .Add(p => p.Options, CreateOptions()));

        cut.Find("input").Focus();
        cut.Find(".combobox-content").ShouldNotBeNull();

        cut.Find("input").KeyDown("Escape");

        cut.FindAll(".combobox-content").ShouldBeEmpty();
    }

    [Fact]
    public void Combobox_BackdropClosesDropdown()
    {
        var cut = RenderComponent<Combobox>(parameters => parameters
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
