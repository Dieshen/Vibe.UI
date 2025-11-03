using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Vibe.UI.CLI.Services;

namespace Vibe.UI.CLI.Commands;

public class UpdateCommand : AsyncCommand<UpdateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Update specific component")]
        [CommandArgument(0, "[component]")]
        public string? Component { get; init; }

        [Description("Skip confirmation prompts")]
        [CommandOption("-y|--yes")]
        [DefaultValue(false)]
        public bool SkipPrompts { get; init; }

        [Description("Project directory path")]
        [CommandOption("-p|--path")]
        [DefaultValue(".")]
        public string ProjectPath { get; init; } = ".";
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var configService = new ConfigService();
        var componentService = new ComponentService();

        var config = await configService.LoadConfigAsync(settings.ProjectPath);
        if (config == null)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Vibe.UI is not initialized in this project.");
            return 1;
        }

        if (!string.IsNullOrEmpty(settings.Component))
        {
            // Update specific component
            AnsiConsole.MarkupLine($"[blue]Updating {settings.Component}...[/]");
            await componentService.InstallComponentAsync(
                settings.ProjectPath,
                config.ComponentsDirectory,
                settings.Component,
                overwrite: true);

            AnsiConsole.MarkupLine($"[green]✓[/] {settings.Component} updated successfully!");
        }
        else
        {
            // Update all components
            if (!settings.SkipPrompts)
            {
                if (!AnsiConsole.Confirm("Update all components?"))
                {
                    return 0;
                }
            }

            var installedComponents = componentService.GetInstalledComponents(
                settings.ProjectPath,
                config.ComponentsDirectory);

            await AnsiConsole.Progress()
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask("[blue]Updating components[/]", maxValue: installedComponents.Count);

                    foreach (var component in installedComponents)
                    {
                        task.Increment(1);
                        await componentService.InstallComponentAsync(
                            settings.ProjectPath,
                            config.ComponentsDirectory,
                            component,
                            overwrite: true);
                    }
                });

            AnsiConsole.MarkupLine($"\n[green]✓[/] All components updated successfully!");
        }

        return 0;
    }
}
