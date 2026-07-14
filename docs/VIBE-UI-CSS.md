# Vibe.UI.CSS

`Vibe.UI.CSS` is a .NET 10 utility CSS scanner and generator for Razor projects.
It recognizes `vibe-*` classes, emits only supported rules that are present in
the scanned source, and can include the shared Vibe.UI design-token foundation.
Node.js is not required for generation.

The versioned Tailwind-style compatibility promise for beta is the
[beta core profile](Vibe.UI.CSS.CoreProfile.md).

For consumer stylesheet loading, start with [CSS setup](CSS-SETUP.md). This
document describes the generator and its build integration.

## Pipeline

The generation path has three primary stages:

1. `ClassScanner` reads configured source patterns and extracts candidate classes.
2. `UtilityGenerator` parses prefixes, variants, utilities, colors, and arbitrary values.
3. `CssEmitter` deduplicates recognized rules and writes the generated stylesheet.

The command output reports classes found, classes generated, unknown classes,
and final CSS size. Unknown classes are not emitted.

## Source patterns

The default scan patterns are:

```text
*.razor,*.cshtml,*.html
```

The scanner handles literal class attributes and Razor class expressions. Add a
pattern explicitly when utility strings live in another file type:

```bash
dotnet Vibe.UI.CSS.dll scan . --patterns "*.razor,*.cs"
```

Avoid constructing class names from fragments at runtime. The complete utility
class must be present in a scanned file for deterministic generation.

## Utility model

The generator covers the utility families used by the component library and
docs application, including:

- display, positioning, overflow, and z-index;
- flexbox, grid, alignment, and gaps;
- spacing and sizing;
- typography and text handling;
- semantic and palette colors;
- borders, radii, rings, shadows, and opacity;
- transforms, transitions, and selected animations;
- responsive, dark, form-state, group, peer, ARIA, data, accessibility-media,
  and structural variants; and
- supported arbitrary values such as `vibe-w-[var(--panel-width)]`.

Examples:

```html
<div class="vibe-flex vibe-gap-4 dark:vibe-bg-slate-900">
    <button class="vibe-px-4 vibe-py-2 hover:vibe-bg-primary">Save</button>
</div>
```

Prefix-only generation is the default. Set `--allow-unprefixed true` only for a
controlled compatibility case. Utility coverage and planned additions are
tracked in [Tailwind parity](Vibe.UI.CSS.TailwindParity.md).

## Command-line interface

The tool assembly supports:

```bash
dotnet Vibe.UI.CSS.dll scan <directory> [options]
dotnet Vibe.UI.CSS.dll generate <directory> [options]
dotnet Vibe.UI.CSS.dll test
```

Generate options:

| Option               | Default           | Purpose                                 |
| -------------------- | ----------------- | --------------------------------------- |
| `-o`, `--output`     | `Vibe.UI.CSS`     | Output path                             |
| `--prefix`           | `vibe`            | Required utility prefix                 |
| `--allow-unprefixed` | `false`           | Also recognize unprefixed utilities     |
| `--with-base`        | `true`            | Include base variables and theme tokens |
| `--patterns`         | Razor/CSHTML/HTML | Comma-separated scan patterns           |
| `--fail-on-unknown`  | `false`           | Exit non-zero for unknown utilities     |
| `--ignore`           | empty             | Known non-utility classes to exclude    |

`Vibe.UI.CLI` exposes the same generator through `vibe css`, with watch,
scan-only, and verbose modes. See [CLI](CLI.md).

For application CI, enable strict scanning so unsupported classes cannot be
silently skipped:

```bash
vibe css . --scan-only --fail-on-unknown
```

Strict generation validates the complete scan before writing, so a failed run
does not replace the existing stylesheet. Use `--ignore` only for intentional
non-utility hooks that share the configured prefix.

## MSBuild integration

Adding the `Vibe.UI.CSS` package imports targets from both `build` and
`buildTransitive`. Generation runs before build when the project owns the output.

