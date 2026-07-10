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

    public Task<int> ExecuteAsync(CommandContext context, Settings settings) =>
        ExecuteAsync(context, settings, CancellationToken.None);

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var configService = new ConfigService();
        var componentService = new ComponentService();
        var projectService = new ProjectService();
        var requestedProjectPath = Path.GetFullPath(settings.ProjectPath);
        var projectPath = await projectService.ResolveInitializedProjectPathAsync(requestedProjectPath);

        if (!PathsEqual(requestedProjectPath, projectPath))
        {
            AnsiConsole.WriteLine($"Using initialized project: {projectPath}");
        }

        var config = await configService.LoadConfigAsync(projectPath);
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
                projectPath,
                config.ComponentsDirectory,
                settings.Component,
                overwrite: true);
            await RazorImportsService.EnsureVibeImportsAsync(projectPath, includeComponents: true);

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
                projectPath,
                config.ComponentsDirectory);

            await AnsiConsole.Progress()
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask("[blue]Updating components[/]", maxValue: installedComponents.Count);

                    foreach (var component in installedComponents)
                    {
                        task.Increment(1);
                        await componentService.InstallComponentAsync(
                            projectPath,
                            config.ComponentsDirectory,
                            component,
                            overwrite: true);
                    }
                });

            if (installedComponents.Count > 0)
            {
                await RazorImportsService.EnsureVibeImportsAsync(projectPath, includeComponents: true);
            }

            AnsiConsole.MarkupLine($"\n[green]✓[/] All components updated successfully!");
        }

        return 0;
    }

    private static bool PathsEqual(string left, string right) =>
        string.Equals(Path.GetFullPath(left), Path.GetFullPath(right), StringComparison.OrdinalIgnoreCase);
}
