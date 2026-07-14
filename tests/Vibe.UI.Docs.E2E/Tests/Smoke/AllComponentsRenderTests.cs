using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Smoke;

/// <summary>
/// Smoke tests that verify every routed component page loads without errors.
/// Route cases are derived from the documentation source so additions and aliases cannot drift.
/// </summary>
[Trait("Category", TestCategories.Smoke)]
public class AllComponentsRenderTests : E2ETestBase
{
    private readonly List<string> _consoleErrors = new();

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Capture console errors
        Page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                _consoleErrors.Add($"[{msg.Type}] {msg.Text}");
            }
        };

        Page.PageError += (_, error) =>
        {
            _consoleErrors.Add($"[PageError] {error}");
        };
    }

    [Fact]
    public async Task AllRoutedComponentPagesRenderTheirExpectedHeadings()
    {
        foreach (var page in ComponentPageManifest.Pages)
        {
            foreach (var route in page.Routes)
            {
                await VerifyComponentPageRenders(route, page.Heading);
            }
        }
    }

    #region Page Content Validation

    [Fact]
    public async Task RepresentativeComponentPagesHaveExamples()
    {
        var componentPaths = new[]
        {
            "button", "card", "alert", "input", "checkbox"
        };

        foreach (var path in componentPaths)
        {
            _consoleErrors.Clear();
            await NavigateAndWaitForBlazorAsync($"/components/{path}");

            // Wait for content to load
            await Page.WaitForTimeoutAsync(1000);

            // Verify page has preview/example sections
            // The docs site uses LivePreview component and example sections with bg-zinc backgrounds
            var hasExamples = await Page.Locator(
                "section, .bg-zinc-50, .dark\\:bg-zinc-800\\/50, [class*='preview'], [class*='example']"
            ).CountAsync();

            hasExamples.ShouldBeGreaterThan(0, $"Component page {path} should have examples");
        }
    }

    [Fact]
    public async Task RepresentativeComponentPagesHaveCodeBlocks()
    {
        var componentPaths = new[]
        {
            "button", "card", "alert", "input"
        };

        foreach (var path in componentPaths)
        {
            _consoleErrors.Clear();
            await NavigateAndWaitForBlazorAsync($"/components/{path}");

            // Wait for code blocks to render
            await Page.WaitForTimeoutAsync(1500);

            // Verify page has code blocks
            var codeBlocks = await Page.Locator("pre, code, .code-block, [class*='shiki']").CountAsync();

            codeBlocks.ShouldBeGreaterThan(0, $"Component page {path} should have code examples");
        }
    }

    [Fact]
    public async Task DropdownPreviewUsesNativeTriggersWithoutNestedButtons()
    {
        await NavigateAndWaitForBlazorAsync("/components/dropdown");

        var triggers = Page.Locator(".dropdown-trigger");
        (await triggers.CountAsync()).ShouldBe(2);
        (await Page.Locator(".dropdown-trigger button").CountAsync()).ShouldBe(0);

        foreach (var trigger in await triggers.AllAsync())
        {
            (await trigger.EvaluateAsync<string>("element => element.tagName")).ShouldBe("BUTTON");
            (await trigger.GetAttributeAsync("aria-haspopup")).ShouldBe("menu");
        }

        await triggers.First.ClickAsync();

        (await Page.Locator("[role='menu'] [role='menuitem']").CountAsync()).ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task MenuPreviewsUseNativeTriggersWithoutNestedButtons()
    {
        await NavigateAndWaitForBlazorAsync("/components/menu");

        var triggers = Page.Locator(".vibe-menu-trigger");
        (await triggers.CountAsync()).ShouldBeGreaterThan(0);
        (await Page.Locator(".vibe-menu-trigger button").CountAsync()).ShouldBe(0);

        foreach (var trigger in await triggers.AllAsync())
        {
            (await trigger.EvaluateAsync<string>("element => element.tagName")).ShouldBe("BUTTON");
            (await trigger.GetAttributeAsync("aria-haspopup")).ShouldBe("menu");
        }

        await triggers.First.ClickAsync();

        (await Page.Locator(".vibe-menu-content [role='menuitem']").CountAsync()).ShouldBeGreaterThan(0);
        (await Page.Locator(".vibe-menu-content .menu-item-icon svg").CountAsync()).ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task PopoverPreviewUsesNativeTriggerWithoutNestedButtons()
    {
        await NavigateAndWaitForBlazorAsync("/components/popover");

        var trigger = Page.Locator(".popover-trigger").First;
        (await trigger.EvaluateAsync<string>("element => element.tagName")).ShouldBe("BUTTON");
        (await Page.Locator(".popover-trigger button").CountAsync()).ShouldBe(0);

        await trigger.ClickAsync();

        await Page.Locator(".popover-content[role='dialog']").WaitForAsync();
        (await trigger.GetAttributeAsync("aria-expanded")).ShouldBe("true");
    }

    [Fact]
    public async Task NotificationPageRendersInteractiveNotificationCenter()
    {
        await NavigateAndWaitForBlazorAsync("/components/notification");

        (await Page.GetByText("Component preview will be shown here when installed.").CountAsync()).ShouldBe(0);

        var trigger = Page.Locator(".notification-trigger");
        await trigger.ClickAsync();

        await Page.Locator(".notification-panel[role='dialog']").WaitForAsync();
        (await Page.Locator(".notification-item").CountAsync()).ShouldBe(4);
        (await Page.Locator(".notification-icon svg").CountAsync()).ShouldBe(4);
        (await Page.Locator(".filter-btn").CountAsync()).ShouldBeGreaterThan(1);

        await Page.Locator(".notification-backdrop").ClickAsync(new() { Position = new() { X = 4, Y = 4 } });
        (await Page.Locator(".notification-panel").CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task DragDropPreviewSupportsKeyboardAndPointerReordering()
    {
        await NavigateAndWaitForBlazorAsync("/components/dragdrop");

        (await Page.GetByText("DragDrop component not found in Advanced folder.").CountAsync()).ShouldBe(0);
        (await Page.Locator(".dragdrop-item").CountAsync()).ShouldBe(4);
        (await Page.Locator(".dragdrop-handle:disabled").CountAsync()).ShouldBe(1);

        var handles = Page.Locator(".dragdrop-handle");
        await handles.First.FocusAsync();
        await Page.Keyboard.PressAsync("Alt+ArrowDown");
        await Page.WaitForTimeoutAsync(100);

        var firstItemContent = await Page.Locator(".dragdrop-item-content").First.TextContentAsync();
        firstItemContent.ShouldNotBeNull();
        firstItemContent.ShouldContain("Prepare release notes");

        var liveAnnouncement = await Page.Locator(".dragdrop-live-region").TextContentAsync();
        liveAnnouncement.ShouldNotBeNull();
        liveAnnouncement.ShouldContain("Moved Run compatibility tests to position 2 of 4");

        await Page.Locator(".dragdrop-handle").Nth(1)
            .DragToAsync(Page.Locator(".dragdrop-item").Last);
        await Page.WaitForTimeoutAsync(100);

        var lastItemContent = await Page.Locator(".dragdrop-item-content").Last.TextContentAsync();
        lastItemContent.ShouldNotBeNull();
        lastItemContent.ShouldContain("Run compatibility tests");
    }

    #endregion

    #region Helper Methods

    private async Task VerifyComponentPageRenders(string route, string expectedHeading)
    {
        _consoleErrors.Clear();

        await NavigateAndWaitForBlazorAsync(route);

        var actualPath = new Uri(Page.Url).AbsolutePath.TrimEnd('/');
        var expectedPath = route.TrimEnd('/');
        string.Equals(actualPath, expectedPath, StringComparison.OrdinalIgnoreCase).ShouldBeTrue(
            $"Navigation should remain on {route}, but the browser resolved to {actualPath}");

        (await Page.GetByText("Sorry, there's nothing at this address.", new() { Exact = true }).CountAsync())
            .ShouldBe(0, $"{route} rendered the NotFound shell");

        var errorUi = Page.Locator("#blazor-error-ui");
        if (await errorUi.CountAsync() > 0)
        {
            (await errorUi.IsVisibleAsync()).ShouldBeFalse($"{route} displayed the Blazor error shell");
        }

        var pageHeading = Page.Locator("main h1").First;
        await pageHeading.WaitForAsync(new()
        {
            State = Microsoft.Playwright.WaitForSelectorState.Visible,
            Timeout = 30000
        });

        var actualHeading = NormalizeWhitespace(await pageHeading.InnerTextAsync());
        actualHeading.ShouldBe(
            expectedHeading,
            $"{route} should render its route-specific component heading");

        var criticalErrors = _consoleErrors
            .Where(e => !IsIgnorableError(e))
            .ToList();

        criticalErrors.ShouldBeEmpty(
            $"{route} should have no console errors. Found:\n{string.Join("\n", criticalErrors)}");
    }

    private static string NormalizeWhitespace(string value) =>
        string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    private static bool IsIgnorableError(string error)
    {
        var ignorable = new[]
        {
            "chrome-extension://",
            "moz-extension://",
            "Failed to load source map",
            "DevTools failed to load",
            "favicon.ico",
            "ResizeObserver loop"
        };

        return ignorable.Any(i => error.Contains(i, StringComparison.OrdinalIgnoreCase));
    }

    #endregion
}
