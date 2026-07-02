## Beta 1.0.0-beta Readiness Checklist

This checklist targets the `1.0.0-beta` release line. Beta means Vibe.UI is ready for broader dogfooding across the supported hosting models, with known limitations still documented before stable 1.0.

### Release Metadata

- [x] Shipping package versions set to `1.0.0-beta`:
  - `src/Vibe.UI`
  - `src/Vibe.UI.CSS`
  - `src/Vibe.UI.CLI`
- [x] README release banner updated from alpha to beta.
- [x] Public component count normalized to the source-backed 110 Razor components.
- [x] NuGet/package README files use beta-ready component counts and setup guidance.
- [x] Beta install commands pin prerelease package/tool versions.
- [x] Publish workflow validates the requested beta version against all shipping project versions before packing.

### Hosting Compatibility

- [x] Standalone Blazor WebAssembly compatibility fixture added.
- [x] Blazor Web App compatibility fixture added with static SSR, Interactive Server, Interactive WebAssembly, and Interactive Auto routes.
- [x] Browser smoke tests verify package assets, DI registration, and Vibe.UI component interactivity across supported hosting models.
- [x] Compatibility browser smoke tests can run under Chromium, Firefox, and WebKit via `BROWSER`.
- [x] Compatibility tests support externally managed app URLs through `VIBE_STANDALONE_BASE_URL` and `VIBE_WEBAPP_BASE_URL`.
- [x] Compatibility fixtures are included in `Vibe.sln`.
- [x] Compatibility behavior is documented in `docs/Compatibility.md`.
- [ ] 80% direct component unit coverage target is complete; see `docs/Beta-Component-Hardening.md`.

### CI And Publish Gates

- [x] CI restores and builds the docs app.
- [x] CI runs TypeScript build/test/deploy for `src/Vibe.UI/ts-src` and `samples/Vibe.UI.Docs/ts-src`.
- [x] CI fails when generated JavaScript is stale after TypeScript deploy.
- [x] CI restores and builds both compatibility fixtures.
- [x] CI builds the E2E test project.
- [x] CI validates `dotnet pack` for `Vibe.UI`, `Vibe.UI.CSS`, and `Vibe.UI.CLI`.
- [x] CI validates required package contents for `Vibe.UI`, `Vibe.UI.CSS`, and `Vibe.UI.CLI`.
- [x] CI builds the standalone and web app compatibility fixtures from locally packed `.nupkg` files.
- [x] CI installs `Vibe.UI.CLI` from the locally packed `.nupkg` and verifies package-bundled templates and CSS generation.
- [x] CI runs the docs `Category=Integration` browser tests under Chromium.
- [x] CI runs the docs `Category=Smoke` browser tests under Chromium.
- [x] CI installs Playwright Chromium, Firefox, and WebKit and runs `Category=Compatibility` browser tests in all three browsers.
- [x] Docs app browser runtime uses committed local Chart.js and Shiki assets instead of browser-time CDN imports.
- [x] NuGet publish workflow runs the all-browser compatibility tests before packing and pushing packages.
- [x] NuGet publish workflow validates required package contents before pushing packages.
- [x] NuGet publish workflow validates local package install/build behavior before pushing packages.
- [x] NuGet publish workflow validates TypeScript source/tests and committed generated JavaScript before packaging.
- [x] NuGet publish workflow runs the docs `Category=Integration` browser tests under Chromium before packaging.
- [x] NuGet publish workflow runs the docs `Category=Smoke` browser tests under Chromium before packaging.

### Validation Commands

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
pwsh tests/Vibe.UI.Docs.E2E/bin/Release/net9.0/playwright.ps1 install chromium firefox webkit
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Integration -- RunConfiguration.TestSessionTimeout=300000
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Smoke -- RunConfiguration.TestSessionTimeout=600000
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
BROWSER=firefox dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
BROWSER=webkit dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
dotnet pack src/Vibe.UI.CSS/Vibe.UI.CSS.csproj --configuration Release --no-build --output ./packages
dotnet pack src/Vibe.UI/Vibe.UI.csproj --configuration Release --no-build --output ./packages
dotnet pack src/Vibe.UI.CLI/Vibe.UI.CLI.csproj --configuration Release --no-build --output ./packages
pwsh scripts/Validate-LocalPackages.ps1 -PackagesPath ./packages
```

PowerShell equivalent for a single compatibility browser run:

```powershell
$env:BROWSER = "chromium"
dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj --configuration Release --no-build --verbosity normal --filter Category=Compatibility -- RunConfiguration.TestSessionTimeout=180000
Remove-Item Env:BROWSER
```

### Known Beta Debt

- [ ] The docs E2E functional/mobile/accessibility suites beyond `Category=Compatibility`, `Category=Integration`, and `Category=Smoke` should be reviewed separately before stable 1.0.
- [ ] Direct component unit coverage is currently below the beta confidence target; use `docs/Beta-Component-Hardening.md` for the next 15-test slice.
- [ ] Docs Shiki browser behavior still has skipped Vitest blocks; the Chromium integration E2E suite is the current browser-level coverage.
- [ ] Committed generated CSS still includes a timestamp header; remove or stabilize it before stable 1.0 to avoid recurring noisy diffs.
- [ ] Publish workflow still depends on `NUGET_API_KEY`; trusted publishing can be considered before stable 1.0.
