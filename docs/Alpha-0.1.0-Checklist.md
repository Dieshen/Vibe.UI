# Alpha 1.0.0-alpha release history

> Note: This checklist targets the `1.0.0-alpha` release (the filename is historical).

- **Release date:** December 16, 2025
- **Status:** Completed and superseded by [1.0.0-beta](Beta-Readiness-Checklist.md)

## Release summary

The alpha release established the source-copy CLI workflow, the .NET-native
utility CSS generator, the initial accessible component baseline, and the
project's prerelease packaging conventions.

The release work included:

- focus-visible styling and invalid-state support across flagship form controls;
- disabled-state handling for dialog and drawer close controls;
- initial dialog focus trapping and focus restoration;
- `1.0.0-alpha` package metadata for `Vibe.UI`, `Vibe.UI.CSS`, and `Vibe.UI.CLI`;
- alpha messaging, known limitations, and parity-roadmap links in the repository docs; and
- packaged CLI templates for source-owned components and infrastructure.

The original memory-bank summary contained conflicting component totals, so
those snapshot counts are intentionally not preserved as release facts. The
source-backed inventory was normalized to 110 components during beta hardening.

Alpha follow-up work called for full validation, local package installation,
NuGet artifact checks, and docs-site updates. Those gates are now represented by
the beta checklist and CI/package validation workflow.

## Readiness checklist

### Docs site (`samples/Vibe.UI.Docs`)
- [x] `dotnet build samples/Vibe.UI.Docs/Vibe.UI.Docs.csproj -c Release` succeeds and regenerates `samples/Vibe.UI.Docs/wwwroot/css/Vibe.UI.CSS`
- [x] No Tailwind import/usage (no CDN, no `@tailwind`, no Tailwind build pipeline)
- [x] No legacy Bootstrap template leftovers shipped/linked (NavMenu/Bootstrap assets removed or not referenced)
- [x] Top-nav UX feels consistent (Docs + Components + search + theme toggle); no sidebar dependency
- [x] "Getting Started", "Installation", "CLI", "Theming" pages match the component pages' look and use `vibe-*` utilities
- [x] Command palette hotkey works (`Ctrl/Cmd+K`) and search results route correctly
- [x] Light + dark mode visually coherent (background, overlays, code blocks, links)

### Vibe.UI.CSS (`src/Vibe.UI.CSS`)
- [x] Docs generation works via MSBuild hook in `samples/Vibe.UI.Docs/Vibe.UI.Docs.csproj`
- [x] Variants work in docs patterns (`dark:*`, `hover:*`, `group-hover:*`, responsive variants)
- [x] Core "shadcn docs" utilities covered (spacing/layout/typography/borders/rings/gradients/transforms)
- [x] `vibe-prose` + `dark:vibe-prose-invert` is usable for long-form docs pages
- [x] Scanner handles Razor class expressions without noisy false-positives
- [x] Docs run prefix-only utilities (`VibeCssAllowUnprefixed=false`) and do not rely on unprefixed Tailwind-like class names

### Vibe.UI (`src/Vibe.UI`)
- [x] Flagship components used in docs feel "shadcn quality"
  - [x] Button: focus-visible ring only; anchor disabled state (`aria-disabled`/`.vibe-button-disabled`)
  - [x] Input: wrapper disabled class; `aria-invalid`/`aria-describedby`/`aria-errormessage`; outlined focus ring
  - [x] Tabs: roving tabindex + keyboard navigation; `aria-controls`/`aria-labelledby`
  - [x] Tooltip: keyboard focus + Escape dismiss; `role="tooltip"` + stable `id`
  - [x] Popover: uses theme tokens; alignment uses `translate` (no transform/animation clash)
  - [x] Toast: dark mode uses `.dark` (no `prefers-color-scheme`)
  - [x] Dialog: focus trap + focus restore; `aria-labelledby`/`aria-describedby` wiring (root + composed title/description)
  - [x] Command: ARIA combobox/listbox semantics; keyboard navigation + disabled handling
- [x] Focus rings / disabled / error states are consistent across the full flagship set
  - Added `:focus-visible` to TextArea
  - Added disabled state to Drawer close button
  - Added `[aria-invalid="true"]` support to Input, TextArea, Select, Checkbox, Radio
- [x] Dark mode support works across the full flagship set (tokens + component CSS)
  - Components use CSS variables that automatically adapt in dark mode
- [x] Basic a11y sanity across the full flagship set (keyboard focus, Escape to close overlays, ARIA labeling)

### CLI (`src/Vibe.UI.CLI`)
- [x] `vibe init` produces a working baseline (theme toggle, tokens, required JS/CSS)
  - Creates `vibe.json` configuration
  - Templates bundled in NuGet package (Pack="true" with PackagePath)
- [x] `vibe add <component>` installs dependencies correctly and produces compilable output
  - 110 components with dependency resolution (see `ComponentService.InitializeComponents()`)
- [x] Config file naming and docs are consistent (`vibe.json`)
- [x] "Getting Started" + "Installation" docs match the current CLI behavior/options

### Release hygiene
- [x] Version numbers set for alpha (`1.0.0-alpha`) in shipping package projects (`src/Vibe.UI`, `src/Vibe.UI.CSS`, `src/Vibe.UI.CLI`)
- [x] Clear "alpha" messaging in README / docs (dogfooding expectations)
- [x] Known limitations documented (utility gaps, missing variants, component rough edges)
  - Added "Known Limitations" section to README.md
  - References roadmap docs for detailed information

