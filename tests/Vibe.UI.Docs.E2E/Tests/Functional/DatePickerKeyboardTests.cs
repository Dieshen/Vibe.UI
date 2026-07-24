using Microsoft.Playwright;
using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Functional;

[Trait("Category", TestCategories.Functional)]
[Trait("Category", "Accessibility")]
public class DatePickerKeyboardTests : E2ETestBase
{
    [Fact]
    public async Task DatePickerSupportsKeyboardSelectionAndRestoresInputFocus()
    {
        await NavigateAndWaitForBlazorAsync("/components/datepicker");

        var preview = Page.Locator("main section").First;
        var input = preview.Locator(".date-input-wrapper input");

        await input.FocusAsync();
        await input.PressAsync("Enter");

        var popup = preview.Locator(".date-popup");
        await popup.WaitForAsync();
        var activeDay = preview.Locator(".date-day[tabindex='0']");
        (await activeDay.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
        (await activeDay.EvaluateAsync<string>("element => getComputedStyle(element).outlineStyle")).ShouldNotBe("none");

        var initialDayId = await activeDay.GetAttributeAsync("id");
        await activeDay.PressAsync("ArrowRight");

        var nextActiveDay = preview.Locator(".date-day[tabindex='0']");
        (await nextActiveDay.GetAttributeAsync("id")).ShouldNotBe(initialDayId);
        (await nextActiveDay.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        await nextActiveDay.PressAsync("Escape");
        await popup.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        var inputId = await input.GetAttributeAsync("id");
        await Page.WaitForFunctionAsync(
            "elementId => document.activeElement?.id === elementId",
            inputId);
        (await input.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        await input.PressAsync("Enter");
        activeDay = preview.Locator(".date-day[tabindex='0']");
        await activeDay.PressAsync("Enter");

        await popup.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        (await input.InputValueAsync()).ShouldNotBeNullOrWhiteSpace();
        await preview.GetByText("Date selected:", new() { Exact = false }).WaitForAsync();
    }

    [Fact]
    public async Task DateRangePickerPublishesAppliedRangeAndRestoresEndInputFocus()
    {
        await NavigateAndWaitForBlazorAsync("/components/daterangepicker");

        var preview = Page.Locator("main section").First;
        var inputs = preview.Locator(".daterange-inputs input");
        var startInput = inputs.Nth(0);
        var endInput = inputs.Nth(1);

        await startInput.FocusAsync();
        await startInput.PressAsync("Enter");

        var popup = preview.Locator(".daterange-popup");
        await popup.WaitForAsync();
        var activeDay = preview.Locator(".daterange-day[tabindex='0']");
        (await activeDay.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        await activeDay.PressAsync("Enter");
        var activeEndDay = preview.Locator(".daterange-calendar").Nth(1).Locator(".daterange-day[tabindex='0']");
        (await activeEndDay.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        await activeEndDay.PressAsync("ArrowRight");
        activeEndDay = preview.Locator(".daterange-calendar").Nth(1).Locator(".daterange-day[tabindex='0']");
        await activeEndDay.PressAsync("Enter");
        await preview.Locator(".daterange-apply-btn").ClickAsync();

        await popup.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        (await startInput.InputValueAsync()).ShouldNotBeNullOrWhiteSpace();
        (await endInput.InputValueAsync()).ShouldNotBeNullOrWhiteSpace();
        await preview.GetByText("Selected Range:", new() { Exact = false }).WaitForAsync();

        await endInput.FocusAsync();
        await endInput.PressAsync("Enter");
        await preview.Locator(".daterange-popup").WaitForAsync();
        await Page.Keyboard.PressAsync("Escape");
        await preview.Locator(".daterange-popup").WaitForAsync(new() { State = WaitForSelectorState.Detached });
        var endInputId = await endInput.GetAttributeAsync("id");
        await Page.WaitForFunctionAsync(
            "elementId => document.activeElement?.id === elementId",
            endInputId);
        (await endInput.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
    }

    [Theory]
    [InlineData("/components/datepicker", ".date-input-wrapper input", ".date-day[tabindex='0']")]
    [InlineData("/components/daterangepicker", ".daterange-inputs input", ".daterange-day[tabindex='0']")]
    public async Task VerticalDayGridArrowsDoNotScrollAndUnrelatedKeysRemainUnblocked(
        string route,
        string inputSelector,
        string activeDaySelector)
    {
        await NavigateAndWaitForBlazorAsync(route);

        var preview = Page.Locator("main section").First;
        var input = preview.Locator(inputSelector).First;
        await input.FocusAsync();
        await input.PressAsync("Enter");

        var activeDay = preview.Locator(activeDaySelector);
        await activeDay.WaitForAsync();
        (await activeDay.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        var initialDayId = await activeDay.GetAttributeAsync("id");
        initialDayId.ShouldNotBeNullOrWhiteSpace();
        var initialScrollPosition = await Page.EvaluateAsync<int>(
            """
            () => {
                const scrollingElement = document.scrollingElement;
                document.documentElement.style.scrollBehavior = 'auto';
                const target = Math.floor((scrollingElement.scrollHeight - scrollingElement.clientHeight) / 2);
                scrollingElement.scrollTop = target;
                return scrollingElement.scrollTop;
            }
            """);
        initialScrollPosition.ShouldBeGreaterThan(0, "The docs page must be scrollable for this regression test.");

        await Page.Keyboard.PressAsync("ArrowDown");
        await Page.WaitForFunctionAsync(
            "elementId => document.activeElement?.id !== elementId",
            initialDayId);
        await Page.WaitForTimeoutAsync(150);
        (await Page.EvaluateAsync<int>("() => document.scrollingElement.scrollTop"))
            .ShouldBe(initialScrollPosition);

        await Page.Keyboard.PressAsync("ArrowUp");
        await Page.WaitForFunctionAsync(
            "elementId => document.activeElement?.id === elementId",
            initialDayId);
        await Page.WaitForTimeoutAsync(150);
        (await Page.EvaluateAsync<int>("() => document.scrollingElement.scrollTop"))
            .ShouldBe(initialScrollPosition);

        activeDay = preview.Locator(activeDaySelector);
        var unrelatedKeyWasPrevented = await activeDay.EvaluateAsync<bool>(
            """
            element => {
                const event = new KeyboardEvent('keydown', {
                    key: 'PageDown',
                    bubbles: true,
                    cancelable: true
                });
                element.dispatchEvent(event);
                return event.defaultPrevented;
            }
            """);
        unrelatedKeyWasPrevented.ShouldBeFalse("Only handled day-grid navigation keys should be canceled.");
    }
}
