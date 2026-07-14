using Microsoft.Playwright;
using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Functional;

[Trait("Category", TestCategories.Functional)]
[Trait("Category", "Accessibility")]
public class MenuKeyboardTests : E2ETestBase
{
    [Fact]
    public async Task DropdownMenuSupportsRovingFocusTypeaheadAndEscapeRestoration()
    {
        await NavigateAndWaitForBlazorAsync("/components/dropdown");

        var trigger = Page.Locator(".dropdown-trigger").First;
        await trigger.FocusAsync();
        await trigger.PressAsync("ArrowDown");

        var menu = Page.Locator(".dropdown-content[role='menu']").First;
        await menu.WaitForAsync();
        await ExpectActiveTextAsync("Profile");

        await Page.Keyboard.PressAsync("ArrowDown");
        await ExpectActiveTextAsync("Settings");
        await Page.Keyboard.PressAsync("End");
        await ExpectActiveTextAsync("Log out");
        await Page.Keyboard.PressAsync("Home");
        await ExpectActiveTextAsync("Profile");
        await Page.Keyboard.PressAsync("s");
        await ExpectActiveTextAsync("Settings");

        await Page.Keyboard.PressAsync("Escape");
        await menu.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        await WaitForFocusedAsync(trigger);

        await trigger.PressAsync("ArrowUp");
        await menu.WaitForAsync();
        await ExpectActiveTextAsync("Log out");

        await Page.Keyboard.PressAsync("Tab");
        await menu.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        (await trigger.GetAttributeAsync("aria-expanded")).ShouldBe("false");
    }

    [Fact]
    public async Task ContextMenuSupportsKeyboardPositioningNavigationAndEscapeRestoration()
    {
        await NavigateAndWaitForBlazorAsync("/components/dropdown");

        var trigger = Page.Locator(".context-trigger").First;
        await trigger.FocusAsync();
        var triggerBounds = await trigger.BoundingBoxAsync();
        triggerBounds.ShouldNotBeNull();

        await trigger.PressAsync("Shift+F10");

        var menu = Page.Locator(".context-content[role='menu']").First;
        await menu.WaitForAsync();
        await ExpectActiveTextAsync("Open");

        var menuBounds = await menu.BoundingBoxAsync();
        menuBounds.ShouldNotBeNull();
        menuBounds.Y.ShouldBeGreaterThanOrEqualTo(triggerBounds.Y);

        await Page.Keyboard.PressAsync("ArrowDown");
        await ExpectActiveTextAsync("Rename");
        await Page.Keyboard.PressAsync("d");
        await ExpectActiveTextAsync("Download");

        await Page.Keyboard.PressAsync("Escape");
        await menu.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        await WaitForFocusedAsync(trigger);

        await trigger.PressAsync("Shift+F10");
        await menu.WaitForAsync();
        await ExpectActiveTextAsync("Open");
        await Page.Keyboard.PressAsync("Enter");
        await menu.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        await WaitForFocusedAsync(trigger);
    }

    private async Task ExpectActiveTextAsync(string expectedText)
    {
        await Page.WaitForFunctionAsync(
            "text => document.activeElement?.textContent?.trim().includes(text) === true",
            expectedText);
    }

    private async Task WaitForFocusedAsync(ILocator locator)
    {
        var elementId = await locator.GetAttributeAsync("id");
        elementId.ShouldNotBeNullOrWhiteSpace();
        await Page.WaitForFunctionAsync(
            "id => document.activeElement?.id === id",
            elementId);
    }
}
