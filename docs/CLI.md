# Vibe.UI CLI

`Vibe.UI.CLI` is a .NET 10 global tool that copies Vibe.UI component source into
a Blazor project. The consuming project owns the copied Razor, CSS, C#, and
JavaScript files; a `Vibe.UI` package reference is not required for the source
installation workflow.

The beta tool and generated package workflow require the .NET 10 SDK.

## Installation

```bash
dotnet tool install --global Vibe.UI.CLI --version 1.0.0-beta
```

The executable command is `vibe`.

## Commands

| Command                   | Purpose                                                    | Important options                                                                          |
| ------------------------- | ---------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| `vibe init`               | Detect the project shape and install shared infrastructure | `--path`, `--yes`, `--minimal`, `--no-theme`, `--with-charts`, `--with-css`                |
| `vibe add [component]`    | Install a component and its declared dependencies          | `--path`, `--yes`, `--overwrite`, `--name`, `--output`                                     |
| `vibe list`               | List registered components by category                     | none                                                                                       |
| `vibe update [component]` | Refresh one component or all installed components          | `--path`, `--yes`                                                                          |
| `vibe css [path]`         | Scan or generate utility CSS                               | `--output`, `--with-base`, `--prefix`, `--watch`, `--verbose`, `--scan-only`, `--patterns`, `--fail-on-unknown`, `--ignore` |

Use `vibe <command> --help` for the executable's current option descriptions.

## Initialization output

The default initialization creates:

```text
vibe.json
Vibe/
  Base/
  Configuration/
  Enums/
  Services/
Components/vibe/
wwwroot/css/vibe-base.css
wwwroot/css/vibe-utilities.css
wwwroot/js/*.js
```

`vibe.json` records the project type, theme mode, component directory, CSS
variable setting, and optional aliases. The default component directory created
by `vibe init` is `Components/vibe`.

Initialization updates the project-level `_Imports.razor` with infrastructure
namespaces. The component namespace is added only after the first component is
installed, which keeps an init-only project compilable.

## Hosting topology

`ProjectService` inspects project SDKs, package references, project references,
and `Program.cs` signals to distinguish:

- generic Blazor projects;
- standalone Blazor WebAssembly projects;
- Blazor Server projects;
- hosted Blazor Web Apps; and
- the `.Client` side of a hosted Blazor Web App.

For a hosted Blazor Web App, source components are installed into the client
project by default because Interactive WebAssembly and Interactive Auto routes
must include them in the client assembly. Commands run later from the Web App
root resolve the initialized client project through its `vibe.json` file.

Register `AddVibeUI()` in every process that renders Vibe.UI components. A
hosted application that uses both server and client render modes normally
registers it in both `Program.cs` files. See [Compatibility](Compatibility.md)
for the complete server/client setup.

### Hosted CSS ownership

With `vibe init --with-css`, the CLI:

1. keeps copied component source in the client project;
2. adds `Vibe.UI.CSS` to the server project with `PrivateAssets="all"`;
3. sets the server scan root to the shared server/client parent; and
4. leaves the client project without a second CSS package reference.

This produces one server-owned `wwwroot/css/Vibe.UI.CSS` static asset and avoids
duplicate static web assets during repeat builds.

### Strict CSS validation

Use strict scanning in application CI:

```bash
vibe css . --scan-only --fail-on-unknown
```

Intentional component or theme hooks that use the `vibe-` prefix can be listed
explicitly with `--ignore "vibe-theme-hook,vibe-app-shell"`. Strict generation
returns a non-zero exit code before modifying the existing output file.

## Component registry and templates

`ComponentService.InitializeComponents()` is the registry for the 111 source
components. Each entry records:

- the command name and Razor component name;
- source category;
- description;
- component dependencies; and
- whether scoped CSS or JavaScript is expected.

The CLI project packs component templates by source category under
`Templates/Components/`, shared C# infrastructure under
`Templates/Infrastructure/`, and all app-local JavaScript under
`Templates/wwwroot/js/`.

Component files install into a flat consumer directory by default, independent
of their source category. `--output` opts into a consumer-defined directory.

## JavaScript source installation

Package components import modules from `_content/Vibe.UI/js/`. A source install
does not use the package static-web-asset path, so `ComponentService` rewrites
those imports to `./js/` while copying the component. `vibe init` copies the
matching JavaScript files into the application's `wwwroot/js` directory.

`--with-charts` copies the Vibe.UI chart interop module. The application must
still load Chart.js itself; see [Charts](CHARTS.md).

## Overwrite safety

The CLI stores SHA-256 hashes in `.vibe/checksums.json`. When `--overwrite` or
`vibe update` replaces a tracked file:

- unchanged generated files are replaced directly;
- modified tracked files are copied to `.vibe/backups/` first; and
- the new installed hash is recorded after the write.

No overwrite occurs by default when the destination already exists.

## Architecture map

| Area                           | Source                                             |
| ------------------------------ | -------------------------------------------------- |
| Command registration           | `src/Vibe.UI.CLI/Program.cs`                       |
| Commands                       | `src/Vibe.UI.CLI/Commands/`                        |
| Component registry and copying | `src/Vibe.UI.CLI/Services/ComponentService.cs`     |
| Project topology               | `src/Vibe.UI.CLI/Services/ProjectService.cs`       |
| Setup guidance                 | `src/Vibe.UI.CLI/Services/ProjectSetupGuidance.cs` |
| Razor imports                  | `src/Vibe.UI.CLI/Services/RazorImportsService.cs`  |
| Configuration                  | `src/Vibe.UI.CLI/Services/ConfigService.cs`        |
| Change detection and backups   | `src/Vibe.UI.CLI/Services/FileChangeDetector.cs`   |
| Packaged template definition   | `src/Vibe.UI.CLI/Vibe.UI.CLI.csproj`               |

## Development validation

Run the CLI tests:

```bash
dotnet test tests/Vibe.UI.CLI.Tests/Vibe.UI.CLI.Tests.csproj --configuration Release
```

Validate the actual packed tool and generated consumers:

```powershell
dotnet pack src/Vibe.UI.CLI/Vibe.UI.CLI.csproj --configuration Release --output packages
pwsh scripts/Validate-LocalPackages.ps1 -PackagesPath ./packages
```

The package validator installs the packed global tool, checks required template
assets, builds a source-installed JavaScript component, and builds a fresh
hosted Web App twice with server-owned CSS.
