using Vibe.UI.CSS.Emitter;
using Vibe.UI.CSS.Generator;
using Vibe.UI.CSS.Scanner;

namespace Vibe.UI.CSS;

/// <summary>
/// Main entry point for Vibe.UI.CSS functionality.
/// </summary>
public static class VibeCss
{
    /// <summary>
    /// Generate CSS for a project by scanning its source files.
    /// </summary>
    /// <param name="projectDirectory">Root directory of the project</param>
    /// <param name="outputPath">Path to write the generated CSS</param>
    /// <param name="options">Generation options</param>
    public static GenerationResult Generate(string projectDirectory, string outputPath, GenerationOptions? options = null)
    {
        options ??= new GenerationOptions();
        return Generate(
            projectDirectory,
            outputPath,
            options.ScanPatterns,
            options.Prefix,
            options.IncludeBase,
            options.AllowUnprefixedUtilities,
            options.FailOnUnknown,
            options.IgnoredClasses);
    }

    /// <summary>
    /// Generate CSS for a project by scanning its source files.
    /// </summary>
    /// <param name="projectDirectory">Root directory of the project</param>
    /// <param name="outputPath">Path to write the generated CSS</param>
    /// <param name="patterns">File patterns to scan</param>
    /// <param name="prefix">CSS class prefix</param>
    /// <param name="includeBase">Whether to include base CSS variables</param>
    /// <param name="allowUnprefixedUtilities">When true, generate utilities without the prefix</param>
    /// <param name="failOnUnknown">When true, fail before writing output if unknown utilities are found</param>
    /// <param name="ignoredClasses">Known non-utility class names to exclude from the scan</param>
    public static GenerationResult Generate(
        string projectDirectory,
        string outputPath,
        string[]? patterns,
        string prefix = "vibe",
        bool includeBase = true,
        bool allowUnprefixedUtilities = false,
        bool failOnUnknown = false,
        string[]? ignoredClasses = null)
    {
        patterns ??= ["*.razor", "*.cshtml", "*.html"];

        try
        {
            var config = new VibeConfig
            {
                Prefix = prefix,
                AllowUnprefixedUtilities = allowUnprefixedUtilities
            };

            var emitter = new CssEmitter(config);
            var scanner = new ClassScanner(prefix, allowUnprefixedUtilities);
            if (ignoredClasses is { Length: > 0 })
            {
                scanner.IgnoreClasses(ignoredClasses);
            }

            // Scan for classes
            var classes = scanner.ScanDirectory(projectDirectory, patterns);

            // Get stats before generating
            var stats = emitter.GetStats(classes);
            if (failOnUnknown && stats.UnknownClasses.Count > 0)
            {
                var unknownSummary = string.Join(", ", stats.UnknownClasses.OrderBy(name => name).Take(20));
                if (stats.UnknownClasses.Count > 20)
                {
                    unknownSummary += $", and {stats.UnknownClasses.Count - 20} more";
                }

                return new GenerationResult
                {
                    Success = false,
                    Error = $"Unknown utility classes found: {unknownSummary}",
                    OutputPath = outputPath,
                    TotalClassesFound = stats.TotalClasses,
                    ClassesGenerated = stats.GeneratedClasses,
                    UnknownClasses = stats.UnknownClasses
                };
            }

            // Generate CSS
            var baseCssPath = Path.Combine(projectDirectory, "wwwroot", "css", "vibe-base.css");
            var css = emitter.GenerateCss(classes, includeBase, baseCssPath);

            // Write output
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            File.WriteAllText(outputPath, css);

            return new GenerationResult
            {
                Success = true,
                OutputPath = outputPath,
                TotalClassesFound = stats.TotalClasses,
                ClassesGenerated = stats.GeneratedClasses,
                UnknownClasses = stats.UnknownClasses,
                CssSize = css.Length
            };
        }
        catch (Exception ex)
        {
            return new GenerationResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    /// <summary>
    /// Generate CSS from a string of content.
    /// </summary>
    public static string GenerateFromContent(string content, GenerationOptions? options = null)
    {
        options ??= new GenerationOptions();

        var config = new VibeConfig
        {
            Prefix = options.Prefix,
            AllowUnprefixedUtilities = options.AllowUnprefixedUtilities
        };

        var emitter = new CssEmitter(config);
        return emitter.GenerateForContent(content, options.IncludeBase);
    }

    /// <summary>
    /// Scan a project for CSS classes without generating.
    /// </summary>
    public static ScanResult Scan(
        string projectDirectory,
        string[]? patterns = null,
        string prefix = "vibe",
        bool allowUnprefixedUtilities = false,
        string[]? ignoredClasses = null)
    {
        var scanner = new ClassScanner(prefix, allowUnprefixedUtilities);
        if (ignoredClasses is { Length: > 0 })
        {
            scanner.IgnoreClasses(ignoredClasses);
        }

        var classes = scanner.ScanDirectory(projectDirectory, patterns);

        var config = new VibeConfig { Prefix = prefix, AllowUnprefixedUtilities = allowUnprefixedUtilities };
        var generator = new UtilityGenerator(config);

        var recognized = new List<string>();
        var unknown = new List<string>();

        foreach (var cls in classes)
        {
            if (generator.Generate(cls) != null)
            {
                recognized.Add(cls);
            }
            else
            {
                unknown.Add(cls);
            }
        }

        return new ScanResult
        {
            TotalClasses = classes.Count,
            RecognizedClasses = recognized,
            UnknownClasses = unknown
        };
    }
}

/// <summary>
/// Options for CSS generation.
/// </summary>
public class GenerationOptions
{
    /// <summary>
    /// CSS class prefix (default: "vibe")
    /// </summary>
    public string Prefix { get; set; } = "vibe";

    /// <summary>
    /// When true, generate utilities for unprefixed class names too.
    /// </summary>
    public bool AllowUnprefixedUtilities { get; set; } = false;

    /// <summary>
    /// When true, generation fails without modifying the output file if unknown utilities are found.
    /// </summary>
    public bool FailOnUnknown { get; set; }

    /// <summary>
    /// Known non-utility class names that should be excluded from scanning.
    /// </summary>
    public string[] IgnoredClasses { get; set; } = [];

    /// <summary>
    /// Whether to include vibe-base.css content
    /// </summary>
    public bool IncludeBase { get; set; } = true;

    /// <summary>
    /// File patterns to scan (default: *.razor, *.cshtml, *.html)
    /// </summary>
    public string[] ScanPatterns { get; set; } = ["*.razor", "*.cshtml", "*.html"];
}

/// <summary>
/// Result of CSS generation.
/// </summary>
public class GenerationResult
{
    /// <summary>
    /// Gets whether the CSS generation was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the path where the generated CSS was written.
    /// </summary>
    public string OutputPath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the total number of class names found during scanning.
    /// </summary>
    public int TotalClassesFound { get; init; }

    /// <summary>
    /// Gets the number of classes that were successfully generated.
    /// </summary>
    public int ClassesGenerated { get; init; }

    /// <summary>
    /// Gets the list of class names that could not be generated.
    /// </summary>
    public List<string> UnknownClasses { get; init; } = [];

    /// <summary>
    /// Gets the size of the generated CSS in bytes.
    /// </summary>
    public int CssSize { get; init; }

    /// <summary>
    /// Gets the error message if generation failed.
    /// </summary>
    public string? Error { get; init; }
}

/// <summary>
/// Result of scanning for CSS classes.
/// </summary>
public class ScanResult
{
    /// <summary>
    /// Gets the total number of class names found during scanning.
    /// </summary>
    public int TotalClasses { get; init; }

    /// <summary>
    /// Gets the list of class names that were recognized by the generator.
    /// </summary>
    public List<string> RecognizedClasses { get; init; } = [];

    /// <summary>
    /// Gets the list of class names that were not recognized by the generator.
    /// </summary>
    public List<string> UnknownClasses { get; init; } = [];
}

