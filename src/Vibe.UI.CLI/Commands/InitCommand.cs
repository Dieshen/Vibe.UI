using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Vibe.UI.CLI.Services;

namespace Vibe.UI.CLI.Commands;

public class InitCommand : AsyncCommand<InitCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Skip confirmation prompts")]
        [CommandOption("-y|--yes")]
        [DefaultValue(false)]
        public bool SkipPrompts { get; init; }

        [Description("Project directory path")]
        [CommandOption("-p|--path")]
        [DefaultValue(".")]
        public string ProjectPath { get; init; } = ".";

        [Description("Minimal infrastructure only")]
        [CommandOption("--minimal")]
        [DefaultValue(false)]
        public bool Minimal { get; init; }

        [Description("Skip theme system")]
        [CommandOption("--no-theme")]
        [DefaultValue(false)]
        public bool NoTheme { get; init; }

        [Description("Include Chart.js support")]
        [CommandOption("--with-charts")]
        [DefaultValue(false)]
        public bool WithCharts { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine("[blue]Initializing Vibe.UI in your project...[/]\n");

        var configService = new ConfigService();

        // Check if already initialized
        var vibeDir = Path.Combine(settings.ProjectPath, "Vibe");
        if (Directory.Exists(vibeDir))
        {
            if (!settings.SkipPrompts)
            {
                if (!AnsiConsole.Confirm("Vibe.UI infrastructure already exists. Do you want to overwrite?"))
                {
                    return 0;
                }
            }
        }

        // Select component directory
        var componentDir = "Components/vibe";
        if (!settings.SkipPrompts)
        {
            componentDir = AnsiConsole.Ask("Where should components be installed?", "Components/vibe");
        }

        // Create configuration
        var config = new Models.VibeConfig
        {
            ProjectType = "Blazor",
            Theme = "both",
            ComponentsDirectory = componentDir,
            CssVariables = true
        };

        await AnsiConsole.Status()
            .StartAsync("Setting up Vibe.UI infrastructure...", async ctx =>
            {
                // Save configuration
                await configService.SaveConfigAsync(settings.ProjectPath, config);

                ctx.Status("Copying infrastructure files...");

                // Copy infrastructure files
                await CopyInfrastructureAsync(settings.ProjectPath, settings.Minimal, settings.NoTheme, settings.WithCharts);

                ctx.Status("Creating component directory...");

                // Create components directory
                Directory.CreateDirectory(Path.Combine(settings.ProjectPath, componentDir));
            });

        AnsiConsole.MarkupLine("\n[green]✓[/] Vibe.UI initialized successfully!");
        AnsiConsole.MarkupLine($"[grey]Infrastructure copied to Vibe/ folder[/]");
        AnsiConsole.MarkupLine($"\n[blue]Next steps:[/]");
        AnsiConsole.MarkupLine($"  1. Add to Program.cs: [yellow]builder.Services.AddVibeUI();[/]");
        AnsiConsole.MarkupLine($"  2. Run [yellow]vibe add button[/] to add your first component");
        AnsiConsole.MarkupLine($"  3. Run [yellow]vibe list[/] to see all available components");

        return 0;
    }

    private async Task CopyInfrastructureAsync(string projectPath, bool minimal, bool noTheme, bool withCharts)
    {
        // Get the template path (either from package or development)
        var templatePath = GetTemplatePath();
        var infrastructurePath = Path.Combine(templatePath, "Infrastructure");

        if (!Directory.Exists(infrastructurePath))
        {
            throw new DirectoryNotFoundException($"Infrastructure templates not found at: {infrastructurePath}");
        }

        var targetVibeDir = Path.Combine(projectPath, "Vibe");

        // Copy Base/ folder (always required)
        await CopyDirectoryAsync(
            Path.Combine(infrastructurePath, "Base"),
            Path.Combine(targetVibeDir, "Base"));

        // Copy Services/ folder
        await CopyDirectoryAsync(
            Path.Combine(infrastructurePath, "Services"),
            Path.Combine(targetVibeDir, "Services"));

        // Copy Themes/ folder (unless --no-theme)
        if (!noTheme)
        {
            await CopyDirectoryAsync(
                Path.Combine(infrastructurePath, "Themes"),
                Path.Combine(targetVibeDir, "Themes"));
        }

        // Copy ServiceCollectionExtensions.cs
        var serviceExtensionsSource = Path.Combine(infrastructurePath, "ServiceCollectionExtensions.cs");
        var serviceExtensionsTarget = Path.Combine(targetVibeDir, "ServiceCollectionExtensions.cs");
        if (File.Exists(serviceExtensionsSource))
        {
            await File.WriteAllTextAsync(serviceExtensionsTarget, await File.ReadAllTextAsync(serviceExtensionsSource));
        }

        // Copy _Imports.razor
        var importsSource = Path.Combine(infrastructurePath, "_Imports.razor");
        var importsTarget = Path.Combine(targetVibeDir, "_Imports.razor");
        if (File.Exists(importsSource))
        {
            await File.WriteAllTextAsync(importsTarget, await File.ReadAllTextAsync(importsSource));
        }

        // Copy JavaScript files
        var jsTemplatePath = Path.Combine(templatePath, "wwwroot", "js");
        var jsTargetPath = Path.Combine(projectPath, "wwwroot", "js");
        Directory.CreateDirectory(jsTargetPath);

        // Always copy themeInterop.js
        var themeInteropSource = Path.Combine(jsTemplatePath, "themeInterop.js");
        var themeInteropTarget = Path.Combine(jsTargetPath, "themeInterop.js");
        if (File.Exists(themeInteropSource))
        {
            await File.WriteAllTextAsync(themeInteropTarget, await File.ReadAllTextAsync(themeInteropSource));
        }

        // Copy vibe-chart.js if --with-charts
        if (withCharts)
        {
            var chartSource = Path.Combine(jsTemplatePath, "vibe-chart.js");
            var chartTarget = Path.Combine(jsTargetPath, "vibe-chart.js");
            if (File.Exists(chartSource))
            {
                await File.WriteAllTextAsync(chartTarget, await File.ReadAllTextAsync(chartSource));
            }
        }
    }

    private async Task CopyDirectoryAsync(string sourceDir, string targetDir)
    {
        if (!Directory.Exists(sourceDir))
            return;

        Directory.CreateDirectory(targetDir);

        // Copy all files
        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDir, file);
            var targetFile = Path.Combine(targetDir, relativePath);
            var targetFileDir = Path.GetDirectoryName(targetFile);

            if (!string.IsNullOrEmpty(targetFileDir))
            {
                Directory.CreateDirectory(targetFileDir);
            }

            await File.WriteAllTextAsync(targetFile, await File.ReadAllTextAsync(file));
        }
    }

    private string GetTemplatePath()
    {
        var assemblyLocation = Path.GetDirectoryName(typeof(InitCommand).Assembly.Location) ?? AppContext.BaseDirectory;

        // Try multiple possible paths for Templates directory
        var possiblePaths = new[]
        {
            // 1. Development mode: relative to CLI project
            Path.GetFullPath(Path.Combine(assemblyLocation, "..", "..", "..", "Templates")),

            // 2. Packaged with CLI in Templates folder (adjacent to executable)
            Path.Combine(assemblyLocation, "Templates"),

            // 3. Dotnet global tool: Templates folder in package root (../../.. from tools/net9.0/any)
            Path.GetFullPath(Path.Combine(assemblyLocation, "..", "..", "..", "Templates")),

            // 4. Using AppContext.BaseDirectory
            Path.Combine(AppContext.BaseDirectory, "Templates"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Templates"))
        };

        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
        }

        throw new DirectoryNotFoundException(
            $"Could not find Templates directory. Please ensure Vibe.UI.CLI is installed correctly. " +
            $"Searched locations: {string.Join(", ", possiblePaths)}");
    }
}
