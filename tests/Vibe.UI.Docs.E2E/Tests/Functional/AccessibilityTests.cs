using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Functional;

/// <summary>
/// Enforceable accessibility contracts for representative documentation and component states.
/// </summary>
[Trait("Category", TestCategories.Functional)]
[Trait("Category", "Accessibility")]
public class AccessibilityTests : E2ETestBase
{
    private static readonly string[] FlagshipRoutes =
    [
        "/components/button",
        "/components/formfield",
        "/components/datatable",
        "/components/kanbanboard",
        "/components/sidebar",
        "/components/alertdialog"
    ];

    [Fact]
    public async Task FlagshipPagesHaveNoSeriousOrCriticalAxeViolations()
    {
        var failures = new List<string>();
        await Page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });

        foreach (var theme in new[] { "light", "dark" })
        {
            foreach (var route in FlagshipRoutes)
            {
                await NavigateAndWaitForBlazorAsync(route);
                await ApplyThemeAsync(theme);

                var result = await Page.RunAxe();
                failures.AddRange(
                    result.Violations
                        .Where(violation =>
                            string.Equals(violation.Impact, "critical", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(violation.Impact, "serious", StringComparison.OrdinalIgnoreCase))
                        .Select(violation => FormatViolation(route, theme, violation)));
            }
        }

        failures.ShouldBeEmpty(
            "Flagship pages must not ship with serious or critical automated accessibility violations." +
            Environment.NewLine + string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public async Task OpenAlertDialogHasNoSeriousOrCriticalAxeViolations()
    {
        await Page.EmulateMediaAsync(new() { ReducedMotion = ReducedMotion.Reduce });
        await NavigateAndWaitForBlazorAsync("/components/alertdialog");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete project", Exact = true }).First.ClickAsync();

        var dialog = Page.GetByRole(AriaRole.Alertdialog, new() { Name = "Delete project?", Exact = true });
        await dialog.WaitForAsync();

        var result = await dialog.RunAxe();
        var failures = result.Violations
            .Where(violation =>
                string.Equals(violation.Impact, "critical", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(violation.Impact, "serious", StringComparison.OrdinalIgnoreCase))
            .Select(violation => FormatViolation("/components/alertdialog#open", "light", violation))
            .ToArray();

        failures.ShouldBeEmpty(
            "The open alert dialog must not have serious or critical automated accessibility violations." +
            Environment.NewLine + string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public async Task AlertDialogTrapsFocusAndRestoresTheOpener()
    {
        await NavigateAndWaitForBlazorAsync("/components/alertdialog");

        var opener = Page.GetByRole(AriaRole.Button, new() { Name = "Delete project", Exact = true }).First;
        await opener.FocusAsync();
        await opener.ClickAsync();

        var dialog = Page.GetByRole(AriaRole.Alertdialog, new() { Name = "Delete project?", Exact = true });
        await dialog.WaitForAsync();
        (await dialog.GetAttributeAsync("aria-modal")).ShouldBe("true");
        (await dialog.GetAttributeAsync("aria-labelledby")).ShouldNotBeNullOrWhiteSpace();
        (await dialog.GetAttributeAsync("aria-describedby")).ShouldNotBeNullOrWhiteSpace();

        for (var index = 0; index < 8; index++)
        {
            await Page.Keyboard.PressAsync("Tab");
            (await dialog.EvaluateAsync<bool>(
                "element => element.contains(document.activeElement)")).ShouldBeTrue(
                "Focus must remain inside the open alert dialog.");
        }

        await Page.Keyboard.PressAsync("Escape");
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        (await opener.EvaluateAsync<bool>(
            "element => document.activeElement === element")).ShouldBeTrue(
            "Closing the alert dialog must restore focus to its opener.");
    }

    [Fact]
    public async Task PreviewFormControlsHaveProgrammaticNames()
    {
        await NavigateAndWaitForBlazorAsync("/components/input");

        var controls = Page.Locator("main section").First.Locator("input:visible, textarea:visible, select:visible");
        var count = await controls.CountAsync();
        count.ShouldBeGreaterThan(0, "The Input preview must expose form controls.");

        for (var index = 0; index < count; index++)
        {
            var hasProgrammaticName = await controls.Nth(index).EvaluateAsync<bool>(
                """
                element => {
                    const labelledBy = element.getAttribute('aria-labelledby');
                    const labelledByText = labelledBy
                        ?.split(/\s+/)
                        .map(id => document.getElementById(id)?.textContent?.trim())
                        .some(Boolean) ?? false;
                    const associatedLabel = Array.from(element.labels ?? [])
                        .some(label => Boolean(label.textContent?.trim()));

                    return associatedLabel ||
                        Boolean(element.getAttribute('aria-label')?.trim()) ||
                        labelledByText;
                }
                """);

            hasProgrammaticName.ShouldBeTrue(
                $"Visible form control {index} must use a label, aria-label, or aria-labelledby; placeholder text is not a label.");
        }
    }

    [Fact]
    public async Task TabsUseRovingKeyboardFocusAndSelection()
    {
        await NavigateAndWaitForBlazorAsync("/components/tabs");

        var preview = Page.Locator("main section").First;
        var account = preview.GetByRole(AriaRole.Tab, new() { Name = "Account", Exact = true });
        var security = preview.GetByRole(AriaRole.Tab, new() { Name = "Security", Exact = true });

        await account.FocusAsync();
        (await account.GetAttributeAsync("tabindex")).ShouldBe("0");
        (await account.GetAttributeAsync("aria-selected")).ShouldBe("true");

        await account.PressAsync("ArrowRight");
        (await security.EvaluateAsync<bool>(
            "element => document.activeElement === element")).ShouldBeTrue();
        (await security.GetAttributeAsync("tabindex")).ShouldBe("0");
        (await security.GetAttributeAsync("aria-selected")).ShouldBe("true");
        (await account.GetAttributeAsync("tabindex")).ShouldBe("-1");
    }

    [Fact]
    public async Task FlagshipPagesKeepControlsUsableAtMobileWidth()
    {
        await Page.SetViewportSizeAsync(390, 844);
        var failures = new List<string>();

        foreach (var route in FlagshipRoutes)
        {
            await NavigateAndWaitForBlazorAsync(route);

            var bodyFits = await Page.EvaluateAsync<bool>(
                "() => document.documentElement.scrollWidth <= window.innerWidth");
            if (!bodyFits)
            {
                failures.Add($"{route}: page content overflows the 390px viewport.");
            }

            var controls = Page.Locator(
                "main section:first-of-type button:visible, " +
                "main section:first-of-type input:visible, " +
                "main section:first-of-type select:visible, " +
                "main section:first-of-type textarea:visible, " +
                "main section:first-of-type [role='button']:visible, " +
                "main section:first-of-type [role='tab']:visible");
            var controlCount = await controls.CountAsync();

            for (var index = 0; index < controlCount; index++)
            {
                var box = await controls.Nth(index).BoundingBoxAsync();
                if (box != null && (box.Width < 23.5 || box.Height < 23.5))
                {
                    failures.Add(
                        $"{route}: interactive control {index} is {box.Width:F1}x{box.Height:F1}px; " +
                        "WCAG 2.2 target size requires at least 24x24px unless an exception applies.");
                }
            }
        }

        failures.ShouldBeEmpty(
            "Flagship mobile previews must fit the viewport and keep interaction targets usable." +
            Environment.NewLine + string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public async Task AlertsExposeLiveAlertSemantics()
    {
        await NavigateAndWaitForBlazorAsync("/components/alert");

        var alerts = Page.Locator("main section:first-of-type [role='alert']:visible");
        (await alerts.CountAsync()).ShouldBeGreaterThan(0);

        foreach (var alert in await alerts.AllAsync())
        {
            (await alert.TextContentAsync()).ShouldNotBeNullOrWhiteSpace();
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

    private static string FormatViolation(
        string route,
        string theme,
        Deque.AxeCore.Commons.AxeResultItem violation)
    {
        var targets = violation.Nodes
            .Select(node =>
                $"{node.Target}: {string.Join(" ", node.Any.Select(check => check.Message))}")
            .Distinct(StringComparer.Ordinal)
            .Take(5);

        return $"{route} ({theme}): [{violation.Impact}] {violation.Id} - {violation.Help}. " +
               $"Targets: {string.Join(" | ", targets)}. {violation.HelpUrl}";
    }
}
