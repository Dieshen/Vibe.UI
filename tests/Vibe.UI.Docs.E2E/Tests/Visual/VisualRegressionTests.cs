using Microsoft.Playwright;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Visual;

[Trait("Category", TestCategories.Visual)]
public class VisualRegressionTests : E2ETestBase
{
    private const string StableScreenshotStyles =
        "*, *::before, *::after { animation: none !important; transition: none !important; caret-color: transparent !important; }";

    private static readonly ViewportProfile[] Viewports =
    [
        new("desktop", 1280, 900),
        new("mobile", 390, 844)
    ];

    private static readonly ThemeProfile[] Themes =
    [
        new("light", ColorScheme.Light),
        new("dark", ColorScheme.Dark)
    ];

    private static readonly SnapshotSurface[] Surfaces =
    [
        new("button", "/components/button", page => page.Locator(".live-preview").First),
        new("form-field", "/components/formfield", page => page.Locator("main section").First.Locator(":scope > div").First),
        new("data-table", "/components/datatable", page => page.Locator("main section").First.Locator(":scope > div").First),
        new("kanban-board", "/components/kanbanboard", page => page.Locator("main section").First.Locator(":scope > div").First),
        new("sidebar", "/components/sidebar", page => page.Locator("main section").First.Locator(":scope > div").First),
        new(
            "alert-dialog-open",
            "/components/alertdialog",
            page => page.Locator(".vibe-alert-dialog").First,
            page => page.GetByRole(AriaRole.Button, new() { Name = "Delete project", Exact = true }).First.ClickAsync())
    ];

    [Fact]
    public async Task FlagshipComponentsMatchReviewedBaselines()
    {
        if (!string.Equals(BrowserType, "chromium", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Visual baselines are Chromium-specific. Run this category with BROWSER=chromium.");
        }

        var failures = new List<string>();

        foreach (var viewport in Viewports)
        {
            await Page.SetViewportSizeAsync(viewport.Width, viewport.Height);

            foreach (var theme in Themes)
            {
                await Page.EmulateMediaAsync(new()
                {
                    ColorScheme = theme.ColorScheme,
                    ReducedMotion = ReducedMotion.Reduce
                });

                foreach (var surface in Surfaces)
                {
                    var snapshotName = $"{surface.Name}-{viewport.Name}-{theme.Name}";

                    try
                    {
                        await NavigateAndWaitForBlazorAsync(surface.Path);
                        await ApplyThemeAsync(theme.Name);

                        if (surface.PrepareAsync != null)
                        {
                            await surface.PrepareAsync(Page);
                        }

                        var target = surface.GetTarget(Page);
                        await target.WaitForAsync(new() { State = WaitForSelectorState.Visible });
                        await target.ScrollIntoViewIfNeededAsync();
                        await Page.Mouse.MoveAsync(0, 0);
                        await Page.EvaluateAsync("() => document.fonts.ready");
                        await Page.WaitForTimeoutAsync(100);

                        var screenshot = await target.ScreenshotAsync(new()
                        {
                            Animations = ScreenshotAnimations.Disabled,
                            Caret = ScreenshotCaret.Hide,
                            Scale = ScreenshotScale.Css,
                            Style = StableScreenshotStyles
                        });

                        await VisualRegression.AssertMatchesAsync(
                            Page,
                            BrowserType,
                            snapshotName,
                            screenshot);
                    }
                    catch (Exception exception)
                    {
                        failures.Add($"{snapshotName}: {exception.Message}");
                    }
                }
            }
        }

        if (failures.Count > 0)
        {
            throw new InvalidOperationException(
                $"{failures.Count} visual regression check(s) failed:{Environment.NewLine}" +
                string.Join(Environment.NewLine, failures));
        }
    }

    private async Task ApplyThemeAsync(string theme)
    {
        await Page.EvaluateAsync(
            """
            theme => {
                const dark = theme === 'dark';
                document.documentElement.classList.toggle('dark', dark);
                localStorage.setItem('vibe-theme', theme);
                localStorage.setItem('theme', theme);
            }
            """,
            theme);
    }

    private sealed record ViewportProfile(string Name, int Width, int Height);

    private sealed record ThemeProfile(string Name, ColorScheme ColorScheme);

    private sealed record SnapshotSurface(
        string Name,
        string Path,
        Func<IPage, ILocator> GetTarget,
        Func<IPage, Task>? PrepareAsync = null);
}
