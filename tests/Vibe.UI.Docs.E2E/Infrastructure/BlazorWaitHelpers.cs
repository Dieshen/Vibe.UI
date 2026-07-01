using Microsoft.Playwright;

namespace Vibe.UI.Docs.E2E.Infrastructure;

/// <summary>
/// Extension methods for waiting on Blazor WASM-specific conditions
/// </summary>
public static class BlazorWaitHelpers
{
    /// <summary>
    /// Wait for Blazor WASM to fully initialize and be ready for interaction
    /// Checks for window._vibeKeyboardHandler (set in MainLayout after Blazor loads)
    /// </summary>
    public static async Task WaitForBlazorReadyAsync(this IPage page, int timeout = 30000)
    {
        // Wait for either the Vibe keyboard handler or the main docs container
        // Both indicate Blazor has loaded and rendered
        await page.WaitForFunctionAsync(
            @"() => {
                return window._vibeKeyboardHandler !== undefined ||
                       document.querySelector('.docs-container') !== null;
            }",
            null,
            new PageWaitForFunctionOptions { Timeout = timeout }
        );

        // Additional small delay to ensure all initial render is complete
        await Task.Delay(100);
    }

    /// <summary>
    /// Wait for Shiki syntax highlighter to be ready
    /// Used when testing code block rendering
    /// </summary>
    public static async Task WaitForShikiReadyAsync(this IPage page, int timeout = 10000)
    {
        await page.WaitForFunctionAsync(
            @"() => {
                if (typeof window.isHighlighterReady === 'function') {
                    return window.isHighlighterReady();
                }

                return window.isHighlighterReady === true;
            }",
            null,
            new PageWaitForFunctionOptions { Timeout = timeout }
        );
    }

    /// <summary>
    /// Wait for a toast notification to appear
    /// </summary>
    /// <returns>Locator for the toast element</returns>
    public static async Task<ILocator> WaitForToastAsync(this IPage page, int timeout = 5000)
    {
        // Toast notifications use the ToastContainer component
        var toast = page.Locator(".toast-notification, [role='alert']").First;
        await toast.WaitForAsync(new() { Timeout = timeout, State = WaitForSelectorState.Visible });
        return toast;
    }

    /// <summary>
    /// Wait for a toast notification to be dismissed/hidden
    /// </summary>
    public static async Task WaitForToastDismissAsync(this ILocator toastLocator, int timeout = 6000)
    {
        await toastLocator.WaitForAsync(new()
        {
            State = WaitForSelectorState.Hidden,
            Timeout = timeout
        });
    }

    /// <summary>
    /// Wait for network to be idle (no pending requests)
    /// Useful after navigation or when waiting for data loads
    /// </summary>
    public static async Task WaitForNetworkIdleAsync(this IPage page, int timeout = 5000)
    {
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = timeout });
    }

    /// <summary>
    /// Wait for a specific element with retry logic
    /// </summary>
    public static async Task<ILocator> WaitForElementAsync(
        this IPage page,
        string selector,
        int timeout = 5000)
    {
        var element = page.Locator(selector);
        await element.WaitForAsync(new() { Timeout = timeout, State = WaitForSelectorState.Visible });
        return element;
    }
}
