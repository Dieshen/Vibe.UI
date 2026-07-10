# Vibe.UI Hosting Compatibility

This repo keeps two small compatibility fixtures under `samples/` to prove Vibe.UI works in the two hosting shapes consumers are likely to use.

## Standalone Blazor WebAssembly

Project:

```bash
samples/Vibe.UI.Compatibility.StandaloneClient/Vibe.UI.Compatibility.StandaloneClient.csproj
```

Purpose:

- Verifies a standalone client app can reference the local Vibe.UI project.
- Verifies Vibe.UI static web assets load through `_content/Vibe.UI/...`.
- Verifies `AddVibeUI()` works in a WebAssembly DI container.
- Verifies core interactive components update on the client runtime.

Run locally:

```bash
dotnet run --project samples/Vibe.UI.Compatibility.StandaloneClient/Vibe.UI.Compatibility.StandaloneClient.csproj
```

## Blazor Web App With Server And Client

Server project:

```bash
samples/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.csproj
```

Client project:

```bash
samples/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.Client/Vibe.UI.Compatibility.WebApp.Client.csproj
```

Purpose:

- Verifies Vibe.UI renders during static SSR.
- Verifies Vibe.UI works in `InteractiveServer`.
- Verifies Vibe.UI works in `InteractiveWebAssembly` from the `.Client` project.
- Verifies Vibe.UI works in `InteractiveAuto`, including server prerendering before WebAssembly activation.
- Verifies `AddVibeUI()` is registered in both server and client DI containers.
- In package mode, keeps `Vibe.UI.CSS` ownership on the server project so hosted rebuilds do not emit duplicate `wwwroot/css/Vibe.UI.CSS` static assets from both server and client.

When `Vibe.UI.CSS` is referenced by both hosted projects, the server project scans the shared sample root and owns the generated `wwwroot/css/Vibe.UI.CSS`. The `.Client` project keeps its `StaticWebAssetProjectMode=Default` role and does not publish its own copy of that generated CSS file.

The CLI follows the same topology for `vibe init --with-css`: source components remain in the recommended client project, while the `Vibe.UI.CSS` package and shared scan-root configuration are written to the server project.

Run locally:

```bash
dotnet run --project samples/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.csproj
```

Routes:

- `/` - static SSR smoke page
- `/server` - Interactive Server smoke page
- `/client` - Interactive WebAssembly smoke page
- `/auto` - Interactive Auto smoke page

## CI Gate

The CI workflow restores and builds both fixtures in Release configuration, installs the Playwright browser runtime, and runs the `Category=Compatibility` browser smoke tests in Chromium, Firefox, and WebKit.

The fixtures use project references by default for normal development. The local package validation script flips them to package references with `VibeUsePackageReferences=true`, restores from the packed `.nupkg` files, and builds both hosting shapes before a package can be published.

The hosted Web App package-mode validation deliberately builds the server project twice back to back. That guards the Windows/.NET SDK `10.0.301` duplicate-static-web-asset regression where both hosted projects used to claim the same generated `wwwroot/css/Vibe.UI.CSS`.

Set `BROWSER=chromium`, `BROWSER=firefox`, or `BROWSER=webkit` to run a specific browser locally. In PowerShell, use `$env:BROWSER = "chromium"` before invoking `dotnet test`. Set `VIBE_STANDALONE_BASE_URL` or `VIBE_WEBAPP_BASE_URL` when the target app is already running and should not be started by the test harness.

The browser tests verify:

- Standalone Blazor WebAssembly client-side interactivity.
- Blazor Web App static SSR rendering.
- Blazor Web App `InteractiveServer` interactivity.
- Blazor Web App `InteractiveWebAssembly` interactivity from the `.Client` project.
- Blazor Web App `InteractiveAuto` prerender and activation behavior.
