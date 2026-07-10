using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Smoke;

/// <summary>
/// Smoke tests that verify each component page loads without errors.
/// These tests ensure basic rendering works for all 57 component pages.
/// </summary>
[Trait("Category", TestCategories.Smoke)]
public class AllComponentsRenderTests : E2ETestBase
{
    private readonly List<string> _consoleErrors = new();

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Capture console errors
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
    }

    #region Layout Components

    [Theory]
    [InlineData("accordion", "Accordion")]
    [InlineData("aspectratio", "Aspect Ratio")]
    [InlineData("card", "Card")]
    [InlineData("collapsible", "Collapsible")]
    [InlineData("container", "Container")]
    [InlineData("divider", "Divider")]
    [InlineData("grid", "Grid")]
    [InlineData("separator", "Separator")]
    [InlineData("stack", "Stack")]
    public async Task LayoutComponentPageRenders(string componentPath, string componentName)
    {
        await VerifyComponentPageRenders(componentPath, componentName);
    }

    #endregion

    #region Form Components

    [Theory]
    [InlineData("button", "Button")]
    [InlineData("checkbox", "Checkbox")]
    [InlineData("colorpicker", "Color Picker")]
    [InlineData("datepicker", "Date Picker")]
    [InlineData("daterangepicker", "Date Range Picker")]
    [InlineData("dropdown", "Dropdown")]
    [InlineData("fileupload", "File Upload")]
    [InlineData("form", "Form")]
    [InlineData("formfield", "Form Field")]
    [InlineData("input", "Input")]
    [InlineData("radiogroup", "Radio Group")]
    [InlineData("select", "Select")]
    [InlineData("slider", "Slider")]
    [InlineData("switch", "Switch")]
    [InlineData("textarea", "Textarea")]
    [InlineData("timepicker", "Time Picker")]
    [InlineData("validatedinput", "Validated Input")]
    public async Task FormComponentPageRenders(string componentPath, string componentName)
    {
        await VerifyComponentPageRenders(componentPath, componentName);
    }

    #endregion

    #region Display Components

    [Theory]
    [InlineData("alert", "Alert")]
    [InlineData("avatar", "Avatar")]
    [InlineData("badge", "Badge")]
    [InlineData("breadcrumb", "Breadcrumb")]
    [InlineData("calendar", "Calendar")]
    [InlineData("carousel", "Carousel")]
    [InlineData("chart", "Chart")]
    [InlineData("link", "Link")]
    [InlineData("notification", "Notification")]
    [InlineData("pagination", "Pagination")]
    [InlineData("progress", "Progress")]
    [InlineData("skeleton", "Skeleton")]
    [InlineData("spinner", "Spinner")]
    [InlineData("stepper", "Stepper")]
    [InlineData("table", "Table")]
    [InlineData("tag", "Tag")]
    [InlineData("tooltip", "Tooltip")]
    public async Task DisplayComponentPageRenders(string componentPath, string componentName)
    {
        await VerifyComponentPageRenders(componentPath, componentName);
    }

    #endregion

    #region Overlay Components

    [Theory]
    [InlineData("dialog", "Dialog")]
    [InlineData("drawer", "Drawer")]
    [InlineData("menu", "Menu")]
    [InlineData("modal", "Modal")]
    [InlineData("popover", "Popover")]
    [InlineData("toast", "Toast")]
    public async Task OverlayComponentPageRenders(string componentPath, string componentName)
    {
        await VerifyComponentPageRenders(componentPath, componentName);
    }

    #endregion

    #region Navigation Components

    [Theory]
    [InlineData("tabs", "Tabs")]
    [InlineData("themetoggle", "Theme Toggle")]
    public async Task NavigationComponentPageRenders(string componentPath, string componentName)
    {
        await VerifyComponentPageRenders(componentPath, componentName);
    }

    #endregion

    #region Data Components

    [Theory]
    [InlineData("datagrid", "Data Grid")]
    [InlineData("treeview", "Tree View")]
    [InlineData("virtualscroll", "Virtual Scroll")]
    public async Task DataComponentPageRenders(string componentPath, string componentName)
    {
        await VerifyComponentPageRenders(componentPath, componentName);
    }

    #endregion

    #region Advanced Components

    [Theory]
    [InlineData("dragdrop", "Drag Drop")]
    [InlineData("kanbanboard", "Kanban Board")]
    [InlineData("richtexteditor", "Rich Text Editor")]
    public async Task AdvancedComponentPageRenders(string componentPath, string componentName)
    {
        await VerifyComponentPageRenders(componentPath, componentName);
    }

    #endregion

    #region Page Content Validation

    [Fact]
    public async Task AllComponentPagesHaveTitle()
    {
        var componentPaths = new[]
        {
            "button", "card", "alert", "modal", "toast", "tabs",
            "input", "checkbox", "select", "slider", "switch"
        };

        foreach (var path in componentPaths)
        {
            _consoleErrors.Clear();
            await NavigateAndWaitForBlazorAsync($"/components/{path}");

            // Verify page has a title
            var titleLocator = Page.Locator("h1").First;
            try
            {
                await titleLocator.WaitForAsync(new()
                {
                    State = Microsoft.Playwright.WaitForSelectorState.Visible,
                    Timeout = 30000
                });
            }
            catch (TimeoutException ex)
            {
                throw new TimeoutException(
                    $"Timed out waiting for title on component page {path}. Current URL: {Page.Url}",
                    ex);
            }

            var title = await titleLocator.TextContentAsync();
            title.ShouldNotBeNullOrWhiteSpace($"Component page {path} should have a title");
        }
    }

    [Fact]
    public async Task AllComponentPagesHaveExamples()
    {
        var componentPaths = new[]
        {
            "button", "card", "alert", "input", "checkbox"
        };

        foreach (var path in componentPaths)
        {
            _consoleErrors.Clear();
            await NavigateAndWaitForBlazorAsync($"/components/{path}");

            // Wait for content to load
            await Page.WaitForTimeoutAsync(1000);

            // Verify page has preview/example sections
            // The docs site uses LivePreview component and example sections with bg-zinc backgrounds
            var hasExamples = await Page.Locator(
                "section, .bg-zinc-50, .dark\\:bg-zinc-800\\/50, [class*='preview'], [class*='example']"
            ).CountAsync();

            hasExamples.ShouldBeGreaterThan(0, $"Component page {path} should have examples");
        }
    }

    [Fact]
    public async Task AllComponentPagesHaveCodeBlocks()
    {
        var componentPaths = new[]
        {
            "button", "card", "alert", "input"
        };

        foreach (var path in componentPaths)
        {
            _consoleErrors.Clear();
            await NavigateAndWaitForBlazorAsync($"/components/{path}");

            // Wait for code blocks to render
            await Page.WaitForTimeoutAsync(1500);

            // Verify page has code blocks
            var codeBlocks = await Page.Locator("pre, code, .code-block, [class*='shiki']").CountAsync();

            codeBlocks.ShouldBeGreaterThan(0, $"Component page {path} should have code examples");
        }
    }

    #endregion

    #region Helper Methods

    private async Task VerifyComponentPageRenders(string componentPath, string componentName)
    {
        // Clear errors
        _consoleErrors.Clear();

        // Navigate
        await NavigateAndWaitForBlazorAsync($"/components/{componentPath}");

        // Wait for content to load - Blazor WASM needs time to render
        await Page.WaitForTimeoutAsync(2000);

        // Verify page loaded by checking for actual content (h1 title, sections, etc.)
        var pageTitle = Page.Locator("h1").First;
        var hasTitleVisible = await pageTitle.IsVisibleAsync();

        // Also check for the component's main section
        var mainContent = Page.Locator("main, .docs-container, .max-w-4xl").First;
        var hasMainContent = await mainContent.IsVisibleAsync();

        // Page should have either title visible or main content rendered
        (hasTitleVisible || hasMainContent).ShouldBeTrue(
            $"{componentName} page at /components/{componentPath} should render with visible content");

        // Verify no critical errors
        var criticalErrors = _consoleErrors
            .Where(e => !IsIgnorableError(e))
            .ToList();

        criticalErrors.ShouldBeEmpty(
            $"{componentName} page should have no console errors. Found:\n{string.Join("\n", criticalErrors)}");
    }

    private static bool IsIgnorableError(string error)
    {
        var ignorable = new[]
        {
            "chrome-extension://",
            "moz-extension://",
            "Failed to load source map",
            "DevTools failed to load",
            "favicon.ico",
            "ResizeObserver loop"
        };

        return ignorable.Any(i => error.Contains(i, StringComparison.OrdinalIgnoreCase));
    }

    #endregion
}
