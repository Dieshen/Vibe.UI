using FluentAssertions;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Services;

public sealed class RazorImportsServiceTests : IDisposable
{
    private readonly string _projectPath = Path.Combine(
        Path.GetTempPath(),
        $"vibe-imports-test-{Guid.NewGuid():N}");

    public RazorImportsServiceTests()
    {
        Directory.CreateDirectory(_projectPath);
    }

    [Fact]
    public async Task EnsureVibeImportsAsync_InfrastructureOnly_DoesNotImportMissingComponentNamespace()
    {
        await RazorImportsService.EnsureVibeImportsAsync(_projectPath, includeComponents: false);

        var content = await File.ReadAllTextAsync(Path.Combine(_projectPath, "_Imports.razor"));
        content.Should().Contain("@using global::Vibe.UI.Base");
        content.Should().Contain("@using global::Vibe.UI.Enums");
        content.Should().NotContain("@using global::Vibe.UI.Components");
    }

    [Fact]
    public async Task EnsureVibeImportsAsync_WithComponents_AddsComponentNamespaceOnce()
    {
        await RazorImportsService.EnsureVibeImportsAsync(_projectPath, includeComponents: false);
        await RazorImportsService.EnsureVibeImportsAsync(_projectPath, includeComponents: true);
        await RazorImportsService.EnsureVibeImportsAsync(_projectPath, includeComponents: true);

        var content = await File.ReadAllTextAsync(Path.Combine(_projectPath, "_Imports.razor"));
        content.Split("@using global::Vibe.UI.Components", StringSplitOptions.None)
            .Should().HaveCount(2);
    }

    public void Dispose()
    {
        if (Directory.Exists(_projectPath))
        {
            Directory.Delete(_projectPath, recursive: true);
        }
    }
}
