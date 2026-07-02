# Beta Component Hardening Tracker

This tracker defines the beta confidence bar for component hardening. It is intentionally stricter than "the solution builds" and separates source coverage, browser smoke coverage, and follow-up test quality.

## Target

- Source component set: 110 Razor components in `src/Vibe.UI/Components`.
- Beta hardening target: at least 80% direct unit-test coverage across source components.
- Numeric target: 88 of 110 components with direct component tests.
- Current direct unit coverage: 76 of 110 components, or 69.1%.
- Remaining direct unit tests needed for 80%: 12 components.
- Current docs browser smoke coverage: 53 component routes.
- Browser smoke target: every top-level documented component page must stay in `Category=Smoke`.

Direct unit coverage means a component has an explicit `*Tests.cs` file under `tests/Vibe.UI.Tests/Components`. Browser smoke coverage means the docs route is included in `tests/Vibe.UI.Docs.E2E/Tests/Smoke/AllComponentsRenderTests.cs`.

## Current Category Snapshot

| Category | Total | Direct unit tests | Docs smoke routes |
| --- | ---: | ---: | ---: |
| Advanced | 4 | 3 | 3 |
| DataDisplay | 8 | 7 | 7 |
| DateTime | 3 | 3 | 3 |
| Disclosure | 5 | 3 | 3 |
| Feedback | 9 | 8 | 4 |
| Form | 7 | 7 | 3 |
| Inputs | 23 | 23 | 11 |
| Layout | 12 | 7 | 7 |
| Navigation | 14 | 3 | 6 |
| Overlay | 17 | 8 | 4 |
| Theme | 2 | 0 | 1 |
| Utility | 6 | 4 | 1 |

## Next 12 Direct Unit Tests

These are the most useful next tests to reach the 80% beta bar. Priority favors interactive, composition-heavy, or public consumer-facing components over simple wrappers.

1. `NavigationMenu`
2. `NavigationMenuItem`
3. `Menubar`
4. `Menu`
5. `MenuItem`
6. `MenuSeparator`
7. `Breadcrumb`
8. `BreadcrumbItem`
9. `ToastContainer`
10. `ThemeProvider`
11. `ThemeToggle`
12. `ScrollArea`

After this slice lands, reassess before adding more lower-risk wrapper tests such as `Kbd`, `GridItem`, `TabItem`, and dialog subcomponents.

## Mutation-Style Quality Checks

For each new component test file, prefer assertions that would catch these breakages:

- Required classes or ARIA attributes disappear.
- Disabled/open/selected state is inverted.
- Event callbacks stop firing or fire with the wrong value.
- Null or omitted optional content throws.
- Child content stops rendering.
- Keyboard or focus state regressions for interactive components.
- JS interop paths are not invoked or cleanup paths stop running.

## Browser Coverage Rules

- Top-level docs pages stay covered by `Category=Smoke`.
- Hosting compatibility stays covered by `Category=Compatibility` across standalone WebAssembly and Blazor Web App static/server/client/auto paths.
- Components with browser-only behavior, JS interop, timers, drag/drop, or focus management should get either a docs smoke route or targeted E2E coverage in addition to unit tests.
