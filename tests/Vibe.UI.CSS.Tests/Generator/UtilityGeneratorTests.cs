using Vibe.UI.CSS.Generator;

namespace Vibe.UI.CSS.Tests.Generator;

public class UtilityGeneratorTests
{
    private readonly UtilityGenerator _generator = new();

    #region Display Utilities

    [Theory]
    [InlineData("vibe-flex", "display: flex;")]
    [InlineData("vibe-hidden", "display: none;")]
    [InlineData("vibe-block", "display: block;")]
    [InlineData("vibe-inline", "display: inline;")]
    [InlineData("vibe-inline-block", "display: inline-block;")]
    [InlineData("vibe-inline-flex", "display: inline-flex;")]
    [InlineData("vibe-grid", "display: grid;")]
    public void Generate_DisplayUtilities_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Flexbox Utilities

    [Theory]
    [InlineData("vibe-flex-row", "flex-direction: row;")]
    [InlineData("vibe-flex-col", "flex-direction: column;")]
    [InlineData("vibe-flex-row-reverse", "flex-direction: row-reverse;")]
    [InlineData("vibe-flex-col-reverse", "flex-direction: column-reverse;")]
    public void Generate_FlexDirection_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-items-start", "align-items: flex-start;")]
    [InlineData("vibe-items-center", "align-items: center;")]
    [InlineData("vibe-items-end", "align-items: flex-end;")]
    [InlineData("vibe-items-stretch", "align-items: stretch;")]
    public void Generate_AlignItems_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-justify-start", "justify-content: flex-start;")]
    [InlineData("vibe-justify-center", "justify-content: center;")]
    [InlineData("vibe-justify-end", "justify-content: flex-end;")]
    [InlineData("vibe-justify-between", "justify-content: space-between;")]
    [InlineData("vibe-justify-around", "justify-content: space-around;")]
    [InlineData("vibe-justify-evenly", "justify-content: space-evenly;")]
    public void Generate_JustifyContent_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-gap-0", "gap: 0;")]
    [InlineData("vibe-gap-1", "gap: 0.25rem;")]
    [InlineData("vibe-gap-4", "gap: 1rem;")]
    [InlineData("vibe-gap-8", "gap: 2rem;")]
    public void Generate_Gap_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-gap-x-2", "column-gap: 0.5rem;")]
    [InlineData("vibe-gap-y-4", "row-gap: 1rem;")]
    public void Generate_GapAxis_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Spacing Utilities (Padding)

    [Theory]
    [InlineData("vibe-p-0", "padding: 0;")]
    [InlineData("vibe-p-1", "padding: 0.25rem;")]
    [InlineData("vibe-p-4", "padding: 1rem;")]
    [InlineData("vibe-p-8", "padding: 2rem;")]
    public void Generate_Padding_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-px-4", "padding-left: 1rem; padding-right: 1rem;")]
    [InlineData("vibe-py-2", "padding-top: 0.5rem; padding-bottom: 0.5rem;")]
    [InlineData("vibe-pt-4", "padding-top: 1rem;")]
    [InlineData("vibe-pr-4", "padding-right: 1rem;")]
    [InlineData("vibe-pb-4", "padding-bottom: 1rem;")]
    [InlineData("vibe-pl-4", "padding-left: 1rem;")]
    public void Generate_PaddingDirectional_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Spacing Utilities (Margin)

    [Theory]
    [InlineData("vibe-m-0", "margin: 0;")]
    [InlineData("vibe-m-4", "margin: 1rem;")]
    [InlineData("vibe-mx-auto", "margin-left: auto; margin-right: auto;")]
    public void Generate_Margin_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe--mt-4", "margin-top: -1rem;")]
    [InlineData("vibe--m-2", "margin: -0.5rem;")]
    public void Generate_NegativeMargin_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Sizing Utilities

