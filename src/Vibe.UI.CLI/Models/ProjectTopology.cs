namespace Vibe.UI.CLI.Models;

public enum BlazorProjectKind
{
    Unknown,
    Blazor,
    BlazorServer,
    BlazorWebAssembly,
    BlazorWebApp,
    BlazorWebAppClient
}

public sealed record ProjectTopology(
    BlazorProjectKind Kind,
    string DisplayName,
    string RootPath,
    string? ProjectPath,
    string? ProjectFile,
    string? ServerProjectPath,
    string? ServerProjectFile,
    string? ClientProjectPath,
    string? ClientProjectFile,
    string? ClientNamespace)
{
    public bool HasServerProject => !string.IsNullOrWhiteSpace(ServerProjectPath);

    public bool HasClientProject => !string.IsNullOrWhiteSpace(ClientProjectPath);

    public bool IsBlazorWebApp =>
        Kind is BlazorProjectKind.BlazorWebApp or BlazorProjectKind.BlazorWebAppClient;

    public static ProjectTopology Unknown(string rootPath) =>
        new(
            BlazorProjectKind.Unknown,
            "Unknown",
            rootPath,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
}
