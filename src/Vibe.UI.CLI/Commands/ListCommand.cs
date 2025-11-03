using Spectre.Console;
using Spectre.Console.Cli;
using Vibe.UI.CLI.Services;

namespace Vibe.UI.CLI.Commands;

public class ListCommand : Command
{
    public override int Execute(CommandContext context)
    {
        var componentService = new ComponentService();
        var components = componentService.GetAvailableComponents();

        AnsiConsole.MarkupLine("[blue]Available Vibe.UI Components:[/]\n");

        // Group by category
        var grouped = components.GroupBy(c => c.Category).OrderBy(g => g.Key);

        foreach (var group in grouped)
        {
            AnsiConsole.MarkupLine($"[yellow]{group.Key}[/]");

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("Component")
                .AddColumn("Description");

            foreach (var component in group.OrderBy(c => c.Name))
            {
                table.AddRow(
                    $"[green]{component.Name}[/]",
                    component.Description ?? "");
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }

        AnsiConsole.MarkupLine($"[grey]Total: {components.Count} components available[/]");
        // Escape the square brackets in the usage example to prevent Spectre.Console from parsing them as markup
        AnsiConsole.MarkupLine($"\n[blue]Usage:[/] vibe add [[component-name]]");

        return 0;
    }
}