```xml
<PackageReference Include="Vibe.UI.CSS"
                  Version="1.0.0-beta"
                  PrivateAssets="all" />

<PropertyGroup>
  <VibeCssEnabled>true</VibeCssEnabled>
  <VibeCssOutput>wwwroot/css/Vibe.UI.CSS</VibeCssOutput>
  <VibeCssScanRoot>$(MSBuildProjectDirectory)</VibeCssScanRoot>
  <VibeCssPrefix>vibe</VibeCssPrefix>
  <VibeCssIncludeBase>true</VibeCssIncludeBase>
  <VibeCssScanPatterns>*.razor,*.cshtml,*.html</VibeCssScanPatterns>
  <VibeCssFailOnUnknown>true</VibeCssFailOnUnknown>
  <VibeCssIgnoredClasses>vibe-theme-hook</VibeCssIgnoredClasses>
</PropertyGroup>
```

### Properties

| Property                       | Default                   | Purpose                                                         |
| ------------------------------ | ------------------------- | --------------------------------------------------------------- |
| `VibeCssEnabled`               | `true`                    | Enables build-time generation                                   |
| `VibeCssOutput`                | `wwwroot/css/Vibe.UI.CSS` | Generated static asset                                          |
| `VibeCssScanRoot`              | project directory         | Root scanned for source files                                   |
| `VibeCssPrefix`                | `vibe`                    | Utility prefix                                                  |
| `VibeCssIncludeBase`           | `true`                    | Includes the embedded base stylesheet                           |
| `VibeCssScanPatterns`          | Razor/CSHTML/HTML         | Source patterns                                                 |
| `VibeCssFailOnUnknown`         | `false`                   | Fails generation before writing when unknown utilities exist    |
| `VibeCssIgnoredClasses`        | empty                     | Comma-separated known non-utility class names                   |
| `VibeCssOwnStaticWebAsset`     | topology-derived          | Controls whether this project generates and publishes the asset |
| `VibeCssUseDotnetToolFallback` | `false`                   | Uses `dotnet vibe-css` if the packaged assembly is unavailable  |
| `VibeCssFailOnMissingTool`     | `false`                   | Promotes a missing generator assembly to an error               |

Paths are normalized relative to the project directory and use cross-platform
MSBuild-compatible separators.

### Incremental builds

The target declares the project, target file, generator assembly, and scanned
files as inputs, with the generated stylesheet as its output. An immediate
repeat build skips generation when those inputs are unchanged. `dotnet clean`
removes the generated stylesheet.

## Standalone and hosted ownership

A standalone project owns and publishes its generated stylesheet.

In a hosted Blazor Web App, the server must be the only owner:

```xml
<VibeCssScanRoot>$(MSBuildProjectDirectory)/..</VibeCssScanRoot>
```

The shared root lets the server scan both server and client Razor source.
Projects with `StaticWebAssetProjectMode=Default` default
`VibeCssOwnStaticWebAsset` to `false`, remove stale generated output, and do not
publish a duplicate client asset.

This ownership model is exercised by package validation and browser
compatibility tests. See [Compatibility](Compatibility.md).

## Source map

| Area                  | Source                                      |
| --------------------- | ------------------------------------------- |
| CLI entry point       | `src/Vibe.UI.CSS/Program.cs`                |
| Scanner               | `src/Vibe.UI.CSS/Scanner/`                  |
| Generator             | `src/Vibe.UI.CSS/Generator/`                |
| Public facade         | `src/Vibe.UI.CSS/VibeCss.cs`                |
| MSBuild targets       | `src/Vibe.UI.CSS/Build/Vibe.UI.CSS.targets` |
| Unit and target tests | `tests/Vibe.UI.CSS.Tests/`                  |

Run the suite with:

```bash
dotnet test tests/Vibe.UI.CSS.Tests/Vibe.UI.CSS.Tests.csproj --configuration Release
```