    [Theory]
    [InlineData("vibe-w-full", "width: 100%;")]
    [InlineData("vibe-w-auto", "width: auto;")]
    [InlineData("vibe-w-1/2", "width: 50%;")]
    [InlineData("vibe-w-64", "width: 16rem;")]
    public void Generate_Width_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-h-full", "height: 100%;")]
    [InlineData("vibe-h-screen", "height: 100vh;")]
    [InlineData("vibe-h-auto", "height: auto;")]
    public void Generate_Height_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-min-w-0", "min-width: 0;")]
    [InlineData("vibe-max-w-lg", "max-width: 32rem;")]
    [InlineData("vibe-max-w-full", "max-width: 100%;")]
    public void Generate_MinMaxWidth_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Typography Utilities

    [Theory]
    [InlineData("vibe-text-xs", "font-size: 0.75rem; line-height: 1rem;")]
    [InlineData("vibe-text-sm", "font-size: 0.875rem; line-height: 1.25rem;")]
    [InlineData("vibe-text-base", "font-size: 1rem; line-height: 1.5rem;")]
    [InlineData("vibe-text-lg", "font-size: 1.125rem; line-height: 1.75rem;")]
    [InlineData("vibe-text-xl", "font-size: 1.25rem; line-height: 1.75rem;")]
    public void Generate_FontSize_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-font-normal", "font-weight: 400;")]
    [InlineData("vibe-font-medium", "font-weight: 500;")]
    [InlineData("vibe-font-semibold", "font-weight: 600;")]
    [InlineData("vibe-font-bold", "font-weight: 700;")]
    public void Generate_FontWeight_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-text-left", "text-align: left;")]
    [InlineData("vibe-text-center", "text-align: center;")]
    [InlineData("vibe-text-right", "text-align: right;")]
    public void Generate_TextAlign_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Fact]
    public void Generate_Truncate_ReturnsCorrectCss()
    {
        var rule = _generator.Generate("vibe-truncate");

        Assert.NotNull(rule);
        Assert.Equal("overflow: hidden; text-overflow: ellipsis; white-space: nowrap;", rule.Declarations);
    }

    #endregion

    #region Color Utilities (Semantic)

    [Theory]
    [InlineData("vibe-bg-primary", "background-color: var(--vibe-primary);")]
    [InlineData("vibe-bg-secondary", "background-color: var(--vibe-secondary);")]
    [InlineData("vibe-bg-muted", "background-color: var(--vibe-muted);")]
    [InlineData("vibe-bg-accent", "background-color: var(--vibe-accent);")]
    [InlineData("vibe-bg-destructive", "background-color: var(--vibe-destructive);")]
    public void Generate_SemanticBackgroundColors_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-text-foreground", "color: var(--vibe-foreground);")]
    [InlineData("vibe-text-muted", "color: var(--vibe-muted);")]
    [InlineData("vibe-text-primary", "color: var(--vibe-primary);")]
    public void Generate_SemanticTextColors_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Color Utilities (Palette)

    [Theory]
    [InlineData("vibe-bg-red-500", "background-color: #ef4444;")]
    [InlineData("vibe-bg-blue-600", "background-color: #2563eb;")]
    [InlineData("vibe-bg-green-400", "background-color: #4ade80;")]
    [InlineData("vibe-bg-slate-100", "background-color: #f1f5f9;")]
    [InlineData("vibe-bg-gray-900", "background-color: #111827;")]
    public void Generate_PaletteBackgroundColors_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-text-red-500", "color: #ef4444;")]
    [InlineData("vibe-text-blue-600", "color: #2563eb;")]
    public void Generate_PaletteTextColors_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-bg-red-500/50", "background-color: rgb(239 68 68 / 0.5);")]
    [InlineData("vibe-bg-blue-600/75", "background-color: rgb(37 99 235 / 0.75);")]
    [InlineData("vibe-text-green-500/25", "color: rgb(34 197 94 / 0.25);")]
    public void Generate_ColorsWithOpacity_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Border Utilities

    [Theory]
    [InlineData("vibe-border", "border-width: 1px;")]
    [InlineData("vibe-border-0", "border-width: 0px;")]
    [InlineData("vibe-border-2", "border-width: 2px;")]
    [InlineData("vibe-border-4", "border-width: 4px;")]
    public void Generate_BorderWidth_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-rounded", "border-radius: 0.25rem;")]
    [InlineData("vibe-rounded-none", "border-radius: 0;")]
    [InlineData("vibe-rounded-sm", "border-radius: 0.125rem;")]
    [InlineData("vibe-rounded-lg", "border-radius: 0.5rem;")]
    [InlineData("vibe-rounded-full", "border-radius: 9999px;")]
    public void Generate_BorderRadius_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Layout Utilities

    [Theory]
    [InlineData("vibe-relative", "position: relative;")]
    [InlineData("vibe-absolute", "position: absolute;")]
    [InlineData("vibe-fixed", "position: fixed;")]
    [InlineData("vibe-sticky", "position: sticky;")]
    [InlineData("vibe-static", "position: static;")]
    public void Generate_Position_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-top-0", "top: 0;")]
    [InlineData("vibe-right-0", "right: 0;")]
    [InlineData("vibe-bottom-0", "bottom: 0;")]
    [InlineData("vibe-left-0", "left: 0;")]
    [InlineData("vibe-inset-0", "inset: 0;")]
    public void Generate_Inset_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-z-0", "z-index: 0;")]
    [InlineData("vibe-z-10", "z-index: 10;")]
    [InlineData("vibe-z-50", "z-index: 50;")]
    [InlineData("vibe-z-auto", "z-index: auto;")]
    public void Generate_ZIndex_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-overflow-auto", "overflow: auto;")]
    [InlineData("vibe-overflow-hidden", "overflow: hidden;")]
    [InlineData("vibe-overflow-visible", "overflow: visible;")]
    [InlineData("vibe-overflow-scroll", "overflow: scroll;")]
    public void Generate_Overflow_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Grid Utilities

    [Theory]
    [InlineData("vibe-grid-cols-1", "grid-template-columns: repeat(1, minmax(0, 1fr));")]
    [InlineData("vibe-grid-cols-3", "grid-template-columns: repeat(3, minmax(0, 1fr));")]
    [InlineData("vibe-grid-cols-12", "grid-template-columns: repeat(12, minmax(0, 1fr));")]
    public void Generate_GridCols_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-col-span-1", "grid-column: span 1 / span 1;")]
    [InlineData("vibe-col-span-2", "grid-column: span 2 / span 2;")]
    [InlineData("vibe-col-span-full", "grid-column: 1 / -1;")]
    public void Generate_ColSpan_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    #endregion

    #region Effect Utilities

    [Theory]
    [InlineData("vibe-opacity-0", "opacity: 0;")]
    [InlineData("vibe-opacity-50", "opacity: 0.5;")]
    [InlineData("vibe-opacity-100", "opacity: 1;")]
    public void Generate_Opacity_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Fact]
    public void Generate_Shadow_ReturnsCorrectCss()
    {
        var rule = _generator.Generate("vibe-shadow");

        Assert.NotNull(rule);
        Assert.Contains("box-shadow:", rule.Declarations);
    }

    [Fact]
    public void Generate_PageEnter_DoesNotRetainTransformAfterAnimation()
    {
        var rules = _generator.GenerateAll("vibe-page-enter");

        var animationRule = Assert.Single(rules, rule => rule.Selector == ".vibe-page-enter");
        Assert.Equal("animation: vibe-pageEnter 0.4s ease-out;", animationRule.Declarations);
        Assert.DoesNotContain("forwards", animationRule.Declarations, StringComparison.Ordinal);
    }

    #endregion

    #region Interactivity Utilities

    [Theory]
    [InlineData("vibe-cursor-pointer", "cursor: pointer;")]
    [InlineData("vibe-cursor-default", "cursor: default;")]
    [InlineData("vibe-cursor-not-allowed", "cursor: not-allowed;")]
    public void Generate_Cursor_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Theory]
    [InlineData("vibe-select-none", "user-select: none;")]
    [InlineData("vibe-select-text", "user-select: text;")]
    [InlineData("vibe-select-all", "user-select: all;")]
    public void Generate_UserSelect_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Fact]
    public void Generate_SrOnly_ReturnsCorrectCss()
    {
        var rule = _generator.Generate("vibe-sr-only");

        Assert.NotNull(rule);
        Assert.Contains("position: absolute", rule.Declarations);
        Assert.Contains("width: 1px", rule.Declarations);
        Assert.Contains("clip: rect(0, 0, 0, 0)", rule.Declarations);
    }

    #endregion

    #region Variant Tests

    [Theory]
    [InlineData("hover:vibe-bg-primary", ":hover")]
    [InlineData("focus:vibe-bg-primary", ":focus")]
    [InlineData("active:vibe-bg-primary", ":active")]
    [InlineData("disabled:vibe-opacity-50", ":disabled")]
    public void Generate_StateVariants_ReturnsCorrectSelector(string className, string expectedPseudo)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Contains(expectedPseudo, rule.Selector);
    }

    [Theory]
    [InlineData("sm:vibe-flex", "640px")]
    [InlineData("md:vibe-hidden", "768px")]
    [InlineData("lg:vibe-grid-cols-4", "1024px")]
    [InlineData("xl:vibe-p-8", "1280px")]
    public void Generate_ResponsiveVariants_ReturnsCorrectMediaQuery(string className, string expectedMinWidth)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.NotNull(rule.MediaQuery);
        Assert.Contains(expectedMinWidth, rule.MediaQuery);
    }

    [Fact]
    public void Generate_DarkVariant_ReturnsCorrectSelector()
    {
        var rule = _generator.Generate("dark:vibe-bg-slate-900");

        Assert.NotNull(rule);
        Assert.StartsWith(".dark ", rule.Selector);
    }

    #endregion

    #region Arbitrary Value Tests

    [Theory]
    [InlineData("vibe-w-[500px]", "width: 500px;")]
    [InlineData("vibe-h-[100vh]", "height: 100vh;")]
    [InlineData("vibe-p-[1.5rem]", "padding: 1.5rem;")]
    [InlineData("vibe-m-[20px]", "margin: 20px;")]
    public void Generate_ArbitraryValues_ReturnsCorrectCss(string className, string expectedDeclarations)
    {
        var rule = _generator.Generate(className);

        Assert.NotNull(rule);
        Assert.Equal(expectedDeclarations, rule.Declarations);
    }

    [Fact]
    public void Generate_ArbitraryColor_ReturnsCorrectCss()
    {
        var rule = _generator.Generate("vibe-bg-[#ff0000]");

        Assert.NotNull(rule);
        Assert.Equal("background-color: #ff0000;", rule.Declarations);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Generate_UnrecognizedClass_ReturnsNull()
    {
        var rule = _generator.Generate("not-a-vibe-class");

        Assert.Null(rule);
    }

    [Fact]
    public void Generate_WrongPrefix_ReturnsNull()
    {
        var rule = _generator.Generate("tw-flex");

        Assert.Null(rule);
    }

    [Fact]
    public void Generate_EmptyString_ReturnsNull()
    {
        var rule = _generator.Generate("");

        Assert.Null(rule);
    }

    [Fact]
    public void Generate_ClassWithEscapedCharacters_ReturnsEscapedSelector()
    {
        var rule = _generator.Generate("vibe-w-1/2");

        Assert.NotNull(rule);
        Assert.Contains("\\/", rule.Selector);
    }

    #endregion
}

