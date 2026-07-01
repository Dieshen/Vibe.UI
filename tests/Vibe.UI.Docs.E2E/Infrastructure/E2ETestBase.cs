using Microsoft.Playwright;
using Xunit;

namespace Vibe.UI.Docs.E2E.Infrastructure;

/// <summary>
/// Base class for E2E tests using Playwright
/// Provides common setup, configuration, and utilities for browser automation
/// </summary>
public abstract class E2ETestBase : IAsyncLifetime
{
    protected IPlaywright Playwright { get; private set; } = null!;
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    /// <summary>
    /// Base URL for the documentation site
    /// Configurable via DOCS_BASE_URL environment variable
    /// </summary>
    protected string BaseUrl { get; } = Environment.GetEnvironmentVariable("DOCS_BASE_URL")
        ?? "http://localhost:5000";

    /// <summary>
    /// Whether to run browser in headless mode
    /// Configurable via HEADLESS environment variable (default: true)
    /// </summary>
    protected bool Headless { get; } =
        !bool.TryParse(Environment.GetEnvironmentVariable("HEADLESS"), out var headless) || headless;

    /// <summary>
    /// Browser type to use for tests
    /// Configurable via BROWSER environment variable (default: chromium)
    /// </summary>
    protected string BrowserType { get; } =
        Environment.GetEnvironmentVariable("BROWSER")?.ToLowerInvariant() ?? "chromium";

    public virtual async Task InitializeAsync()
    {
        // Ensure the docs site is reachable when using the default base URL.
        // If DOCS_BASE_URL points elsewhere, assume the caller/CI starts the server externally.
        await DocsServerManager.AcquireAsync(BaseUrl, CancellationToken.None);

        // Create Playwright instance
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        // Launch browser based on configured type
        Browser = BrowserType switch
        {
            "firefox" => await Playwright.Firefox.LaunchAsync(new()
            {
                Headless = Headless
            }),
            "webkit" => await Playwright.Webkit.LaunchAsync(new()
            {
                Headless = Headless
            }),
            _ => await Playwright.Chromium.LaunchAsync(new()
            {
                Headless = Headless
            })
        };

        // Create browser context with tracing enabled
        Context = await Browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1280, Height = 720 },
            RecordVideoDir = null, // Disable video recording by default
        });

        // Start tracing for debugging
        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        // Create page
        Page = await Context.NewPageAsync();

        // Set default timeouts for Blazor WASM
        Page.SetDefaultNavigationTimeout(30000); // 30s for WASM load
        Page.SetDefaultTimeout(10000); // 10s for actions
    }

    public virtual async Task DisposeAsync()
    {
        // Save trace on failure (determined by test framework)
        var tracePath = Path.Combine("Artifacts", $"trace-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.zip");
        Directory.CreateDirectory("Artifacts");

        try
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = tracePath
            });
        }
        catch
        {
            // Tracing might fail, ignore
        }

        // Cleanup
        await Page?.CloseAsync()!;
        await Context?.CloseAsync()!;
        await Browser?.CloseAsync()!;
        Playwright?.Dispose();

        DocsServerManager.Release(BaseUrl);
    }

    /// <summary>
    /// Navigate to a path and wait for Blazor to be ready
    /// </summary>
    /// <param name="path">Relative path (e.g., "/components/button")</param>
    protected async Task NavigateAndWaitForBlazorAsync(string path = "/")
    {
        var url = $"{BaseUrl.TrimEnd('/')}{path}";
        await Page.GotoAsync(url, new()
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });
        await Page.WaitForBlazorReadyAsync();
    }

    /// <summary>
    /// Capture screenshot for debugging
    /// </summary>
    protected async Task CaptureScreenshotAsync(string name)
    {
        var screenshotPath = Path.Combine("Artifacts", $"{name}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png");
        Directory.CreateDirectory("Artifacts");
        await Page.ScreenshotAsync(new() { Path = screenshotPath });
    }

}
