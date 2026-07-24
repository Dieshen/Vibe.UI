# Vibe.UI.CSS beta core profile

This document is the compatibility contract for `Vibe.UI.CSS` in
`1.0.0-beta`. It defines the Tailwind-style surface that is release-gated. It
does not claim that Vibe.UI.CSS is a drop-in replacement for the Tailwind CSS
compiler or its plugin ecosystem.

## Compatibility rule

Every class in this profile must:

1. be recognized by the scanner and generator;
2. emit deterministic CSS;
3. have representative automated coverage; and
4. remain backward compatible within the `1.0.0-beta` release line unless a
   release note documents the change.

The library source, CLI source, and docs application are scanned in strict mode
in CI. Classes backed by component-scoped `*.razor.css` selectors and Vibe's
versioned reserved component/runtime manifest are classified as non-utilities.
Application-specific hooks must still be listed explicitly, so every remaining
utility candidate must be recognized.

## Utility families

The beta profile covers the daily application surface:

| Area | Supported surface |
| --- | --- |
| Layout | display, position, inset, z-index, overflow, object fit, visibility |
| Flexbox | direction, wrap, grow/shrink, alignment, justification, self alignment, gap |
| Grid | column and row templates, column and row spans |
| Spacing | margin, padding, axis/directional forms, logical start/end forms, negative margins, child spacing |
| Sizing | width, height, min/max dimensions, shared `size-*`, fractions, viewport and content values |
| Typography | size, weight, family, alignment, transforms, decoration, wrap/overflow, line height, tracking, clamp, whitespace, word breaking |
| Color | semantic design tokens, Tailwind-style palette colors, opacity modifiers, text/background/border/ring/accent/caret colors |
| Borders | widths, directional widths, styles, radii, child dividers, ring widths/colors/inset |
| Effects | shadows, opacity, gradients, backdrop blur, translate/rotate/scale, transitions, durations, easing, baseline animations |
| Interactivity | outline reset, appearance, cursor, pointer events, selection, touch, resize, smooth scrolling, screen-reader helpers |

The exact scales are defined in `VibeConfig`; unsupported scale values are
reported as unknown instead of producing approximate CSS.

## Variants

The beta profile supports stacking the following variants:

| Category | Variants |
| --- | --- |
| Responsive | `sm`, `md`, `lg`, `xl`, `2xl` |
| Theme | `dark` using the Vibe `.dark` class contract |
| Interaction | `hover`, `focus`, `focus-visible`, `focus-within`, `active`, `visited`, `disabled` |
| Form state | `checked`, `indeterminate`, `required`, `optional`, `invalid`, `valid`, `read-only` |
| Disclosure | `open` for open/details and popover-open states |
| Structural | `first`, `last`, `only`, `odd`, `even`, `empty`, first/last/only-of-type |
| Composition | group and peer hover/focus/active/checked/disabled/invalid/required/open variants |
| Attributes | built-in `aria-*`, arbitrary `aria-[name=value]`, `data-[name=value]`, and group/peer forms |
| User preferences | `motion-safe`, `motion-reduce`, `contrast-more`, `contrast-less`, `forced-colors` |
| Environment | `portrait`, `landscape`, `print` |

Vibe keeps its prefix after variants, for example
`sm:motion-reduce:hover:vibe-transition-none`.

## Arbitrary values

Arbitrary values are supported for practical sizing, spacing, positioning,
typography, color, border, ring, grid, flex-basis, order, SVG color, accent, and
caret use cases.

```html
<div class="vibe-w-[var(--panel-width)] vibe-grid-cols-[12rem_minmax(0,1fr)]">
    <span class="vibe-text-[color:var(--vibe-foreground)]">Content</span>
</div>
```

Underscores in an arbitrary value represent spaces, while `\_` emits a literal
underscore. Underscores remain literal inside `url(...)` values and CSS custom
property names passed to `var(...)`. `color:` and `length:` type hints
disambiguate values such as CSS variables. Arbitrary custom properties are also
supported, for example `vibe-[--panel-width:20rem]`. Values containing
declaration delimiters or CSS block syntax are rejected.

## Strict mode

Use strict mode in application CI to catch unsupported utilities:

```bash
vibe css . --scan-only --fail-on-unknown
dotnet Vibe.UI.CSS.dll scan . --fail-on-unknown
```

Known non-utility hooks that share the `vibe-` prefix can be declared explicitly:

```bash
vibe css . --scan-only --fail-on-unknown --ignore "vibe-theme-hook,vibe-app-shell"
```

MSBuild consumers can use the same contract:

```xml
<PropertyGroup>
  <VibeCssFailOnUnknown>true</VibeCssFailOnUnknown>
  <VibeCssIgnoredClasses>vibe-theme-hook,vibe-app-shell</VibeCssIgnoredClasses>
</PropertyGroup>
```

Strict generation fails before modifying the existing output file.

Source-installed Vibe components do not require consumers to maintain a long
ignore list: scoped component selectors and reserved Vibe runtime hooks are
classified by the packaged scanner. `--ignore` remains intentionally explicit
for application-owned classes that happen to share the utility prefix.

## Outside the beta profile

The following are v1 roadmap work, not beta promises:

- full Tailwind utility and modifier breadth, including filters, blend modes,
  container queries, `supports-*`, and plugin APIs;
- a general parser for dynamically constructed or interpolated class fragments;
- every arbitrary-value form accepted by the Tailwind compiler;
- exact selector ordering or generated CSS identity with Tailwind; and
- watch caching, machine-readable reports, and third-party plugin compatibility.

Applications that require the complete Tailwind compiler surface can use real
Tailwind CSS alongside Vibe.UI. The component package is compatible with that
setup because its visual contract is based on CSS custom properties and scoped
component styles.
