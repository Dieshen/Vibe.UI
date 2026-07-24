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
        var incrementalStateTarget = project.Elements(ns + "Target")
            .Single(target => string.Equals((string?)target.Attribute("Name"), "_VibeCssWriteIncrementalState", StringComparison.Ordinal));
        var unownedItemGroup = project.Elements(ns + "ItemGroup")
            .Single(itemGroup => ((string?)itemGroup.Attribute("Condition"))?.Contains(
                "VibeCssOwnStaticWebAsset)' != 'true'",
                StringComparison.Ordinal) == true);
        var generationCommands = generateTarget.Elements(ns + "Exec")
            .Select(exec => (string?)exec.Attribute("Command"))
            .OfType<string>()
            .ToList();
        var includeBaseTrueArgument = generateTarget.Elements(ns + "PropertyGroup")
            .Elements(ns + "_VibeCssIncludeBaseArg")
            .Single(element => ((string?)element.Attribute("Condition"))?.Contains(
                "VibeCssIncludeBase)' == 'true'",
                StringComparison.Ordinal) == true);
        var writeOptionsTask = incrementalStateTarget.Element(ns + "WriteLinesToFile")
            ?? throw new InvalidOperationException("Missing generation options writer.");
        var generationOptions = (string?)writeOptionsTask.Attribute("Lines")
            ?? throw new InvalidOperationException("Missing generation options state.");

        Assert.Contains("$(_VibeCssGenerationOptionsFile)", (string?)generateTarget.Attribute("Inputs"), StringComparison.Ordinal);
        Assert.False(string.IsNullOrWhiteSpace((string?)generateTarget.Attribute("Outputs")));
        Assert.Contains("_VibeCssWriteIncrementalState", (string?)generateTarget.Attribute("DependsOnTargets"), StringComparison.Ordinal);
        Assert.Contains("VibeCssPrefix=$(VibeCssPrefix)", generationOptions, StringComparison.Ordinal);
        Assert.Contains("VibeCssIncludeBase=$(VibeCssIncludeBase)", generationOptions, StringComparison.Ordinal);
        Assert.Contains("VibeCssScanPatterns=$(VibeCssScanPatterns)", generationOptions, StringComparison.Ordinal);
        Assert.Contains("VibeCssFailOnUnknown=$(VibeCssFailOnUnknown)", generationOptions, StringComparison.Ordinal);
        Assert.Contains("VibeCssIgnoredClasses=$(VibeCssIgnoredClasses)", generationOptions, StringComparison.Ordinal);
        Assert.Equal("true", (string?)writeOptionsTask.Attribute("WriteOnlyWhenDifferent"));
        Assert.Equal("BeforeBuild", (string?)cleanupTarget.Attribute("BeforeTargets"));
        Assert.Equal("$(_VibeCssOutputFullPath)", (string?)unownedItemGroup.Element(ns + "Content")?.Attribute("Remove"));
        Assert.Equal("$(_VibeCssOutputFullPath)", (string?)unownedItemGroup.Element(ns + "None")?.Attribute("Remove"));
        Assert.NotEmpty(generationCommands);
        Assert.Equal("--with-base true", includeBaseTrueArgument.Value);
        Assert.All(generationCommands, command =>
        {
            Assert.Contains("$(_VibeCssIncludeBaseArg)", command, StringComparison.Ordinal);
            Assert.Contains("--patterns \"$(VibeCssScanPatterns)\"", command, StringComparison.Ordinal);
            Assert.Contains("$(_VibeCssFailOnUnknownArg)", command, StringComparison.Ordinal);
            Assert.Contains("$(_VibeCssIgnoredClassesArg)", command, StringComparison.Ordinal);
        });
    }

    [Fact]
    public void GenerateTarget_ExecutesPackagedToolArgumentsAndWritesCss()
    {
        var projectPath = CreateProject("Root", executeGenerator: true, failOnUnknown: true);
        var projectDirectory = Path.GetDirectoryName(projectPath)!;
        File.WriteAllText(
            Path.Combine(projectDirectory, "Index.razor"),
            "<div class=\"vibe-flex vibe-ring-2\"></div>");

        var result = RunDotnetMsbuild(projectPath, "VibeCssGenerate");

        Assert.Equal(0, result.ExitCode);
        var css = File.ReadAllText(Path.Combine(projectDirectory, "wwwroot", "css", "Vibe.UI.CSS"));
        Assert.Contains("--vibe-background", css, StringComparison.Ordinal);
        Assert.Contains(".vibe-flex", css, StringComparison.Ordinal);
        Assert.Contains("var(--tw-ring-offset-width, 0px)", css, StringComparison.Ordinal);
    }

    [Fact]
    public void GenerateTarget_StrictFailurePreservesExistingOutput()
    {
        var projectPath = CreateProject("Root", executeGenerator: true, failOnUnknown: true);
        var projectDirectory = Path.GetDirectoryName(projectPath)!;
        var outputPath = Path.Combine(projectDirectory, "wwwroot", "css", "Vibe.UI.CSS");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        File.WriteAllText(outputPath, "existing-output");
        File.SetLastWriteTimeUtc(outputPath, DateTime.UtcNow.AddMinutes(-5));
        File.WriteAllText(
            Path.Combine(projectDirectory, "Index.razor"),
            "<div class=\"vibe-not-a-real-utility\"></div>");

        var result = RunDotnetMsbuild(projectPath, "VibeCssGenerate");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Unknown utility classes found", result.StandardError + result.StandardOutput, StringComparison.Ordinal);
        Assert.Equal("existing-output", File.ReadAllText(outputPath));
    }

    [Fact]
    public void GenerateTarget_EnablingStrictModeInvalidatesIncrementalOutput()
    {
        var projectPath = CreateProject("Root", executeGenerator: true);
        var projectDirectory = Path.GetDirectoryName(projectPath)!;
        var outputPath = Path.Combine(projectDirectory, "wwwroot", "css", "Vibe.UI.CSS");
        File.WriteAllText(
            Path.Combine(projectDirectory, "Index.razor"),
            "<div class=\"vibe-not-a-real-utility\"></div>");

        var nonStrictResult = RunDotnetMsbuild(projectPath, "VibeCssGenerate");
        Assert.Equal(0, nonStrictResult.ExitCode);
        Assert.True(File.Exists(outputPath));

        Thread.Sleep(100);
        var strictResult = RunDotnetMsbuild(
            projectPath,
            "VibeCssGenerate",
            "VibeCssFailOnUnknown=true");

        Assert.NotEqual(0, strictResult.ExitCode);
        Assert.Contains(
            "Unknown utility classes found",
            strictResult.StandardError + strictResult.StandardOutput,
            StringComparison.Ordinal);
    }

    [Fact]
    public void GenerateTarget_ChangingGenerationOptionsInvalidatesIncrementalOutput()
    {
        var projectPath = CreateProject("Root", executeGenerator: true);
        var projectDirectory = Path.GetDirectoryName(projectPath)!;
        var outputPath = Path.Combine(projectDirectory, "wwwroot", "css", "Vibe.UI.CSS");
        File.WriteAllText(
            Path.Combine(projectDirectory, "Index.razor"),
            "<div class=\"vibe-flex\"></div>");

        var withBaseResult = RunDotnetMsbuild(projectPath, "VibeCssGenerate");
        Assert.Equal(0, withBaseResult.ExitCode);
        Assert.Contains("--vibe-background", File.ReadAllText(outputPath), StringComparison.Ordinal);

        Thread.Sleep(100);
        var withoutBaseResult = RunDotnetMsbuild(
            projectPath,
            "VibeCssGenerate",
            "VibeCssIncludeBase=false");

        Assert.Equal(0, withoutBaseResult.ExitCode);
        var css = File.ReadAllText(outputPath);
        Assert.DoesNotContain("--vibe-background", css, StringComparison.Ordinal);
        Assert.Contains(".vibe-flex", css, StringComparison.Ordinal);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    private string CreateProject(
        string staticWebAssetProjectMode,
        bool useRelativePaths = false,
        bool executeGenerator = false,
        bool failOnUnknown = false)
    {
        Directory.CreateDirectory(_tempRoot);

        var repoRoot = FindRepositoryRoot();
        var targetsPath = Path.Combine(repoRoot, "src", "Vibe.UI.CSS", "Build", "Vibe.UI.CSS.targets");
        var projectPath = Path.Combine(_tempRoot, $"{staticWebAssetProjectMode}.csproj");
        var relativePathProperties = useRelativePaths
            ? "<VibeCssOutput>wwwroot/css/Vibe.UI.CSS</VibeCssOutput><VibeCssScanRoot>.</VibeCssScanRoot>"
            : string.Empty;
        var generatorProperties = executeGenerator
            ? $$"""
                <VibeCssToolPath>{{EscapeXml(typeof(VibeCss).Assembly.Location)}}</VibeCssToolPath>
                <VibeCssOutput>wwwroot/css/Vibe.UI.CSS</VibeCssOutput>
                <VibeCssScanRoot>.</VibeCssScanRoot>
                <VibeCssScanPatterns>*.razor</VibeCssScanPatterns>
                <VibeCssIncludeBase>true</VibeCssIncludeBase>
                <VibeCssFailOnUnknown>{{failOnUnknown.ToString().ToLowerInvariant()}}</VibeCssFailOnUnknown>
                <VibeCssFailOnMissingTool>true</VibeCssFailOnMissingTool>
              """
            : string.Empty;

        var projectContent = $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
                <StaticWebAssetProjectMode>{{staticWebAssetProjectMode}}</StaticWebAssetProjectMode>
                {{relativePathProperties}}
                {{generatorProperties}}
              </PropertyGroup>
              <Import Project="{{EscapeXml(targetsPath)}}" />
            </Project>
            """;

        File.WriteAllText(projectPath, projectContent);
        return projectPath;
    }

    private static string EscapeXml(string value) =>
        value.Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

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

    private static ProcessResult RunDotnetMsbuild(
        string projectPath,
        string target,
        params string[] properties)
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
        startInfo.ArgumentList.Add("-nologo");
        startInfo.ArgumentList.Add("-verbosity:minimal");
        startInfo.ArgumentList.Add($"-target:{target}");
        foreach (var property in properties)
        {
            startInfo.ArgumentList.Add($"-property:{property}");
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet.");
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        process.WaitForExit();

        return new ProcessResult(
            process.ExitCode,
            stdoutTask.GetAwaiter().GetResult(),
            stderrTask.GetAwaiter().GetResult());
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
