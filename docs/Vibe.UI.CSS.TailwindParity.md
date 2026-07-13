# Vibe.UI.CSS Tailwind Parity Roadmap

Goal: make `Vibe.UI.CSS` feel like “Tailwind for Blazor/.NET” in day-to-day usage: broad utility coverage, predictable variants, excellent arbitrary-value support, and a great developer experience (DX).

This is intentionally written as a phased checklist so we can ship value early (docs + common app patterns) while steadily closing Tailwind gaps.

## Current Reality Check (Docs Scan)

Command:

`dotnet run --project src/Vibe.UI.CSS/Vibe.UI.CSS.csproj -- scan "samples/Vibe.UI.Docs" --patterns "*.razor,*.cshtml,*.html,*.cs"`

Latest output (summary):
- Total classes found: `408`
- Recognized: `405`
- Unknown: `3`

The remaining names (`vibe-theme`, `vibe-my-app-theme`, and
`vibe-theme-toggle`) are intentional component/theme hooks, not utility classes.

## Phase 0 — Ship the “Docs Experience” (Immediate ROI)

### Navigation + layout
- [x] Remove/replace legacy `NavMenu.razor`/Bootstrap classes in docs.
- [x] Ensure docs pages are centered and readable with utilities plus scoped docs CSS.

### Typography (“Tailwind typography plugin” equivalent)
- [x] `vibe-prose` (multi-rule) + `dark:vibe-prose-invert` (minimal but usable)
- [ ] `vibe-prose-sm`/`vibe-prose-lg` sizing variants (optional)
- [ ] Better tables in prose (striping, header emphasis, code blocks) while staying minimal

### Docs-only helpers (stay docs-only)
- [ ] Keep a small `site.css` for hero/background/animations only; avoid “docs.css v2”.

## Phase 1 — Core Utility Coverage (Most-used Tailwind set)

### Layout / display
- [ ] `container` (with breakpoint-aware max widths)
- [ ] `isolate`, `isolation-auto`
- [ ] `box-decoration-slice/clone`

### Flex / grid (fill missing “daily drivers”)
- [ ] `basis-*` (flex-basis scale + arbitrary)
- [ ] `order-*`
- [ ] `place-items-*`, `place-content-*`, `place-self-*`
- [ ] `auto-cols-*`, `auto-rows-*`, `grid-flow-*`

### Spacing
- [ ] Negative support everywhere it exists in Tailwind (not just inset)
- [ ] `space-x-*`/`space-y-*` with reverse variants (`space-x-reverse`, etc.)
- [ ] Logical spacing utilities (`ps-*`, `pe-*`, `ms-*`, `me-*`) for RTL friendliness

### Typography
- [ ] `whitespace-*` full set
- [ ] `break-*` full set
- [ ] `hyphens-*`
- [ ] `tab-size-*`
- [ ] `list-*` (style + position)
- [ ] `text-decoration-*` (thickness/style) + `underline-offset-*`
- [ ] `decoration-*`, `accent-*`, `caret-*`

### Borders / outlines / rings
- [ ] Full `outline-*` surface area (style/width/offset/color)
- [ ] `ring-offset-*` complete + `ring-inset`
- [ ] `divide-x-*`/`divide-y-*` width variants + `divide-*-reverse`

### Effects / filters
- [ ] `filter` suite (blur/brightness/contrast/drop-shadow/grayscale/hue-rotate/invert/saturate/sepia)
- [ ] `backdrop-filter` suite to match Tailwind
- [ ] `mix-blend-*`, `bg-blend-*`

### Transforms / transitions / animations
- [ ] `skew-*`, `transform-origin-*`, `transform-gpu`
- [ ] `transition-*` variants (`transition`, `transition-colors`, etc.)
- [ ] `duration-*`, `delay-*`, `ease-*` complete
- [ ] `animate-*` baseline set (spin/ping/pulse/bounce) + custom keyframes config

### Interactivity / a11y
- [ ] `sr-only` / `not-sr-only`
- [ ] `cursor-*` complete
- [ ] `select-*`, `pointer-events-*`
- [ ] `scroll-*` (scroll-behavior, snap, margin/padding)
- [ ] `touch-*`

### SVG
- [ ] `fill-*`, `stroke-*`, `stroke-width-*`

## Phase 2 — Variants Parity (The “Tailwind Feel”)

### State variants
- [ ] `checked`, `indeterminate`
- [ ] `required`, `optional`
- [ ] `invalid`, `valid`
- [ ] `open` (details/summary, popovers, dialogs)

### Group / peer
- [ ] `peer-*` variants (peer-hover, peer-focus, peer-checked, peer-invalid, etc.)
- [ ] `group-aria-*` / `aria-*` variants (optional, but modern Tailwind feature)

### Structural
- [ ] `only`, `empty`
- [ ] `first-of-type`, `last-of-type`, `only-of-type`

### Media / feature queries
- [ ] `print`
- [ ] reduced motion (`motion-reduce`, `motion-safe`)
- [ ] contrast (`contrast-more`, `contrast-less`)
- [ ] forced colors (`forced-colors`)
- [ ] orientation (`portrait`, `landscape`)
- [ ] `supports-*` variants

### Container queries (modern Tailwind)
- [ ] `@container`-style generation + `container-*`/`cq:*` variants (design TBD)

## Phase 3 — Arbitrary Values + Scanner Robustness

- [ ] Arbitrary value support for most utilities (`bg-[...]`, `shadow-[...]`, etc.)
- [ ] Better Razor scanning for:
  - [x] utility strings in Razor markup, C# expressions, and `@code` blocks
  - [ ] interpolated strings / conditional concatenations
  - [ ] `@attributes` patterns (where class is provided indirectly)

## Phase 4 — DX / Tooling (What makes Tailwind *pleasant*)

- [ ] `--watch` mode (incremental scan + regenerate)
- [ ] Caching (skip regenerate when no class set changes)
- [ ] `--fail-on-unknown` (optional strict mode)
- [x] Stable output ordering and formatting guarantees
- [ ] `--report json` (recognized/unknown lists for CI dashboards)
- [ ] First-class docs: supported utilities + variants + examples

## Success Criteria

- Docs site uses `Vibe.UI` components + `Vibe.UI.CSS` utilities with near-zero custom CSS.
- Most developers can “translate Tailwind muscle memory” to `vibe-*` with minimal friction.
- Unknown-class reporting is actionable (either fix the class or implement the utility).


