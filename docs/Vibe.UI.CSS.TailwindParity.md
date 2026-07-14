# Vibe.UI.CSS Tailwind compatibility roadmap

Vibe.UI.CSS aims to make Tailwind-style utility authoring feel natural in
Blazor and Razor projects without requiring Node.js. Compatibility is defined
by a published, versioned matrix rather than by claiming compiler identity with
Tailwind CSS.

## Release boundaries

### 1.0.0-beta

Beta guarantees the complete [beta core profile](Vibe.UI.CSS.CoreProfile.md):

- all documented core utility families and variants are tested;
- strict scanning is available through both CLIs and MSBuild;
- every utility candidate in the docs application is recognized;
- practical arbitrary sizing, spacing, color, grid, and CSS-variable values
  are supported; and
- generated output is deterministic.

The enforced docs scan currently recognizes `407` of `407` utility candidates.
CI also strict-scans the component and CLI source trees. Scoped component hooks
and reserved Vibe runtime tokens are classified as non-utilities; intentional
application hooks remain explicit in the CI ignore list.

### Stable 1.0

Stable 1.0 targets 100% correctness for the expanded, published Vibe.UI.CSS
Tailwind 4 compatibility matrix. It does not promise Tailwind plugin ABI,
compiler implementation, or byte-for-byte generated CSS parity.

## Completed beta surface

- [x] Display, position, overflow, z-index, object fit, and visibility
- [x] Core flexbox, grid, alignment, spacing, and sizing
- [x] Logical margin and padding plus negative margins
- [x] Typography, semantic/palette colors, opacity modifiers, borders, radii,
  rings, shadows, transforms, transitions, and baseline animations
- [x] Cursor, pointer, selection, touch, resize, scrolling, and screen-reader helpers
- [x] Responsive, dark, interaction, form-state, disclosure, structural,
  group, peer, ARIA, data, reduced-motion, contrast, forced-colors,
  orientation, and print variants
- [x] Variant stacking across selectors and media queries
- [x] Practical arbitrary values and arbitrary custom properties
- [x] Razor markup, Razor expression, C# literal, and code-block scanning
- [x] Strict unknown-class failure with scoped/reserved classification and explicit app ignores
- [x] Stable output ordering and deterministic headers
- [x] CI gates for component source, CLI source, and the docs application's complete utility set

## Stable 1.0 backlog

### Utility breadth

- [ ] Breakpoint-aware `container`, isolation, and box-decoration utilities
- [ ] Static flex-basis, order, place, auto-grid, and grid-flow scales
- [ ] Complete child-spacing reverse behavior
- [ ] Hyphenation, tab size, list, decoration thickness/style, and underline offset
- [ ] Full outline and ring-offset surface
- [ ] Complete filter, backdrop-filter, and blend-mode families
- [ ] Skew, transform origin/GPU, delay, and configurable keyframes
- [ ] Scroll snap/margin/padding and complete SVG fill/stroke scales

### Variant breadth

- [ ] `supports-*` and arbitrary selector variants
- [ ] Container-query utilities and variants
- [ ] Remaining pointer, scripting, direction, and modern media variants
- [ ] Named group/peer and advanced compound selector forms

### Scanner and arbitrary values

- [ ] Broader interpolated-string and conditional-concatenation analysis
- [ ] Indirect `@attributes` and dictionary-provided class discovery
- [ ] Full arbitrary-value type inference and escaping compatibility
- [ ] Source safelists for classes that cannot be present literally

### Tooling

- [ ] Incremental watch caching
- [ ] Machine-readable JSON reports
- [ ] Generated supported-utility reference from the tested matrix
- [ ] Migration diagnostics for unsupported Tailwind classes

## Success criteria

Beta is complete when the core-profile tests, strict docs scan, package
validation, and standalone/hosted compatibility suites all pass for the same
candidate commit.

Stable 1.0 is complete when every item in the published v1 matrix is tested,
unsupported Tailwind behavior is listed explicitly, and the migration guide
shows when to use Vibe.UI.CSS versus the full Tailwind compiler.
