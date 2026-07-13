using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Vibe.UI.Docs.E2E.PageObjects;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Functional;

/// <summary>
/// Functional tests for Command Palette (Ctrl+K search modal)
/// Tests fuzzy search, keyboard navigation, and selection
/// </summary>
[Trait("Category", TestCategories.Functional)]
public class CommandPaletteTests : E2ETestBase
{
    [Fact]
    public async Task FuzzySearchFindsComponents()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var commandPalette = new CommandPalettePage(Page);

        // Act
        await commandPalette.OpenWithKeyboardAsync();
        await commandPalette.SearchAsync("butt");

        // Assert
        var resultCount = await commandPalette.GetResultCountAsync();
        resultCount.ShouldBeGreaterThan(0, "Should find results for 'butt' (Button component)");

        // Verify "Button" appears in results
        var results = commandPalette.SearchResults;
        var firstResult = results.First;
        var text = await firstResult.TextContentAsync();
        text.ShouldNotBeNull();
        text.ShouldContain("Button", Case.Insensitive);
    }

    [Fact]
    public async Task ArrowKeysNavigateResults()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var commandPalette = new CommandPalettePage(Page);

        // Act
        await commandPalette.OpenWithKeyboardAsync();
        await commandPalette.SearchAsync("to");

        var initialIndex = await commandPalette.GetSelectedIndexAsync();

        // Press arrow down
        await commandPalette.PressArrowDownAsync();
        var afterDown = await commandPalette.GetSelectedIndexAsync();

        // Press arrow down again
        await commandPalette.PressArrowDownAsync();
        var afterSecondDown = await commandPalette.GetSelectedIndexAsync();

        // Press arrow up
        await commandPalette.PressArrowUpAsync();
        var afterUp = await commandPalette.GetSelectedIndexAsync();

        // Assert
        initialIndex.ShouldBe(0, "First result should be selected initially");
        afterDown.ShouldBe(1, "Selection should move down");
        afterSecondDown.ShouldBe(2, "Selection should move down again");
        afterUp.ShouldBe(1, "Selection should move up");
    }

    [Fact]
    public async Task EnterKeyNavigatesToSelectedComponent()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var commandPalette = new CommandPalettePage(Page);

        // Act
        await commandPalette.OpenWithKeyboardAsync();
        await commandPalette.SearchAsync("alert");
        await commandPalette.PressEnterAsync();

        // Assert
        // Blazor updates browser history without a full document navigation.
        await Page.WaitForFunctionAsync("() => window.location.pathname === '/components/alert'");
        await commandPalette.WaitForModalCloseAsync();

        var url = Page.Url;
        url.ShouldContain("/components/alert", Case.Insensitive, "Should navigate to Alert component page");

        // Command palette should close
        var isOpen = await commandPalette.IsOpenAsync();
        isOpen.ShouldBeFalse("Command palette should close after selection");
    }

    [Fact]
    public async Task EscapeKeyClosesCommandPalette()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var commandPalette = new CommandPalettePage(Page);

        // Act
        await commandPalette.OpenWithKeyboardAsync();
        var isOpenBefore = await commandPalette.IsOpenAsync();

        await commandPalette.CloseWithEscapeAsync();
        var isOpenAfter = await commandPalette.IsOpenAsync();

        // Assert
        isOpenBefore.ShouldBeTrue("Command palette should be open before Escape");
        isOpenAfter.ShouldBeFalse("Command palette should close after Escape");
    }

    [Fact]
    public async Task NoResultsMessageDisplaysForInvalidSearch()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var commandPalette = new CommandPalettePage(Page);

        // Act
        await commandPalette.OpenWithKeyboardAsync();
        await commandPalette.SearchAsync("xyznonexistent123");

        // Assert
        var hasNoResults = await commandPalette.HasNoResultsMessageAsync();
        hasNoResults.ShouldBeTrue("No results message should be displayed for invalid search");

        var resultCount = await commandPalette.GetResultCountAsync();
        resultCount.ShouldBe(0, "Should have zero results");
    }

    [Fact]
    public async Task QuickActionsDisplayWhenNoSearchQuery()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var commandPalette = new CommandPalettePage(Page);

        // Act
        await commandPalette.OpenWithKeyboardAsync();

        // Assert
        var hasQuickActions = await commandPalette.HasQuickActionsAsync();
        hasQuickActions.ShouldBeTrue("Quick Actions should be displayed when no search query");

        var hasPopularComponents = await commandPalette.HasPopularComponentsAsync();
        hasPopularComponents.ShouldBeTrue("Popular Components should be displayed when no search query");
    }

    [Fact]
    public async Task ToggleThemeQuickActionWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var commandPalette = new CommandPalettePage(Page);
        var basePage = new BasePage(Page);

        // Get initial theme
        var initialTheme = await basePage.GetCurrentThemeAsync();

        // Act
        await commandPalette.OpenWithKeyboardAsync();
        await commandPalette.ClickToggleThemeActionAsync();

        // Wait for theme toggle and modal close
        await Page.WaitForTimeoutAsync(500);

        // Assert
        var afterToggle = await basePage.GetCurrentThemeAsync();
        afterToggle.ShouldNotBe(initialTheme, "Theme should toggle when clicking quick action");

        // Command palette should close
        var isOpen = await commandPalette.IsOpenAsync();
        isOpen.ShouldBeFalse("Command palette should close after theme toggle");
    }
}
