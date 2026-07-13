# Beta 1.0.0-beta readiness checklist

This checklist targets the `1.0.0-beta` release line. Beta means Vibe.UI is ready for broader dogfooding across the supported hosting models, with known limitations still documented before stable 1.0.

- **Baseline release gate:** Passed and merged to `main` in commit `5ea53d9` on July 10, 2026.
- **Current hardening:** The `codex/visual-system-hardening` branch adds enforceable accessibility, functional, and reviewed visual-regression gates before the beta package is cut.
- **Final pull request:** [#5 - Harden Vibe.UI for beta](https://github.com/Narcoleptic-Fox/Vibe.UI/pull/5)

## Release Metadata

- [x] Shipping package versions set to `1.0.0-beta`:
  - `src/Vibe.UI`
  - `src/Vibe.UI.CSS`
  - `src/Vibe.UI.CLI`
- [x] README release banner updated from alpha to beta.
- [x] Public component count normalized to the source-backed 110 Razor components.
- [x] NuGet/package README files use beta-ready component counts and setup guidance.
- [x] Beta install commands pin prerelease package/tool versions.
- [x] Publish workflow validates the requested beta version against all shipping project versions before packing.

## Hosting Compatibility

- [x] Standalone Blazor WebAssembly compatibility fixture added.
- [x] Blazor Web App compatibility fixture added with static SSR, Interactive Server, Interactive WebAssembly, and Interactive Auto routes.
- [x] Browser smoke tests verify package assets, DI registration, and Vibe.UI component interactivity across supported hosting models.
- [x] Compatibility browser smoke tests can run under Chromium, Firefox, and WebKit via `BROWSER`.
- [x] Compatibility tests support externally managed app URLs through `VIBE_STANDALONE_BASE_URL` and `VIBE_WEBAPP_BASE_URL`.
- [x] Compatibility fixtures are included in `Vibe.sln`.
- [x] Compatibility behavior is documented in `docs/Compatibility.md`.
- [x] Hosted package-mode CSS generation uses a single server-owned `wwwroot/css/Vibe.UI.CSS` and survives an immediate repeat build on Windows/.NET SDK `10.0.301`.
- [x] 100% direct component unit coverage is complete; see `docs/Beta-Component-Hardening.md`.

## CI And Publish Gates

- [x] CI restores and builds the docs app.
- [x] CI runs TypeScript build/test/deploy for `src/Vibe.UI/ts-src` and `samples/Vibe.UI.Docs/ts-src`.
- [x] CI fails when generated JavaScript is stale after TypeScript deploy.
- [x] CI restores and builds both compatibility fixtures.
- [x] CI builds the E2E test project.
- [x] CI validates `dotnet pack` for `Vibe.UI`, `Vibe.UI.CSS`, and `Vibe.UI.CLI`.
- [x] CI validates required package contents for `Vibe.UI`, `Vibe.UI.CSS`, and `Vibe.UI.CLI`.
- [x] CI builds the standalone and web app compatibility fixtures from locally packed `.nupkg` files.
- [x] CI and local package validation build the hosted Web App fixture from locally packed `.nupkg` files twice in a row to catch duplicate static web asset regressions.
- [x] CI installs `Vibe.UI.CLI` from the locally packed `.nupkg` and verifies package-bundled templates and CSS generation.
- [x] Local package validation verifies `vibe-dialog.js` and `vibe-richtext.js` ship in the CLI package/init output, rewrites generated component JS imports to `./js/...`, and builds a generated JS-backed consumer.
- [x] Local package validation creates a fresh hosted Web App with the packed CLI, configures `--with-css` on the server, and builds the init-only project twice with warnings as errors.
- [x] CI runs the docs `Category=Integration` browser tests under Chromium.
- [x] CI runs the docs `Category=Smoke` browser tests under Chromium.
- [x] CI runs the complete docs `Category=Functional` browser suite under Chromium, including Axe serious/critical checks and keyboard/focus contracts.
- [x] CI compares 24 reviewed Chromium/Windows visual baselines across six flagship surfaces, light/dark themes, and desktop/mobile viewports.
- [x] CI installs Playwright Chromium, Firefox, and WebKit and runs `Category=Compatibility` browser tests in all three browsers.
- [x] Docs app browser runtime uses committed local Chart.js and Shiki assets instead of browser-time CDN imports.
- [x] NuGet publish workflow runs the all-browser compatibility tests before packing and pushing packages.
- [x] NuGet publish workflow validates required package contents before pushing packages.
- [x] NuGet publish workflow validates local package install/build behavior before pushing packages.
- [x] NuGet publish workflow rebuilds compatibility fixtures from locally packed `.nupkg` files and reruns all-browser compatibility tests before pushing packages.
- [x] NuGet publish workflow defaults manual dispatches to dry-run mode and validates `NUGET_API_KEY` before non-dry-run publish.
- [x] NuGet publish workflow validates TypeScript source/tests and committed generated JavaScript before packaging.
- [x] NuGet publish workflow runs the docs `Category=Integration` browser tests under Chromium before packaging.
- [x] NuGet publish workflow runs the docs `Category=Smoke` browser tests under Chromium before packaging.
- [x] NuGet publish workflow runs the docs `Category=Functional` browser suite under Chromium before packaging.
- [x] NuGet publish workflow requires the reviewed Chromium/Windows visual-regression job before publishing.

## Validation Commands

Run these before cutting the beta release:

```bash
dotnet restore Vibe.sln
npm --prefix src/Vibe.UI/ts-src ci
npm --prefix src/Vibe.UI/ts-src test
npm --prefix src/Vibe.UI/ts-src run deploy
npm --prefix samples/Vibe.UI.Docs/ts-src ci
npm --prefix samples/Vibe.UI.Docs/ts-src test
npm --prefix samples/Vibe.UI.Docs/ts-src run deploy
git diff --exit-code -- src/Vibe.UI/wwwroot/js/vibe-chart.js samples/Vibe.UI.Docs/wwwroot/js/chart.umd.js samples/Vibe.UI.Docs/wwwroot/js/shiki-interop.js samples/Vibe.UI.Docs/wwwroot/js/docs.js
dotnet build Vibe.sln --configuration Release --no-restore -p:TreatWarningsAsErrors=true
dotnet test tests/Vibe.UI.CSS.Tests/Vibe.UI.CSS.Tests.csproj --configuration Release --no-build --verbosity normal
dotnet test tests/Vibe.UI.Tests/Vibe.UI.Tests.csproj --configuration Release --no-build --verbosity normal
dotnet test tests/Vibe.UI.CLI.Tests/Vibe.UI.CLI.Tests.csproj --configuration Release --no-build --verbosity normal
pwsh tests/Vibe.UI.Docs.E2E/bin/Release/net10.0/playwright.ps1 install chromium firefox webkit
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Integration -- RunConfiguration.TestSessionTimeout=300000
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Smoke -- RunConfiguration.TestSessionTimeout=600000
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Functional -- RunConfiguration.TestSessionTimeout=1200000
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Visual -- RunConfiguration.TestSessionTimeout=1200000
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
BROWSER=firefox dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
BROWSER=webkit dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
dotnet pack src/Vibe.UI.CSS/Vibe.UI.CSS.csproj --configuration Release --no-build --output ./packages
dotnet pack src/Vibe.UI/Vibe.UI.csproj --configuration Release --no-build --output ./packages
dotnet pack src/Vibe.UI.CLI/Vibe.UI.CLI.csproj --configuration Release --no-build --output ./packages
pwsh scripts/Validate-LocalPackages.ps1 -PackagesPath ./packages
dotnet restore samples/Vibe.UI.Compatibility.StandaloneClient/Vibe.UI.Compatibility.StandaloneClient.csproj --source ./packages --source https://api.nuget.org/v3/index.json -p:VibeUsePackageReferences=true -p:VibePackageVersion=1.0.0-beta
dotnet restore samples/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.csproj --source ./packages --source https://api.nuget.org/v3/index.json -p:VibeUsePackageReferences=true -p:VibePackageVersion=1.0.0-beta
dotnet build samples/Vibe.UI.Compatibility.StandaloneClient/Vibe.UI.Compatibility.StandaloneClient.csproj --configuration Release --no-restore -p:TreatWarningsAsErrors=true -p:VibeUsePackageReferences=true -p:VibePackageVersion=1.0.0-beta
dotnet build samples/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.csproj --configuration Release --no-restore -p:TreatWarningsAsErrors=true -p:VibeUsePackageReferences=true -p:VibePackageVersion=1.0.0-beta
dotnet build samples/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.csproj --configuration Release --no-restore -p:TreatWarningsAsErrors=true -p:VibeUsePackageReferences=true -p:VibePackageVersion=1.0.0-beta
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
BROWSER=firefox dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
BROWSER=webkit dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
```

PowerShell equivalent for a single compatibility browser run:

```powershell
$env:BROWSER = "chromium"
dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
Remove-Item Env:BROWSER
```

## Known Beta Debt

- [x] The complete docs functional suite is enforced in CI, including mobile layout contracts, Axe serious/critical scans, keyboard behavior, focus trapping, and focus restoration.
- [x] Reviewed visual baselines cover Button, FormField, DataTable, KanbanBoard, Sidebar, and an open AlertDialog in light/dark desktop/mobile states.
- [x] Direct component unit coverage covers 110 of 110 source components.
- [ ] Manual screen-reader, 200% zoom/reflow, and forced-colors passes remain required before claiming formal WCAG conformance.
- [ ] Visual baselines currently target Chromium on Windows; expand browser/platform coverage when rendering stability justifies maintaining additional baseline sets.
- [ ] Docs Shiki browser behavior still has skipped Vitest blocks; the Chromium integration E2E suite is the current browser-level coverage.
- [x] Generated CSS output is deterministic and no longer introduces timestamp-only diffs.
- [ ] Publish workflow still depends on `NUGET_API_KEY` for non-dry-run publishing; trusted publishing can be considered before stable 1.0.
