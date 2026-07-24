using Microsoft.Playwright;

namespace Vibe.UI.Docs.E2E.PageObjects;

/// <summary>
/// Page object for the Command Palette (Ctrl+K search modal)
/// Based on CommandPalette.razor component
/// </summary>
public class CommandPalettePage
{
    private readonly IPage _page;

    public CommandPalettePage(IPage page)
    {
        _page = page;
    }

    // Modal Elements
    public ILocator Overlay => _page.Locator(".vibe-fixed.vibe-inset-0.vibe-z-50");
    public ILocator ModalContent => Overlay.Locator(".vibe-bg-white.dark\\:vibe-bg-zinc-900.vibe-rounded-xl");
    public ILocator SearchInput => _page.GetByPlaceholder("Search components...");
    public ILocator EscHint => _page.GetByText("ESC");

    // Results Section
    public ILocator ResultsContainer => Overlay.Locator(".vibe-max-h-\\[400px\\].vibe-overflow-y-auto");
    public ILocator QuickActionsSection => Overlay.GetByText("Quick Actions", new() { Exact = true });
    public ILocator PopularComponentsSection => Overlay.GetByText("Popular Components", new() { Exact = true });
    public ILocator SearchResultsSection => _page.Locator("text=/Components \\(/");

    // Quick Actions
    public ILocator ToggleThemeAction => Overlay.GetByRole(AriaRole.Button, new()
    {
        NameString = "Toggle theme",
        Exact = false
    });

    public ILocator GitHubAction => Overlay.GetByRole(AriaRole.Link, new()
    {
        NameString = "View on GitHub",
        Exact = false
    });

    // Search Results
    public ILocator SearchResults => Overlay.Locator("button.vibe-w-full");
    public ILocator SelectedResult => _page.Locator("button.vibe-bg-brand-teal\\/10, button.dark\\:vibe-bg-brand-teal\\/20");

    // No Results
    public ILocator NoResultsMessage => Overlay.Locator("text=/No components found for/");

    // Footer
    public ILocator Footer => _page.Locator(".vibe-border-t.vibe-border-zinc-200");
    public ILocator ComponentCount => _page.Locator("text=/\\d+ components/");

    /// <summary>
    /// Open command palette using Ctrl+K keyboard shortcut
    /// </summary>
    public async Task OpenWithKeyboardAsync()
    {
        await _page.Keyboard.PressAsync("Control+K");
        await WaitForModalAsync();
    }

    /// <summary>
    /// Wait for modal to be visible
    /// </summary>
    public async Task WaitForModalAsync(int timeout = 5000)
    {
        await Overlay.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }

    /// <summary>
    /// Check if command palette is open
    /// </summary>
    public async Task<bool> IsOpenAsync()
    {
        return await Overlay.IsVisibleAsync();
    }

    /// <summary>
    /// Close command palette using Escape key
    /// </summary>
    public async Task CloseWithEscapeAsync()
    {
        if (await SearchInput.IsVisibleAsync())
        {
            await SearchInput.FocusAsync();
        }
        await _page.Keyboard.PressAsync("Escape");
        await WaitForModalCloseAsync();
    }

    /// <summary>
    /// Wait for modal to close/disappear
    /// </summary>
    public async Task WaitForModalCloseAsync(int timeout = 8000)
    {
        await Overlay.WaitForAsync(new()
        {
            State = WaitForSelectorState.Hidden,
            Timeout = timeout
        });
    }

    /// <summary>
    /// Type a search query into the search input
    /// </summary>
    public async Task SearchAsync(string query)
    {
        await SearchInput.FillAsync(query);
        await SearchInput.FocusAsync();
        // Wait for search results to update
        await _page.WaitForTimeoutAsync(300);
    }

    /// <summary>
    /// Get the count of search results displayed
    /// </summary>
    public async Task<int> GetResultCountAsync()
    {
        return await SearchResults.CountAsync();
    }

    /// <summary>
    /// Navigate search results using arrow keys
    /// </summary>
    public async Task PressArrowDownAsync()
    {
        if (await SearchInput.IsVisibleAsync())
        {
            await SearchInput.FocusAsync();
        }
        await _page.Keyboard.PressAsync("ArrowDown");
        await _page.WaitForTimeoutAsync(150);
    }

    /// <summary>
    /// Navigate search results using arrow up
    /// </summary>
    public async Task PressArrowUpAsync()
    {
        if (await SearchInput.IsVisibleAsync())
        {
            await SearchInput.FocusAsync();
        }
        await _page.Keyboard.PressAsync("ArrowUp");
        await _page.WaitForTimeoutAsync(150);
    }

    /// <summary>
    /// Select current result using Enter key
    /// </summary>
    public async Task PressEnterAsync()
    {
        await _page.Keyboard.PressAsync("Enter");
    }

    /// <summary>
    /// Get currently selected result index (0-based)
    /// </summary>
    public async Task<int> GetSelectedIndexAsync()
    {
        var selected = SelectedResult;
        if (!await selected.IsVisibleAsync())
        {
            return -1;
        }

        // Find index among all results
        var allResults = await SearchResults.AllAsync();
        for (int i = 0; i < allResults.Count; i++)
        {
            var classes = await allResults[i].GetAttributeAsync("class") ?? "";
            if (classes.Contains("vibe-bg-brand-teal/10") || classes.Contains("dark:vibe-bg-brand-teal/20"))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Click on a specific search result by name
    /// </summary>
    public async Task ClickResultAsync(string componentName)
    {
        var result = _page.Locator($"button:has-text('{componentName}')").First;
        await result.ClickAsync();
    }

    /// <summary>
    /// Click the "Toggle theme" quick action
    /// </summary>
    public async Task ClickToggleThemeActionAsync()
    {
        await ToggleThemeAction.ClickAsync();
    }

    /// <summary>
    /// Check if "No results" message is displayed
    /// </summary>
    public async Task<bool> HasNoResultsMessageAsync()
    {
        return await NoResultsMessage.IsVisibleAsync();
    }

    /// <summary>
    /// Check if Quick Actions are displayed (no search query)
    /// </summary>
    public async Task<bool> HasQuickActionsAsync()
    {
        return await QuickActionsSection.IsVisibleAsync();
    }

    /// <summary>
    /// Check if Popular Components are displayed (no search query)
    /// </summary>
    public async Task<bool> HasPopularComponentsAsync()
    {
        return await PopularComponentsSection.IsVisibleAsync();
    }
}
