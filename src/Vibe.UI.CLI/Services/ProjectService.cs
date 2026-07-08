using System.Xml.Linq;
using Vibe.UI.CLI.Infrastructure;
using Vibe.UI.CLI.Models;

namespace Vibe.UI.CLI.Services;

public class ProjectService
{
    public async Task<string> DetectProjectTypeAsync(string projectPath)
    {
        var topology = await DetectProjectTopologyAsync(projectPath);
        return topology.DisplayName;
    }

    public async Task<ProjectTopology> DetectProjectTopologyAsync(string projectPath)
    {
        var rootPath = Path.GetFullPath(projectPath);
        var projectFiles = DiscoverProjectFiles(rootPath, includeChildDirectories: true);
        var candidates = await LoadProjectCandidatesAsync(projectFiles);

        if (ShouldScanSiblingProjects(rootPath, candidates))
        {
            var siblingRoot = GetSiblingSearchRoot(rootPath);
            if (siblingRoot != null)
            {
                var siblingFiles = DiscoverProjectFiles(siblingRoot, includeChildDirectories: true);
                candidates.AddRange(await LoadProjectCandidatesAsync(siblingFiles));
            }
        }

        await AddReferencedProjectCandidatesAsync(candidates);
        candidates = candidates
            .DistinctBy(candidate => candidate.ProjectFile, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (candidates.Count == 0)
        {
            return ProjectTopology.Unknown(rootPath);
        }

        var primary = SelectPrimaryCandidate(rootPath, candidates);
        var server = FindWebAppServer(candidates);
        var client = FindWebAppClient(candidates, server);

        if (server != null && client != null)
        {
            var primaryIsClient = primary != null && PathsEqual(primary.ProjectPath, client.ProjectPath);
            var kind = primaryIsClient
                ? BlazorProjectKind.BlazorWebAppClient
                : BlazorProjectKind.BlazorWebApp;

            return new ProjectTopology(
                kind,
                primaryIsClient ? "Blazor Web App Client" : "Blazor Web App",
                rootPath,
                primary?.ProjectPath ?? server.ProjectPath,
                primary?.ProjectFile ?? server.ProjectFile,
                server.ProjectPath,
                server.ProjectFile,
                client.ProjectPath,
                client.ProjectFile,
                client.ProjectNamespace);
        }

        var selected = primary ?? candidates[0];

        if (selected.IsWebAssemblyProject)
        {
            return new ProjectTopology(
                BlazorProjectKind.BlazorWebAssembly,
                "Blazor WebAssembly",
                rootPath,
                selected.ProjectPath,
                selected.ProjectFile,
                null,
                null,
                selected.ProjectPath,
                selected.ProjectFile,
                selected.ProjectNamespace);
        }

        if (selected.IsBlazorServerProject)
        {
            return new ProjectTopology(
                BlazorProjectKind.BlazorServer,
                "Blazor Server",
                rootPath,
                selected.ProjectPath,
                selected.ProjectFile,
                selected.ProjectPath,
                selected.ProjectFile,
                null,
                null,
                null);
        }

        return new ProjectTopology(
            BlazorProjectKind.Blazor,
            "Blazor",
            rootPath,
            selected.ProjectPath,
            selected.ProjectFile,
            null,
            null,
            null,
            null,
            null);
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
            new XAttribute("Version", CliVersion.Current));

        itemGroup.Add(packageRef);

        // Save
        doc.Save(csprojPath);
        await Task.CompletedTask;
    }

    public async Task CopyThemeFilesAsync(string projectPath, string theme)
    {
        var wwwrootPath = Path.Combine(projectPath, "wwwroot");
        Directory.CreateDirectory(wwwrootPath);

        var cssPath = Path.Combine(wwwrootPath, "Vibe.UI.CSS");

        var cssContent = theme switch
        {
            "light" => GetLightThemeCss(),
            "dark" => GetDarkThemeCss(),
            "both" => GetBothThemesCss(),
            _ => GetLightThemeCss()
        };

        await File.WriteAllTextAsync(cssPath, cssContent);
    }

    private static List<string> DiscoverProjectFiles(string projectPath, bool includeChildDirectories)
    {
        var projectFiles = new List<string>();

        if (File.Exists(projectPath))
        {
            if (Path.GetExtension(projectPath).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                projectFiles.Add(Path.GetFullPath(projectPath));
            }

            return projectFiles;
        }

        if (!Directory.Exists(projectPath))
        {
            return projectFiles;
        }

        AddProjectFilesFromDirectory(projectPath, projectFiles);

        if (!includeChildDirectories)
        {
            return projectFiles;
        }

        foreach (var directory in Directory.EnumerateDirectories(projectPath))
        {
            if (ShouldSkipDirectory(directory))
            {
                continue;
            }

            AddProjectFilesFromDirectory(directory, projectFiles);
        }

        return projectFiles;
    }

