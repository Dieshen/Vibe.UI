# Vibe.UI 1.0.0-beta

`1.0.0-beta` is the first release candidate intended for broad dogfooding on
.NET 10. It supports both package consumption and shadcn-style source ownership.

## Highlights

- 111 Razor component source files with complete direct unit-test inventory
- standalone Blazor WebAssembly and hosted Blazor Web App compatibility fixtures
- Interactive Server, WebAssembly, Auto, and static SSR package validation
- topology-aware CLI initialization for standalone and client/server solutions
- deterministic Vibe.UI.CSS generation with an enforced beta core profile
- strict unknown-utility detection for CLI and MSBuild consumers
- local Chart.js and Shiki runtime assets with no browser-time CDN dependency
- focused accessibility, keyboard, focus, functional, and reviewed visual gates

## Compatibility contracts

- [Component beta profile](Vibe.UI.ComponentBetaProfile.md)
- [Vibe.UI.CSS beta core profile](Vibe.UI.CSS.CoreProfile.md)
- [Hosting compatibility](Compatibility.md)
- [CLI behavior](CLI.md)

Beta does not claim full shadcn/ui or Tailwind compiler parity. Stable 1.0
targets 100% of the two published Vibe compatibility matrices.

## Known limitations

- APIs may still change before stable 1.0.
- Formal WCAG conformance requires manual screen-reader, 200% zoom/reflow, and
  forced-colors evidence.
- Reviewed visual baselines cover named high-risk surfaces on Chromium/Windows,
  not every component and platform.
- Advanced menu composition, full API normalization, and universal compiling
  sample coverage remain stable 1.0 work.
- Vibe.UI.CSS supports its beta core profile, not Tailwind plugins or the full
  Tailwind compiler surface.

## Release evidence

The exact candidate commit must pass the full
[beta readiness checklist](Beta-Readiness-Checklist.md), package validation,
all supported hosting/browser compatibility gates, and a dry-run of the NuGet
publish workflow before packages are pushed.
