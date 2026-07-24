# Vibe.UI shadcn/ui compatibility roadmap

Vibe.UI aims to provide the shadcn/ui experience for Blazor: strong defaults,
composable source-owned components, predictable APIs, and an ergonomic CLI.
Compatibility is measured by behavior and workflow, not by copying React APIs
or generated markup exactly.

## Release boundaries

### 1.0.0-beta

Beta guarantees the documented
[component beta profile](Vibe.UI.ComponentBetaProfile.md). It is a dogfooding
release for supported Blazor hosting models, not a claim of complete shadcn/ui
parity or formal WCAG conformance.

The repository contains 111 Razor component files. That count includes roots,
items, helpers, and composed primitives; it must not be read as 111 independent
catalog products. Every source component has a direct unit-test file, while
browser, visual, and accessibility depth is risk-based and tracked separately.

### Stable 1.0

Stable 1.0 targets 100% of the published Vibe.UI shadcn compatibility matrix.
Each supported equivalent must have:

1. a documented public API and copyable compiling example;
2. equivalent pointer and keyboard behavior for the supported feature set;
3. focus, ARIA, disabled, error, loading, and empty-state coverage;
4. reviewed light/dark and responsive visual evidence; and
5. package and CLI source-install coverage.

Features that do not map cleanly to Blazor may use an idiomatic Vibe API, but
the difference must be explicit in the matrix and migration documentation.

## Beta surface

- [x] CSS-variable theming with a `.dark` root contract
- [x] Scoped component styles and shared `Class`/`AdditionalAttributes` base API
- [x] Package consumption and CLI source ownership workflows
- [x] Standalone WebAssembly and Blazor Web App static/server/client/auto fixtures
- [x] Core input, form, disclosure, dialog, tabs, date, and basic menu contracts
- [x] Direct unit-test files for all 111 source Razor components
- [x] Automated serious/critical Axe checks on representative high-risk routes
- [x] Reviewed light/dark desktop/mobile baselines for six high-risk surfaces
- [ ] Formal WCAG conformance evidence
- [ ] Reviewed visual and browser interaction coverage for every catalog surface

## Stable 1.0 parity work

### API and composition

- [ ] Publish a versioned component-by-component shadcn compatibility matrix
- [ ] Normalize `Variant`, `Size`, `Class`, attributes, callbacks, and bindable APIs
- [ ] Finish root/trigger/content/item composition consistency across primitives
- [ ] Define headless and full-style override guidance

### Interaction and accessibility

- [ ] Complete advanced menu submenus, hover intent, and compound typeahead behavior
- [ ] Verify every interactive component at keyboard-only, 200% zoom, and forced colors
- [ ] Complete manual screen-reader passes for supported desktop and mobile flows
- [ ] Add performance budgets for DataTable, VirtualScroll, Chart, and other heavy surfaces

### Visual and documentation quality

- [ ] Review every catalog surface in light/dark desktop/mobile states
- [ ] Ensure every public example compiles against the shipped API
- [ ] Publish recipes for auth, settings, dashboards, forms, and table/filter flows
- [ ] Keep component counts and catalog routes generated or test-verified

### CLI workflow

- [x] `vibe init` detects standalone and hosted client/server topology
- [x] `vibe add` installs dependencies and required JS assets
- [x] Package validation builds generated standalone and hosted consumers
- [ ] Add a reviewable `vibe diff` workflow
- [ ] Publish registry metadata for dependencies, files, and documentation links
- [ ] Define safe update/merge behavior for locally customized components

## Success criteria

Beta is complete when the component beta profile, CSS beta core profile,
package validation, hosting fixtures, and release dry-run pass for the same
candidate commit.

Stable 1.0 is complete when the published shadcn and Tailwind compatibility
matrices are both at 100%, every supported row has automated and reviewed
evidence, and all intentional differences are documented.