    private static void AddProjectFilesFromDirectory(string directory, List<string> projectFiles)
    {
        try
        {
            foreach (var projectFile in Directory.EnumerateFiles(directory, "*.csproj", SearchOption.TopDirectoryOnly))
            {
                projectFiles.Add(Path.GetFullPath(projectFile));
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Project discovery is best-effort across sibling folders.
        }
        catch (IOException)
        {
            // Ignore transient file-system entries while probing project topology.
        }
    }

    private static bool ShouldSkipDirectory(string directory)
    {
        var name = Path.GetFileName(directory);
        return name.Equals("bin", StringComparison.OrdinalIgnoreCase)
            || name.Equals("obj", StringComparison.OrdinalIgnoreCase)
            || name.Equals(".git", StringComparison.OrdinalIgnoreCase)
            || name.Equals("node_modules", StringComparison.OrdinalIgnoreCase)
            || name.Equals("TestResults", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<List<ProjectCandidate>> LoadProjectCandidatesAsync(IEnumerable<string> projectFiles)
    {
        var candidates = new List<ProjectCandidate>();
        foreach (var projectFile in projectFiles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var candidate = await ProjectCandidate.LoadAsync(projectFile);
            if (candidate != null)
            {
                candidates.Add(candidate);
            }
        }

        return candidates;
    }

    private static bool ShouldScanSiblingProjects(string rootPath, IReadOnlyCollection<ProjectCandidate> candidates)
    {
        if (candidates.Count != 1)
        {
            return false;
        }

        var candidate = candidates.First();
        return candidate.ProjectName.EndsWith(".Client", StringComparison.OrdinalIgnoreCase)
            || rootPath.EndsWith(".Client", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetSiblingSearchRoot(string rootPath)
    {
        var directory = File.Exists(rootPath)
            ? Path.GetDirectoryName(rootPath)
            : rootPath;

        if (string.IsNullOrWhiteSpace(directory))
        {
            return null;
        }

        return Directory.GetParent(directory)?.FullName;
    }

    private static async Task AddReferencedProjectCandidatesAsync(List<ProjectCandidate> candidates)
    {
        var knownProjectFiles = new HashSet<string>(
            candidates.Select(candidate => candidate.ProjectFile),
            StringComparer.OrdinalIgnoreCase);

        foreach (var projectReference in candidates.SelectMany(candidate => candidate.ProjectReferences).ToArray())
        {
            if (!File.Exists(projectReference) || !knownProjectFiles.Add(projectReference))
            {
                continue;
            }

            var referencedCandidate = await ProjectCandidate.LoadAsync(projectReference);
            if (referencedCandidate != null)
            {
                candidates.Add(referencedCandidate);
            }
        }
    }

    private static ProjectCandidate? SelectPrimaryCandidate(string rootPath, IReadOnlyList<ProjectCandidate> candidates)
    {
        if (File.Exists(rootPath))
        {
            return candidates.FirstOrDefault(candidate => PathsEqual(candidate.ProjectFile, rootPath));
        }

        var directProject = candidates.FirstOrDefault(candidate => PathsEqual(candidate.ProjectPath, rootPath));
        if (directProject != null)
        {
            return directProject;
        }

        if (candidates.Count == 1)
        {
            return candidates[0];
        }

        return candidates.FirstOrDefault(candidate => candidate.IsWebAppServerProject)
            ?? candidates.FirstOrDefault(candidate => !candidate.ProjectName.Contains("Test", StringComparison.OrdinalIgnoreCase))
            ?? candidates[0];
    }

    private static ProjectCandidate? FindWebAppServer(IReadOnlyList<ProjectCandidate> candidates)
    {
        var clientProjectFiles = candidates
            .Where(candidate => candidate.IsWebAssemblyProject)
            .Select(candidate => candidate.ProjectFile)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return candidates.FirstOrDefault(candidate =>
                candidate.IsWebAppServerProject
                || candidate.ProjectReferences.Any(reference => clientProjectFiles.Contains(reference)))
            ?? candidates.FirstOrDefault(candidate => candidate.IsWebAppServerProject);
    }

    private static ProjectCandidate? FindWebAppClient(IReadOnlyList<ProjectCandidate> candidates, ProjectCandidate? server)
    {
        if (server != null)
        {
            foreach (var projectReference in server.ProjectReferences)
            {
                var referencedClient = candidates.FirstOrDefault(candidate =>
                    candidate.IsWebAssemblyProject && PathsEqual(candidate.ProjectFile, projectReference));

                if (referencedClient != null)
                {
                    return referencedClient;
                }
            }

            var expectedClientName = $"{server.ProjectName}.Client";
            var namedClient = candidates.FirstOrDefault(candidate =>
                candidate.IsWebAssemblyProject
                && candidate.ProjectName.Equals(expectedClientName, StringComparison.OrdinalIgnoreCase));

            if (namedClient != null)
            {
                return namedClient;
            }
        }

        return candidates.FirstOrDefault(candidate =>
                candidate.IsWebAssemblyProject
                && candidate.ProjectName.EndsWith(".Client", StringComparison.OrdinalIgnoreCase))
            ?? candidates.FirstOrDefault(candidate => candidate.IsWebAssemblyProject);
    }

    private static bool PathsEqual(string left, string right) =>
        string.Equals(Path.GetFullPath(left), Path.GetFullPath(right), StringComparison.OrdinalIgnoreCase);

    private sealed class ProjectCandidate
    {
        private ProjectCandidate(
            string projectFile,
            string sdk,
            IReadOnlySet<string> packageReferences,
            IReadOnlyList<string> projectReferences,
            string projectNamespace,
            string programContent)
        {
            ProjectFile = projectFile;
            Sdk = sdk;
            PackageReferences = packageReferences;
            ProjectReferences = projectReferences;
            ProjectNamespace = projectNamespace;
            ProgramContent = programContent;
        }

        public string ProjectFile { get; }

        public string ProjectPath => Path.GetDirectoryName(ProjectFile) ?? Directory.GetCurrentDirectory();

        public string ProjectName => Path.GetFileNameWithoutExtension(ProjectFile);

        public string ProjectNamespace { get; }

        public string Sdk { get; }

        public IReadOnlySet<string> PackageReferences { get; }

        public IReadOnlyList<string> ProjectReferences { get; }

        public string ProgramContent { get; }

        public bool IsWebSdk => Sdk.Contains("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase);

        public bool IsWebAssemblyProject =>
            Sdk.Contains("Microsoft.NET.Sdk.BlazorWebAssembly", StringComparison.OrdinalIgnoreCase)
            || (HasPackage("Microsoft.AspNetCore.Components.WebAssembly")
                && !HasPackage("Microsoft.AspNetCore.Components.WebAssembly.Server"));

        public bool IsWebAppServerProject =>
            IsWebSdk
            && (HasPackage("Microsoft.AspNetCore.Components.WebAssembly.Server")
                || ProgramContent.Contains("AddInteractiveWebAssemblyComponents", StringComparison.Ordinal)
                || ProgramContent.Contains("AddInteractiveWebAssemblyRenderMode", StringComparison.Ordinal));

        public bool IsBlazorServerProject =>
            HasPackage("Microsoft.AspNetCore.Components.Web")
            || ProgramContent.Contains("AddInteractiveServerComponents", StringComparison.Ordinal)
            || (IsWebSdk && ProgramContent.Contains("AddRazorComponents", StringComparison.Ordinal));

        public static async Task<ProjectCandidate?> LoadAsync(string projectFile)
        {
            try
            {
                var projectFullPath = Path.GetFullPath(projectFile);
                var projectContent = await File.ReadAllTextAsync(projectFullPath);
                var projectDirectory = Path.GetDirectoryName(projectFullPath) ?? Directory.GetCurrentDirectory();
                var projectName = Path.GetFileNameWithoutExtension(projectFullPath);
                var sdk = string.Empty;
                var packageReferences = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var projectReferences = new List<string>();
                var projectNamespace = ToNamespace(projectName);

                try
                {
                    var doc = XDocument.Parse(projectContent);
                    sdk = doc.Root?.Attribute("Sdk")?.Value ?? string.Empty;

                    foreach (var packageReference in doc.Descendants()
                        .Where(element => element.Name.LocalName == "PackageReference"))
                    {
                        var include = packageReference.Attribute("Include")?.Value;
                        if (!string.IsNullOrWhiteSpace(include))
                        {
                            packageReferences.Add(include);
                        }
                    }

                    foreach (var projectReference in doc.Descendants()
                        .Where(element => element.Name.LocalName == "ProjectReference"))
                    {
                        var include = projectReference.Attribute("Include")?.Value;
                        if (!string.IsNullOrWhiteSpace(include))
                        {
                            projectReferences.Add(Path.GetFullPath(Path.Combine(projectDirectory, include)));
                        }
                    }

                    projectNamespace = ReadProjectProperty(doc, "RootNamespace")
                        ?? ReadProjectProperty(doc, "AssemblyName")
                        ?? projectNamespace;
                }
                catch (System.Xml.XmlException)
                {
                    // Some SDK-style projects can contain MSBuild constructs that are not useful
                    // for detection. Fall back to text/program signals in that case.
                }

                var programPath = Path.Combine(projectDirectory, "Program.cs");
                var programContent = File.Exists(programPath)
                    ? await File.ReadAllTextAsync(programPath)
                    : string.Empty;

                return new ProjectCandidate(
                    projectFullPath,
                    sdk,
                    packageReferences,
                    projectReferences,
                    ToNamespace(projectNamespace),
                    programContent);
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        private static string? ReadProjectProperty(XDocument doc, string propertyName)
        {
            var value = doc.Descendants()
                .FirstOrDefault(element => element.Name.LocalName == propertyName)
                ?.Value;

            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string ToNamespace(string value) =>
            value.Replace('-', '_').Replace(' ', '_');

        private bool HasPackage(string packageName) =>
            PackageReferences.Contains(packageName);

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

