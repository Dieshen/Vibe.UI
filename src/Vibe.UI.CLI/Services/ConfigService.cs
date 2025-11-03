using System.Text.Json;
using Vibe.UI.CLI.Models;

namespace Vibe.UI.CLI.Services;

public class ConfigService
{
    private const string ConfigFileName = "vibe.json";

    public bool ConfigExists(string projectPath)
    {
        var configPath = Path.Combine(projectPath, ConfigFileName);
        return File.Exists(configPath);
    }

    public async Task<VibeConfig?> LoadConfigAsync(string projectPath)
    {
        var configPath = Path.Combine(projectPath, ConfigFileName);

        if (!File.Exists(configPath))
            return null;

        var json = await File.ReadAllTextAsync(configPath);

        // Use the same JsonSerializerOptions as SaveConfigAsync for consistent serialization/deserialization
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Deserialize<VibeConfig>(json, options);
    }

    public async Task SaveConfigAsync(string projectPath, VibeConfig config)
    {
        var configPath = Path.Combine(projectPath, ConfigFileName);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(config, options);
        await File.WriteAllTextAsync(configPath, json);
    }
}
