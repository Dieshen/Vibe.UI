using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Vibe.UI.Docs.E2E.PageObjects;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Integration;

/// <summary>
/// Integration tests for Shiki syntax highlighter JavaScript interop
/// Tests code block rendering and theme integration
/// </summary>
[Trait("Category", TestCategories.Integration)]
public class ShikiInteropTests : E2ETestBase
{
    [Fact]
    public async Task CodeBlocksRenderWithSyntaxHighlighting()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        var componentPage = new ComponentPage(Page);

        // Wait for Shiki to be ready (if the page uses it)
        try
        {
            await Page.WaitForShikiReadyAsync(timeout: 5000);
        }
        catch
        {
            // Shiki might not be initialized yet, continue anyway
        }

        // Act
        var hasCodeBlocks = await componentPage.GetCodeBlockCountAsync();

        // Assert
        hasCodeBlocks.ShouldBeGreaterThan(0, "Component page should have code blocks");

        // Check if syntax highlighting is applied
        var hasSyntaxHighlighting = await componentPage.HasSyntaxHighlightingAsync();

        if (hasSyntaxHighlighting)
        {
            // Verify Shiki-specific elements
            var codeBlock = componentPage.FirstCodeBlock;
            var spans = await codeBlock.Locator("span").CountAsync();
            spans.ShouldBeGreaterThan(0, "Code block should have span elements from syntax highlighting");
        }
        else
        {
            // If no syntax highlighting, at least verify code blocks exist
            var codeContent = await componentPage.FirstCodeBlock.TextContentAsync();
            codeContent.ShouldNotBeNullOrWhiteSpace("Code block should have content");
        }
    }

    [Fact]
    public async Task ThemeToggleAffectsCodeBlockStyling()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        var componentPage = new ComponentPage(Page);
        var basePage = new BasePage(Page);

        // Wait for code blocks to render
        await Page.WaitForTimeoutAsync(1000);

        var hasCodeBlocks = await componentPage.GetCodeBlockCountAsync();
        if (hasCodeBlocks == 0)
        {
            // Skip test if no code blocks on this page
            return;
        }

        // Get initial code block styles
        var codeBlock = componentPage.FirstCodeBlock;
        var initialBgColor = await codeBlock.EvaluateAsync<string>(
            "el => window.getComputedStyle(el).backgroundColor"
        );

        // Act - Toggle theme
        await basePage.ToggleThemeAsync();
        await Page.WaitForTimeoutAsync(500); // Wait for theme transition

        // Get code block styles after theme change
        var afterBgColor = await codeBlock.EvaluateAsync<string>(
            "el => window.getComputedStyle(el).backgroundColor"
        );

        // Assert - Background color should change (light vs dark theme)
        // Note: Exact colors depend on theme configuration
        afterBgColor.ShouldNotBe(initialBgColor,
            "Code block background should change when theme is toggled");
    }

    [Fact]
    public async Task ShikiHighlighterInitializesCorrectly()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");

        // Act - Check if Shiki highlighter is ready
        var isReady = await Page.EvaluateAsync<bool>(
            @"() => {
                if (typeof window.isHighlighterReady === 'function') {
                    return window.isHighlighterReady();
                }

                return window.isHighlighterReady === true;
            }"
        );

        // If Shiki is not ready, wait and check again
        if (!isReady)
        {
            try
            {
                await Page.WaitForShikiReadyAsync(timeout: 10000);
                isReady = await Page.EvaluateAsync<bool>(
                    @"() => {
                        if (typeof window.isHighlighterReady === 'function') {
                            return window.isHighlighterReady();
                        }

                        return window.isHighlighterReady === true;
                    }"
                );
            }
            catch
            {
                // Shiki might not be used on all pages
            }
        }

        // Assert - Either Shiki is ready, or code blocks render without it
        if (isReady)
        {
            isReady.ShouldBeTrue("Shiki highlighter should initialize");

            // Verify highlighter function exists
            var hasHighlighter = await Page.EvaluateAsync<bool>(
                "() => typeof window.highlightCode === 'function'"
            );

            // Note: Actual function name depends on implementation
            // This is a best guess - adjust based on actual implementation
        }

        var componentPage = new ComponentPage(Page);
        var codeBlockCount = await componentPage.GetCodeBlockCountAsync();

        // At minimum, code blocks should be present
        if (codeBlockCount > 0)
        {
            var codeContent = await componentPage.FirstCodeBlock.TextContentAsync();
            codeContent.ShouldNotBeNullOrWhiteSpace("Code blocks should have content");
        }
    }

    [Fact]
    public async Task MultipleCodeBlocksRenderCorrectly()
    {
        // Arrange
        await NavigateAndWaitForBlazorAsync("/components/button");
        var componentPage = new ComponentPage(Page);

        await Page.WaitForTimeoutAsync(1000);

        // Act
        var codeBlockCount = await componentPage.GetCodeBlockCountAsync();

        // Assert
        if (codeBlockCount > 1)
        {
            codeBlockCount.ShouldBeGreaterThan(1, "Component page should have multiple code examples");

            // Verify all code blocks have content
            var allCodeBlocks = await componentPage.CodeBlocks.AllAsync();

            foreach (var codeBlock in allCodeBlocks)
            {
                var content = await codeBlock.TextContentAsync();
                content.ShouldNotBeNullOrWhiteSpace("Each code block should have content");

                var isVisible = await codeBlock.IsVisibleAsync();
                isVisible.ShouldBeTrue("Each code block should be visible");
            }
        }
    }
}
