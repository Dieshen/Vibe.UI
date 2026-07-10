# Vibe.UI documentation

This directory is the canonical home for project documentation. The source code,
tests, and shipping project files remain the authority when documentation and
implementation disagree.

## Start here

- [Repository README](../README.md) - installation, quick start, component inventory, and support
- [Architecture](ARCHITECTURE.md) - component installation structure and architectural decisions
- [Compatibility](Compatibility.md) - standalone WebAssembly and hosted Blazor Web App support
- [CLI](CLI.md) - command behavior, topology detection, templates, and overwrite safety
- [CSS setup](CSS-SETUP.md) - consumer setup for package and source-install workflows
- [Vibe.UI.CSS](VIBE-UI-CSS.md) - scanner, generator, MSBuild integration, and hosted ownership

## Guides

- [Accessibility](ACCESSIBILITY.md) - semantics, focus, keyboard, validation, and live-region patterns
- [Charts](CHARTS.md) - Chart.js setup and the Chart component API
- [Theming](THEMING.md) - tokens, light and dark themes, and customization
- [CSS setup](CSS-SETUP.md) - stylesheet loading and troubleshooting
- [Hosting compatibility](Compatibility.md) - supported render and deployment shapes

## Architecture and maintenance

- [Architecture](ARCHITECTURE.md) - installation layout decision record
- [Architecture patterns](ARCHITECTURE_PATTERNS.md) - component authoring patterns
- [CLI architecture](CLI.md) - source-copy workflow and project detection
- [Vibe.UI.CSS architecture](VIBE-UI-CSS.md) - utility generation pipeline and build targets

## Release evidence

- [Beta readiness](Beta-Readiness-Checklist.md) - completed beta release gates and validation commands
- [Beta component hardening](Beta-Component-Hardening.md) - direct unit and browser coverage policy
- [Alpha release history](Alpha-0.1.0-Checklist.md) - historical alpha checklist and release summary

Release documents are evidence for a specific release line. They are not the
current installation guide and should not be edited to describe a later release.

## Roadmaps

- [shadcn/ui parity](Vibe.UI.ShadcnParity.md) - component and workflow parity goals
- [Tailwind parity](Vibe.UI.CSS.TailwindParity.md) - utility and variant coverage goals

Roadmaps contain planned work. An unchecked item is not a promise that the
feature exists, and a checked release gate does not imply every roadmap item is
complete.

## Documentation rules

1. Put durable repository documentation in `docs/`.
2. Keep the root `README.md` focused on installation, first use, and project navigation.
3. Keep package readmes under `src/` self-contained because they ship inside NuGet packages.
4. Link to source and tests instead of copying implementation details that will drift.
5. Mark historical release material clearly and link it to the release that superseded it.
6. Update this index when adding or removing a document.

