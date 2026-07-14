namespace Vibe.UI.CSS.Tests;

public sealed class StrictGenerationTests : IDisposable
{
    private readonly string _tempRoot = Path.Combine(
        Path.GetTempPath(),
        $"vibe-css-strict-tests-{Guid.NewGuid():N}");

    [Fact]
    public void Generate_FailOnUnknown_DoesNotOverwriteOutput()
    {
        Directory.CreateDirectory(_tempRoot);
        File.WriteAllText(
            Path.Combine(_tempRoot, "Index.razor"),
            "<div class=\"vibe-flex vibe-not-a-utility\"></div>");
        var outputPath = Path.Combine(_tempRoot, "wwwroot", "css", "Vibe.UI.CSS");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        File.WriteAllText(outputPath, "existing output");

        var result = VibeCss.Generate(
            _tempRoot,
            outputPath,
            new GenerationOptions
            {
                IncludeBase = false,
                FailOnUnknown = true
            });

        Assert.False(result.Success);
        Assert.Contains("vibe-not-a-utility", result.UnknownClasses);
        Assert.Equal("existing output", File.ReadAllText(outputPath));
    }

    [Fact]
    public void Generate_IgnoredNonUtility_AllowsStrictGeneration()
    {
        Directory.CreateDirectory(_tempRoot);
        File.WriteAllText(
            Path.Combine(_tempRoot, "Index.razor"),
            "<div class=\"vibe-flex vibe-theme-hook\"></div>");
        var outputPath = Path.Combine(_tempRoot, "Vibe.UI.CSS");

        var result = VibeCss.Generate(
            _tempRoot,
            outputPath,
            new GenerationOptions
            {
                IncludeBase = false,
                FailOnUnknown = true,
                IgnoredClasses = ["vibe-theme-hook"]
            });

        Assert.True(result.Success, result.Error);
        Assert.Empty(result.UnknownClasses);
        Assert.Contains(".vibe-flex", File.ReadAllText(outputPath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
