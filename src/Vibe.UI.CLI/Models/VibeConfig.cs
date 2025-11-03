namespace Vibe.UI.CLI.Models;

public class VibeConfig
{
    public string ProjectType { get; set; } = "Blazor";
    public string Theme { get; set; } = "light";
    public string ComponentsDirectory { get; set; } = "Components";
    public bool CssVariables { get; set; } = true;
    public string? Tailwind { get; set; }
    public Dictionary<string, string>? Aliases { get; set; }
}
