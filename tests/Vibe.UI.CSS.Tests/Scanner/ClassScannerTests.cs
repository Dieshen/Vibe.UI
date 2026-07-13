using Vibe.UI.CSS.Scanner;

namespace Vibe.UI.CSS.Tests.Scanner;

public class ClassScannerTests
{
    private readonly ClassScanner _scanner = new();

    #region Basic Extraction Tests

    [Fact]
    public void ScanContent_ClassAttribute_ExtractsClasses()
    {
        var content = @"<div class=""vibe-flex vibe-gap-4"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-flex", classes);
        Assert.Contains("vibe-gap-4", classes);
    }

    [Fact]
    public void ScanContent_CapitalClassAttribute_ExtractsClasses()
    {
        var content = @"<div Class=""vibe-flex vibe-items-center"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-flex", classes);
        Assert.Contains("vibe-items-center", classes);
    }

    [Fact]
    public void ScanContent_MultipleElements_ExtractsAllClasses()
    {
        var content = @"
            <div class=""vibe-flex"">
                <span class=""vibe-text-sm vibe-text-muted"">Text</span>
                <button class=""vibe-p-4 vibe-bg-primary"">Click</button>
            </div>";

        var classes = _scanner.ScanContent(content);

        Assert.Equal(5, classes.Count);
        Assert.Contains("vibe-flex", classes);
        Assert.Contains("vibe-text-sm", classes);
        Assert.Contains("vibe-text-muted", classes);
        Assert.Contains("vibe-p-4", classes);
        Assert.Contains("vibe-bg-primary", classes);
    }

    [Fact]
    public void ScanContent_DuplicateClasses_ReturnsUniqueSet()
    {
        var content = @"
            <div class=""vibe-flex"">Content</div>
            <div class=""vibe-flex"">More Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Single(classes);
        Assert.Contains("vibe-flex", classes);
    }

    #endregion

    #region Blazor-Specific Tests

    [Fact]
    public void ScanContent_BlazorAtClass_ExtractsClasses()
    {
        var content = @"<div @class=""vibe-flex vibe-gap-2"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-flex", classes);
        Assert.Contains("vibe-gap-2", classes);
    }

    [Fact]
    public void ScanContent_BlazorInterpolatedClass_ExtractsClasses()
    {
        var content = @"<div class=@""vibe-flex vibe-items-center"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-flex", classes);
        Assert.Contains("vibe-items-center", classes);
    }

    [Fact]
    public void ScanContent_BlazorExpression_ExtractsStringLiterals()
    {
        var content = @"<div class=""@(isActive ? ""vibe-bg-primary"" : ""vibe-bg-muted"")"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-bg-primary", classes);
        Assert.Contains("vibe-bg-muted", classes);
    }

    [Fact]
    public void ScanContent_RazorCodeBlock_ExtractsDynamicVariantClasses()
    {
        var content = """
            <span class="@BadgeClass">Badge</span>

            @code {
                private string BadgeClass =>
                    "vibe-bg-blue-100 vibe-text-blue-700 dark:vibe-bg-blue-900/30 dark:vibe-text-blue-400";
            }
            """;

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-bg-blue-100", classes);
        Assert.Contains("vibe-text-blue-700", classes);
        Assert.Contains("dark:vibe-bg-blue-900/30", classes);
        Assert.Contains("dark:vibe-text-blue-400", classes);
    }

    #endregion

    #region Custom Attribute Tests

    [Fact]
    public void ScanContent_AdditionalClassesAttribute_ExtractsClasses()
    {
        var content = @"<Button AdditionalClasses=""vibe-mt-4 vibe-w-full"" />";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-mt-4", classes);
        Assert.Contains("vibe-w-full", classes);
    }

    [Fact]
    public void ScanContent_CssClassAttribute_ExtractsClasses()
    {
        var content = @"<MyComponent CssClass=""vibe-p-4 vibe-rounded"" />";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-p-4", classes);
        Assert.Contains("vibe-rounded", classes);
    }

    #endregion

    #region C# File Tests

    [Fact]
    public void ScanContent_CSharpStringLiterals_ExtractsClasses()
    {
        var content = @"
            public class MyComponent
            {
                private string GetClasses() => ""vibe-flex vibe-gap-4"";
            }";

        var classes = _scanner.ScanContent(content, ".cs");

        Assert.Contains("vibe-flex", classes);
        Assert.Contains("vibe-gap-4", classes);
    }

    [Fact]
    public void ScanContent_CSharpCssClassAssignment_ExtractsClasses()
    {
        var content = @"
            CssClass = ""vibe-p-4 vibe-bg-primary"";
            Class = ""vibe-flex"";";

        var classes = _scanner.ScanContent(content, ".cs");

        Assert.Contains("vibe-p-4", classes);
        Assert.Contains("vibe-bg-primary", classes);
        Assert.Contains("vibe-flex", classes);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ScanContent_EmptyClass_ReturnsEmptySet()
    {
        var content = @"<div class="""">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Empty(classes);
    }

    [Fact]
    public void ScanContent_NoClasses_ReturnsEmptySet()
    {
        var content = @"<div>Content without classes</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Empty(classes);
    }

    [Fact]
    public void ScanContent_WhitespaceVariations_HandlesCorrectly()
    {
        var content = @"<div class=""  vibe-flex   vibe-gap-4  "">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Equal(2, classes.Count);
        Assert.Contains("vibe-flex", classes);
        Assert.Contains("vibe-gap-4", classes);
    }

    [Fact]
    public void ScanContent_VariantClasses_ExtractsCorrectly()
    {
        var content = @"<div class=""hover:vibe-bg-primary sm:vibe-flex dark:vibe-text-white"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("hover:vibe-bg-primary", classes);
        Assert.Contains("sm:vibe-flex", classes);
        Assert.Contains("dark:vibe-text-white", classes);
    }

    [Fact]
    public void ScanContent_ArbitraryValues_ExtractsCorrectly()
    {
        var content = @"<div class=""vibe-w-[500px] vibe-p-[1.5rem]"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-w-[500px]", classes);
        Assert.Contains("vibe-p-[1.5rem]", classes);
    }

    [Fact]
    public void ScanContent_FractionClasses_ExtractsCorrectly()
    {
        var content = @"<div class=""vibe-w-1/2 vibe-w-2/3"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-w-1/2", classes);
        Assert.Contains("vibe-w-2/3", classes);
    }

    [Fact]
    public void ScanContent_OpacityModifiers_ExtractsCorrectly()
    {
        var content = @"<div class=""vibe-bg-red-500/50 vibe-text-blue-600/75"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Contains("vibe-bg-red-500/50", classes);
        Assert.Contains("vibe-text-blue-600/75", classes);
    }

    #endregion

    #region Ignore Tests

    [Fact]
    public void ScanContent_IgnoredClasses_NotIncluded()
    {
        var scanner = new ClassScanner();
        scanner.IgnoreClasses("ignored-class");

        var content = @"<div class=""vibe-flex ignored-class"">Content</div>";
        var classes = scanner.ScanContent(content);

        Assert.Contains("vibe-flex", classes);
        Assert.DoesNotContain("ignored-class", classes);
    }

    [Fact]
    public void ScanContent_AtSymbolPrefix_Skipped()
    {
        var content = @"<div class=""@someVariable vibe-flex"">Content</div>";

        var classes = _scanner.ScanContent(content);

        Assert.Single(classes);
        Assert.Contains("vibe-flex", classes);
    }

    #endregion
}

