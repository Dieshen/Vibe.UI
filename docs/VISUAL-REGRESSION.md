# Visual regression

Vibe.UI keeps reviewed browser screenshots for the component surfaces where a
small styling change can affect a large amount of product UI. The visual suite
is a release gate, not a screenshot generator that silently accepts new output.

## Current baseline

The suite covers six flagship surfaces:

- Button
- FormField
- DataTable
- KanbanBoard
- Sidebar
- AlertDialog in its open state

Each surface is captured in light and dark themes at desktop (`1280x900`) and
mobile (`390x844`) viewports. That produces 24 reviewed images under
`tests/Vibe.UI.Docs.E2E/Baselines/chromium/windows`.

The browser context emulates reduced motion, waits for fonts, disables animation
and caret rendering, and moves the pointer away from the component before each
capture. Comparison allows a channel delta of 16 and rejects a snapshot when
more than 0.5% of its pixels change.

## Compare baselines

Build the docs and E2E projects, install Playwright Chromium, then run:

```powershell
$env:BROWSER = "chromium"
dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj `
  --configuration Release --no-build --filter Category=Visual `
  -- RunConfiguration.TestSessionTimeout=1200000
Remove-Item Env:BROWSER
```

Failures preserve the actual image and a highlighted diff under
`output/playwright/visual-regression/chromium/windows`. CI uploads that directory
and the Playwright artifacts for diagnosis.

## Update baselines

Only update baselines after reviewing every affected state and confirming that
the visual change is intentional:

```powershell
$env:BROWSER = "chromium"
$env:UPDATE_VISUAL_BASELINES = "true"
dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj `
  --configuration Release --no-build --filter Category=Visual `
  -- RunConfiguration.TestSessionTimeout=1200000
Remove-Item Env:UPDATE_VISUAL_BASELINES
Remove-Item Env:BROWSER
```

Commit the reviewed PNG changes with the component or token change that caused
them. Do not regenerate baselines merely to make a failing build pass.

Chromium on Windows is the current deterministic visual target. Firefox,
WebKit, Linux, and macOS remain behavior-compatibility targets but do not yet
carry maintained screenshot baselines.
