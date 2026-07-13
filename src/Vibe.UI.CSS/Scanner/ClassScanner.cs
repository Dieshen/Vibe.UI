using System.Text.RegularExpressions;

namespace Vibe.UI.CSS.Scanner;

/// <summary>
/// Scans source files (.razor, .cshtml, .html, .cs) for CSS class names.
/// </summary>
public partial class ClassScanner
{
    private readonly string _prefix;
    private readonly bool _allowUnprefixed;
    private readonly HashSet<string> _ignoredClasses = [];

    /// <summary>
    /// Initializes a new instance of the ClassScanner.
    /// </summary>
    /// <param name="prefix">The CSS class prefix to scan for (default: "vibe")</param>
    /// <param name="allowUnprefixed">Whether to allow scanning unprefixed utility classes</param>
    public ClassScanner(string prefix = "vibe", bool allowUnprefixed = false)
    {
        _prefix = prefix;
        _allowUnprefixed = allowUnprefixed;
    }

    /// <summary>
    /// Scan a directory for all CSS class usages.
    /// </summary>
    /// <param name="directory">Root directory to scan</param>
    /// <param name="patterns">File patterns to include (default: *.razor, *.cshtml, *.html)</param>
    /// <returns>Set of unique class names found</returns>
    public HashSet<string> ScanDirectory(string directory, string[]? patterns = null)
    {
        patterns ??= ["*.razor", "*.cshtml", "*.html", "*.cs"];
        var classes = new HashSet<string>();

        foreach (var pattern in patterns)
        {
            var files = Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileClasses = ScanFile(file);
                foreach (var cls in fileClasses)
                {
                    classes.Add(cls);
                }
            }
        }

