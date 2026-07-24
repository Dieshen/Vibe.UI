using FluentAssertions;
using Spectre.Console.Cli;
using Vibe.UI.CLI.Commands;
using Vibe.UI.CLI.Tests.Helpers;
using Xunit;

namespace Vibe.UI.CLI.Tests.Commands;

/// <summary>
/// Tests for CssCommand (Vibe.UI.CSS JIT generation via CLI).
/// Uses SpectreConsole collection to prevent parallel execution.
/// </summary>
[Collection("SpectreConsole")]
public class CssCommandTests : IDisposable
{
    private readonly string _testProjectPath;
    private readonly CssCommand _command;

    public CssCommandTests()
    {
        _testProjectPath = Path.Combine(Path.GetTempPath(), $"vibe-css-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testProjectPath);
        _command = new CssCommand();
    }

    #region Settings Tests

    [Fact]
    public void Settings_DefaultValues_AreCorrect()
    {
        var settings = new CssCommand.Settings();

        settings.ProjectPath.Should().Be(".");
        settings.OutputPath.Should().Be("wwwroot/css/Vibe.UI.CSS");
        settings.IncludeBase.Should().BeTrue();
        settings.Prefix.Should().Be("vibe");
        settings.Watch.Should().BeFalse();
        settings.Verbose.Should().BeFalse();
        settings.ScanOnly.Should().BeFalse();
        settings.Patterns.Should().Be("*.razor,*.cshtml,*.html");
        settings.FailOnUnknown.Should().BeFalse();
        settings.IgnoredClasses.Should().BeEmpty();
    }

    #endregion

    #region ScanOnly Tests

    [Fact]
    public async Task ExecuteAsync_ScanOnly_ReturnsSuccessWithNoOutput()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"<div class=""vibe-flex vibe-p-4"">Test</div>");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            ScanOnly = true
        };

        var context = CreateContext();

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        // ScanOnly should not create output file
        File.Exists(Path.Combine(_testProjectPath, "wwwroot", "css", "Vibe.UI.CSS")).Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_ScanOnly_FindsVibeClasses()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"
            <div class=""vibe-flex vibe-gap-4"">
                <span class=""vibe-text-sm vibe-text-muted"">Text</span>
            </div>");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            ScanOnly = true,
            Verbose = true
        };

        var context = CreateContext();

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_ScanOnlyStrict_ReturnsErrorForUnknownUtility()
    {
        await CreateTestRazorFileAsync(
            "Test.razor",
            @"<div class=""vibe-flex vibe-not-a-utility"">Test</div>");
        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            ScanOnly = true,
            FailOnUnknown = true
        };

        var result = await _command.ExecuteAsync(CreateContext(), settings);

        result.Should().Be(1);
    }

    #endregion

    #region Generate Tests

    [Fact]
    public async Task ExecuteAsync_Generate_CreatesCssFile()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"<div class=""vibe-flex vibe-p-4"">Test</div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        File.Exists(outputPath).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_Generate_ContainsExpectedRules()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"<div class=""vibe-flex vibe-p-4 vibe-bg-primary"">Test</div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain(".vibe-flex");
        css.Should().Contain("display: flex");
        css.Should().Contain(".vibe-p-4");
        css.Should().Contain("padding: 1rem");
        css.Should().Contain(".vibe-bg-primary");
    }

    [Fact]
    public async Task ExecuteAsync_Generate_WithIncludeBase_ContainsBaseStyles()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"<div class=""vibe-flex"">Test</div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = true
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain("--vibe-"); // Base CSS contains CSS variables
    }

    [Fact]
    public async Task ExecuteAsync_Generate_WithoutIncludeBase_NoBaseStyles()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"<div class=""vibe-flex"">Test</div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain(".vibe-flex");
        css.Should().NotContain(":root {"); // No root rules without base
    }

    #endregion

    #region Custom Prefix Tests

    [Fact]
    public async Task ExecuteAsync_CustomPrefix_ScansCorrectClasses()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"<div class=""tw-flex tw-p-4"">Test</div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "custom.css");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            Prefix = "tw",
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain(".tw-flex");
        css.Should().Contain(".tw-p-4");
    }

    #endregion

    #region Pattern Tests

    [Fact]
    public async Task ExecuteAsync_CustomPatterns_ScansOnlyMatchingFiles()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"<div class=""vibe-flex"">Razor</div>");
        await CreateTestFileAsync("Test.html", @"<div class=""vibe-hidden"">HTML</div>");
        await CreateTestFileAsync("Test.txt", @"<div class=""vibe-block"">TXT should be ignored</div>");

        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            Patterns = "*.razor", // Only scan .razor files
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain(".vibe-flex"); // From .razor
        css.Should().NotContain(".vibe-hidden"); // From .html (not scanned)
        css.Should().NotContain(".vibe-block"); // From .txt (not scanned)
    }

    #endregion

    #region Variant Tests

    [Fact]
    public async Task ExecuteAsync_Generate_HandlesVariants()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"
            <button class=""vibe-bg-primary hover:vibe-bg-secondary focus:vibe-ring-2"">
                Button
            </button>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain(".vibe-bg-primary");
        css.Should().Contain(":hover");
        css.Should().Contain(":focus");
    }

    [Fact]
    public async Task ExecuteAsync_Generate_HandlesResponsiveVariants()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"
            <div class=""vibe-flex sm:vibe-hidden md:vibe-block lg:vibe-grid"">
                Content
            </div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain("@media (min-width: 640px)"); // sm
        css.Should().Contain("@media (min-width: 768px)"); // md
        css.Should().Contain("@media (min-width: 1024px)"); // lg
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task ExecuteAsync_InvalidDirectory_ReturnsError()
    {
        // Arrange
        var settings = new CssCommand.Settings
        {
            ProjectPath = Path.Combine(_testProjectPath, "nonexistent")
        };

        var context = CreateContext();

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyDirectory_ReturnsSuccess()
    {
        // Arrange - Empty directory with no files
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        var result = await _command.ExecuteAsync(context, settings);

        // Assert
        result.Should().Be(0);
        File.Exists(outputPath).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_StrictGeneration_DoesNotWriteUnknownUtilities()
    {
        await CreateTestRazorFileAsync(
            "Test.razor",
            @"<div class=""vibe-flex vibe-not-a-utility"">Test</div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");
        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false,
            FailOnUnknown = true
        };

        var result = await _command.ExecuteAsync(CreateContext(), settings);

        result.Should().Be(1);
        File.Exists(outputPath).Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_StrictGeneration_AllowsExplicitIgnoreList()
    {
        await CreateTestRazorFileAsync(
            "Test.razor",
            @"<div class=""vibe-flex vibe-theme-hook"">Test</div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");
        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false,
            FailOnUnknown = true,
            IgnoredClasses = "vibe-theme-hook"
        };

        var result = await _command.ExecuteAsync(CreateContext(), settings);

        result.Should().Be(0);
        File.Exists(outputPath).Should().BeTrue();
    }

    #endregion

    #region Color Palette Tests

    [Fact]
    public async Task ExecuteAsync_Generate_HandlesTailwindColors()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"
            <div class=""vibe-bg-red-500 vibe-text-blue-600 vibe-border-emerald-300"">
                Colored content
            </div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain("#ef4444"); // red-500
        css.Should().Contain("#2563eb"); // blue-600
        css.Should().Contain("#6ee7b7"); // emerald-300
    }

    [Fact]
    public async Task ExecuteAsync_Generate_HandlesOpacityModifiers()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"
            <div class=""vibe-bg-red-500/50 vibe-text-blue-600/75"">
                Semi-transparent
            </div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain("rgb("); // Opacity requires rgb format
        css.Should().Contain("/ 0.5"); // 50% opacity
        css.Should().Contain("/ 0.75"); // 75% opacity
    }

    #endregion

    #region Arbitrary Value Tests

    [Fact]
    public async Task ExecuteAsync_Generate_HandlesArbitraryValues()
    {
        // Arrange
        await CreateTestRazorFileAsync("Test.razor", @"
            <div class=""vibe-w-[500px] vibe-p-[1.5rem] vibe-mt-[20px]"">
                Arbitrary values
            </div>");
        var outputPath = Path.Combine(_testProjectPath, "output", "Vibe.UI.CSS");

        var settings = new CssCommand.Settings
        {
            ProjectPath = _testProjectPath,
            OutputPath = outputPath,
            IncludeBase = false
        };

        var context = CreateContext();

        // Act
        await _command.ExecuteAsync(context, settings);

        // Assert
        var css = await File.ReadAllTextAsync(outputPath);
        css.Should().Contain("width: 500px");
        css.Should().Contain("padding: 1.5rem");
        css.Should().Contain("margin-top: 20px");
    }

    #endregion

    #region Helper Methods

    private async Task CreateTestRazorFileAsync(string fileName, string content)
    {
        var filePath = Path.Combine(_testProjectPath, fileName);
        await File.WriteAllTextAsync(filePath, content);
    }

    private async Task CreateTestFileAsync(string fileName, string content)
    {
        var filePath = Path.Combine(_testProjectPath, fileName);
        await File.WriteAllTextAsync(filePath, content);
    }

    private static CommandContext CreateContext()
    {
        return new CommandContext(
            Array.Empty<string>(),
            new TestRemainingArguments(),
            "css",
            null);
    }

    #endregion

    public void Dispose()
    {
        if (Directory.Exists(_testProjectPath))
        {
            try
            {
                Directory.Delete(_testProjectPath, true);
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }
    }
}

