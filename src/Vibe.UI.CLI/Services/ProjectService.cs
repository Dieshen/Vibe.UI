using System.Xml.Linq;

namespace Vibe.UI.CLI.Services;

public class ProjectService
{
    public async Task<string> DetectProjectTypeAsync(string projectPath)
    {
        var csprojFiles = Directory.GetFiles(projectPath, "*.csproj");

        if (csprojFiles.Length == 0)
            return "Unknown";

        var csprojContent = await File.ReadAllTextAsync(csprojFiles[0]);

        // Check WebAssembly first since it contains "Components.Web" as a substring
        if (csprojContent.Contains("Microsoft.AspNetCore.Components.WebAssembly"))
            return "Blazor WebAssembly";

        if (csprojContent.Contains("Microsoft.AspNetCore.Components.Web"))
            return "Blazor Server";

        return "Blazor";
    }

    public async Task AddPackageReferenceAsync(string projectPath, string packageName)
    {
        var csprojFiles = Directory.GetFiles(projectPath, "*.csproj");

        if (csprojFiles.Length == 0)
            return;

        var csprojPath = csprojFiles[0];
        var doc = XDocument.Load(csprojPath);

        // Check if package already exists
        var existingPackage = doc.Descendants("PackageReference")
            .FirstOrDefault(p => p.Attribute("Include")?.Value == packageName);

        if (existingPackage != null)
            return; // Already exists

        // Find or create ItemGroup
        var itemGroup = doc.Descendants("ItemGroup")
            .FirstOrDefault(ig => ig.Elements("PackageReference").Any());

        if (itemGroup == null)
        {
            itemGroup = new XElement("ItemGroup");
            doc.Root?.Add(itemGroup);
        }

        // Add package reference
        var packageRef = new XElement("PackageReference",
            new XAttribute("Include", packageName),
            new XAttribute("Version", "1.0.0"));

        itemGroup.Add(packageRef);

        // Save
        doc.Save(csprojPath);
        await Task.CompletedTask;
    }

    public async Task CopyThemeFilesAsync(string projectPath, string theme)
    {
        var wwwrootPath = Path.Combine(projectPath, "wwwroot");
        Directory.CreateDirectory(wwwrootPath);

        var cssPath = Path.Combine(wwwrootPath, "vibe.css");

        var cssContent = theme switch
        {
            "light" => GetLightThemeCss(),
            "dark" => GetDarkThemeCss(),
            "both" => GetBothThemesCss(),
            _ => GetLightThemeCss()
        };

        await File.WriteAllTextAsync(cssPath, cssContent);
    }

    private string GetLightThemeCss()
    {
        return @":root {
  --vibe-background: #ffffff;
  --vibe-foreground: #111111;
  --vibe-primary: #0066cc;
  --vibe-primary-foreground: #ffffff;
  --vibe-secondary: #f4f4f4;
  --vibe-secondary-foreground: #333333;
  --vibe-accent: #ff4500;
  --vibe-accent-foreground: #ffffff;
  --vibe-muted: #f1f1f1;
  --vibe-muted-foreground: #666666;
  --vibe-card: #ffffff;
  --vibe-card-foreground: #111111;
  --vibe-popover: #ffffff;
  --vibe-popover-foreground: #111111;
  --vibe-border: #e2e2e2;
  --vibe-input: #ffffff;
  --vibe-ring: #0066cc;
  --vibe-radius: 0.5rem;
  --vibe-destructive: #dc2626;
  --vibe-destructive-foreground: #ffffff;
}";
    }

    private string GetDarkThemeCss()
    {
        return @":root {
  --vibe-background: #1a1a1a;
  --vibe-foreground: #ffffff;
  --vibe-primary: #0099ff;
  --vibe-primary-foreground: #ffffff;
  --vibe-secondary: #2a2a2a;
  --vibe-secondary-foreground: #f7f7f7;
  --vibe-accent: #ff4500;
  --vibe-accent-foreground: #ffffff;
  --vibe-muted: #313131;
  --vibe-muted-foreground: #a0a0a0;
  --vibe-card: #222222;
  --vibe-card-foreground: #ffffff;
  --vibe-popover: #222222;
  --vibe-popover-foreground: #ffffff;
  --vibe-border: #404040;
  --vibe-input: #2a2a2a;
  --vibe-ring: #0099ff;
  --vibe-radius: 0.5rem;
  --vibe-destructive: #dc2626;
  --vibe-destructive-foreground: #ffffff;
}";
    }

    private string GetBothThemesCss()
    {
        return GetLightThemeCss() + "\n\n.dark {\n" + GetDarkThemeCss().Replace(":root", "  ") + "\n}";
    }
}
