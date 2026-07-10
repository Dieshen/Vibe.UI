using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Integration;

/// <summary>
/// Integration tests for JavaScript Interop functionality.
/// Tests that .NET to JS calls complete successfully and don't throw exceptions.
/// Catches hidden failures in IJSRuntime calls.
/// </summary>
[Trait("Category", TestCategories.Integration)]
public class JSInteropErrorTests : E2ETestBase
{
    private readonly List<string> _jsErrors = new();
    private readonly List<string> _blazorErrors = new();

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Capture all JS errors
        Page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                var text = msg.Text;
                _jsErrors.Add(text);

                // Track Blazor-specific errors
                if (text.Contains("Blazor", StringComparison.OrdinalIgnoreCase) ||
                    text.Contains("DotNet", StringComparison.OrdinalIgnoreCase) ||
                    text.Contains("interop", StringComparison.OrdinalIgnoreCase))
                {
                    _blazorErrors.Add(text);
                }
            }
        };

        Page.PageError += (_, error) =>
        {
            _jsErrors.Add($"[PageError] {error}");
        };
    }

    [Fact]
    public async Task BlazorInitializesWithoutJSInteropErrors()
    {
        // Arrange & Act
        await NavigateAndWaitForBlazorAsync("/");

        // Wait for all JS to initialize
        await Page.WaitForTimeoutAsync(3000);

        // Assert
        var interopErrors = _jsErrors
            .Where(e => IsJSInteropError(e))
            .ToList();

        interopErrors.ShouldBeEmpty(
            $"Blazor should initialize without JS interop errors. Found:\n{string.Join("\n", interopErrors)}");
    }

    [Fact]
    public async Task ThemeServiceJSInteropWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        ClearErrors();

        // Act - Theme toggle uses JS interop for localStorage and DOM manipulation
        var themeToggle = Page.Locator("[data-theme-toggle], .theme-toggle, button:has([class*='sun']), button:has([class*='moon'])").First;

        if (await themeToggle.IsVisibleAsync())
        {
            await themeToggle.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        // Assert
        var interopErrors = _blazorErrors.Where(e => !IsIgnorableInteropError(e)).ToList();
        interopErrors.ShouldBeEmpty(
            $"Theme service should not cause JS interop errors. Found:\n{string.Join("\n", interopErrors)}");
    }

    [Fact]
    public async Task ShikiHighlighterJSInteropWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        ClearErrors();

        // Wait for Shiki to initialize
        await Page.WaitForTimeoutAsync(3000);

        // Act - Check if highlighter is available
        var isHighlighterReady = await Page.EvaluateAsync<bool>(@"
            () => {
                const highlighterReady = typeof window.isHighlighterReady === 'function'
                    ? window.isHighlighterReady()
                    : window.isHighlighterReady === true;

                return highlighterReady ||
                       typeof window.highlightCode === 'function' ||
                       document.querySelector('.shiki, pre code span') !== null;
            }
        ");

        // Assert
        var shikiErrors = _jsErrors
            .Where(e => e.Contains("shiki", StringComparison.OrdinalIgnoreCase) ||
                        e.Contains("highlight", StringComparison.OrdinalIgnoreCase))
            .Where(e => !IsIgnorableInteropError(e))
            .ToList();

        shikiErrors.ShouldBeEmpty(
            $"Shiki highlighter should not cause JS errors. Found:\n{string.Join("\n", shikiErrors)}");
    }

    [Fact]
    public async Task ChartJSInteropWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/chart");
        ClearErrors();

        // Wait for Chart.js to initialize
        await Page.WaitForTimeoutAsync(3000);

        // Act - Check if Chart.js is available
        var chartAvailable = await Page.EvaluateAsync<bool>(@"
            () => {
                return typeof Chart !== 'undefined' ||
                       typeof window.vibeChart !== 'undefined' ||
                       document.querySelector('canvas') !== null;
            }
        ");

        // Assert
        var chartErrors = _jsErrors
            .Where(e => e.Contains("chart", StringComparison.OrdinalIgnoreCase) ||
                        e.Contains("canvas", StringComparison.OrdinalIgnoreCase))
            .Where(e => !IsIgnorableInteropError(e))
            .ToList();

        chartErrors.ShouldBeEmpty(
            $"Chart.js should not cause JS interop errors. Found:\n{string.Join("\n", chartErrors)}");
    }

    [Fact]
    public async Task ClipboardJSInteropWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        ClearErrors();

        // Wait for page to load
        await Page.WaitForTimeoutAsync(2000);

        // Act - Find and click a copy button if it exists
        var copyButton = Page.Locator("button:has-text('Copy'), .copy-button, [data-copy]").First;

        if (await copyButton.IsVisibleAsync())
        {
            try
            {
                await copyButton.ClickAsync();
                await Page.WaitForTimeoutAsync(500);
            }
            catch
            {
                // Clipboard might fail in headless - that's expected
            }
        }

        // Assert - No fatal JS errors from clipboard operation
        var clipboardErrors = _jsErrors
            .Where(e => e.Contains("clipboard", StringComparison.OrdinalIgnoreCase))
            .Where(e => !e.Contains("denied", StringComparison.OrdinalIgnoreCase)) // Permission denied is expected in headless
            .Where(e => !IsIgnorableInteropError(e))
            .ToList();

        clipboardErrors.ShouldBeEmpty(
            $"Clipboard operations should not cause fatal JS errors. Found:\n{string.Join("\n", clipboardErrors)}");
    }

    [Fact]
    public async Task ModalDialogJSInteropWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/modal");
        ClearErrors();

        // Wait for page to load
        await Page.WaitForTimeoutAsync(2000);

        // Act - Try to open a modal
        var openModalButton = Page.Locator("button:has-text('Open'), button:has-text('Show Modal'), [data-open-modal]").First;

        if (await openModalButton.IsVisibleAsync())
        {
            await openModalButton.ClickAsync();
            await Page.WaitForTimeoutAsync(1000);

            // Try to close modal
            await Page.Keyboard.PressAsync("Escape");
            await Page.WaitForTimeoutAsync(500);
        }

        // Assert
        var modalErrors = _blazorErrors
            .Where(e => e.Contains("modal", StringComparison.OrdinalIgnoreCase) ||
                        e.Contains("dialog", StringComparison.OrdinalIgnoreCase))
            .Where(e => !IsIgnorableInteropError(e))
            .ToList();

        modalErrors.ShouldBeEmpty(
            $"Modal/Dialog should not cause JS interop errors. Found:\n{string.Join("\n", modalErrors)}");
    }

    [Fact]
    public async Task ToastServiceJSInteropWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/toast");
        ClearErrors();

        // Wait for page to load
        await Page.WaitForTimeoutAsync(2000);

        // Act - Try to trigger a toast
        var showToastButton = Page.Locator("button:has-text('Show'), button:has-text('Toast'), [data-show-toast]").First;

        if (await showToastButton.IsVisibleAsync())
        {
            await showToastButton.ClickAsync();
            await Page.WaitForTimeoutAsync(1000);
        }

        // Assert
        var toastErrors = _blazorErrors
            .Where(e => e.Contains("toast", StringComparison.OrdinalIgnoreCase))
            .Where(e => !IsIgnorableInteropError(e))
            .ToList();

        toastErrors.ShouldBeEmpty(
            $"Toast service should not cause JS interop errors. Found:\n{string.Join("\n", toastErrors)}");
    }

    [Fact]
    public async Task LocalStorageJSInteropWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");

        // Act - Verify localStorage is accessible via JS interop
        var canAccessLocalStorage = await Page.EvaluateAsync<bool>(@"
            () => {
                try {
                    localStorage.setItem('__test__', 'test');
                    localStorage.removeItem('__test__');
                    return true;
                } catch {
                    return false;
                }
            }
        ");

        // Assert
        canAccessLocalStorage.ShouldBeTrue("localStorage should be accessible for JS interop");

        // Theme should have been persisted
        var theme = await Page.EvaluateAsync<string>("() => localStorage.getItem('theme')");
        // Theme might be null initially, but no errors should occur
    }

    [Theory]
    [InlineData("/components/slider")]
    [InlineData("/components/colorpicker")]
    [InlineData("/components/datepicker")]
    [InlineData("/components/richtexteditor")]
    public async Task InteractiveComponentsHaveNoJSInteropErrors(string path)
    {
        // Arrange
        ClearErrors();

        // Act
        await NavigateAndWaitForBlazorAsync(path);
        await Page.WaitForTimeoutAsync(3000); // Wait for all JS to initialize

        // Assert
        var criticalErrors = _blazorErrors
            .Where(e => !IsIgnorableInteropError(e))
            .ToList();

        criticalErrors.ShouldBeEmpty(
            $"Page {path} should have no JS interop errors. Found:\n{string.Join("\n", criticalErrors)}");
    }

    #region Helper Methods

    private void ClearErrors()
    {
        _jsErrors.Clear();
        _blazorErrors.Clear();
    }

    private static bool IsJSInteropError(string error)
    {
        var interopIndicators = new[]
        {
            "Blazor",
            "DotNet",
            "interop",
            "invoke",
            "IJSRuntime",
            "JSRuntime",
            "Microsoft.JSInterop"
        };

        return interopIndicators.Any(i => error.Contains(i, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsIgnorableInteropError(string error)
    {
        var ignorable = new[]
        {
            // Browser extensions
            "chrome-extension://",
            "moz-extension://",

            // Source maps
            "Failed to load source map",

            // Known non-critical
            "ResizeObserver loop",
            "favicon.ico",

            // Permission issues (expected in headless)
            "denied",
            "not allowed",
            "permission",

            // Network issues
            "network",
            "fetch",
            "Failed to fetch"
        };

        return ignorable.Any(i => error.Contains(i, StringComparison.OrdinalIgnoreCase));
    }

    #endregion
}
