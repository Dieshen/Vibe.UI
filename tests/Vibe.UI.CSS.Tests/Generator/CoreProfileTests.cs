using Vibe.UI.CSS.Generator;
using Vibe.UI.CSS.Scanner;

namespace Vibe.UI.CSS.Tests.Generator;

public sealed class CoreProfileTests
{
    private readonly UtilityGenerator _generator = new();

    [Theory]
    [InlineData("vibe-flex")]
    [InlineData("vibe-grid-cols-3")]
    [InlineData("vibe-gap-4")]
    [InlineData("vibe-px-4")]
    [InlineData("vibe-w-full")]
    [InlineData("vibe-text-sm")]
    [InlineData("vibe-bg-primary")]
    [InlineData("vibe-border")]
    [InlineData("vibe-ring-2")]
    [InlineData("vibe-transition-colors")]
    public void CoreUtilityFamilies_AreRecognized(string className)
    {
        Assert.NotNull(_generator.Generate(className));
    }

    [Theory]
    [InlineData("vibe-ring", "calc(3px + var(--tw-ring-offset-width, 0px))")]
    [InlineData("vibe-ring-2", "calc(2px + var(--tw-ring-offset-width, 0px))")]
    [InlineData("vibe-ring-[5px]", "calc(5px + var(--tw-ring-offset-width, 0px))")]
    public void RingWidths_HaveStandaloneSafeDefaults(string className, string expectedWidth)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Contains("var(--tw-ring-inset,)", rule.Declarations, StringComparison.Ordinal);
        Assert.Contains(expectedWidth, rule.Declarations, StringComparison.Ordinal);
        Assert.Contains("var(--tw-ring-color, var(--vibe-ring, currentColor))", rule.Declarations, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("checked:vibe-bg-primary", ":checked")]
    [InlineData("indeterminate:vibe-bg-primary", ":indeterminate")]
    [InlineData("required:vibe-border", ":required")]
    [InlineData("optional:vibe-border", ":optional")]
    [InlineData("invalid:vibe-border-destructive", ":invalid")]
    [InlineData("valid:vibe-border-primary", ":valid")]
    [InlineData("open:vibe-block", ":is([open], :popover-open)")]
    [InlineData("only:vibe-block", ":only-child")]
    [InlineData("empty:vibe-hidden", ":empty")]
    [InlineData("first-of-type:vibe-block", ":first-of-type")]
    public void StateAndStructuralVariants_UseExpectedSelectors(string className, string expectedSelector)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Contains(expectedSelector, rule.Selector, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("group-hover:vibe-bg-primary", ".group:hover ")]
    [InlineData("peer-checked:vibe-bg-primary", ".peer:checked ~ ")]
    [InlineData("aria-expanded:vibe-block", "[aria-expanded=\"true\"]")]
    [InlineData("aria-[sort=ascending]:vibe-block", "[aria-sort=\"ascending\"]")]
    [InlineData("data-[state=open]:vibe-block", "[data-state=\"open\"]")]
    [InlineData("group-aria-expanded:vibe-block", ".group[aria-expanded=\"true\"] ")]
    [InlineData("peer-data-[state=checked]:vibe-block", ".peer[data-state=\"checked\"] ~ ")]
    public void CompositionAndAttributeVariants_UseExpectedSelectors(string className, string expectedSelector)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Contains(expectedSelector, rule.Selector, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("data-[state=open:active]:vibe-block", "[data-state=\"open:active\"]")]
    [InlineData("group-data-[state=open:active]:vibe-block", ".group[data-state=\"open:active\"] ")]
    [InlineData("peer-data-[state=open:active]:vibe-block", ".peer[data-state=\"open:active\"] ~ ")]
    public void ArbitraryAttributeVariants_IgnoreColonsInsideBrackets(
        string className,
        string expectedSelector)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Contains(expectedSelector, rule.Selector, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("motion-safe:vibe-transition", "(prefers-reduced-motion: no-preference)")]
    [InlineData("motion-reduce:vibe-transition-none", "(prefers-reduced-motion: reduce)")]
    [InlineData("contrast-more:vibe-border", "(prefers-contrast: more)")]
    [InlineData("contrast-less:vibe-border", "(prefers-contrast: less)")]
    [InlineData("forced-colors:vibe-border", "(forced-colors: active)")]
    [InlineData("portrait:vibe-block", "(orientation: portrait)")]
    [InlineData("landscape:vibe-block", "(orientation: landscape)")]
    [InlineData("print:vibe-hidden", "@media print")]
    public void MediaVariants_UseExpectedQueries(string className, string expectedQuery)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.NotNull(rule.MediaQuery);
        Assert.Contains(expectedQuery, rule.MediaQuery, StringComparison.Ordinal);
    }

    [Fact]
    public void StackedResponsiveMotionAndStateVariants_Compose()
    {
        var rule = _generator.Generate("sm:motion-reduce:hover:vibe-opacity-50");

        Assert.NotNull(rule);
        Assert.Contains(":hover", rule.Selector, StringComparison.Ordinal);
        Assert.Equal(
            "@media (prefers-reduced-motion: reduce) and (min-width: 640px)",
            rule.MediaQuery);
    }

    [Theory]
    [InlineData("print:sm:vibe-hidden")]
    [InlineData("sm:print:vibe-hidden")]
    public void PrintAndResponsiveVariants_StackWithMediaTypeFirst(string className)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal("@media print and (min-width: 640px)", rule.MediaQuery);
    }

    [Theory]
    [InlineData("vibe-w-[var(--panel-width)]", "width: var(--panel-width);")]
    [InlineData("vibe-bg-[var(--surface)]", "background-color: var(--surface);")]
    [InlineData("vibe-text-[color:var(--foreground)]", "color: var(--foreground);")]
    [InlineData("vibe-grid-cols-[12rem_minmax(0,1fr)]", "grid-template-columns: 12rem minmax(0,1fr);")]
    [InlineData("vibe-size-[2rem]", "width: 2rem; height: 2rem;")]
    [InlineData("vibe-[--panel-width:20rem]", "--panel-width: 20rem;")]
    public void PracticalArbitraryValues_AreRecognized(string className, string expectedDeclaration)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclaration, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-[--label:hello_world]", "--label: hello world;")]
    [InlineData(@"vibe-[--label:hello\_world]", "--label: hello_world;")]
    [InlineData("vibe-w-[var(--panel_width)]", "width: var(--panel_width);")]
    [InlineData(@"vibe-w-[var(--panel\_width)]", "width: var(--panel_width);")]
    [InlineData("vibe-w-[var(--panel_width,_20rem)]", "width: var(--panel_width, 20rem);")]
    [InlineData(
        "vibe-bg-[url(https://cdn.example.com/hero_banner.svg)]",
        "background-image: url(https://cdn.example.com/hero_banner.svg);")]
    public void ArbitraryValueDecoding_UsesCompatibleUnderscoreRules(
        string className,
        string expectedDeclaration)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclaration, rule.Declarations);
    }

    [Fact]
    public void Scanner_PreservesCoreProfileArbitraryAndAttributeClasses()
    {
        const string content = """
            <div class="vibe-w-[var(--panel-width)] data-[state=open]:vibe-block aria-[sort=ascending]:vibe-text-primary vibe-[--panel-width:20rem]"></div>
            """;
        var classes = new ClassScanner().ScanContent(content);

        Assert.Contains("vibe-w-[var(--panel-width)]", classes);
        Assert.Contains("data-[state=open]:vibe-block", classes);
        Assert.Contains("aria-[sort=ascending]:vibe-text-primary", classes);
        Assert.Contains("vibe-[--panel-width:20rem]", classes);
    }

    [Fact]
    public void ArbitraryValues_RejectDeclarationInjection()
    {
        Assert.Null(_generator.Generate("vibe-w-[1px;color:red]"));
    }

    [Theory]
    [InlineData("vibe-w-[var(--foo bar)]")]
    [InlineData("vibe-w-[var(foo)]")]
    public void ArbitraryValues_RejectInvalidCustomPropertyReferences(string className)
    {
        Assert.Null(_generator.Generate(className));
    }
}
