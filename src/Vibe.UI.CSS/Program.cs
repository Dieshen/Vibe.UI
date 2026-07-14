// Vibe.UI.CSS - .NET-Native CSS Framework CLI
// Run with: dotnet run --project src/Vibe.UI.CSS
// Or when packaged: dotnet Vibe.UI.CSS.dll generate <directory> -o <output.css>

using System.Reflection;
using Vibe.UI.CSS;
using Vibe.UI.CSS.Generator;

if (args.Length == 0)
{
    PrintUsage();
    return 0;
}

var command = args[0].ToLower();

return command switch
{
    "scan" => RunScan(args.Skip(1).ToArray()),
    "generate" => RunGenerate(args.Skip(1).ToArray()),
    "test" => RunTest(),
    "--help" or "-h" or "help" => PrintUsage(),
    "--version" or "-v" => PrintVersion(),
    _ => HandleUnknownCommand(command),
};

static int PrintUsage()
{
    Console.WriteLine("Vibe.UI.CSS - .NET-Native CSS Framework");
    Console.WriteLine("=====================================");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  vibe-css scan <directory> [options]");
    Console.WriteLine("  vibe-css generate <directory> [options]");
    Console.WriteLine("  vibe-css test");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  scan       Scan directory for CSS classes");
    Console.WriteLine("  generate   Generate CSS file from scanned classes");
    Console.WriteLine("  test       Run built-in tests");
    Console.WriteLine();
    Console.WriteLine("Scan and Generate Options:");
    Console.WriteLine("  -o, --output <file>     Output CSS file path (default: Vibe.UI.CSS)");
    Console.WriteLine("  --prefix <prefix>       CSS class prefix (default: vibe)");
    Console.WriteLine(
        "  --allow-unprefixed [true|false] Generate unprefixed utilities too (default: false)"
    );
    Console.WriteLine("  --with-base [true|false] Include base CSS variables (default: true)");
    Console.WriteLine(
        "  --with-preflight [true|false] Include the opt-in Tailwind reset (default: false)"
    );
    Console.WriteLine("  --patterns <patterns>   Comma-separated file patterns to scan");
    Console.WriteLine("                          (default: *.razor,*.cshtml,*.html)");
    Console.WriteLine("  --ignore <classes>      Comma-separated known non-utility class names");
    Console.WriteLine(
        "  --fail-on-unknown [true|false] Exit non-zero when unknown utilities are found"
    );
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  vibe-css generate . -o wwwroot/css/Vibe.UI.CSS");
    Console.WriteLine("  vibe-css generate ./src -o output.css --prefix tw");
    Console.WriteLine("  vibe-css scan ./Components --patterns \"*.razor,*.cs\"");
    Console.WriteLine();
    return 0;
}

static int PrintVersion()
{
    var informational = Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion;

    var version = !string.IsNullOrWhiteSpace(informational)
        ? informational.Split('+', 2)[0]
        : Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.0.0";

    Console.WriteLine($"Vibe.UI.CSS version {version}");
    return 0;
}

static int HandleUnknownCommand(string command)
{
    Console.WriteLine($"Unknown command: {command}");
    Console.WriteLine("Run 'vibe-css --help' for usage information.");
    return 1;
}

static int RunScan(string[] args)
{
    var directory = Directory.GetCurrentDirectory();
    var patterns = new[] { "*.razor", "*.cshtml", "*.html" };
    var prefix = "vibe";
    var allowUnprefixed = false;
    var failOnUnknown = false;
    var ignoredClasses = Array.Empty<string>();

    // Parse arguments
    for (int i = 0; i < args.Length; i++)
    {
        var arg = args[i];
        var (option, inlineValue) = SplitOption(arg);

        if (!option.StartsWith('-'))
        {
            // Positional argument - directory
            directory = arg;
            continue;
        }

        switch (option)
        {
            case "--patterns":
                if (ReadOptionValue(args, ref i, inlineValue) is { } patternValue)
                    patterns = SplitList(patternValue);
                break;
            case "--prefix":
                if (ReadOptionValue(args, ref i, inlineValue) is { } prefixValue)
                    prefix = prefixValue;
                break;
            case "--allow-unprefixed":
                allowUnprefixed = ReadBooleanOption(args, ref i, inlineValue);
                break;
            case "--fail-on-unknown":
                failOnUnknown = ReadBooleanOption(args, ref i, inlineValue);
                break;
            case "--ignore":
                if (ReadOptionValue(args, ref i, inlineValue) is { } ignoreValue)
                    ignoredClasses = SplitList(ignoreValue);
                break;
        }
    }

    if (!Directory.Exists(directory))
    {
        Console.Error.WriteLine($"Error: Directory not found: {directory}");
        return 1;
    }

    Console.WriteLine($"Scanning: {directory}");
    Console.WriteLine($"Patterns: {string.Join(", ", patterns)}");
    Console.WriteLine($"Prefix: {prefix}");
    Console.WriteLine($"Allow unprefixed: {allowUnprefixed}");
    Console.WriteLine();

    var result = VibeCss.Scan(directory, patterns, prefix, allowUnprefixed, ignoredClasses);

    Console.WriteLine($"Total classes found: {result.TotalClasses}");
    Console.WriteLine($"Recognized: {result.RecognizedClasses.Count}");
    Console.WriteLine($"Unknown: {result.UnknownClasses.Count}");
    Console.WriteLine();

    if (result.UnknownClasses.Count > 0)
    {
        Console.WriteLine("Unknown classes (first 20):");
        foreach (var cls in result.UnknownClasses.Take(20))
        {
            Console.WriteLine($"  - {cls}");
        }
        if (result.UnknownClasses.Count > 20)
        {
            Console.WriteLine($"  ... and {result.UnknownClasses.Count - 20} more");
        }
    }

    if (failOnUnknown && result.UnknownClasses.Count > 0)
    {
        Console.Error.WriteLine("Strict scan failed because unknown utility classes were found.");
        return 1;
    }

    return 0;
}

