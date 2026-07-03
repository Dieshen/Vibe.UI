using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Vibe.UI.CSS;

namespace Vibe.UI.CLI.Commands;

/// <summary>
/// Command for generating CSS using Vibe.UI.CSS JIT engine.
/// </summary>
public class CssCommand : AsyncCommand<CssCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Project directory path to scan")]
        [CommandArgument(0, "[path]")]
        [DefaultValue(".")]
        public string ProjectPath { get; init; } = ".";

        [Description("Output CSS file path")]
        [CommandOption("-o|--output")]
        [DefaultValue("wwwroot/css/Vibe.UI.CSS")]
        public string OutputPath { get; init; } = "wwwroot/css/Vibe.UI.CSS";

        [Description("Include vibe-base.css content")]
        [CommandOption("--with-base")]
        [DefaultValue(true)]
        public bool IncludeBase { get; init; } = true;

        [Description("CSS class prefix")]
        [CommandOption("--prefix")]
        [DefaultValue("vibe")]
        public string Prefix { get; init; } = "vibe";

        [Description("Watch for changes and regenerate")]
        [CommandOption("-w|--watch")]
        [DefaultValue(false)]
        public bool Watch { get; init; }

        [Description("Show verbose output")]
        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }

        [Description("Only scan, don't generate CSS")]
        [CommandOption("--scan-only")]
        [DefaultValue(false)]
        public bool ScanOnly { get; init; }

        [Description("File patterns to scan (comma-separated)")]
        [CommandOption("--patterns")]
        [DefaultValue("*.razor,*.cshtml,*.html")]
        public string Patterns { get; init; } = "*.razor,*.cshtml,*.html";
    }

    public Task<int> ExecuteAsync(CommandContext context, Settings settings) =>
        ExecuteAsync(context, settings, CancellationToken.None);

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var projectPath = Path.GetFullPath(settings.ProjectPath);

        if (!Directory.Exists(projectPath))
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] Directory not found: {projectPath}");
            return 1;
        }

        var patterns = settings.Patterns.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (settings.ScanOnly)
        {
            return await ScanOnlyAsync(projectPath, patterns, settings);
        }

        if (settings.Watch)
        {
            return await WatchAndGenerateAsync(projectPath, settings, patterns);
        }

        return await GenerateOnceAsync(projectPath, settings, patterns);
    }

    private static async Task<int> ScanOnlyAsync(string projectPath, string[] patterns, Settings settings)
    {
        AnsiConsole.MarkupLine($"[blue]Scanning[/] {projectPath} for CSS classes...\n");

        var result = VibeCss.Scan(projectPath, patterns, settings.Prefix);

        // Display results in a table
        var table = new Table();
        table.AddColumn("Category");
        table.AddColumn("Count");

        table.AddRow("Total classes found", result.TotalClasses.ToString());
        table.AddRow("[green]Recognized classes[/]", result.RecognizedClasses.Count.ToString());
        table.AddRow("[yellow]Unknown classes[/]", result.UnknownClasses.Count.ToString());

        AnsiConsole.Write(table);

        if (settings.Verbose && result.RecognizedClasses.Count > 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Recognized classes:[/]");
            foreach (var cls in result.RecognizedClasses.OrderBy(c => c).Take(50))
            {
                AnsiConsole.MarkupLine($"  - {cls}");
            }
            if (result.RecognizedClasses.Count > 50)
            {
                AnsiConsole.MarkupLine($"  ... and {result.RecognizedClasses.Count - 50} more");
            }
        }

        if (result.UnknownClasses.Count > 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Unknown classes (not generated):[/]");
            foreach (var cls in result.UnknownClasses.OrderBy(c => c).Take(20))
            {
                AnsiConsole.MarkupLine($"  - {cls}");
            }
            if (result.UnknownClasses.Count > 20)
            {
                AnsiConsole.MarkupLine($"  ... and {result.UnknownClasses.Count - 20} more");
            }
        }

        return await Task.FromResult(0);
    }

    private static async Task<int> GenerateOnceAsync(string projectPath, Settings settings, string[] patterns)
    {
        var outputPath = Path.IsPathRooted(settings.OutputPath)
            ? settings.OutputPath
            : Path.Combine(projectPath, settings.OutputPath);

        AnsiConsole.MarkupLine($"[blue]Generating CSS[/] from {projectPath}...");

        try
        {
            var options = new GenerationOptions
            {
                Prefix = settings.Prefix,
                IncludeBase = settings.IncludeBase,
                ScanPatterns = patterns
            };

            var result = VibeCss.Generate(projectPath, outputPath, options);

            if (!result.Success)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {result.Error}");
                return 1;
            }

            // Display results
            AnsiConsole.WriteLine();
            var table = new Table();
            table.AddColumn("Metric");
            table.AddColumn("Value");

            table.AddRow("Classes found", result.TotalClassesFound.ToString());
            table.AddRow("CSS rules generated", result.ClassesGenerated.ToString());
            table.AddRow("Output file", result.OutputPath);
            table.AddRow("File size", FormatFileSize(result.CssSize));

            AnsiConsole.Write(table);

            if (result.UnknownClasses.Count > 0 && settings.Verbose)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[yellow]Warning:[/] {result.UnknownClasses.Count} unknown classes were skipped:");
                foreach (var cls in result.UnknownClasses.Take(10))
                {
                    AnsiConsole.MarkupLine($"  - {cls}");
                }
                if (result.UnknownClasses.Count > 10)
                {
                    AnsiConsole.MarkupLine($"  ... and {result.UnknownClasses.Count - 10} more");
                }
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]OK[/] CSS generated successfully at [blue]{outputPath}[/]");

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
            if (settings.Verbose)
            {
                AnsiConsole.WriteException(ex);
            }
            return 1;
        }
    }

    private static async Task<int> WatchAndGenerateAsync(string projectPath, Settings settings, string[] patterns)
    {
        var outputPath = Path.IsPathRooted(settings.OutputPath)
            ? settings.OutputPath
            : Path.Combine(projectPath, settings.OutputPath);

        AnsiConsole.MarkupLine($"[blue]Watching[/] {projectPath} for changes...");
        AnsiConsole.MarkupLine("[grey]Press Ctrl+C to stop[/]\n");

        // Initial generation
        await GenerateOnceAsync(projectPath, settings, patterns);

        using var watcher = new FileSystemWatcher(projectPath)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
        };

        // Set up filters for each pattern
        foreach (var pattern in patterns)
        {
            watcher.Filters.Add(pattern);
        }
        // Also watch .cs files for C# class definitions
        watcher.Filters.Add("*.cs");

        var debounceState = new DebounceState();

        watcher.Changed += async (s, e) => await OnFileChanged(e, projectPath, settings, patterns, debounceState);
        watcher.Created += async (s, e) => await OnFileChanged(e, projectPath, settings, patterns, debounceState);
        watcher.Deleted += async (s, e) => await OnFileChanged(e, projectPath, settings, patterns, debounceState);

        watcher.EnableRaisingEvents = true;

        // Wait indefinitely until Ctrl+C
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            AnsiConsole.MarkupLine("\n[yellow]Watch stopped.[/]");
        }

        return 0;
    }

    private class DebounceState
    {
        public DateTime LastGeneration { get; set; } = DateTime.MinValue;
        public int DebounceMs { get; } = 500;
    }

    private static async Task OnFileChanged(FileSystemEventArgs e, string projectPath, Settings settings, string[] patterns, DebounceState debounceState)
    {
        // Simple debounce
        var now = DateTime.Now;
        if ((now - debounceState.LastGeneration).TotalMilliseconds < debounceState.DebounceMs)
            return;

        debounceState.LastGeneration = now;

        // Skip output file changes to avoid infinite loop
        var outputPath = Path.IsPathRooted(settings.OutputPath)
            ? settings.OutputPath
            : Path.Combine(projectPath, settings.OutputPath);

        if (Path.GetFullPath(e.FullPath).Equals(Path.GetFullPath(outputPath), StringComparison.OrdinalIgnoreCase))
            return;

        AnsiConsole.MarkupLine($"\n[grey]{DateTime.Now:HH:mm:ss}[/] File changed: {Path.GetFileName(e.FullPath)}");

        // Small delay to let file system settle
        await Task.Delay(100);

        await GenerateOnceAsync(projectPath, settings, patterns);
    }

    private static string FormatFileSize(int bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F2} MB";
    }
}