        return classes;
    }

    /// <summary>
    /// Scan a single file for CSS class usages.
    /// </summary>
    public HashSet<string> ScanFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        return ScanContent(content, Path.GetExtension(filePath));
    }

    /// <summary>
    /// Scan content string for CSS class usages.
    /// </summary>
    public HashSet<string> ScanContent(string content, string fileExtension = ".razor")
    {
        var classes = new HashSet<string>();

        // Different extraction strategies based on file type
        if (fileExtension is ".cs")
        {
            ExtractFromCSharp(content, classes);
        }
        else
        {
            ExtractFromMarkup(content, classes);
            ExtractFromCSharp(content, classes);
        }

        return classes;
    }

    private void ExtractFromMarkup(string content, HashSet<string> classes)
    {
        // Pattern 1: class="..." or Class="..."
        foreach (Match match in ClassAttributeRegex().Matches(content))
        {
            var classValue = match.Groups[1].Value;
            ExtractClasses(classValue, classes);
            if (classValue.Contains("@("))
            {
                ExtractFromCSharpExpression(classValue, classes);
            }
        }

        // Pattern 2: @class="..." (Blazor)
        foreach (Match match in BlazorClassRegex().Matches(content))
        {
            var classValue = match.Groups[1].Value;
            ExtractClasses(classValue, classes);
            if (classValue.Contains("@("))
            {
                ExtractFromCSharpExpression(classValue, classes);
            }
        }

        // Pattern 3: class=@"..." (Blazor interpolated)
        foreach (Match match in BlazorInterpolatedClassRegex().Matches(content))
        {
            var classValue = match.Groups[1].Value;
            ExtractClasses(classValue, classes);
            if (classValue.Contains("@("))
            {
                ExtractFromCSharpExpression(classValue, classes);
            }
        }

        // Pattern 4: class="@(...)" (Blazor expression - extract string literals)
        foreach (Match match in BlazorExpressionClassRegex().Matches(content))
        {
            var expression = match.Groups[1].Value;
            ExtractFromCSharpExpression(expression, classes);
        }

        // Pattern 5: className="..." (React-style, just in case)
        foreach (Match match in ClassNameAttributeRegex().Matches(content))
        {
            var classValue = match.Groups[1].Value;
            ExtractClasses(classValue, classes);
            if (classValue.Contains("@("))
            {
                ExtractFromCSharpExpression(classValue, classes);
            }
        }

        // Pattern 6: AdditionalClasses="..." or similar custom attributes
        foreach (Match match in AdditionalClassesRegex().Matches(content))
        {
            var classValue = match.Groups[1].Value;
            ExtractClasses(classValue, classes);
            if (classValue.Contains("@("))
            {
                ExtractFromCSharpExpression(classValue, classes);
            }
        }
    }

    private void ExtractFromCSharp(string content, HashSet<string> classes)
    {
        // Look for string literals that might contain class names
        // Pattern: "class-name" or "class-name another-class"
        foreach (Match match in CSharpStringLiteralRegex().Matches(content))
        {
            var stringValue = match.Groups[1].Value;

            // Only process if it looks like it contains CSS classes
            if (LooksLikeCssClasses(stringValue))
            {
                ExtractClasses(stringValue, classes);
            }
        }

        // Look for specific patterns like: CssClass = "..."
        foreach (Match match in CssClassAssignmentRegex().Matches(content))
        {
            var classValue = match.Groups[1].Value;
            ExtractClasses(classValue, classes);
        }
    }

    private void ExtractFromCSharpExpression(string expression, HashSet<string> classes)
    {
        // Extract string literals from C# expressions
        // e.g., @(isActive ? "vibe-bg-primary" : "vibe-bg-secondary")
        foreach (Match match in CSharpStringLiteralRegex().Matches(expression))
        {
            var stringValue = match.Groups[1].Value;
            ExtractClasses(stringValue, classes);
        }
    }

    private void ExtractClasses(string classString, HashSet<string> classes)
    {
        // Split by whitespace and process each token
        var tokens = classString.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            var trimmed = token.Trim();

            // Common cleanups for tokens coming from Razor expressions, e.g. "foo-bar") or 'foo'
            trimmed = trimmed.Trim('"', '\'');
            trimmed = trimmed.TrimEnd(')', ',', ';');
            trimmed = trimmed.TrimStart('(');

            // Skip empty, ignored, or obviously non-class values
            if (string.IsNullOrEmpty(trimmed) ||
                _ignoredClasses.Contains(trimmed) ||
                trimmed.StartsWith("@") ||
                trimmed.Contains("(") ||
                trimmed.Contains("{") ||
                trimmed.Contains("}"))
            {
                continue;
            }

            // Filter out common C# operators that appear in Razor class expressions
            if (trimmed is "==" or "!=" or "&&" or "||" or "?" or ":" or "=>" or "=")
            {
                continue;
            }

            // Require tokens to look like CSS class names (letters/digits plus common utility punctuation)
            if (!CssClassTokenRegex().IsMatch(trimmed))
            {
                continue;
            }

            // In strict-prefix mode, only keep utility-like tokens; ignore component-specific class names.
            if (!_allowUnprefixed && !LooksLikeUtilityToken(trimmed))
            {
                continue;
            }

            classes.Add(trimmed);
        }
    }

    private bool LooksLikeCssClasses(string value)
    {
        // Heuristics to determine if a string might contain CSS classes
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (_allowUnprefixed)
            return true;

        return LooksLikeUtilityToken(value);
    }

    private bool LooksLikeUtilityToken(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (string.IsNullOrEmpty(_prefix))
            return true;

        // Plain utility token: vibe-...
        if (value.StartsWith(_prefix + "-", StringComparison.Ordinal))
            return true;

        // Variant utility token(s): hover:vibe-..., dark:sm:hover:vibe-...
        var lastColon = value.LastIndexOf(':');
        if (lastColon > 0)
        {
            var tail = value[(lastColon + 1)..];
            if (tail.StartsWith(_prefix + "-", StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Add classes to the ignore list (won't be extracted).
    /// </summary>
    public void IgnoreClasses(params string[] classNames)
    {
        foreach (var cls in classNames)
        {
            _ignoredClasses.Add(cls);
        }
    }

    #region Regex Patterns

    // class="..." or Class="..."
    [GeneratedRegex(@"(?:class|Class)\s*=\s*""([^""]*)""", RegexOptions.Compiled)]
    private static partial Regex ClassAttributeRegex();

    // @class="..."
    [GeneratedRegex(@"@class\s*=\s*""([^""]*)""", RegexOptions.Compiled)]
    private static partial Regex BlazorClassRegex();

    // class=@"..."
    [GeneratedRegex(@"class\s*=\s*@""([^""]*)""", RegexOptions.Compiled)]
    private static partial Regex BlazorInterpolatedClassRegex();

    // class="@(...)" - captures the expression inside
    [GeneratedRegex(@"class\s*=\s*""@\(([^)]+)\)""", RegexOptions.Compiled)]
    private static partial Regex BlazorExpressionClassRegex();

    // className="..."
    [GeneratedRegex(@"className\s*=\s*""([^""]*)""", RegexOptions.Compiled)]
    private static partial Regex ClassNameAttributeRegex();

    // AdditionalClasses="..." or CssClass="..."
    [GeneratedRegex(@"(?:AdditionalClasses|CssClass|ExtraClasses|ClassNames)\s*=\s*""([^""]*)""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex AdditionalClassesRegex();

    // C# string literals "..."
    [GeneratedRegex(@"""([^""\\]*(?:\\.[^""\\]*)*)""", RegexOptions.Compiled)]
    private static partial Regex CSharpStringLiteralRegex();

    // CssClass = "...", Class = "...", etc.
    [GeneratedRegex(@"(?:CssClass|Class|ClassName|Classes)\s*=\s*""([^""]*)""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex CssClassAssignmentRegex();

    // Rough validation for class tokens (supports variants + arbitrary values)
    [GeneratedRegex(@"^[A-Za-z0-9_:\[\]\-./%]+$", RegexOptions.Compiled)]
    private static partial Regex CssClassTokenRegex();

    #endregion
}

