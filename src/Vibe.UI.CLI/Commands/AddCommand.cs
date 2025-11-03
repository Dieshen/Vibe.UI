using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Vibe.UI.CLI.Services;

namespace Vibe.UI.CLI.Commands;

public class AddCommand : AsyncCommand<AddCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Component name to add")]
        [CommandArgument(0, "[component]")]
        public string? Component { get; init; }

        [Description("Skip confirmation prompts")]
        [CommandOption("-y|--yes")]
        [DefaultValue(false)]
        public bool SkipPrompts { get; init; }

        [Description("Overwrite existing files")]
        [CommandOption("-o|--overwrite")]
        [DefaultValue(false)]
        public bool Overwrite { get; init; }

        [Description("Project directory path")]
        [CommandOption("-p|--path")]
        [DefaultValue(".")]
        public string ProjectPath { get; init; } = ".";

        [Description("Custom name for the component")]
        [CommandOption("-n|--name")]
        public string? Name { get; init; }

        [Description("Custom output directory for the component")]
        [CommandOption("--output")]
        public string? Output { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var configService = new ConfigService();
        var componentService = new ComponentService();

        // Check if vibe init was run
        var infrastructurePath = Path.Combine(settings.ProjectPath, "Vibe", "Base", "ThemedComponentBase.cs");
        if (!File.Exists(infrastructurePath))
        {
            AnsiConsole.MarkupLine("[yellow]Warning:[/] Vibe.UI infrastructure not found.");
            AnsiConsole.MarkupLine("Run [blue]vibe init[/] first to copy the required infrastructure.");
            return 1;
        }

        // Load configuration
        var config = await configService.LoadConfigAsync(settings.ProjectPath);
        if (config == null)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Vibe.UI is not initialized in this project.");
            AnsiConsole.MarkupLine("[yellow]Run 'vibe init' first.[/]");
            return 1;
        }

        // Get component name
        var componentName = settings.Component;
        if (string.IsNullOrEmpty(componentName))
        {
            var availableComponents = componentService.GetAvailableComponents();
            componentName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which [green]component[/] would you like to add?")
                    .PageSize(15)
                    .AddChoices(availableComponents.Select(c => c.Name)));
        }

        // Get component info
        var component = componentService.GetComponent(componentName);
        if (component == null)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] Component '{componentName}' not found.");
            AnsiConsole.MarkupLine("[yellow]Run 'vibe list' to see available components.[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[blue]Adding {component.Name} component...[/]\n");

        // Show component info
        var table = new Table();
        table.AddColumn("Property");
        table.AddColumn("Value");
        table.AddRow("Component", component.Name);
        table.AddRow("Category", component.Category);
        table.AddRow("Description", component.Description);
        table.AddRow("Dependencies", string.Join(", ", component.Dependencies ?? new List<string>()));

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        // Confirm
        if (!settings.SkipPrompts)
        {
            if (!AnsiConsole.Confirm($"Add {component.Name} to your project?"))
            {
                return 0;
            }
        }

        // Install component
        await AnsiConsole.Status()
            .StartAsync($"Installing {component.Name}...", async ctx =>
            {
                // Install dependencies first (dependencies don't get renamed or custom output)
                if (component.Dependencies?.Any() == true)
                {
                    foreach (var dep in component.Dependencies)
                    {
                        ctx.Status($"Installing dependency: {dep}...");
                        await componentService.InstallComponentAsync(
                            settings.ProjectPath,
                            config.ComponentsDirectory,
                            dep,
                            settings.Overwrite,
                            customName: null,
                            customOutputDir: settings.Output);
                    }
                }

                // Install the component with custom name and output directory
                ctx.Status($"Installing {component.Name}...");
                await componentService.InstallComponentAsync(
                    settings.ProjectPath,
                    config.ComponentsDirectory,
                    componentName,
                    settings.Overwrite,
                    customName: settings.Name,
                    customOutputDir: settings.Output);
            });

        var displayName = settings.Name ?? component.Name;
        AnsiConsole.MarkupLine($"\n[green]✓[/] {displayName} added successfully!");

        if (!string.IsNullOrEmpty(settings.Name))
        {
            AnsiConsole.MarkupLine($"[dim]Renamed from {component.Name} to {settings.Name}[/]");
        }

        if (!string.IsNullOrEmpty(settings.Output))
        {
            AnsiConsole.MarkupLine($"[dim]Output directory: {settings.Output}[/]");
        }

        // Show usage example
        if (!string.IsNullOrEmpty(component.Example))
        {
            AnsiConsole.MarkupLine($"\n[blue]Example usage:[/]");
            var panel = new Panel(component.Example)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1)
            };
            AnsiConsole.Write(panel);
        }

        return 0;
    }
}
