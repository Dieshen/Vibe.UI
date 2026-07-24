using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Vibe.UI.Docs.E2E.PageObjects;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Smoke;

/// <summary>
/// Smoke tests for basic navigation and critical functionality
/// These tests validate that the application loads and core features work
/// </summary>
[Trait("Category", TestCategories.Smoke)]
public class NavigationSmokeTests : E2ETestBase
{
    [Fact]
    public async Task HomePageLoadsSuccessfully()
    {
        // Arrange
        var homePage = new HomePage(Page);

        // Act
        await NavigateAndWaitForBlazorAsync("/");

        // Assert
        var isLoaded = await homePage.IsLoadedAsync();
        isLoaded.ShouldBeTrue("Home page should load with logo and hero visible");

        var logo = homePage.Logo;
        var logoVisible = await logo.IsVisibleAsync();
        logoVisible.ShouldBeTrue("Logo should be visible");

        var heroTitle = await homePage.GetHeroTitleAsync();
        heroTitle.ShouldNotBeNullOrWhiteSpace("Hero title should be present");
    }

    [Fact]
    public async Task SidebarNavigationWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");

        // Act - Navigate to a component page via the Components index
        await Page.GotoAsync($"{BaseUrl}/components");
        await Page.WaitForBlazorReadyAsync();

        var componentLink = Page.Locator("a[data-component-route^='/components/']").First;
        await componentLink.ClickAsync();

        // Wait for navigation
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        // Assert
        var currentUrl = Page.Url;
        currentUrl.ShouldContain("/components/");

        var componentPage = new ComponentPage(Page);
        var title = await componentPage.GetComponentTitleAsync();
        title.ShouldNotBeNullOrWhiteSpace("Component page should have a title");
    }

    [Fact]
    public async Task CommandPaletteOpensWithKeyboardShortcut()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var commandPalette = new CommandPalettePage(Page);

        // Act
        await commandPalette.OpenWithKeyboardAsync();

        // Assert
        var isOpen = await commandPalette.IsOpenAsync();
        isOpen.ShouldBeTrue("Command palette should open when Ctrl+K is pressed");

        var searchInput = commandPalette.SearchInput;
        var searchInputVisible = await searchInput.IsVisibleAsync();
        searchInputVisible.ShouldBeTrue("Search input should be visible");

        // Verify search input is focused
        await Page.WaitForFunctionAsync(
            "() => document.activeElement?.getAttribute('placeholder') === 'Search components...'");
        var isFocused = await searchInput.EvaluateAsync<bool>("el => el === document.activeElement");
        isFocused.ShouldBeTrue("Search input should be auto-focused");
    }

    [Fact]
    public async Task ThemeTogglePersistsAcrossRefresh()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/");
        var basePage = new BasePage(Page);

        // Act - Get initial theme
        var initialTheme = await basePage.GetCurrentThemeAsync();

        // Toggle theme
        await basePage.ToggleThemeAsync();
        var afterToggle = await basePage.GetCurrentThemeAsync();

        // Verify theme changed
        afterToggle.ShouldNotBe(initialTheme, "Theme should change after toggle");

        // Refresh page
        await Page.ReloadAsync();
        await Page.WaitForBlazorReadyAsync();

        // Assert - Theme should persist
        var afterRefresh = await basePage.GetCurrentThemeAsync();
        afterRefresh.ShouldBe(afterToggle, "Theme should persist after page refresh");

        // Verify localStorage
        var storedTheme = await basePage.GetStoredThemeAsync();
        storedTheme.ShouldBe(afterToggle, "Theme should be stored in localStorage");
    }
}
