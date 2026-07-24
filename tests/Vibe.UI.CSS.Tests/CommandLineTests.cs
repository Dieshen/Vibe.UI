using System.Diagnostics;

namespace Vibe.UI.CSS.Tests;

public sealed class CommandLineTests : IDisposable
{
    private readonly string _tempRoot = Path.Combine(
        Path.GetTempPath(),
        $"vibe-css-cli-tests-{Guid.NewGuid():N}");

    [Fact]
    public void Generate_BareWithBaseBeforeEqualsPatterns_PreservesBaseStyles()
    {
        Directory.CreateDirectory(_tempRoot);
        File.WriteAllText(Path.Combine(_tempRoot, "Index.razor"), "<div class=\"vibe-flex\"></div>");
        var outputPath = Path.Combine(_tempRoot, "Vibe.UI.CSS");

        var result = RunCssCli(
            "generate",
            _tempRoot,
            "-o",
            outputPath,
            "--with-base",
            "--patterns=*.razor");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--vibe-", File.ReadAllText(outputPath), StringComparison.Ordinal);
    }

    [Fact]
    public void Scan_FailOnUnknownAndIgnore_UsesStrictExitCodes()
    {
        Directory.CreateDirectory(_tempRoot);
        File.WriteAllText(
            Path.Combine(_tempRoot, "Index.razor"),
            "<div class=\"vibe-flex vibe-theme-hook\"></div>");

        var failed = RunCssCli(
            "scan",
            _tempRoot,
            "--patterns",
            "*.razor",
            "--fail-on-unknown");
        var passed = RunCssCli(
            "scan",
            _tempRoot,
            "--patterns",
            "*.razor",
            "--fail-on-unknown",
            "--ignore=vibe-theme-hook");

        Assert.Equal(1, failed.ExitCode);
        Assert.Equal(0, passed.ExitCode);
    }

    private static ProcessResult RunCssCli(params string[] arguments)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(VibeCss).Assembly.Location);
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start Vibe.UI.CSS CLI.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return new ProcessResult(process.ExitCode, stdout, stderr);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
