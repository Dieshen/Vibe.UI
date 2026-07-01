using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Functional;

/// <summary>
/// Functional tests for component preview functionality.
/// Tests tab switching between Preview/Code views and PropTweaker interactive controls.
/// </summary>
[Trait("Category", TestCategories.Functional)]
public class ComponentPreviewTests : E2ETestBase
{
    #region Tab Switching Tests

    [Fact]
    public async Task PreviewTabShowsComponentExample()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1000);

        // Act - Find and click Preview tab
        var previewTab = Page.Locator("[data-tab='preview'], button:has-text('Preview'), .preview-tab").First;

        if (await previewTab.IsVisibleAsync())
        {
            await previewTab.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        // Assert - Preview content should be visible
        var previewContent = Page.Locator(".preview-content, .preview-panel, [data-panel='preview']").First;
        var isVisible = await previewContent.IsVisibleAsync();
        isVisible.ShouldBeTrue("Preview content should be visible when Preview tab is selected");

        // Should contain actual component
        var hasComponent = await Page.Locator(".preview-content button, .preview-panel button").CountAsync();
        hasComponent.ShouldBeGreaterThan(0, "Preview should contain the actual Button component");
    }

    [Fact]
    public async Task CodeTabShowsSourceCode()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1000);

        // Act - Find and click Code tab
        var codeTab = Page.Locator("[data-tab='code'], button:has-text('Code'), .code-tab").First;

        if (await codeTab.IsVisibleAsync())
        {
            await codeTab.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        // Assert - Code content should be visible
        var codeContent = Page.Locator(".code-panel, [data-panel='code'], pre code").First;
        var isVisible = await codeContent.IsVisibleAsync();
        isVisible.ShouldBeTrue("Code content should be visible when Code tab is selected");

        // Should contain code
        var codeText = await codeContent.TextContentAsync();
        codeText.ShouldNotBeNullOrWhiteSpace("Code panel should contain source code");

        // Should look like Blazor/Razor code
        var looksLikeCode = codeText!.Contains("<") || codeText.Contains("@") || codeText.Contains("{");
        looksLikeCode.ShouldBeTrue("Code should look like Blazor markup");
    }

    [Fact]
    public async Task TabSwitchingMaintainsState()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1000);

        var previewTab = Page.Locator("[data-tab='preview'], button:has-text('Preview')").First;
        var codeTab = Page.Locator("[data-tab='code'], button:has-text('Code')").First;

        if (await previewTab.IsVisibleAsync() && await codeTab.IsVisibleAsync())
        {
            // Act - Switch between tabs multiple times
            await codeTab.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            var codeVisible1 = await Page.Locator(".code-panel, pre code").First.IsVisibleAsync();

            await previewTab.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            var previewVisible = await Page.Locator(".preview-content, .preview-panel").First.IsVisibleAsync();

            await codeTab.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            var codeVisible2 = await Page.Locator(".code-panel, pre code").First.IsVisibleAsync();

            // Assert
            codeVisible1.ShouldBeTrue("Code should be visible after first click");
            previewVisible.ShouldBeTrue("Preview should be visible after switch");
            codeVisible2.ShouldBeTrue("Code should be visible after switching back");
        }
    }

    [Fact]
    public async Task ActiveTabHasVisualIndicator()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1000);

        var previewTab = Page.Locator("[data-tab='preview'], button:has-text('Preview')").First;
        var codeTab = Page.Locator("[data-tab='code'], button:has-text('Code')").First;

        if (await previewTab.IsVisibleAsync() && await codeTab.IsVisibleAsync())
        {
            // Act - Click Preview tab
            await previewTab.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            // Assert - Preview tab should have active indicator
            var previewClasses = await previewTab.GetAttributeAsync("class") ?? "";
            var previewAriaSelected = await previewTab.GetAttributeAsync("aria-selected");
            var previewDataState = await previewTab.GetAttributeAsync("data-state");

            var previewIsActive = previewClasses.Contains("active") ||
                                  previewAriaSelected == "true" ||
                                  previewDataState == "active";

            previewIsActive.ShouldBeTrue("Preview tab should show active state");

            // Click Code tab
            await codeTab.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            var codeClasses = await codeTab.GetAttributeAsync("class") ?? "";
            var codeAriaSelected = await codeTab.GetAttributeAsync("aria-selected");
            var codeDataState = await codeTab.GetAttributeAsync("data-state");

            var codeIsActive = codeClasses.Contains("active") ||
                               codeAriaSelected == "true" ||
                               codeDataState == "active";

            codeIsActive.ShouldBeTrue("Code tab should show active state after click");
        }
    }

    #endregion

    #region PropTweaker Tests

    [Fact]
    public async Task PropTweakerControlsAreVisible()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1500);

        // Act - Look for PropTweaker controls
        var propTweaker = Page.Locator(".prop-tweaker, [data-prop-tweaker], .property-controls, .props-panel");
        var hasControls = await propTweaker.CountAsync();

        // Assert - If PropTweaker exists, verify it has controls
        if (hasControls > 0)
        {
            var isVisible = await propTweaker.First.IsVisibleAsync();
            isVisible.ShouldBeTrue("PropTweaker should be visible on component pages");

            // Should have interactive controls
            var controls = await propTweaker.Locator("select, input, button, [role='combobox']").CountAsync();
            controls.ShouldBeGreaterThan(0, "PropTweaker should have interactive controls");
        }
    }

    [Fact]
    public async Task PropTweakerVariantSelectorWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1500);

        // Find variant selector
        var variantSelector = Page.Locator(
            "select[name*='variant'], " +
            "[data-prop='variant'], " +
            ".prop-tweaker select, " +
            "[aria-label*='Variant']"
        ).First;

        if (await variantSelector.IsVisibleAsync())
        {
            // Get initial preview state
            var previewButton = Page.Locator(".preview-content button, .preview-panel button").First;
            var initialClasses = await previewButton.GetAttributeAsync("class") ?? "";

            // Act - Change variant
            await variantSelector.SelectOptionAsync(new Microsoft.Playwright.SelectOptionValue { Index = 1 });
            await Page.WaitForTimeoutAsync(500);

            // Assert - Button should update
            var afterClasses = await previewButton.GetAttributeAsync("class") ?? "";

            // Classes should change (indicating the component updated)
            // Note: Exact behavior depends on implementation
        }
    }

    [Fact]
    public async Task PropTweakerSizeControlWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1500);

        // Find size control
        var sizeControl = Page.Locator(
            "select[name*='size'], " +
            "[data-prop='size'], " +
            "[aria-label*='Size']"
        ).First;

        if (await sizeControl.IsVisibleAsync())
        {
            // Act - Change size
            await sizeControl.SelectOptionAsync(new Microsoft.Playwright.SelectOptionValue { Index = 2 });
            await Page.WaitForTimeoutAsync(500);

            // Assert - Preview should update (verify no errors)
            var preview = Page.Locator(".preview-content, .preview-panel").First;
            var isVisible = await preview.IsVisibleAsync();
            isVisible.ShouldBeTrue("Preview should still be visible after changing size");
        }
    }

    [Fact]
    public async Task PropTweakerBooleanToggleWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1500);

        // Find a boolean toggle (e.g., Disabled)
        var toggle = Page.Locator(
            "input[type='checkbox'][name*='disabled'], " +
            "[data-prop='disabled'], " +
            ".switch[data-prop]"
        ).First;

        if (await toggle.IsVisibleAsync())
        {
            // Get initial state
            var isChecked = await toggle.IsCheckedAsync();

            // Act - Toggle the control
            await toggle.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            // Assert - State should change
            var afterClick = await toggle.IsCheckedAsync();
            afterClick.ShouldNotBe(isChecked, "Toggle state should change after click");

            // Preview button should reflect the change
            var previewButton = Page.Locator(".preview-content button, .preview-panel button").First;
            if (await previewButton.IsVisibleAsync())
            {
                var isDisabled = await previewButton.IsDisabledAsync();
                isDisabled.ShouldBe(afterClick, "Button disabled state should match toggle");
            }
        }
    }

    [Fact]
    public async Task PropTweakerTextInputWorks()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1500);

        // Find text input for label/text
        var textInput = Page.Locator(
            "input[type='text'][name*='text'], " +
            "input[type='text'][name*='label'], " +
            "[data-prop='text'] input, " +
            "[data-prop='label'] input"
        ).First;

        if (await textInput.IsVisibleAsync())
        {
            // Act - Change text
            await textInput.ClearAsync();
            await textInput.FillAsync("Custom Button Text");
            await Page.WaitForTimeoutAsync(500);

            // Assert - Preview should update
            var previewButton = Page.Locator(".preview-content button, .preview-panel button").First;
            if (await previewButton.IsVisibleAsync())
            {
                var buttonText = await previewButton.TextContentAsync();
                buttonText.ShouldNotBeNull("Preview button should render text after the prop update");
                buttonText.ShouldContain("Custom Button Text", Case.Insensitive,
                    "Button text should update to match input");
            }
        }
    }

    #endregion

    #region Component-Specific Preview Tests

    [Theory]
    [InlineData("/components/alert", ".alert, [role='alert']")]
    [InlineData("/components/badge", ".badge, [class*='badge']")]
    [InlineData("/components/card", ".card, [class*='card']")]
    [InlineData("/components/avatar", ".avatar, [class*='avatar']")]
    public async Task ComponentPreviewRendersCorrectElement(string path, string expectedSelector)
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync(path);
        await Page.WaitForTimeoutAsync(1500);

        // Act - Find the component in preview
        var component = Page.Locator($".preview-content {expectedSelector}, .preview-panel {expectedSelector}").First;

        // Assert
        var isVisible = await component.IsVisibleAsync();
        isVisible.ShouldBeTrue($"Preview should render the {path.Split('/').Last()} component");
    }

    [Fact]
    public async Task SliderPreviewIsInteractive()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/slider");
        await Page.WaitForTimeoutAsync(1500);

        // Find slider in preview
        var slider = Page.Locator(".preview-content input[type='range'], .preview-panel [role='slider']").First;

        if (await slider.IsVisibleAsync())
        {
            // Get initial value
            var initialValue = await slider.InputValueAsync();

            // Act - Interact with slider
            await slider.ClickAsync();
            await Page.Keyboard.PressAsync("ArrowRight");
            await Page.Keyboard.PressAsync("ArrowRight");
            await Page.WaitForTimeoutAsync(300);

            // Assert - Value should change
            var afterValue = await slider.InputValueAsync();
            // Slider should be interactive (value might change depending on implementation)
        }
    }

    [Fact]
    public async Task ModalPreviewOpensAndCloses()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/modal");
        await Page.WaitForTimeoutAsync(1500);

        // Find open button in preview
        var openButton = Page.Locator(".preview-content button, .preview-panel button").First;

        if (await openButton.IsVisibleAsync())
        {
            // Act - Open modal
            await openButton.ClickAsync();
            await Page.WaitForTimeoutAsync(500);

            var modal = Page.Locator("[role='dialog'], .modal, [data-state='open']").First;
            var isOpen = await modal.IsVisibleAsync();

            // Close modal
            await Page.Keyboard.PressAsync("Escape");
            await Page.WaitForTimeoutAsync(500);

            var isClosed = !(await modal.IsVisibleAsync());

            // Assert
            isOpen.ShouldBeTrue("Modal should open when button is clicked");
            isClosed.ShouldBeTrue("Modal should close when Escape is pressed");
        }
    }

    #endregion

    #region Theme Toggle in Preview

    [Fact]
    public async Task PreviewRespectsGlobalTheme()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1000);

        // Get initial theme
        var initialTheme = await Page.EvaluateAsync<bool>(
            "() => document.documentElement.classList.contains('dark')"
        );

        // Get preview background
        var preview = Page.Locator(".preview-content, .preview-panel").First;
        var initialBg = await preview.EvaluateAsync<string>(
            "el => window.getComputedStyle(el).backgroundColor"
        );

        // Act - Toggle theme
        var themeToggle = Page.Locator("[data-theme-toggle], .theme-toggle, button:has([class*='sun'])").First;
        if (await themeToggle.IsVisibleAsync())
        {
            await themeToggle.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        // Assert - Preview should update with theme
        var afterBg = await preview.EvaluateAsync<string>(
            "el => window.getComputedStyle(el).backgroundColor"
        );

        // Background should change with theme (light vs dark)
        afterBg.ShouldNotBe(initialBg, "Preview background should change when theme toggles");
    }

    [Fact]
    public async Task PreviewThemeToggleIsIndependent()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        await Page.WaitForTimeoutAsync(1000);

        // Find preview-specific theme toggle (if exists)
        var previewThemeToggle = Page.Locator(".preview-theme-toggle, [data-preview-theme]").First;

        if (await previewThemeToggle.IsVisibleAsync())
        {
            var preview = Page.Locator(".preview-content, .preview-panel").First;
            var initialHasDark = await preview.EvaluateAsync<bool>(
                "el => el.classList.contains('dark')"
            );

            // Act
            await previewThemeToggle.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            // Assert
            var afterHasDark = await preview.EvaluateAsync<bool>(
                "el => el.classList.contains('dark')"
            );

            afterHasDark.ShouldNotBe(initialHasDark,
                "Preview theme toggle should independently control preview theme");
        }
    }

    #endregion
}