static int RunGenerate(string[] args)
{
    var directory = Directory.GetCurrentDirectory();
    var output = "Vibe.UI.CSS";
    var prefix = "vibe";
    var allowUnprefixed = false;
    var includeBase = true;
    var patterns = new[] { "*.razor", "*.cshtml", "*.html" };
    var failOnUnknown = false;
    var includePreflight = false;
    var ignoredClasses = Array.Empty<string>();

    // Parse arguments
    for (int i = 0; i < args.Length; i++)
    {
        var arg = args[i];
        var (option, inlineValue) = SplitOption(arg);

        if (!option.StartsWith('-'))
        {
            // Positional argument - directory
            directory = arg;
            continue;
        }

        switch (option)
        {
            case "-o":
            case "--output":
                if (ReadOptionValue(args, ref i, inlineValue) is { } outputValue)
                    output = outputValue;
                break;
            case "--prefix":
                if (ReadOptionValue(args, ref i, inlineValue) is { } prefixValue)
                    prefix = prefixValue;
                break;
            case "--allow-unprefixed":
                allowUnprefixed = ReadBooleanOption(args, ref i, inlineValue);
                break;
            case "--with-base":
                includeBase = ReadBooleanOption(args, ref i, inlineValue);
                break;
            case "--with-preflight":
                includePreflight = ReadBooleanOption(args, ref i, inlineValue);
                break;
            case "--patterns":
                if (ReadOptionValue(args, ref i, inlineValue) is { } patternValue)
                    patterns = SplitList(patternValue);
                break;
            case "--fail-on-unknown":
                failOnUnknown = ReadBooleanOption(args, ref i, inlineValue);
                break;
            case "--ignore":
                if (ReadOptionValue(args, ref i, inlineValue) is { } ignoreValue)
                    ignoredClasses = SplitList(ignoreValue);
                break;
        }
    }

    if (!Directory.Exists(directory))
    {
        Console.Error.WriteLine($"Error: Directory not found: {directory}");
        return 1;
    }

    Console.WriteLine($"Vibe.UI.CSS: Generating CSS for: {directory}");
    Console.WriteLine($"  Output: {output}");
    Console.WriteLine($"  Prefix: {prefix}");
    Console.WriteLine($"  Allow unprefixed: {allowUnprefixed}");
    Console.WriteLine($"  Include base: {includeBase}");
    Console.WriteLine($"  Include preflight: {includePreflight}");
    Console.WriteLine($"  Patterns: {string.Join(", ", patterns)}");

    var options = new GenerationOptions
    {
        Prefix = prefix,
        IncludeBase = includeBase,
        IncludePreflight = includePreflight,
        ScanPatterns = patterns,
        AllowUnprefixedUtilities = allowUnprefixed,
        FailOnUnknown = failOnUnknown,
        IgnoredClasses = ignoredClasses,
    };

    var result = VibeCss.Generate(directory, output, options);

    if (!result.Success)
    {
        Console.Error.WriteLine($"Error: {result.Error}");
        return 1;
    }

    Console.WriteLine($"  Classes found: {result.TotalClassesFound}");
    Console.WriteLine($"  Classes generated: {result.ClassesGenerated}");
    Console.WriteLine($"  CSS size: {result.CssSize:N0} bytes");

    if (result.UnknownClasses.Count > 0)
    {
        Console.WriteLine($"  Unknown classes: {result.UnknownClasses.Count}");
    }

    return 0;
}

static (string Option, string? InlineValue) SplitOption(string argument)
{
    var equalsIndex = argument.IndexOf('=');
    return equalsIndex > 0
        ? (argument[..equalsIndex], argument[(equalsIndex + 1)..])
        : (argument, null);
}

