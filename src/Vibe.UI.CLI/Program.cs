using Spectre.Console;
using Spectre.Console.Cli;
using Vibe.UI.CLI.Commands;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("vibe");

    config.AddCommand<InitCommand>("init")
        .WithDescription("Initialize Vibe.UI in your project")
        .WithExample(new[] { "init" });

    config.AddCommand<AddCommand>("add")
        .WithDescription("Add a component to your project")
        .WithExample(new[] { "add", "button" })
        .WithExample(new[] { "add", "dialog" });

    config.AddCommand<ListCommand>("list")
        .WithDescription("List all available components")
        .WithExample(new[] { "list" });

    config.AddCommand<UpdateCommand>("update")
        .WithDescription("Update Vibe.UI components")
        .WithExample(new[] { "update" });
});

// Display banner
AnsiConsole.Write(
    new FigletText("Vibe.UI")
        .Centered()
        .Color(Color.Blue));

AnsiConsole.MarkupLine($"[grey]v1.0.0[/]\n");

return await app.RunAsync(args);
