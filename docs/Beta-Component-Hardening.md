# Beta component hardening tracker

This tracker separates inventory coverage from behavioral, browser, visual,
and accessibility depth. A passing unit test file is useful evidence, but it is
not by itself a production-readiness claim.

## Inventory target

- Source set: 111 Razor files under `src/Vibe.UI/Components`.
- Beta target: direct unit-test files for at least 89 of 111 source components.
- Current direct unit-test inventory: 111 of 111.
- Remaining files needed for the 80% target: 0.
- Browser target: every real component catalog route is covered by a smoke test
  that rejects NotFound and error shells.

Direct coverage means an explicit component test file exists under
`tests/Vibe.UI.Tests/Components`. It does not mean every component has equal
interaction, accessibility, edge-case, JS-boundary, or visual coverage.

## Beta depth contract

The release-gated behavioral scope is defined in
[the component beta profile](Vibe.UI.ComponentBetaProfile.md). High-risk
contracts receive focused assertions for callbacks, binding, disabled/read-only
behavior, keyboard and focus management, ARIA, null inputs, and browser-only
boundaries.

The repository does not claim that all 111 source files are independently
production-hardened. Stable 1.0 requires a published component matrix with
reviewed evidence for every supported row.

## Test quality rules

Focused tests should fail when:

- required classes, semantics, or ARIA attributes disappear;
- disabled, open, selected, or invalid state is inverted;
- a callback stops firing, fires twice, or returns the wrong value;
- a documented two-way binding callback is missing;
- null or omitted optional content throws;
- keyboard navigation or focus restoration regresses;
- JS interop paths, cleanup, subscriptions, or timers leak; or
- a docs route renders NotFound while the smoke test still passes.

## Browser coverage rules

- Every routed docs component page stays in `Category=Smoke`.
- Browser-only behavior gets targeted `Category=Functional` coverage.
- Hosting compatibility stays in `Category=Compatibility` across standalone
  WebAssembly and Blazor Web App static/server/client/auto routes.
- Visual baselines are reviewed evidence for named surfaces only; their coverage
  must not be generalized to the full component inventory.

## Stable 1.0 follow-up

- complete the versioned shadcn compatibility matrix;
- compile-check every copyable public example;
- expand reviewed light/dark desktop/mobile visual coverage;
- complete manual screen-reader, zoom/reflow, and forced-colors passes; and
- normalize legacy API naming and composition patterns.