static string? ReadOptionValue(string[] args, ref int index, string? inlineValue)
{
    if (inlineValue != null)
        return inlineValue;

    if (index + 1 >= args.Length || args[index + 1].StartsWith('-'))
        return null;

    return args[++index];
}

static bool ReadBooleanOption(string[] args, ref int index, string? inlineValue)
{
    if (inlineValue != null)
        return ParseBoolean(inlineValue, defaultValue: true);

    if (index + 1 < args.Length && TryParseBoolean(args[index + 1], out var value))
    {
        index++;
        return value;
    }

    return true;
}

static bool ParseBoolean(string value, bool defaultValue) =>
    TryParseBoolean(value, out var parsed) ? parsed : defaultValue;

static bool TryParseBoolean(string value, out bool parsed)
{
    switch (value.ToLowerInvariant())
    {
        case "true":
        case "1":
        case "yes":
        case "on":
            parsed = true;
            return true;
        case "false":
        case "0":
        case "no":
        case "off":
            parsed = false;
            return true;
        default:
            parsed = false;
            return false;
    }
}

static string[] SplitList(string value) =>
    value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

static int RunTest()
{
    Console.WriteLine("Testing Vibe.UI.CSS Generator");
    Console.WriteLine("==========================");
    Console.WriteLine();

    var generator = new UtilityGenerator();

    // Test cases
    var testCases = new[]
    {
        // Display
        "vibe-flex",
        "vibe-hidden",
        "vibe-block",
        "vibe-grid",
        // Flexbox
        "vibe-flex-row",
        "vibe-flex-col",
        "vibe-items-center",
        "vibe-justify-between",
        "vibe-gap-4",
        "vibe-gap-x-2",
        // Spacing
        "vibe-p-4",
        "vibe-px-6",
        "vibe-py-2",
        "vibe-pt-8",
        "vibe-m-4",
        "vibe-mx-auto",
        "vibe-mt-2",
        "vibe--mt-4",
        // Sizing
        "vibe-w-full",
        "vibe-w-1/2",
        "vibe-w-64",
        "vibe-h-screen",
        "vibe-min-w-0",
        "vibe-max-w-lg",
        // Typography
        "vibe-text-sm",
        "vibe-text-xl",
        "vibe-text-center",
        "vibe-font-bold",
        "vibe-font-medium",
        "vibe-truncate",
        "vibe-leading-tight",
        // Colors (semantic)
        "vibe-bg-primary",
        "vibe-bg-muted",
        "vibe-text-foreground",
        "vibe-border-destructive",
        // Colors (Tailwind palette)
        "vibe-bg-red-500",
        "vibe-text-blue-600",
        "vibe-border-emerald-300",
        "vibe-bg-slate-100",
        "vibe-text-gray-900",
        // Colors with opacity
        "vibe-bg-red-500/50",
        "vibe-text-blue-600/75",
        // Borders
        "vibe-border",
        "vibe-border-2",
        "vibe-border-t",
        "vibe-rounded",
        "vibe-rounded-lg",
        "vibe-rounded-full",
        // Effects
        "vibe-shadow",
        "vibe-shadow-lg",
        "vibe-opacity-50",
        "vibe-transition",
        "vibe-duration-300",
        // Layout
        "vibe-relative",
        "vibe-absolute",
        "vibe-fixed",
        "vibe-top-0",
        "vibe-inset-0",
        "vibe-z-50",
        "vibe-overflow-hidden",
        "vibe-overflow-auto",
        // Interactivity
        "vibe-cursor-pointer",
        "vibe-select-none",
        "vibe-sr-only",
        // Grid
        "vibe-grid-cols-3",
        "vibe-grid-cols-12",
        "vibe-col-span-2",
        // Variants
        "hover:vibe-bg-primary",
        "focus:vibe-ring",
        "sm:vibe-flex",
        "md:vibe-hidden",
        "lg:vibe-grid-cols-4",
        "dark:vibe-bg-slate-900",
        // Arbitrary values
        "vibe-w-[500px]",
        "vibe-p-[1.5rem]",
        "vibe-bg-[#ff0000]",
    };

    var passed = 0;
    var failed = new List<string>();

    foreach (var testCase in testCases)
    {
        var rule = generator.Generate(testCase);
        if (rule != null)
        {
            Console.WriteLine($"✓ {testCase}");
            Console.WriteLine($"  → {rule.ToCss()}");
            passed++;
        }
        else
        {
            Console.WriteLine($"✗ {testCase} - NOT RECOGNIZED");
            failed.Add(testCase);
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Results: {passed}/{testCases.Length} passed");

    if (failed.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Failed cases:");
        foreach (var f in failed)
        {
            Console.WriteLine($"  - {f}");
        }
        return 1;
    }

    return 0;
}
