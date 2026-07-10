using System.Diagnostics;
using System.Xml.Linq;

namespace Vibe.UI.CSS.Tests.Build;

public sealed class VibeCssTargetsTests : IDisposable
{
    private readonly string _tempRoot = Path.Combine(
        Path.GetTempPath(),
        $"vibe-css-target-tests-{Guid.NewGuid():N}");

    [Fact]
    public void DefaultStaticWebAssetMode_DisablesCssOwnership()
    {
        var projectPath = CreateProject("Default");

        var propertyValue = RunDotnetMsbuildGetProperty(projectPath, "VibeCssOwnStaticWebAsset");

        Assert.Equal("false", propertyValue);
    }

    [Fact]
    public void RootStaticWebAssetMode_KeepsCssOwnership()
    {
        var projectPath = CreateProject("Root", useRelativePaths: true);

        var propertyValue = RunDotnetMsbuildGetProperty(projectPath, "VibeCssOwnStaticWebAsset");

        Assert.Equal("true", propertyValue);
    }

    [Fact]
    public void RelativeOutputAndScanRoot_AreResolvedFromProjectDirectory()
    {
        var projectPath = CreateProject("Root");
        var projectDirectory = Path.GetDirectoryName(projectPath)!;

        var outputPath = RunDotnetMsbuildGetProperty(projectPath, "_VibeCssOutputFullPath");
        var scanRoot = RunDotnetMsbuildGetProperty(projectPath, "_VibeCssScanRootFullPath");

        Assert.Equal(
            Path.GetFullPath(Path.Combine(projectDirectory, "wwwroot", "css", "Vibe.UI.CSS")),
            outputPath);
        Assert.Equal(projectDirectory, scanRoot);
    }

    [Fact]
    public void TargetsFile_DeclaresIncrementalGenerationAndCleanup()
    {
        var repoRoot = FindRepositoryRoot();
        var targetsPath = Path.Combine(repoRoot, "src", "Vibe.UI.CSS", "Build", "Vibe.UI.CSS.targets");
        var document = XDocument.Load(targetsPath);
        var project = document.Root ?? throw new InvalidOperationException("Missing targets root.");
        var ns = project.Name.Namespace;

        var generateTarget = project.Elements(ns + "Target")
            .Single(target => string.Equals((string?)target.Attribute("Name"), "VibeCssGenerate", StringComparison.Ordinal));
        var cleanupTarget = project.Elements(ns + "Target")
            .Single(target => string.Equals((string?)target.Attribute("Name"), "VibeCssRemoveUnownedOutput", StringComparison.Ordinal));
        var unownedItemGroup = project.Elements(ns + "ItemGroup")
            .Single(itemGroup => ((string?)itemGroup.Attribute("Condition"))?.Contains(
                "VibeCssOwnStaticWebAsset)' != 'true'",
                StringComparison.Ordinal) == true);

        Assert.False(string.IsNullOrWhiteSpace((string?)generateTarget.Attribute("Inputs")));
        Assert.False(string.IsNullOrWhiteSpace((string?)generateTarget.Attribute("Outputs")));
        Assert.Equal("BeforeBuild", (string?)cleanupTarget.Attribute("BeforeTargets"));
        Assert.Equal("$(_VibeCssOutputFullPath)", (string?)unownedItemGroup.Element(ns + "Content")?.Attribute("Remove"));
        Assert.Equal("$(_VibeCssOutputFullPath)", (string?)unownedItemGroup.Element(ns + "None")?.Attribute("Remove"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    private string CreateProject(string staticWebAssetProjectMode, bool useRelativePaths = false)
    {
        Directory.CreateDirectory(_tempRoot);

        var repoRoot = FindRepositoryRoot();
        var targetsPath = Path.Combine(repoRoot, "src", "Vibe.UI.CSS", "Build", "Vibe.UI.CSS.targets");
        var projectPath = Path.Combine(_tempRoot, $"{staticWebAssetProjectMode}.csproj");
        var relativePathProperties = useRelativePaths
            ? "<VibeCssOutput>wwwroot/css/Vibe.UI.CSS</VibeCssOutput><VibeCssScanRoot>.</VibeCssScanRoot>"
            : string.Empty;

        var projectContent = $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
                <StaticWebAssetProjectMode>{{staticWebAssetProjectMode}}</StaticWebAssetProjectMode>
                {{relativePathProperties}}
              </PropertyGroup>
              <Import Project="{{targetsPath}}" />
            </Project>
            """;

        File.WriteAllText(projectPath, projectContent);
        return projectPath;
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Vibe.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate Vibe.sln from the test output directory.");
    }

    private static string RunDotnetMsbuildGetProperty(string projectPath, string propertyName)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("msbuild");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add($"-getProperty:{propertyName}");

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet.");

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Xunit.Sdk.XunitException(
                $"dotnet msbuild failed with exit code {process.ExitCode}.{Environment.NewLine}STDOUT:{Environment.NewLine}{stdout}{Environment.NewLine}STDERR:{Environment.NewLine}{stderr}");
        }

        return stdout.Trim();
    }
}
