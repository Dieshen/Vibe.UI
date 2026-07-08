using Vibe.UI.CLI.Models;

namespace Vibe.UI.CLI.Services;

public static class ProjectSetupGuidance
{
    public static IReadOnlyList<string> BuildNextSteps(
        ProjectTopology topology,
        string installProjectPath,
        bool withCss)
    {
        if (topology.IsBlazorWebApp && topology.HasClientProject && topology.HasServerProject)
        {
            return BuildWebAppNextSteps(topology, installProjectPath, withCss);
        }

        if (topology.Kind == BlazorProjectKind.BlazorWebAssembly)
        {
            return BuildStandaloneWebAssemblyNextSteps(withCss);
        }

        return BuildDefaultBlazorNextSteps(withCss);
    }

    private static IReadOnlyList<string> BuildWebAppNextSteps(
        ProjectTopology topology,
        string installProjectPath,
        bool withCss)
    {
        var steps = new List<string>
        {
            $"Register Vibe.UI in server project {ProjectLabel(topology.ServerProjectFile, "Program.cs")}: builder.Services.AddVibeUI();",
            $"Register Vibe.UI in client project {ProjectLabel(topology.ClientProjectFile, "Program.cs")}: builder.Services.AddVibeUI();",
            "Ensure the server Program.cs chains AddInteractiveWebAssemblyComponents() on AddRazorComponents().",
            $"Ensure the server endpoint chains AddInteractiveWebAssemblyRenderMode().AddAdditionalAssemblies(typeof({topology.ClientNamespace ?? "Client"}._Imports).Assembly)."
        };

        if (withCss)
        {
            steps.Add("In the server App.razor head, reference the generated stylesheet with <link rel=\"stylesheet\" href=\"@Assets[\"css/Vibe.UI.CSS\"]\" />.");
            steps.Add("Run vibe css --watch during development, or rely on build-time CSS generation.");
        }
        else
        {
            steps.Add("In the server App.razor head, reference <link rel=\"stylesheet\" href=\"@Assets[\"css/vibe-base.css\"]\" /> and <link rel=\"stylesheet\" href=\"@Assets[\"css/vibe-utilities.css\"]\" />.");
        }

        if (!string.IsNullOrWhiteSpace(topology.ClientProjectPath)
            && PathsEqual(installProjectPath, topology.ClientProjectPath))
        {
            steps.Add($"Run future component commands from {topology.ClientProjectPath} or pass --path \"{topology.ClientProjectPath}\".");
        }

        steps.Add("Add <ThemeToggle /> to your layout for light/dark mode.");
        steps.Add("Run vibe add button to add your first component.");
        steps.Add("Run vibe list to see all available components.");

        return steps;
    }

    private static IReadOnlyList<string> BuildStandaloneWebAssemblyNextSteps(bool withCss)
    {
        var steps = new List<string>
        {
            "Register Vibe.UI in Program.cs: builder.Services.AddVibeUI();"
        };

        if (withCss)
        {
            steps.Add("Add <link href=\"css/Vibe.UI.CSS\" rel=\"stylesheet\" /> to wwwroot/index.html.");
            steps.Add("Run vibe css --watch during development, or rely on build-time CSS generation.");
        }
        else
        {
            steps.Add("Add <link href=\"css/vibe-base.css\" rel=\"stylesheet\" /> and <link href=\"css/vibe-utilities.css\" rel=\"stylesheet\" /> to wwwroot/index.html.");
        }

        steps.Add("Add <ThemeToggle /> to your layout for light/dark mode.");
        steps.Add("Run vibe add button to add your first component.");
        steps.Add("Run vibe list to see all available components.");

        return steps;
    }

    private static IReadOnlyList<string> BuildDefaultBlazorNextSteps(bool withCss)
    {
        var steps = new List<string>
        {
            "Register Vibe.UI in Program.cs: builder.Services.AddVibeUI();"
        };

        if (withCss)
        {
            steps.Add("Add <link href=\"css/Vibe.UI.CSS\" rel=\"stylesheet\" /> to your host page or root component.");
            steps.Add("Run vibe css --watch during development, or rely on build-time CSS generation.");
        }
        else
        {
            steps.Add("Add @import 'css/vibe-base.css'; and @import 'css/vibe-utilities.css'; to your app.css, host page, or root component.");
        }

        steps.Add("Add <ThemeToggle /> to your layout for light/dark mode.");
        steps.Add("Run vibe add button to add your first component.");
        steps.Add("Run vibe list to see all available components.");

        return steps;
    }

    private static string ProjectLabel(string? projectFile, string fallback) =>
        string.IsNullOrWhiteSpace(projectFile)
            ? fallback
            : Path.GetFileNameWithoutExtension(projectFile);

    private static bool PathsEqual(string left, string right) =>
        string.Equals(Path.GetFullPath(left), Path.GetFullPath(right), StringComparison.OrdinalIgnoreCase);
}
