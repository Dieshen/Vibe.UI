using Microsoft.Playwright;
using Shouldly;
using Xunit;

namespace Vibe.UI.Docs.E2E.Infrastructure;

public abstract class CompatibilityE2ETestBase : IAsyncLifetime
{
    private readonly List<string> _consoleErrors = [];
    private readonly List<string> _failedRequests = [];

    protected IPlaywright Playwright { get; private set; } = null!;
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected string BaseUrl { get; private set; } = string.Empty;

    protected abstract CompatibilityApp App { get; }

    protected bool Headless { get; } =
        !bool.TryParse(Environment.GetEnvironmentVariable("HEADLESS"), out var headless) || headless;

    protected string BrowserType { get; } =
        Environment.GetEnvironmentVariable("BROWSER")?.ToLowerInvariant() ?? "chromium";

    public virtual async Task InitializeAsync()
    {
        BaseUrl = CompatibilityServerManager.GetBaseUrl(App);
        await CompatibilityServerManager.AcquireAsync(App, BaseUrl, CancellationToken.None);

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = BrowserType switch
        {
            "chromium" => await Playwright.Chromium.LaunchAsync(new() { Headless = Headless }),
            "firefox" => await Playwright.Firefox.LaunchAsync(new() { Headless = Headless }),
            "webkit" => await Playwright.Webkit.LaunchAsync(new() { Headless = Headless }),
            _ => throw new InvalidOperationException($"Unsupported browser '{BrowserType}'. Set BROWSER to chromium, firefox, or webkit.")
        };

        Context = await Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1280, Height = 720 }
        });

        Page = await Context.NewPageAsync();
        Page.SetDefaultNavigationTimeout(45000);
        Page.SetDefaultTimeout(15000);

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

        Page.RequestFailed += (_, request) =>
        {
            _failedRequests.Add($"[{request.Method}] {request.Url} - {request.Failure}");
        };
    }

    public virtual async Task DisposeAsync()
    {
        if (Page != null)
        {
            await Page.CloseAsync();
        }

        if (Context != null)
        {
            await Context.CloseAsync();
        }

        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();
        CompatibilityServerManager.Release(App, BaseUrl);
    }

    protected async Task NavigateToSmokePageAsync(string path, string smokeTestId)
    {
        _consoleErrors.Clear();
        _failedRequests.Clear();

        await Page.GotoAsync($"{BaseUrl.TrimEnd('/')}{path}", new()
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });

        await Page.Locator($"[data-testid='{smokeTestId}']").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 45000
        });

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 15000 });
    }

    protected async Task VerifyInteractiveSmokeAsync(string prefix)
    {
        var countText = Page.Locator("text=Current interaction count:").First;
        await countText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await ClickUntilTextContainsAsync(
            $"[data-testid='{prefix}-increment']",
            "Current interaction count: 1");

        var dialogTrigger = Page.Locator($"[data-testid='{prefix}-dialog']");
        await dialogTrigger.FocusAsync();
        await dialogTrigger.PressAsync("Enter");

        var dialog = Page.Locator("[role='dialog']").First;
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var dialogText = await dialog.TextContentAsync();
        dialogText.ShouldNotBeNull();
        dialogText.ShouldContain("dialog", Case.Insensitive);

        await Page.WaitForFunctionAsync(
            "() => document.querySelector(\"[role='dialog']\")?.contains(document.activeElement) === true");
        await Page.Keyboard.PressAsync("Escape");
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
        await Page.WaitForFunctionAsync(
            $"() => document.activeElement === document.querySelector(\"[data-testid='{prefix}-dialog']\")");
        (await dialogTrigger.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        await dialogTrigger.ClickAsync();
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await dialog.Locator("button:has-text('Close')").ClickAsync();
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        await VerifyBetaInteractionControlsAsync(prefix);
    }

    private async Task VerifyBetaInteractionControlsAsync(string prefix)
    {
        var datePicker = Page.Locator($"[data-testid='{prefix}-date-picker']");
        var dateInput = datePicker.Locator("input").First;
        await dateInput.FocusAsync();
        await dateInput.PressAsync("ArrowDown");

        var dateDialog = datePicker.GetByRole(AriaRole.Dialog);
        await dateDialog.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.WaitForFunctionAsync(
            "() => document.activeElement?.getAttribute('role') === 'gridcell'");
        await Page.Keyboard.PressAsync("Escape");
        await dateDialog.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        await Page.WaitForFunctionAsync(
            $"() => document.activeElement === document.querySelector(\"[data-testid='{prefix}-date-picker'] input\")");

        var dropdown = Page.Locator($"[data-testid='{prefix}-dropdown']");
        var dropdownTrigger = dropdown.Locator(".dropdown-trigger");
        await dropdownTrigger.FocusAsync();
        await dropdownTrigger.PressAsync("ArrowDown");

        var dropdownMenu = dropdown.GetByRole(AriaRole.Menu);
        await dropdownMenu.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.WaitForFunctionAsync(
            "() => document.activeElement?.textContent?.trim() === 'Profile'");
        await Page.Keyboard.PressAsync("ArrowDown");
        await Page.WaitForFunctionAsync(
            "() => document.activeElement?.textContent?.trim() === 'Settings'");
        await Page.Keyboard.PressAsync("Tab");
        await dropdownMenu.WaitForAsync(new() { State = WaitForSelectorState.Detached });

        var navigationMenu = Page.Locator($"[data-testid='{prefix}-navigation-menu']");
        var navigationTrigger = navigationMenu.Locator(".navigation-menu-item-trigger");
        await navigationTrigger.PressAsync("ArrowDown");

        var navigationContent = navigationMenu.Locator(".navigation-menu-item-content");
        await navigationContent.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var filter = navigationContent.Locator($"[data-testid='{prefix}-nav-filter']");
        await filter.FocusAsync();
        await filter.PressAsync("q");
        (await filter.InputValueAsync()).ShouldBe("q");

        var sort = navigationContent.Locator($"[data-testid='{prefix}-nav-sort']");
        await sort.FocusAsync();
        await sort.PressAsync("ArrowDown");
        await Page.WaitForFunctionAsync(
            $"() => document.querySelector(\"[data-testid='{prefix}-nav-sort']\")?.value === 'recent'");

        await filter.FocusAsync();
        await filter.PressAsync("Escape");
        await navigationContent.WaitForAsync(new() { State = WaitForSelectorState.Detached });
    }

    protected void AssertNoBrowserErrors()
    {
        var criticalErrors = _consoleErrors
            .Where(error => !IsIgnorableBrowserError(error))
            .ToList();

        var criticalFailures = _failedRequests
            .Where(failure => !IsIgnorableNetworkFailure(failure))
            .ToList();

        criticalErrors.ShouldBeEmpty($"Browser console errors found:{Environment.NewLine}{string.Join(Environment.NewLine, criticalErrors)}");
        criticalFailures.ShouldBeEmpty($"Failed browser requests found:{Environment.NewLine}{string.Join(Environment.NewLine, criticalFailures)}");
    }

    private async Task ClickUntilTextContainsAsync(string selector, string expectedText)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(30);
        Exception? last = null;

        while (DateTimeOffset.UtcNow < deadline)
        {
            try
            {
                await Page.Locator(selector).ClickAsync();
                await Page.Locator($"text={expectedText}").WaitForAsync(new()
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 1000
                });
                return;
            }
            catch (Exception ex) when (ex is TimeoutException or PlaywrightException)
            {
                last = ex;
                await Task.Delay(500);
            }
        }

        throw new TimeoutException($"Timed out waiting for '{expectedText}' after clicking '{selector}'.", last);
    }

    private static bool IsIgnorableBrowserError(string error)
    {
        var ignorable = new[]
        {
            "chrome-extension://",
            "moz-extension://",
            "Failed to load source map",
            "DevTools failed to load",
            "ResizeObserver loop"
        };

        return ignorable.Any(value => error.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsIgnorableNetworkFailure(string failure)
    {
        var ignorable = new[]
        {
            "favicon.ico"
        };

        return ignorable.Any(value => failure.Contains(value, StringComparison.OrdinalIgnoreCase));
    }
}
