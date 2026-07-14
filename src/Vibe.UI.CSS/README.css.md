# Vibe.UI.CSS

Vibe.UI.CSS is a .NET-native utility CSS generator for Blazor and Razor projects. It scans your project for `vibe-*` utility classes and generates a small CSS file at build time without requiring Node.js.

Requires the .NET 10 SDK. The beta compatibility contract is documented in the
[Vibe.UI.CSS beta core profile](https://github.com/Narcoleptic-Fox/Vibe.UI/blob/main/docs/Vibe.UI.CSS.CoreProfile.md).

## Installation

```bash
dotnet add package Vibe.UI.CSS --version 1.0.0-beta
```

## MSBuild Usage

After adding the package, configure the generated output path in your project file if the default is not right for your app:

```xml
<PropertyGroup>
  <VibeCssOutput>wwwroot/css/Vibe.UI.CSS</VibeCssOutput>
  <VibeCssPrefix>vibe</VibeCssPrefix>
  <VibeCssFailOnUnknown>true</VibeCssFailOnUnknown>
</PropertyGroup>
```

The package includes `build` and `buildTransitive` targets, so CSS generation runs during build when `VibeCssEnabled` is `true`.

## CLI Usage

The packed tool assembly can also be invoked directly:

```bash
dotnet Vibe.UI.CSS.dll generate . -o wwwroot/css/Vibe.UI.CSS
```

Most consumers should use the MSBuild integration or the `vibe css` command from `Vibe.UI.CLI`.

Use `--fail-on-unknown` in CI. Add intentional non-utility hooks with
`--ignore "vibe-theme-hook,vibe-app-shell"`; strict generation leaves the
existing output untouched when validation fails.
